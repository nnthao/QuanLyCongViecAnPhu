using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DanhSachCongTrinhController : Controller
    {
        private readonly IDanhSachCongTrinhService _danhSachCongTrinhService;

        public DanhSachCongTrinhController(IDanhSachCongTrinhService danhSachCongTrinhService)
        {
            _danhSachCongTrinhService = danhSachCongTrinhService;
        }

        [JwtAuthorize("DanhSachCongTrinh")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetListDanhSachCongTrinh(QueryParamViewModel queryParams)
        {
            var data = _danhSachCongTrinhService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DanhSachCongTrinh")]
        public ActionResult ThemOrCapNhat(DanhSachCongTrinhViewModel danhSachCongTrinhViewModel)
        {
            var data = _danhSachCongTrinhService.ThemOrCapNhat(danhSachCongTrinhViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DanhSachCongTrinh")]
        public ActionResult Xoa(int Id)
        {
            var data = _danhSachCongTrinhService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DanhSachCongTrinh")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _danhSachCongTrinhService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DanhSachCongTrinh")]
        public ActionResult Form(int id = 0)
        {
            ViewBag.Id = id;
            return View();
        }

        public JsonResult GetById(int id)
        {
            var data = _danhSachCongTrinhService.GetById(id);
            return Json(new
            {
                Item = data,
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult DanhSachCongTrinhLookup()
        {
            var data = _danhSachCongTrinhService.DanhSachCongTrinhLookup(new LookupQueryViewModel());
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
