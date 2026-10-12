using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DotXetDuyetController : Controller
    {
        private readonly IDotXetDuyetService _dotXetDuyetService;
        private readonly ISangKienPhanThuongService _sangKienPhanThuongService;

        public DotXetDuyetController(IDotXetDuyetService dotXetDuyetService,
            ISangKienPhanThuongService sangKienPhanThuongService)
        {
            _dotXetDuyetService = dotXetDuyetService;
            _sangKienPhanThuongService = sangKienPhanThuongService;
        }

        [JwtAuthorize("DotXetDuyet")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("DotXetDuyet")]
        public ActionResult GetListDotXetDuyet(QueryParamViewModel queryParams)
        {
            var data = _dotXetDuyetService.GetAll(queryParams);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DotXetDuyet")]
        public ActionResult ThemOrCapNhat(DotXetDuyetViewModel DotXetDuyetViewModel)
        {
            var data = _dotXetDuyetService.ThemOrCapNhat(DotXetDuyetViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DotXetDuyet")]
        public ActionResult Xoa(int Id)
        {
            var data = _dotXetDuyetService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DotXetDuyet")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _dotXetDuyetService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DotXetDuyet")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("DotXetDuyet")]
        public JsonResult GetById(int id)
        {
            var data = _dotXetDuyetService.GetById(id);

            return Json(new
            {
                Item = data,
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult DotXetDuyetLookup(DateTime? tuNgay, DateTime? denNgay)
        {
            var lookupQuery = new LookupQueryViewModel
            {
                TuNgayFilter = tuNgay,
                DenNgayFilter = denNgay
            };
            var data = _dotXetDuyetService.DotXetDuyetLookup(lookupQuery);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DotXetDuyet")]
        public JsonResult GetPhanThuongByDotXetDuyet(int dotXetDuyetId)
        {
            var data = _sangKienPhanThuongService.GetByDotXetDuyet(dotXetDuyetId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DotXetDuyet")]
        public JsonResult LuuDanhSachPhanThuong(int dotXetDuyetId, List<SangKienPhanThuongViewModel> danhSach)
        {
            if (danhSach == null)
            {
                danhSach = new List<SangKienPhanThuongViewModel>();
            }

            var identity = (ClaimsIdentity)User.Identity;
            var userId = UserClaimHelper.GetCurrentUserId(identity);
            var result = _sangKienPhanThuongService.LuuDanhSachPhanThuong(dotXetDuyetId, danhSach, userId);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }
    }
}

