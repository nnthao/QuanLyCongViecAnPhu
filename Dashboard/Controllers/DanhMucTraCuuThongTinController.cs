using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DanhMucTraCuuThongTinController : Controller
    {
        private readonly IDanhMucTraCuuThongTinService _danhMucTraCuuThongTinService;
        public DanhMucTraCuuThongTinController(IDanhMucTraCuuThongTinService danhMucTraCuuThongTinService)
        {
            _danhMucTraCuuThongTinService = danhMucTraCuuThongTinService;
        }

        [JwtAuthorize("DanhMucTraCuuThongTin")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("DanhMucTraCuuThongTin")]
        public ActionResult GetListDanhMucTraCuuThongTin(QueryParamViewModel queryParams)
        {
            var data = _danhMucTraCuuThongTinService.GetAll(queryParams);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DanhMucTraCuuThongTin")]
        public ActionResult ThemOrCapNhat(DanhMucTraCuuThongTinViewModel DanhMucTraCuuThongTinViewModel)
        {
            var data = _danhMucTraCuuThongTinService.ThemOrCapNhat(DanhMucTraCuuThongTinViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DanhMucTraCuuThongTin")]
        public ActionResult Xoa(int Id)
        {
            var data = _danhMucTraCuuThongTinService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DanhMucTraCuuThongTin")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _danhMucTraCuuThongTinService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DanhMucTraCuuThongTin")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("DanhMucTraCuuThongTin")]
        public JsonResult GetById(int id)
        {
            var data = _danhMucTraCuuThongTinService.GetById(id);

            return Json(new
            {
                Item = data,
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult DanhMucTraCuuThongTinLookup()
        {
            var data = _danhMucTraCuuThongTinService.DanhMucTraCuuThongTinLookup(new LookupQueryViewModel());

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}