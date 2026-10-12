using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DoKhanController : Controller
    {
        private readonly IDoKhanService _service;
        public DoKhanController(IDoKhanService service)
        {
            _service = service;
        }

        [JwtAuthorize("DoKhan")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("DoKhan")]
        public ActionResult GetListDoKhan(QueryParamViewModel queryParams)
        {
            var data = _service.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoKhan")]
        public ActionResult ThemOrCapNhat(DoKhanViewModel model)
        {
            var data = _service.ThemOrCapNhat(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoKhan")]
        public ActionResult Xoa(int Id)
        {
            var data = _service.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoKhan")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _service.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DoKhan")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("DoKhan")]
        public JsonResult GetById(int id)
        {
            var data = _service.GetById(id);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }
    }
}


