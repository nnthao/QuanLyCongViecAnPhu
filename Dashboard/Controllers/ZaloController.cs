using Dashboard.Models.ViewModel;
using Dashboard.Services;
using log4net;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class ZaloController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ZaloController));
        private readonly IZaloService _zaloService;
        private readonly string code_verifier;
        private readonly string code_challenge;
        private readonly SHA256 sha256;
        private static Random random = new Random();

        public ZaloController(IZaloService zaloService)
        {
            _zaloService = zaloService;
            sha256 = SHA256.Create();
            code_verifier = RandomString();
            code_challenge = Convert.ToBase64String(sha256.ComputeHash(Encoding.ASCII.GetBytes(code_verifier)));
        }

        public static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 43)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // GET: Zalo
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void Zalo(Zalo_NguoiDungNhanTin thongtin)
        {
            try
            {
                string retryHeader = Request.Headers["Number-Retry"];
                log.Info($"Zalo webhook nhan duoc: app_id={thongtin?.app_id}, retryHeader={retryHeader}, sender_id={thongtin?.sender?.id}, event={thongtin?.event_name}, text={thongtin?.message?.text}");
                if (thongtin != null && thongtin.app_id != null && string.IsNullOrEmpty(retryHeader))
                {
                    if (thongtin.message != null && thongtin.sender != null)
                    {
                        var lienKetRequest = new ZaloLienKetRequest
                        {
                            SoDienThoai = thongtin.message.text,
                            MessageId = thongtin.message.msg_id,
                            UserZaloId = thongtin.sender.id
                        };
                        _zaloService.ReplyMessageZalo(lienKetRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi xử lý Zalo webhook: {ex.Message}", ex);
            }
        }

        [HttpPost]
        public ActionResult GuiTinNhan(ZaloTestMessageRequest request)
        {
            var data = _zaloService.ThongBaoQLCV(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}