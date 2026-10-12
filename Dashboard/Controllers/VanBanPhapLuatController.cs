using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class VanBanPhapLuatController : Controller
    {
        private readonly IVanBanPhapLuatService _service;
        private readonly IHinhThucVanBanService _hinhThucVanBanService;
        private readonly ICoQuanBanHanhService _coQuanBanHanhService;
        private readonly IDoKhanService _doKhanService;

        public VanBanPhapLuatController(IVanBanPhapLuatService service, IHinhThucVanBanService hinhThucVanBanService,
            ICoQuanBanHanhService coQuanBanHanhService, IDoKhanService doKhanService)
        {
            _service = service;
            _hinhThucVanBanService = hinhThucVanBanService;
            _coQuanBanHanhService = coQuanBanHanhService;
            _doKhanService = doKhanService;
        }

        [JwtAuthorize("VanBanPhapLuat")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("Public")]
        public ActionResult GetListVanBanPhapLuat(QueryParamViewModel queryParams)
        {
            var data = _service.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VanBanPhapLuat")]
        public ActionResult ThemOrCapNhat(VanBanPhapLuatViewModel model)
        {
            model.CurrentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            var data = _service.ThemOrCapNhat(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VanBanPhapLuat")]
        public ActionResult Xoa(int Id)
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            var data = _service.Xoa(Id, currentUserId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VanBanPhapLuat")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _service.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("VanBanPhapLuat")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("Public")]
        public ActionResult GetFileDinhKemById(int id = 0)
        {
            var files = _service.GetFileDinhKemById(id);
            return Json(files, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult GetById(int id)
        {
            var data = _service.GetById(id);
            var hinhThucs = _hinhThucVanBanService.HinhThucVanBanLookup(new LookupQueryViewModel { });
            var coquans = _coQuanBanHanhService.CoQuanBanHanhLookup(new LookupQueryViewModel { });
            var doKhans = _doKhanService.DoKhanLookup(new LookupQueryViewModel { });

            return Json(new
            {
                Item = data,
                HinhThucs = hinhThucs,
                CoQuans = coquans,
                DoKhans = doKhans
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult GetChiTietViewById(int id)
        {
            var data = _service.GetById(id);
            if (data != null && data.Id != 0)
            {
                var hinhThucs = _hinhThucVanBanService.HinhThucVanBanLookup(new LookupQueryViewModel { });
                var coquans = _coQuanBanHanhService.CoQuanBanHanhLookup(new LookupQueryViewModel { });
                var doKhans = _doKhanService.DoKhanLookup(new LookupQueryViewModel { });

                data.HinhThucVanBan = hinhThucs.Data.FirstOrDefault(o => o.id == data.HinhThucVanBanId)?.text;
                data.CoQuanBanHanh = coquans.Data.FirstOrDefault(o => o.id == data.CoQuanBanHanhId)?.text;
                data.DoKhan = doKhans.Data.FirstOrDefault(o => o.id == data.DoKhanId)?.text;
            }

            return Json(new
            {
                Item = data,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}


