using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DuAnHangMucController : Controller
    {
        private readonly IDuAnHangMucService _duAnHangMucService;

        public DuAnHangMucController(IDuAnHangMucService duAnHangMucService)
        {
            _duAnHangMucService = duAnHangMucService;
        }

        [JwtAuthorize("DuAn")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("DuAn")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("DuAn")]
        public ActionResult GetListDuAnHangMuc(QueryParamViewModel queryParams)
        {
            var data = _duAnHangMucService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAn")]
        public JsonResult GetById(int id = 0)
        {
            var data = _duAnHangMucService.GetById(id);
            return Json(new
            {
                Item = data
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DuAn")]
        public ActionResult ThemOrCapNhat(DuAnHangMucInfo duAnHangMucViewModel)
        {
            dynamic result;
            if (duAnHangMucViewModel.Id.GetValueOrDefault() == 0)
                result = _duAnHangMucService.Them(duAnHangMucViewModel);
            else
                result = _duAnHangMucService.CapNhat(duAnHangMucViewModel);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DuAn")]
        public ActionResult Xoa(int Id)
        {
            var data = _duAnHangMucService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}