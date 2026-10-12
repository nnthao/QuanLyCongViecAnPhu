using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class CoQuanBanHanhController : Controller
    {
        private readonly ICoQuanBanHanhService _service;
        public CoQuanBanHanhController(ICoQuanBanHanhService service)
        {
            _service = service;
        }

        [JwtAuthorize("CoQuanBanHanh")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("CoQuanBanHanh")]
        public ActionResult GetListCoQuanBanHanh(QueryParamViewModel queryParams)
        {
            var data = _service.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("CoQuanBanHanh")]
        public ActionResult ThemOrCapNhat(CoQuanBanHanhViewModel model)
        {
            var data = _service.ThemOrCapNhat(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("CoQuanBanHanh")]
        public ActionResult Xoa(int Id)
        {
            var data = _service.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("CoQuanBanHanh")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _service.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("CoQuanBanHanh")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("CoQuanBanHanh")]
        public JsonResult GetById(int id)
        {
            var data = _service.GetById(id);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }
    }
}


