using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class NhomNhanViecController : Controller
    {
        private readonly INhomNhanViecService _nhomNhanViecService;
        private readonly IUserService _userService;
        private readonly IPhongBanService _phongBanService;
        public NhomNhanViecController(INhomNhanViecService nhomNhanViecService, IUserService userService, IPhongBanService phongBanService)
        {
            _nhomNhanViecService = nhomNhanViecService;
            _userService = userService;
            _phongBanService = phongBanService;
        }

        [JwtAuthorize("NhomNhanViec")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("NhomNhanViec")]
        public ActionResult GetListNhomNhanViec(NhomNhanViecQueryViewModel queryParams)
        {
            queryParams.CurrentUserId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _nhomNhanViecService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("NhomNhanViec")]
        public ActionResult ThemOrCapNhat(NhomNhanViecViewModel nhomNhanViecViewModel)
        {
            nhomNhanViecViewModel.NguoiTaoId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _nhomNhanViecService.ThemOrCapNhat(nhomNhanViecViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("NhomNhanViec")]
        public ActionResult Xoa(int Id)
        {
            var data = _nhomNhanViecService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("NhomNhanViec")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _nhomNhanViecService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("NhomNhanViec")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("NhomNhanViec")]
        public JsonResult GetById(int id)
        {
            var data = _nhomNhanViecService.GetById(id);

            return Json(new
            {
                Item = data,
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public ActionResult GetListNhomNhanViecDeChon()
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _nhomNhanViecService.GetListNhomNhanViecDeChon(currentUserId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}

