using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Linq;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class LyDoController : Controller
    {
        private readonly ILyDoService _lyDoService;
        public LyDoController(ILyDoService lyDoService)
        {
            _lyDoService = lyDoService;
        }

        [JwtAuthorize("LyDo")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("LyDo")]
        public ActionResult GetListLyDo(QueryParamViewModel queryParams)
        {
            var data = _lyDoService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("LyDo")]
        public ActionResult ThemOrCapNhat(LyDoViewModel lyDoViewModel)
        {
            dynamic result;
            if (lyDoViewModel.Id == 0)
                result = _lyDoService.Them(lyDoViewModel);
            else
                result = _lyDoService.CapNhat(lyDoViewModel);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("LyDo")]
        public ActionResult Xoa(int Id)
        {
            var data = _lyDoService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("LyDo")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _lyDoService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("LyDo")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("LyDo")]
        public JsonResult GetById(int id = 0)
        {
            var data = _lyDoService.GetById(id);
            return Json(new
            {
                Item = data
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult LyDoLookup(int id = 0)
        {
            var data = _lyDoService.LyDoLookup(new LookupQueryViewModel());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult GetListLyDoWithColor()
        {
            var data = _lyDoService.GetAll(new QueryParamViewModel { limit = 1000, offset = 0 });
            var lyDos = data.rows.Where(x => !x.IsDeleted.GetValueOrDefault()).Select(x => new
            {
                Id = x.Id,
                Ten = x.Ten,
                MaMau = x.MaMau
            }).ToList();
            return Json(lyDos, JsonRequestBehavior.AllowGet);
        }
    }
}