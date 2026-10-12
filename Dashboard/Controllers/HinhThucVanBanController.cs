using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class HinhThucVanBanController : Controller
    {
        private readonly IHinhThucVanBanService _service;
        public HinhThucVanBanController(IHinhThucVanBanService service)
        {
            _service = service;
        }

        [JwtAuthorize("HinhThucVanBan")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("HinhThucVanBan")]
        public ActionResult GetListHinhThucVanBan(QueryParamViewModel queryParams)
        {
            var data = _service.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("HinhThucVanBan")]
        public ActionResult ThemOrCapNhat(HinhThucVanBanViewModel model)
        {
            var data = _service.ThemOrCapNhat(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("HinhThucVanBan")]
        public ActionResult Xoa(int Id)
        {
            var data = _service.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("HinhThucVanBan")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _service.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("HinhThucVanBan")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("HinhThucVanBan")]
        public JsonResult GetById(int id)
        {
            var data = _service.GetById(id);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }
    }
}


