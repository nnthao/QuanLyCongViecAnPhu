using Dashboard.Models.ViewModel;
using log4net;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class HisBenhNhanController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HisBenhNhanController));

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LayThongTinTheoMaBenhNhan(HisThongTinBenhNhanRequestViewModel request)
        {
            const string hisBaseUrl = "https://bvquangngai.vncare.vn";
            const string hisRestServicePath = "/vnpthis/RestService";
            const string hisHost = "bvquangngai.vncare.vn";
            const string hisReferer = "https://bvquangngai.vncare.vn/vnpthis/main/manager.jsp?func=../noitru/NTU02D059_TraCuuBenhNhan";
            const string hisPatientLookupDataset = "NTUD059.DS_TCBN";
            const string hisUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/146.0.0.0 Safari/537.36";

            var tuNgay = new DateTime(2025, 1, 1);
            var denNgay = new DateTime(2025, 12, 31);

            if (request == null)
            {
                return Json(new { Errors = new[] { new { Name = "", Description = "Khong nhan duoc du lieu dau vao." } } }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrWhiteSpace(request.MaBenhNhan))
            {
                return Json(new { Errors = new[] { new { Name = nameof(request.MaBenhNhan), Description = "Ma benh nhan la bat buoc." } } }, JsonRequestBehavior.AllowGet);
            }

            var uuid = GetFirstNotEmpty(request.Uuid);
            if (string.IsNullOrWhiteSpace(uuid))
            {
                return Json(new { Errors = new[] { new { Name = nameof(request.Uuid), Description = "Thieu uuid phien HIS trong request." } } }, JsonRequestBehavior.AllowGet);
            }

            var cookie = GetFirstNotEmpty(request.Cookie);
            if (string.IsNullOrWhiteSpace(cookie))
            {
                return Json(new { Errors = new[] { new { Name = nameof(request.Cookie), Description = "Thieu Cookie phien HIS trong request." } } }, JsonRequestBehavior.AllowGet);
            }

            var jsessionId = GetCookieValue(cookie, "JSESSIONID");
            if (string.IsNullOrWhiteSpace(jsessionId))
            {
                return Json(new { Errors = new[] { new { Name = nameof(request.Cookie), Description = "Cookie thieu JSESSIONID." } } }, JsonRequestBehavior.AllowGet);
            }

            var sessionId = GetCookieValue(cookie, "SESSIONID");
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return Json(new { Errors = new[] { new { Name = nameof(request.Cookie), Description = "Cookie thieu SESSIONID." } } }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var filter = new HisThongTinBenhNhanFilterViewModel
                {
                    VVTUNGAY = tuNgay.ToString("dd/MM/yyyy"),
                    VVDENNGAY = denNgay.ToString("dd/MM/yyyy"),
                    RVTUNGAY = NormalizeHisValue(request.TuNgayRaVien),
                    RVDENNGAY = NormalizeHisValue(request.DenNgayRaVien),
                    MABENHNHAN = request.MaBenhNhan.Trim(),
                    TENBENHNHAN = NormalizeHisValue(request.TenBenhNhan),
                    MACHANDOANRAVIEN = NormalizeHisValue(request.MaChanDoanRaVien),
                    CHANDOANRAVIEN = NormalizeHisValue(request.ChanDoanRaVien),
                    MABENHAN = NormalizeHisValue(request.MaBenhAn),
                    MA_BHYT = NormalizeHisValue(request.MaBHYT),
                    MACAP = NormalizeHisValue(request.MaCap),
                    SOLUUTRU = NormalizeHisValue(request.SoLuuTru),
                    MALUUTRUKHOA = NormalizeHisValue(request.MaLuuTruKhoa),
                    NGAYSINH = NormalizeHisValue(request.NgaySinh),
                    KHOA = NormalizeHisValue(request.Khoa),
                    SOVAOVIEN = NormalizeHisValue(request.SoVaoVien),
                    DIACHI = NormalizeHisValue(request.DiaChi),
                    SDT = NormalizeHisValue(request.SoDienThoai),
                    MA_LK = NormalizeHisValue(request.MaLK),
                    CCCD = NormalizeHisValue(request.CCCD)
                };

                var payload = new HisExecuteQueryPagingRequestViewModel
                {
                    func = "ajaxExecuteQueryPaging",
                    uuid = uuid,
                    @params = new List<string> { hisPatientLookupDataset },
                    options = new List<HisRequestOptionViewModel>
                    {
                        new HisRequestOptionViewModel
                        {
                            name = "[0]",
                            value = JsonConvert.SerializeObject(filter)
                        }
                    }
                };

                var payloadJson = JsonConvert.SerializeObject(payload);
                var encodedPayloadJson = HttpUtility.UrlEncode(payloadJson);
                var queryString =
                    $"?postData={encodedPayloadJson}" +
                    $"&_search={HttpUtility.UrlEncode("false")}" +
                    $"&nd={HttpUtility.UrlEncode(DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString())}" +
                    $"&rows={HttpUtility.UrlEncode(request.Rows > 0 ? request.Rows.ToString() : "20")}" +
                    $"&page={HttpUtility.UrlEncode(request.Page > 0 ? request.Page.ToString() : "1")}" +
                    $"&sidx={HttpUtility.UrlEncode(string.Empty)}" +
                    $"&sord={HttpUtility.UrlEncode("asc")}";
                var resource = hisRestServicePath + queryString;
                var fullRequestUrl = hisBaseUrl + resource;

                var client = new RestClient(hisBaseUrl)
                {
                    Timeout = 30000,
                    UserAgent = hisUserAgent
                };

                var restRequest = new RestRequest(resource, Method.GET);
                restRequest.AddHeader("Host", hisHost);
                restRequest.AddHeader("sec-ch-ua-platform", "\"Windows\"");
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
                restRequest.AddHeader("sec-ch-ua", "\"Chromium\";v=\"146\", \"Not-A.Brand\";v=\"24\", \"Google Chrome\";v=\"146\"");
                restRequest.AddHeader("DNT", "1");
                restRequest.AddHeader("sec-ch-ua-mobile", "?0");
                restRequest.AddHeader("Sec-Fetch-Site", "same-origin");
                restRequest.AddHeader("Sec-Fetch-Mode", "cors");
                restRequest.AddHeader("Sec-Fetch-Dest", "empty");
                restRequest.AddHeader("Referer", hisReferer);
                restRequest.AddHeader("Accept-Language", "vi");
                restRequest.AddHeader("Cookie", cookie);

                log.Info($"HisBenhNhan fullRequestUrl: {fullRequestUrl}");

                var response = await client.ExecuteTaskAsync(restRequest);
                var responseContent = response?.Content;

                if (response == null || response.ResponseStatus != ResponseStatus.Completed)
                {
                    log.Warn($"Lay thong tin benh nhan HIS khong thanh cong do ResponseStatus={response?.ResponseStatus}, MaBenhNhan={request.MaBenhNhan}, Response={responseContent}");
                    return Json(new
                    {
                        Errors = new[]
                        {
                            new
                            {
                                Name = "",
                                Description = "Khong nhan duoc phan hoi hop le tu HIS.",
                                Response = responseContent
                            }
                        }
                    }, JsonRequestBehavior.AllowGet);
                }

                if ((int)response.StatusCode < 200 || (int)response.StatusCode >= 300)
                {
                    log.Warn($"Lay thong tin benh nhan HIS khong thanh cong. Status={(int)response.StatusCode}, MaBenhNhan={request.MaBenhNhan}, Response={responseContent}");
                    return Json(new
                    {
                        Errors = new[]
                        {
                            new
                            {
                                Name = "",
                                Description = $"HIS tra ve ma loi {(int)response.StatusCode}.",
                                Response = responseContent
                            }
                        }
                    }, JsonRequestBehavior.AllowGet);
                }

                var responseData = JsonConvert.DeserializeObject<HisThongTinBenhNhanResponseViewModel>(responseContent);
                return Json(responseData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error($"Loi lay thong tin benh nhan HIS theo ma {request.MaBenhNhan}: {ex.Message}", ex);
                return Json(new { Errors = new[] { new { Name = "", Description = $"Loi goi HIS: {ex.Message}" } } }, JsonRequestBehavior.AllowGet);
            }
        }

        private static string NormalizeHisValue(string value, string defaultValue = "-1")
        {
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
        }

        private static string GetCookieValue(string cookieHeader, string cookieName)
        {
            if (string.IsNullOrWhiteSpace(cookieHeader) || string.IsNullOrWhiteSpace(cookieName))
            {
                return string.Empty;
            }

            var cookieParts = cookieHeader.Split(';');
            foreach (var cookiePart in cookieParts)
            {
                var item = cookiePart.Trim();
                if (!item.StartsWith(cookieName + "=", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                return item.Substring(cookieName.Length + 1).Trim();
            }

            return string.Empty;
        }

        private static string GetFirstNotEmpty(params string[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return string.Empty;
        }
    }
}
