using Dashboard.Common;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Linq;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class GiaoViecDaChuyenGiaoController : Controller
    {
        private readonly IGiaoViecService _service;

        public GiaoViecDaChuyenGiaoController(IGiaoViecService service)
        {
            _service = service;
        }

        [JwtAuthorize("GiaoViecDaChuyenGiao")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("GiaoViecDaChuyenGiao")]
        public ActionResult GetListCongViecDaChuyenGiao(GiaoViecQueryViewModel queryParams)
        {
            queryParams.NguoiDuocGiaoId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _service.GetListCongViecDaChuyenGiao(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("GiaoViecDaChuyenGiao")]
        public JsonResult GetNguoiNhanChuyenGiaoLookup()
        {
            var currentPhongBanId = UserClaimHelper.GetPhongBanId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var phongBanIds = new System.Collections.Generic.List<int>();
            if (currentPhongBanId > 0)
            {
                phongBanIds.Add(currentPhongBanId);
                phongBanIds.AddRange(UltiHelper.GetPhongBanChildrenIds(currentPhongBanId));
            }

            var users = CacheManagerHelper.GetListUserFromCacheNotDeleted()
                .Where(o => !o.PhongBanId.HasValue || currentPhongBanId == 0 || phongBanIds.Contains(o.PhongBanId.Value))
                .Select(o => new
                {
                    id = o.Id,
                    text = o.Name
                })
                .OrderBy(o => o.text)
                .ToList();

            users.Insert(0, new
            {
                id = 0,
                text = "---Chọn---"
            });

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("GiaoViecDaChuyenGiao")]
        public ActionResult GuiNhacNho(GuiNhacNhoViewModel viewModel)
        {
            var data = _service.GuiNhacNho(viewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("GiaoViecDaChuyenGiao")]
        public ActionResult LayLaiCongViecChuyenGiao(int giaoViecThanhVienId)
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _service.LayLaiCongViecChuyenGiao(giaoViecThanhVienId, currentUserId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
