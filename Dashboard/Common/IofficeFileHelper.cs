using Dashboard.AdoNetHelper;
using Dashboard.Models.Entities;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Dashboard.Common
{
    public class IofficeFileDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("attachmentId")]
        public string AttachmentId { get; set; }
    }

    public class IofficeApiResponse<T>
    {
        [JsonProperty("status")]
        public IofficeApiStatus Status { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

    public class IofficeApiStatus
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class IofficeLoginDataDto
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }

    public class IofficeDownloadDataDto
    {
        [JsonProperty("data")]
        public string Data { get; set; }
    }

    public static class IofficeFileHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MvcApplication));
        private const string TokenCacheKey = "IofficeAuthTokenCacheKey";

        public static async Task<string> GetTokenAsync(bool forceRefresh = false)
        {
            if (!forceRefresh)
            {
                var cachedToken = HttpRuntime.Cache[TokenCacheKey] as string;
                if (!string.IsNullOrWhiteSpace(cachedToken))
                {
                    return cachedToken;
                }
            }

            try
            {
                var host = BienDungChung.IofficeHost?.TrimEnd('/');
                var username = BienDungChung.IofficeUsername;
                var password = BienDungChung.IofficePassword;

                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    log.Warn("Ioffice Host, Username hoặc Password chưa được cấu hình.");
                    return null;
                }

                log.Info($"Bắt đầu đăng nhập iOffice: host={host}, username={username}");
                var loginUrl = $"{host}/api/login/v3/";
                var loginBody = JsonConvert.SerializeObject(new
                {
                    username = username,
                    password = password
                });

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    var content = new StringContent(loginBody, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(loginUrl, content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var apiRes = JsonConvert.DeserializeObject<IofficeApiResponse<IofficeLoginDataDto>>(responseContent);
                        if (apiRes != null && apiRes.Status?.Code == "0" && apiRes.Data != null && !string.IsNullOrWhiteSpace(apiRes.Data.Token))
                        {
                            var token = apiRes.Data.Token;
                            HttpRuntime.Cache.Insert(
                                TokenCacheKey,
                                token,
                                null,
                                DateTime.UtcNow.AddMinutes(55),
                                Cache.NoSlidingExpiration);

                            log.Info($"Đăng nhập iOffice thành công, token: {token.Substring(0, Math.Min(15, token.Length))}...");
                            return token;
                        }
                        else
                        {
                            log.Warn($"Đăng nhập iOffice thất bại: code={apiRes?.Status?.Code}, message={apiRes?.Status?.Message}");
                        }
                    }
                    else
                    {
                        log.Warn($"Đăng nhập iOffice HTTP thất bại: StatusCode={response.StatusCode}, Response={responseContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi ngoại lệ khi gọi API login iOffice: {ex.Message}", ex);
            }

            return null;
        }

        public static async Task<List<IofficeFileDto>> GetFileAttachAsync(string vanBanId)
        {
            var result = new List<IofficeFileDto>();
            if (string.IsNullOrWhiteSpace(vanBanId))
            {
                return result;
            }

            try
            {
                var token = await GetTokenAsync();
                if (string.IsNullOrWhiteSpace(token))
                {
                    log.Warn("Không lấy được token iOffice để gọi getfileattach.");
                    return result;
                }

                var host = BienDungChung.IofficeHost?.TrimEnd('/');
                var url = $"{host}/api/file/getfileattach/{vanBanId.Trim()}/";

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    client.DefaultRequestHeaders.Add("X-AUTHENTICATION-TOKEN", token);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(url);
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        log.Warn($"Token hết hạn hoặc không hợp lệ khi gọi getfileattach (StatusCode={response.StatusCode}). Tiến hành refresh token...");
                        token = await GetTokenAsync(forceRefresh: true);
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            using (var retryClient = new HttpClient())
                            {
                                retryClient.Timeout = TimeSpan.FromSeconds(30);
                                retryClient.DefaultRequestHeaders.Add("X-AUTHENTICATION-TOKEN", token);
                                retryClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                response = await retryClient.GetAsync(url);
                            }
                        }
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var apiRes = JsonConvert.DeserializeObject<IofficeApiResponse<List<IofficeFileDto>>>(content);
                        if (apiRes != null && apiRes.Data != null)
                        {
                            result = apiRes.Data;
                            log.Info($"Lấy danh sách file iOffice thành công cho vanBanId={vanBanId}, số lượng: {result.Count}");
                        }
                    }
                    else
                    {
                        log.Warn($"GetFileAttachAsync thất bại: StatusCode={response.StatusCode}, Url={url}, Response={content}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi ngoại lệ khi gọi API getfileattach từ iOffice (vanBanId={vanBanId}): {ex.Message}", ex);
            }

            return result;
        }

        public static async Task<byte[]> DownloadFileAsync(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                return null;
            }

            try
            {
                var token = await GetTokenAsync();
                if (string.IsNullOrWhiteSpace(token))
                {
                    log.Warn("Không lấy được token iOffice để download file.");
                    return null;
                }

                var host = BienDungChung.IofficeHost?.TrimEnd('/');
                var url = $"{host}/api/file/downloaddocument/{fileId.Trim()}/";

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(60);
                    client.DefaultRequestHeaders.Add("x-authentication-token", token);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(url);
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        log.Warn($"Token hết hạn khi download document fileId={fileId} (StatusCode={response.StatusCode}). Tiến hành refresh token...");
                        token = await GetTokenAsync(forceRefresh: true);
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            using (var retryClient = new HttpClient())
                            {
                                retryClient.Timeout = TimeSpan.FromSeconds(60);
                                retryClient.DefaultRequestHeaders.Add("x-authentication-token", token);
                                retryClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                response = await retryClient.GetAsync(url);
                            }
                        }
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var apiRes = JsonConvert.DeserializeObject<IofficeApiResponse<IofficeDownloadDataDto>>(responseContent);
                        if (apiRes != null && apiRes.Data != null && !string.IsNullOrWhiteSpace(apiRes.Data.Data))
                        {
                            var bytes = Convert.FromBase64String(apiRes.Data.Data.Trim());
                            log.Info($"Download document fileId={fileId} thành công, dung lượng: {bytes.Length} bytes");
                            return bytes;
                        }
                        else
                        {
                            log.Warn($"Download document fileId={fileId} trả về rỗng hoặc không có dữ liệu base64.");
                        }
                    }
                    else
                    {
                        log.Warn($"DownloadFileAsync thất bại: StatusCode={response.StatusCode}, Url={url}, Response={responseContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi ngoại lệ khi tải file từ iOffice (fileId={fileId}): {ex.Message}", ex);
            }

            return null;
        }

        public static async Task DownloadAndSaveFilesForGiaoViecAsync(int giaoViecId, List<IofficeFileAttachmentViewModel> iofficeFiles)
        {
            if (giaoViecId <= 0 || iofficeFiles == null || iofficeFiles.Count == 0)
            {
                return;
            }

            string uploadsPhysicalPath;
            if (HostingEnvironment.IsHosted)
            {
                uploadsPhysicalPath = HostingEnvironment.MapPath(BienDungChung.UploadsPath);
            }
            else
            {
                uploadsPhysicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");
            }

            if (!Directory.Exists(uploadsPhysicalPath))
            {
                Directory.CreateDirectory(uploadsPhysicalPath);
            }

            log.Info($"Bắt đầu tải và lưu {iofficeFiles.Count} file iOffice cho công việc Id={giaoViecId}...");

            foreach (var file in iofficeFiles)
            {
                if (string.IsNullOrWhiteSpace(file.FileId))
                {
                    continue;
                }

                try
                {
                    var fileBytes = await DownloadFileAsync(file.FileId);
                    if (fileBytes != null && fileBytes.Length > 0)
                    {
                        var extension = Path.GetExtension(file.FileName);
                        if (string.IsNullOrEmpty(extension))
                        {
                            extension = ".pdf";
                        }

                        var savedFileName = DateTime.Now.ToFileTime() + "_" + Guid.NewGuid().ToString("N").Substring(0, 6) + extension;
                        var fullPath = Path.Combine(uploadsPhysicalPath, savedFileName);
                        File.WriteAllBytes(fullPath, fileBytes);

                        var relativePath = $"{BienDungChung.UploadsPath.TrimEnd('/')}/{savedFileName}";
                        GiaoViecFileAdoNetHelper.Them(giaoViecId, file.FileName, relativePath, BienDungChung.ConnectDBStr);
                        log.Info($"Đã lưu file iOffice {file.FileName} (Id={file.FileId}) thành công vào DB và ổ đĩa: {relativePath}");
                    }
                    else
                    {
                        log.Warn($"Không nhận được dữ liệu file từ iOffice cho fileId={file.FileId}, tên file={file.FileName}");
                    }
                }
                catch (Exception ex)
                {
                    log.Error($"Lỗi khi tải và lưu file iOffice {file.FileName} (Id={file.FileId}) cho công việc {giaoViecId}: {ex.Message}", ex);
                }
            }
        }
    }
}
