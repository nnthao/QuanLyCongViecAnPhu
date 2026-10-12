using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class PhongBanController : Controller
    {
        private readonly IPhongBanService _phongBanService;
        public PhongBanController(IPhongBanService phongBanService)
        {
            _phongBanService = phongBanService;
        }

        [JwtAuthorize("PhongBan")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("PhongBan")]
        public ActionResult GetListPhongBan(QueryParamViewModel queryParams)
        {
            var data = _phongBanService.GetAll(queryParams);
            var phongBan = _phongBanService.PhongBanLookup(new LookupQueryViewModel());

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("PhongBan")]
        public ActionResult ThemOrCapNhat(PhongBanViewModel PhongBanViewModel)
        {
            var data = _phongBanService.ThemOrCapNhat(PhongBanViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("PhongBan")]
        public ActionResult CapNhatThuTu(CapNhatThuTuPhongBanRequest request)
        {
            var data = _phongBanService.CapNhatThuTu(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("PhongBan")]
        public ActionResult CapNhatIsPhongNhanViec(CapNhatIsPhongNhanViecRequest request)
        {
            var data = _phongBanService.CapNhatIsPhongNhanViec(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("PhongBan")]
        public ActionResult Xoa(int Id)
        {
            var data = _phongBanService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("PhongBan")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _phongBanService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PhongBanLookup()
        {
            var data = _phongBanService.PhongBanLookup(new LookupQueryViewModel());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("PhongBan")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("PhongBan")]
        public JsonResult GetById(int id)
        {
            var data = _phongBanService.GetById(id);

            var phongBan = _phongBanService.PhongBanLookup(new LookupQueryViewModel
            {
                phongBanLoaiTruId = data.Id
            });

            var users = _phongBanService.UserLookup(new LookupQueryViewModel());

            return Json(new
            {
                Item = data,
                PhongBans = phongBan,
                Users = users
            }, JsonRequestBehavior.AllowGet);
        }
    }
}