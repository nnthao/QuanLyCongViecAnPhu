using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class TraCuuThongTinController : Controller
    {
        private readonly IQuanLyThongTinTraCuuService _quanLyThongTinTraCuuService;
        private readonly IDanhMucTraCuuThongTinService _danhMucTraCuuThongTinService;
        private readonly IVanBanPhapLuatService _vanBanPhapLuatService;

        public TraCuuThongTinController(IQuanLyThongTinTraCuuService quanLyThongTinTraCuuService, IDanhMucTraCuuThongTinService danhMucTraCuuThongTinService, IVanBanPhapLuatService vanBanPhapLuatService)
        {
            _quanLyThongTinTraCuuService = quanLyThongTinTraCuuService;
            _danhMucTraCuuThongTinService = danhMucTraCuuThongTinService;
            _vanBanPhapLuatService = vanBanPhapLuatService;
        }

        [JwtAuthorize("TraCuuThongTin")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("TraCuuThongTin")]
        public ActionResult GetListTraCuuThongTin(QuanLyThongTinTraCuuQueryViewModel queryParams)
        {
            var data = _quanLyThongTinTraCuuService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("TraCuuThongTin")]
        public ActionResult ChiTiet(int id)
        {
            return View(id);
        }

        [JwtAuthorize("TraCuuThongTin")]
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

        [JwtAuthorize("TraCuuThongTin")]
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

        [JwtAuthorize("TraCuuThongTin")]
        public ActionResult GetListVanBanPhapLuat(int id)
        {
            var data = _quanLyThongTinTraCuuService.GetVanBanLienQuan(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("TraCuuThongTin")]
        public ActionResult GetFileDinhKemById(int id)
        {
            var data = _quanLyThongTinTraCuuService.GetFileDinhKemById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}

