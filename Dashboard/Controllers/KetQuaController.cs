using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class KetQuaController : Controller
    {
        private readonly ISangKienService _sangKienService;
        private readonly IChamSangKienService _chamSangKienService;
        private readonly IPhanCongChamService _phanCongChamService;
        public KetQuaController(ISangKienService sangKienService, IChamSangKienService chamSangKienService, IPhanCongChamService phanCongChamService)
        {
            _sangKienService = sangKienService;
            _chamSangKienService = chamSangKienService;
            _phanCongChamService = phanCongChamService;
        }

        [JwtAuthorize("KetQua")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("Public")]
        public ActionResult ChamDiem(int id = 0)
        {
            if (id == 0)
            {
                return Redirect("/Error");
            }

            var userId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser((ClaimsIdentity)User.Identity);

            if (!isSystemUser)
            {
                var phanCongCham = _phanCongChamService.GetBySangKien(id);
                var isPhanCong = phanCongCham != null && phanCongCham.Any(o => o.UserId == userId);
                if (!isPhanCong)
                {
                    return Redirect("/Error");
                }
            }

            return View(id);
        }

        [JwtAuthorize("KetQua")]
        public ActionResult GetListKetQua(QueryParamViewModel queryParams)
        {
            var data = _sangKienService.GetKetQua(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult GetChamDiemBySangKien(int sangKienId)
        {
            var userId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser((ClaimsIdentity)User.Identity);

            if (!isSystemUser)
            {
                var phanCongCham = _phanCongChamService.GetBySangKien(sangKienId);
                var isPhanCong = phanCongCham != null && phanCongCham.Any(o => o.UserId == userId);
                if (!isPhanCong)
                {
                    return Json(new { Error = "Bạn không có quyền chấm sáng kiến này." }, JsonRequestBehavior.AllowGet);
                }
            }

            var data = _chamSangKienService.GetBySangKien(sangKienId, userId);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("Public")]
        public JsonResult LuuChamDiem(ChamSangKienViewModel viewModel)
        {
            var userId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser((ClaimsIdentity)User.Identity);

            if (!isSystemUser)
            {
                var phanCongCham = _phanCongChamService.GetBySangKien(viewModel.SangKienId);
                var isPhanCong = phanCongCham != null && phanCongCham.Any(o => o.UserId == userId);
                if (!isPhanCong)
                {
                    return Json(new { Success = false, Error = "Bạn không có quyền chấm sáng kiến này." }, JsonRequestBehavior.AllowGet);
                }
            }

            viewModel.UserId = userId;
            var tongDiem = _chamSangKienService.LuuChamDiem(viewModel);
            _sangKienService.CapNhatTongDiemVaTrangThai(viewModel.SangKienId, tongDiem);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}

