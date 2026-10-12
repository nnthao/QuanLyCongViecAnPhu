using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DanhMucBaoCaoController : Controller
    {
        private readonly IDanhMucBaoCaoService _service;
        public DanhMucBaoCaoController(IDanhMucBaoCaoService service)
        {
            _service = service;
        }

        [JwtAuthorize("DanhMucBaoCao")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("DanhMucBaoCao")]
        public ActionResult GetListDanhMucBaoCao(QueryParamViewModel queryParams)
        {
            var data = _service.GetAll(queryParams);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DanhMucBaoCao")]
        public ActionResult ThemOrCapNhat(DanhMucBaoCaoViewModel danhMucBaoCaoViewModel)
        {
            var data = _service.ThemOrCapNhat(danhMucBaoCaoViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DanhMucBaoCao")]
        public ActionResult Xoa(int Id)
        {
            var data = _service.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DanhMucBaoCao")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _service.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DanhMucBaoCao")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("DanhMucBaoCao")]
        public JsonResult GetById(int id)
        {
            var data = _service.GetById(id);

            return Json(new
            {
                Item = data,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}


