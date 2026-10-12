using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class ChucDanhController : Controller
    {
        private readonly IChucDanhService _chucDanhService;
        public ChucDanhController(IChucDanhService chucDanhService)
        {
            _chucDanhService = chucDanhService;
        }

        [JwtAuthorize("ChucDanh")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("ChucDanh")]
        public ActionResult GetListChucDanh(QueryParamViewModel queryParams)
        {
            var data = _chucDanhService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("ChucDanh")]
        public ActionResult ThemOrCapNhat(ChucDanhViewModel chucDanhViewModel)
        {
            dynamic result;
            if (chucDanhViewModel.Id == 0)
                result = _chucDanhService.Them(chucDanhViewModel);
            else
                result = _chucDanhService.CapNhat(chucDanhViewModel);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("ChucDanh")]
        public ActionResult CapNhatThuTu(CapNhatThuTuChucDanhRequest request)
        {
            var data = _chucDanhService.CapNhatThuTu(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("ChucDanh")]
        public ActionResult Xoa(int Id)
        {
            var data = _chucDanhService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("ChucDanh")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _chucDanhService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("ChucDanh")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("ChucDanh")]
        public JsonResult GetById(int id = 0)
        {
            var data = _chucDanhService.GetById(id);
            return Json(new
            {
                Item = data
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult ChucDanhLookup(int id = 0)
        {
            var data = _chucDanhService.ChucDanhLookup(new LookupQueryViewModel());
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}