using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DonViController : Controller
    {
        private readonly IDonViService _service;
        public DonViController(IDonViService service)
        {
            _service = service;
        }

        [JwtAuthorize("DuAn")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("DuAn")]
        public ActionResult GetAll(QueryParamViewModel queryParams)
        {
            var data = _service.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DuAn")]
        public ActionResult ThemOrCapNhat(DonViViewModel model)
        {
            var data = _service.ThemOrCapNhat(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DuAn")]
        public ActionResult Xoa(int id)
        {
            var data = _service.Xoa(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DuAn")]
        public ActionResult KhoiPhuc(int id)
        {
            var data = _service.KhoiPhuc(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAn")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("DuAn")]
        public JsonResult GetById(int id)
        {
            var data = _service.GetById(id);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAn")]
        public JsonResult Lookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var data = _service.Lookup(lookupQueryViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
