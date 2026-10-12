using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class LinhVucSangKienController : Controller
    {
        private readonly ILinhVucSangKienService _linhVucSangKienService;
        public LinhVucSangKienController(ILinhVucSangKienService linhVucSangKienService)
        {
            _linhVucSangKienService = linhVucSangKienService;
        }

        [JwtAuthorize("LinhVucSangKien")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("LinhVucSangKien")]
        public ActionResult GetListLinhVucSangKien(QueryParamViewModel queryParams)
        {
            var data = _linhVucSangKienService.GetAll(queryParams);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("LinhVucSangKien")]
        public ActionResult ThemOrCapNhat(LinhVucSangKienViewModel LinhVucSangKienViewModel)
        {
            var data = _linhVucSangKienService.ThemOrCapNhat(LinhVucSangKienViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("LinhVucSangKien")]
        public ActionResult Xoa(int Id)
        {
            var data = _linhVucSangKienService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("LinhVucSangKien")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _linhVucSangKienService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("LinhVucSangKien")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("LinhVucSangKien")]
        public JsonResult GetById(int id)
        {
            var data = _linhVucSangKienService.GetById(id);

            return Json(new
            {
                Item = data
            }, JsonRequestBehavior.AllowGet);
        }
    }
}

