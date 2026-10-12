using Dashboard.Common;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class GiaoViecTuGiaoCanXuLyController : Controller
    {
        private readonly IGiaoViecService _service;
        public GiaoViecTuGiaoCanXuLyController(IGiaoViecService service)
        {
            _service = service;
        }

        [JwtAuthorize("GiaoViecTuGiaoCanXuLy")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("GiaoViecTuGiaoCanXuLy")]
        public ActionResult GetListCongViecTuGiaoCanXuLy(GiaoViecQueryViewModel queryParams)
        {
            queryParams.NguoiDuocGiaoId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _service.GetListCongViecTuGiaoCanXuLy(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("GiaoViecTuGiaoCanXuLy")]
        public ActionResult Xoa(int giaoViecId)
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _service.XoaCongViecTuGiao(giaoViecId, currentUserId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}

