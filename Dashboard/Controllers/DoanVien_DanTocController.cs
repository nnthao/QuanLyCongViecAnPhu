using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DoanVien_DanTocController : Controller
    {
        private readonly IDoanVien_DanTocService _service;
        public DoanVien_DanTocController(IDoanVien_DanTocService service)
        {
            _service = service;
        }

        [JwtAuthorize("DoanVien_DanToc")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("DoanVien_DanToc")]
        public ActionResult GetAll(QueryParamViewModel queryParams)
        {
            var data = _service.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoanVien_DanToc")]
        public ActionResult ThemOrCapNhat(DoanVien_DanTocViewModel model)
        {
            var data = _service.ThemOrCapNhat(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoanVien_DanToc")]
        public ActionResult Xoa(int Id)
        {
            var data = _service.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoanVien_DanToc")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _service.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DoanVien_DanToc")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("DoanVien_DanToc")]
        public JsonResult GetById(int id)
        {
            var data = _service.GetById(id);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }
    }
}

