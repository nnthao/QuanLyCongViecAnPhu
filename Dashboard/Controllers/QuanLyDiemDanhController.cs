using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class QuanLyDiemDanhController : Controller
    {
        private readonly IDiemDanhService _diemDanhService;
        private readonly IUserService _userService;
        private readonly ICuocHopService _cuocHopService;

        public QuanLyDiemDanhController(IDiemDanhService diemDanhService, IUserService userService, ICuocHopService cuocHopService)
        {
            _diemDanhService = diemDanhService;
            _userService = userService;
            _cuocHopService = cuocHopService;
        }

        [JwtAuthorize("QuanLyDiemDanh")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("QuanLyDiemDanh")]
        public ActionResult Form(int? id = 0)
        {
            ViewBag.Id = id ?? 0;
            return View(id ?? 0);
        }

        [JwtAuthorize("QuanLyDiemDanh")]
        public ActionResult GetDataForForm(int id = 0)
        {
            var users = _userService.UserLookup(new LookupQueryViewModel { });
            var item = id > 0 ? _diemDanhService.GetById(id) : null;

            return Json(new
            {
                Users = users,
                Item = item
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyDiemDanh")]
        public ActionResult GetList(DiemDanhQueryViewModel search)
        {
            if (search == null) search = new DiemDanhQueryViewModel();
            var data = _diemDanhService.GetAll(search);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyDiemDanh")]
        public ActionResult GetById(int id = 0)
        {
            var data = _diemDanhService.GetById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyDiemDanh")]
        public ActionResult ThemOrCapNhat(DiemDanhViewModel model)
        {
            try
            {

                var result = _diemDanhService.ThemOrCapNhat(model);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        [JwtAuthorize("QuanLyDiemDanh")]
        public ActionResult Xoa(int id)
        {
            var result = _diemDanhService.Xoa(id);
            return Json(new { status = result, message = result ? "Xóa thành công!" : "Lỗi khi xóa!" });
        }

        [JwtAuthorize("QuanLyDiemDanh")]
        public ActionResult ThongKe()
        {
            return View();
        }

        [JwtAuthorize("QuanLyDiemDanh")]
        public ActionResult GetBaoCao(DiemDanhSearchViewModel search)
        {
            var data = _diemDanhService.GetBaoCao(search);
            return Json(new DataTableResult<DiemDanhReportViewModel> { rows = data, total = data.Count }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyDiemDanh")]
        public ActionResult GetBaoCaoSummary(DiemDanhSearchViewModel search)
        {
            var summary = _diemDanhService.GetBaoCaoSummary(search);
            return Json(summary, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyDiemDanh")]
        public ActionResult GetChiTietNgayNghi(DiemDanhSearchViewModel search)
        {
            var data = _diemDanhService.GetChiTietNgayNghi(search);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyDiemDanh")]
        public ActionResult TaoLichHopTuDiemDanh(TaoLichHopTuDiemDanhRequest request)
        {
            try
            {
                request.CurrentUserId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
                var result = _diemDanhService.TaoLichHopTuDiemDanh(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "Error",
                            Description = ex.Message
                        }
                    }
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
