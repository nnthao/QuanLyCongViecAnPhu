using System.Web.Mvc;
using Dashboard.Common;
using Dashboard.Services;

namespace Dashboard.Controllers
{
    public class ThongKeBaoCaoController : Controller
    {
        private readonly ISangKienCNTTService _sangKienCNTTService;

        public ThongKeBaoCaoController(ISangKienCNTTService sangKienCNTTService)
        {
            _sangKienCNTTService = sangKienCNTTService;
        }

        [JwtAuthorize("SangKien")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("SangKien")]
        public JsonResult GetThongKeTongSoSangKien()
        {
            var data = _sangKienCNTTService.GetThongKeTongSoSangKien();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("SangKien")]
        public JsonResult GetThongKeXepLoai()
        {
            var data = _sangKienCNTTService.GetThongKeXepLoai();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("SangKien")]
        public JsonResult GetThongKeQuyTienThuong()
        {
            var data = _sangKienCNTTService.GetThongKeQuyTienThuong();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("SangKien")]
        public JsonResult GetThongKeTongTienThuongThanhVien()
        {
            var data = _sangKienCNTTService.GetThongKeTongTienThuongThanhVien();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}


