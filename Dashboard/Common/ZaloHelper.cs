using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using log4net;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;

namespace Dashboard.Common
{
    public class ZaloHelper
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(typeof(ZaloHelper));

        public static ZaloToken GetTokenFromDB(string connStr = null)
        {
            try
            {
                var connectionString = !string.IsNullOrEmpty(connStr) ? connStr : BienDungChung.ConnectDBStr;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT TOP 1 Id, AccessToken, RefreshToken FROM ZaloToken";
                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ZaloToken
                            {
                                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                AccessToken = reader["AccessToken"] != DBNull.Value ? reader["AccessToken"].ToString() : null,
                                RefreshToken = reader["RefreshToken"] != DBNull.Value ? reader["RefreshToken"].ToString() : null
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi lấy ZaloToken từ DB: {ex.Message}", ex);
            }
            return null;
        }

        public static bool ThemOrCapNhatToken(string accessToken, string refreshToken, string connStr = null)
        {
            try
            {
                var connectionString = !string.IsNullOrEmpty(connStr) ? connStr : BienDungChung.ConnectDBStr;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sqlCheck = "SELECT TOP 1 Id FROM ZaloToken";
                    int existingId = 0;
                    using (var cmdCheck = new SqlCommand(sqlCheck, conn))
                    {
                        var result = cmdCheck.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            existingId = Convert.ToInt32(result);
                        }
                    }

                    if (existingId > 0)
                    {
                        string sqlUpdate = "UPDATE ZaloToken SET AccessToken = @AccessToken, RefreshToken = @RefreshToken WHERE Id = @Id";
                        using (var cmdUpdate = new SqlCommand(sqlUpdate, conn))
                        {
                            cmdUpdate.Parameters.AddWithValue("@AccessToken", (object)accessToken ?? DBNull.Value);
                            cmdUpdate.Parameters.AddWithValue("@RefreshToken", (object)refreshToken ?? DBNull.Value);
                            cmdUpdate.Parameters.AddWithValue("@Id", existingId);
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string sqlInsert = "INSERT INTO ZaloToken (AccessToken, RefreshToken) VALUES (@AccessToken, @RefreshToken)";
                        using (var cmdInsert = new SqlCommand(sqlInsert, conn))
                        {
                            cmdInsert.Parameters.AddWithValue("@AccessToken", (object)accessToken ?? DBNull.Value);
                            cmdInsert.Parameters.AddWithValue("@RefreshToken", (object)refreshToken ?? DBNull.Value);
                            cmdInsert.ExecuteNonQuery();
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi lưu ZaloToken vào DB: {ex.Message}", ex);
                return false;
            }
        }

        public static string GetAccessTokenFromRefreshToken(string refreshtoken = null)
        {
            try
            {
                string tokenToUse = !string.IsNullOrEmpty(refreshtoken) ? refreshtoken : BienDungChung.RefreshToken;
                string param = $"?refresh_token={tokenToUse}&app_id={BienDungChung.AppId}&grant_type=refresh_token";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://oauth.zaloapp.com/v4/oa/access_token" + param);
                request.Method = "POST";
                request.Headers["secret_key"] = BienDungChung.AppSecret;
                request.ContentType = "application/x-www-form-urlencoded";

                using (HttpWebResponse webResp = (HttpWebResponse)request.GetResponse())
                using (Stream stream = webResp.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi lấy AccessToken từ RefreshToken Zalo: {ex.Message}", ex);
                return null;
            }
        }

        public static bool GuiTinNhanQuaZaloOA(string text, string userId, string messageId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(messageId))
                {
                    log.Warn("Không thể gửi tin nhắn Zalo: UserId và MessageId đều rỗng.");
                    return false;
                }

                ZaloToken token = GetTokenFromDB();
                string refreshToken = token != null ? token.RefreshToken : null;
                string accessToken = token != null ? token.AccessToken : null;

                if (string.IsNullOrEmpty(accessToken))
                {
                    accessToken = BienDungChung.AccessToken;
                }
                if (string.IsNullOrEmpty(refreshToken))
                {
                    refreshToken = BienDungChung.RefreshToken;
                }

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                    | SecurityProtocolType.Tls11
                    | SecurityProtocolType.Tls12
                    | SecurityProtocolType.Ssl3;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://openapi.zalo.me/v2.0/oa/message");
                request.Method = "POST";
                request.Headers["access_token"] = accessToken;
                request.ContentType = "application/json";

                var data = new MessageZaloViewModelHCC
                {
                    message = new Messages { text = text }
                };

                if (!string.IsNullOrEmpty(messageId))
                {
                    data.recipient = new MessageId { message_id = messageId, user_id = "" };
                }
                else if (!string.IsNullOrEmpty(userId))
                {
                    data.recipient = new MessageId { user_id = userId, message_id= "" };
                }

                string strJson = JsonConvert.SerializeObject(data);
                byte[] databyte = System.Text.Encoding.UTF8.GetBytes(strJson);
                request.ContentLength = databyte.Length;

                using (var reqStrm = request.GetRequestStream())
                {
                    reqStrm.Write(databyte, 0, databyte.Length);
                }

                string jsonString;
                using (HttpWebResponse webResp = (HttpWebResponse)request.GetResponse())
                using (Stream stream = webResp.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                {
                    jsonString = reader.ReadToEnd();
                }

                dynamic jsonStringDes = JsonConvert.DeserializeObject(jsonString);
                string errorCode = Convert.ToString(jsonStringDes["error"]);

                if (errorCode == "-216")
                {
                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        string refreshTokenResponse = GetAccessTokenFromRefreshToken(refreshToken);
                        if (!string.IsNullOrEmpty(refreshTokenResponse))
                        {
                            dynamic jsondata = JsonConvert.DeserializeObject(refreshTokenResponse);
                            string refreshErrorCode = Convert.ToString(jsondata["error"]);
                            if (refreshErrorCode == "-14020" || refreshErrorCode == "-14014")
                            {
                                log.Error("Zalo RefreshToken đã hết hạn hoặc không hợp lệ.");
                                return false;
                            }
                            else if (string.IsNullOrEmpty(refreshErrorCode) || refreshErrorCode == "0")
                            {
                                string newAccessToken = Convert.ToString(jsondata["access_token"]);
                                string newRefreshToken = Convert.ToString(jsondata["refresh_token"]);
                                ThemOrCapNhatToken(newAccessToken, newRefreshToken);
                                return GuiTinNhanQuaZaloOA(text, userId, messageId);
                            }
                            else
                            {
                                log.Error($"Lỗi khi refresh Zalo token: {refreshTokenResponse}");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        log.Error("Zalo RefreshToken rỗng, không thể cấp mới AccessToken.");
                        return false;
                    }
                }
                else if (errorCode == "0")
                {
                    log.Info($"Gửi tin nhắn Zalo thành công cho user: {userId}");
                    return true;
                }
                else
                {
                    log.Error($"Lỗi gửi tin nhắn Zalo: {jsonString}");
                    return false;
                }

                return false;
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi ngoại lệ khi gửi tin nhắn Zalo: {ex.Message}", ex);
                return false;
            }
        }
    }
}
