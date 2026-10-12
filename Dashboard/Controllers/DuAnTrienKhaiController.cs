using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Linq;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DuAnTrienKhaiController : Controller
    {
        private readonly IDuAnTrienKhaiService _service;
        private readonly IDuAnService _duAnService;
        private readonly IDonViService _donViService;

        public DuAnTrienKhaiController(IDuAnTrienKhaiService service, IDuAnService duAnService, IDonViService donViService)
        {
            _service = service;
            _duAnService = duAnService;
            _donViService = donViService;
        }

        [JwtAuthorize("DuAnTrienKhai")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("DuAnTrienKhai")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("DuAnTrienKhai")]
        public ActionResult GetAll(DuAnTrienKhaiQueryViewModel queryParams)
        {
            var data = _service.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnTrienKhai")]
        public JsonResult GetById(int id = 0)
        {
            DuAnTrienKhaiGridVo data = null;
            if (id > 0)
            {
                data = _service.GetById(id);
            }

            var duAns = _duAnService.GetListAllDuAnActive()
                .Select(x => new { id = x.Id, text = x.Ten })
                .ToList();

            var donVis = _donViService.Lookup(new LookupQueryViewModel()).Data;

            return Json(new
            {
                Item = data,
                DuAn = new { Data = duAns },
                DonVi = new { Data = donVis }
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnTrienKhai")]
        public JsonResult GetHangMucMau(int duAnId = 0)
        {
            var data = _service.GetHangMucMau(duAnId);
            return Json(new { Data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DuAnTrienKhai")]
        public ActionResult ThemOrCapNhat(DuAnTrienKhaiViewModel model)
        {
            var data = _service.ThemOrCapNhat(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DuAnTrienKhai")]
        public ActionResult Xoa(int id)
        {
            var data = _service.Xoa(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DuAnTrienKhai")]
        public ActionResult KhoiPhuc(int id)
        {
            var data = _service.KhoiPhuc(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
