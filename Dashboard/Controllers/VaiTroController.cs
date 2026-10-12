using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class VaiTroController : Controller
    {
        private readonly IVaiTroService _vaiTroService;

        public VaiTroController(IVaiTroService vaiTroService)
        {
            _vaiTroService = vaiTroService;
        }

        [JwtAuthorize("VaiTro")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("VaiTro")]
        public ActionResult GetListVaiTro(QueryParamViewModel queryParams)
        {
            var data = _vaiTroService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VaiTro")]
        public ActionResult Them(VaiTroViewModel vaiTroViewModel)
        {
            var data = _vaiTroService.Them(vaiTroViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VaiTro")]
        public ActionResult CapNhat(VaiTroViewModel vaiTroViewModel)
        {
            var data = _vaiTroService.CapNhat(vaiTroViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VaiTro")]
        public ActionResult GanVaiTroKhacChoPhongBan(GanVaiTroKhacViewModel viewModel)
        {
            var data = _vaiTroService.GanVaiTroKhacChoPhongBan(viewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VaiTro")]
        public ActionResult Xoa(int Id)
        {
            var data = _vaiTroService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("VaiTro")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _vaiTroService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
