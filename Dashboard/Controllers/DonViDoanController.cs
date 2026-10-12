using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DonViDoanController : Controller
    {
        private readonly IDonViDoanService _service;
        public DonViDoanController(IDonViDoanService service)
        {
            _service = service;
        }

        [JwtAuthorize("DonViDoan")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("DonViDoan")]
        public ActionResult GetAll(QueryParamViewModel queryParams)
        {
            var data = _service.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DonViDoan")]
        public ActionResult ThemOrCapNhat(DonViDoanViewModel model)
        {
            var data = _service.ThemOrCapNhat(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DonViDoan")]
        public ActionResult Xoa(int Id)
        {
            var data = _service.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DonViDoan")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _service.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DonViDoan")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("DonViDoan")]
        public JsonResult GetById(int id)
        {
            var data = _service.GetById(id);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }
    }
}

