using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class GiaoViecDanhGiaController : Controller
    {
        private readonly IGiaoViecDanhGiaService _service;

        public GiaoViecDanhGiaController(IGiaoViecDanhGiaService service)
        {
            _service = service;
        }

        [JwtAuthorize("GiaoViecDanhGia")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("GiaoViecDanhGia")]
        public ActionResult GetListDanhGia(GiaoViecDanhGiaQueryViewModel queryParams)
        {
            var identity = (ClaimsIdentity)User.Identity;
            queryParams.CurrentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var data = _service.GetListDanhGia(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("GiaoViecDanhGia")]
        public JsonResult GetChiTietDanhGia(int giaoViecThanhVienId)
        {
            var data = _service.GetChiTietDanhGia(giaoViecThanhVienId);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("GiaoViecDanhGia")]
        public ActionResult LuuDanhGia(LuuDanhGiaViewModel viewModel)
        {
            var identity = (ClaimsIdentity)User.Identity;
            viewModel.CurrentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var result = _service.LuuDanhGia(viewModel);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult DemSoLuongChoDanhGia()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var count = _service.DemSoLuongCongViecChoDanhGia(currentUserId);
            return Json(new { count = count }, JsonRequestBehavior.AllowGet);
        }
    }
}

