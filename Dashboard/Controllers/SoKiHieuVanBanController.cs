using Dashboard.Common;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class SoKiHieuVanBanController : Controller
    {
        private readonly ISoKiHieuVanBanService _soKiHieuVanBanService;
        private readonly IHinhThucVanBanService _hinhThucVanBanService;

        public SoKiHieuVanBanController(ISoKiHieuVanBanService soKiHieuVanBanService, IHinhThucVanBanService hinhThucVanBanService)
        {
            _soKiHieuVanBanService = soKiHieuVanBanService;
            _hinhThucVanBanService = hinhThucVanBanService;
        }

        [JwtAuthorize("Public")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("Public")]
        public ActionResult GetAll(SoKiHieuVanBanQueryViewModel queryParams)
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            var data = _soKiHieuVanBanService.GetAll(queryParams, currentUserId > 0 ? (int?)currentUserId : null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("Public")]
        public ActionResult LaySo(int Nam, int HinhThucVanBanId)
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            if (currentUserId == 0)
            {
                return Json(new { Errors = new[] { new { Name = "", Description = "Vui lòng đăng nhập" } } }, JsonRequestBehavior.AllowGet);
            }

            if (Nam <= 0)
            {
                return Json(new { Errors = new[] { new { Name = "", Description = "Vui lòng chọn năm hợp lệ" } } }, JsonRequestBehavior.AllowGet);
            }

            if (HinhThucVanBanId <= 0)
            {
                return Json(new { Errors = new[] { new { Name = "", Description = "Vui lòng chọn hình thức văn bản" } } }, JsonRequestBehavior.AllowGet);
            }

            var data = _soKiHieuVanBanService.LaySo(currentUserId, Nam, HinhThucVanBanId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("Public")]
        public ActionResult Xoa(int Id)
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            if (currentUserId == 0)
            {
                return Json(new { Errors = new[] { new { Name = "", Description = "Vui lòng đăng nhập" } } }, JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra record gần nhất
            var soGanNhat = _soKiHieuVanBanService.LaySoGanNhat();
            if (soGanNhat == null)
            {
                return Json(new { Errors = new[] { new { Name = "", Description = "Không có số nào" } } }, JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra record cần xóa có phải là record gần nhất không
            if (soGanNhat.Id != Id)
            {
                return Json(new { Errors = new[] { new { Name = "", Description = "Chỉ có thể xoá số gần nhất" } } }, JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra record thuộc về user đang đăng nhập
            if (soGanNhat.UserId.GetValueOrDefault() != currentUserId)
            {
                return Json(new { Errors = new[] { new { Name = "", Description = "Bạn không có quyền xoá số này" } } }, JsonRequestBehavior.AllowGet);
            }

            var data = _soKiHieuVanBanService.Xoa(Id, currentUserId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public ActionResult LaySoGanNhat()
        {
            var data = _soKiHieuVanBanService.LaySoGanNhat();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("Public")]
        public ActionResult KhoiTaoSoKiHieuBanDau(int SoKiHieu)
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            if (currentUserId == 0)
            {
                return Json(new { Errors = new[] { new { Name = "", Description = "Vui lòng đăng nhập" } } }, JsonRequestBehavior.AllowGet);
            }

            var isSystemUser = UserClaimHelper.GetIsSystemUser((ClaimsIdentity)User.Identity);
            if (!isSystemUser)
            {
                return Json(new { Errors = new[] { new { Name = "", Description = "Bạn không có quyền thực hiện thao tác này. Chỉ người quản trị mới được phép." } } }, JsonRequestBehavior.AllowGet);
            }

            if (SoKiHieu < 1)
            {
                return Json(new { Errors = new[] { new { Name = "", Description = "Số ký hiệu phải là số nguyên dương." } } }, JsonRequestBehavior.AllowGet);
            }

            var data = _soKiHieuVanBanService.KhoiTaoSoKiHieuBanDau(SoKiHieu, currentUserId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public ActionResult GetHinhThucVanBanLookup()
        {
            var lookupQuery = new LookupQueryViewModel
            {
                isGetAll = false,
                isHideOptionChonValueZero = false
            };
            var data = _hinhThucVanBanService.HinhThucVanBanLookup(lookupQuery);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}

