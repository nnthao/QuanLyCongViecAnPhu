using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Linq;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DuAnController : Controller
    {
        private readonly IDuAnService _duAnService;

        public DuAnController(IDuAnService duAnService)
        {
            _duAnService = duAnService;
        }

        [JwtAuthorize("DuAn")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("DuAn")]
        public ActionResult GetListDuAn(DuAnQueryViewModel queryParams)
        {
            var data = _duAnService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DuAn")]
        public ActionResult ThemOrCapNhat(DuAnViewModel duAnViewModel)
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            duAnViewModel.CurrentUserId = currentUserId;
            var data = _duAnService.ThemOrCapNhat(duAnViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DuAn")]
        public ActionResult Xoa(int Id)
        {
            var data = _duAnService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DuAn")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _duAnService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAn")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("DuAn")]
        public JsonResult GetById(int id)
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var users = CacheManagerHelper.GetListUserFromCacheNotDeleted()
                .Where(o => o.Id != currentUserId)
                .Select(o => new { id = o.Id, text = o.Name })
                .ToList();

            DuAnGridVo data = null;
            if (id != 0)
            {
                data = _duAnService.GetById(id);
            }

            return Json(new
            {
                Item = data,
                User = new { Data = users }
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAn")]
        public ActionResult ThongKeThanhVienThamGiaNhieuNhat()
        {
            var data = _duAnService.ThongKeThanhVienThamGiaNhieuNhat();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAn")]
        public ActionResult ThongKeThanhVienMucDoHoTroCaoNhat()
        {
            var data = _duAnService.ThongKeThanhVienMucDoHoTroCaoNhat();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
