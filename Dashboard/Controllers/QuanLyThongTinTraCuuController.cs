using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class QuanLyThongTinTraCuuController : Controller
    {
        private readonly IQuanLyThongTinTraCuuService _quanLyThongTinTraCuuService;
        private readonly IDanhMucTraCuuThongTinService _danhMucTraCuuThongTinService;
        private readonly IVanBanPhapLuatService _vanBanPhapLuatService;
        public QuanLyThongTinTraCuuController(IQuanLyThongTinTraCuuService quanLyThongTinTraCuuService, IDanhMucTraCuuThongTinService danhMucTraCuuThongTinService, IVanBanPhapLuatService vanBanPhapLuatService)
        {
            _quanLyThongTinTraCuuService = quanLyThongTinTraCuuService;
            _danhMucTraCuuThongTinService = danhMucTraCuuThongTinService;
            _vanBanPhapLuatService = vanBanPhapLuatService;
        }

        [JwtAuthorize("QuanLyThongTinTraCuu")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("QuanLyThongTinTraCuu")]
        public ActionResult GetListQuanLyThongTinTraCuu(QuanLyThongTinTraCuuQueryViewModel queryParams)
        {
            var data = _quanLyThongTinTraCuuService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyThongTinTraCuu")]
        public ActionResult ThemOrCapNhat(QuanLyThongTinTraCuuViewModel quanLyThongTinTraCuuViewModel)
        {
            quanLyThongTinTraCuuViewModel.CurrentUserId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _quanLyThongTinTraCuuService.ThemOrCapNhat(quanLyThongTinTraCuuViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyThongTinTraCuu")]
        public ActionResult Xoa(int Id)
        {
            var data = _quanLyThongTinTraCuuService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyThongTinTraCuu")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _quanLyThongTinTraCuuService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyThongTinTraCuu")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("QuanLyThongTinTraCuu")]
        public JsonResult GetById(int id)
        {
            var data = _quanLyThongTinTraCuuService.GetById(id);
            var trangThaiHoatDong = UltiHelper.TrangThaiHoatDongEnum();

            var dmTraCuuThongTin = _danhMucTraCuuThongTinService.DanhMucTraCuuThongTinLookup(new LookupQueryViewModel());
            return Json(new
            {
                Item = data,
                TrangThai = trangThaiHoatDong,
                DanhMucTraCuuThongTin = dmTraCuuThongTin,
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyThongTinTraCuu")]
        public ActionResult LookupDataForSearch()
        {
            var trangThaiHoatDong = UltiHelper.TrangThaiHoatDongEnum();
            var dmTraCuuThongTin = _danhMucTraCuuThongTinService.DanhMucTraCuuThongTinLookup(new LookupQueryViewModel());

            return Json(new
            {
                TrangThai = trangThaiHoatDong,
                DanhMucTraCuuThongTin = dmTraCuuThongTin
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public ActionResult GetListVanBanPhapLuat(int id)
        {
            var data = _quanLyThongTinTraCuuService.GetVanBanLienQuan(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public ActionResult GetFileDinhKemById(int id)
        {
            var data = _quanLyThongTinTraCuuService.GetFileDinhKemById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
