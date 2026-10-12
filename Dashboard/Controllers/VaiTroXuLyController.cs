using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class VaiTroXuLyController : Controller
    {
        private readonly IVaiTroXuLyService _service;

        public VaiTroXuLyController(IVaiTroXuLyService service)
        {
            _service = service;
        }

        [JwtAuthorize("VaiTroXuLy")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("VaiTroXuLy")]
        public ActionResult GetListVaiTroXuLy(QueryParamViewModel queryParams)
        {
            var data = _service.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VaiTroXuLy")]
        public ActionResult ThemOrCapNhat(VaiTroXuLyViewModel model)
        {
            var data = _service.ThemOrCapNhat(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VaiTroXuLy")]
        public ActionResult CapNhatThuTu(CapNhatThuTuVaiTroXuLyRequest request)
        {
            var data = _service.CapNhatThuTu(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VaiTroXuLy")]
        public ActionResult CapNhatIsThongKeGiaoViec(CapNhatIsThongKeGiaoViecRequest request)
        {
            var data = _service.CapNhatIsThongKeGiaoViec(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VaiTroXuLy")]
        public ActionResult CapNhatIsCanDinhKemFileKhiXacNhanHoanThanh(CapNhatIsCanDinhKemFileKhiXacNhanHoanThanhRequest request)
        {
            var data = _service.CapNhatIsCanDinhKemFileKhiXacNhanHoanThanh(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VaiTroXuLy")]
        public ActionResult Xoa(int Id)
        {
            var data = _service.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VaiTroXuLy")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _service.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("VaiTroXuLy")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("VaiTroXuLy")]
        public JsonResult GetById(int id = 0)
        {
            var data = _service.GetById(id);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult VaiTroXuLyLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            if (lookupQueryViewModel == null)
            {
                lookupQueryViewModel = new LookupQueryViewModel();
            }
            var data = _service.VaiTroXuLyLookup(lookupQueryViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
