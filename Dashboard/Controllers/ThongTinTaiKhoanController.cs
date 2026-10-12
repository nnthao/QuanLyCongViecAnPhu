using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Services;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class ThongTinTaiKhoanController : Controller
    {
        private readonly IUserService _userService;
        public ThongTinTaiKhoanController(IUserService userService)
        {
            _userService = userService;
        }

        [JwtAuthorize("Public")]
        public ActionResult Index()
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            var user = _userService.GetUserByIdFromCahe(currentUserId);
            return View(user);
        }

        [JwtAuthorize("Public")]
        public ActionResult DoiMatKhau()
        {
            return View();
        }

        [HttpPost]
        [JwtAuthorize("Public")]
        public ActionResult CapNhatMatKhau(UserViewModel userViewModel)
        {
            userViewModel.Id = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);

            return Json(_userService.CapNhatMatKhau(userViewModel), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [JwtAuthorize("Public")]
        public ActionResult CapNhatTelegramId(UserViewModel userViewModel)
        {
            userViewModel.Id = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);

            return Json(_userService.CapNhatTelegramId(userViewModel), JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public ActionResult GetHeaderInfo()
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            var user = _userService.GetUserByIdFromCahe(currentUserId);
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public ActionResult DemSoLuongMenu()
        {
            ////ql dat xe
            //var queryQldx = new DieuHanhXeQueryParamViewModel();
            //queryQldx.UserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);

            ////dang ly
            //queryQldx.TrangThaiXuLy = new List<string> { BienDungChung.STATUS_CHODUYET, BienDungChung.STATUS_TRADUYET, BienDungChung.STATUS_TRAPHIEUDADUYET };
            //var quanLyDatXeChoXuLy = _dieuHanhXeRepository.GetAll(queryQldx);

            ////da duyet
            //queryQldx.TrangThaiXuLy = new List<string> { BienDungChung.STATUS_DADUYET };
            //var quanLyDatXeDaDuyet = _dieuHanhXeRepository.GetAll(queryQldx);

            ////dang chạy
            //queryQldx.TrangThaiXuLy = new List<string> { BienDungChung.STATUS_DANGCHAY };
            //var quanLyDatXeDangChay = _dieuHanhXeRepository.GetAll(queryQldx);

            ////ket thuc
            //queryQldx.TrangThaiXuLy = new List<string> { BienDungChung.STATUS_DADONG };
            //var quanLyDatXeKetThuc = _dieuHanhXeRepository.GetAll(queryQldx);

            ////Đã huỷ
            //queryQldx.TrangThaiXuLy = new List<string> { BienDungChung.STATUS_HUYBO };
            //var quanLyDatXeDaHuy = _dieuHanhXeRepository.GetAll(queryQldx);

            ////ql dieu hanh xe
            //var queryDieuHanhXe = new DieuHanhXeQueryParamViewModel();
            ////cho duyet
            //queryDieuHanhXe.TrangThaiXuLy = new List<string> { BienDungChung.STATUS_CHODUYET };
            //queryDieuHanhXe.NguoiDuyetId = 0;
            //var dieuHanhXeChoToiDuyet = _dieuHanhXeRepository.GetAll(queryDieuHanhXe);

            ////da duyet
            //queryDieuHanhXe.TrangThaiXuLy = new List<string> { BienDungChung.STATUS_DADUYET };
            //queryDieuHanhXe.NguoiDuyetId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            //var dieuHanhXeDaDuyet = _dieuHanhXeRepository.GetAll(queryDieuHanhXe);

            ////dang chay
            //queryDieuHanhXe.TrangThaiXuLy = new List<string> { BienDungChung.STATUS_DANGCHAY };
            //queryDieuHanhXe.NguoiDuyetId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            //var dieuHanhXeDangChay = _dieuHanhXeRepository.GetAll(queryDieuHanhXe);

            ////dang chay
            //queryDieuHanhXe.TrangThaiXuLy = new List<string> { BienDungChung.STATUS_DADONG };
            //queryDieuHanhXe.NguoiDuyetId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            //var dieuHanhXeDaKetThuc = _dieuHanhXeRepository.GetAll(queryDieuHanhXe);

            ////dang chay
            //queryDieuHanhXe.TrangThaiXuLy = new List<string> { BienDungChung.STATUS_HUYBO };
            //queryDieuHanhXe.NguoiDuyetId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            //var dieuHanhXeHuyBoXe = _dieuHanhXeRepository.GetAll(queryDieuHanhXe);


            //lich


            return Json(new
            {
                //QuanLyDatXeChoXuLyMenu = quanLyDatXeChoXuLy.total,
                //QuanLyDatXeDaDuyetMenu = quanLyDatXeDaDuyet.total,
                //QuanLyDatXeDangChayMenu = quanLyDatXeDangChay.total,
                //QuanLyDatXeDaKetThucMenu = quanLyDatXeKetThuc.total,
                //QuanLyDatXeDaHuyBoMenu = quanLyDatXeDaHuy.total,

                //DieuHanhXeChoToiDuyetMenu = dieuHanhXeChoToiDuyet.total,
                //DieuHanhXeDaDuyetMenu = dieuHanhXeDaDuyet.total,
                //DieuHanhXeDangChayMenu = dieuHanhXeDangChay.total,
                //DieuHanhXeDaKetThucMenu = dieuHanhXeDaKetThuc.total,
                //DieuHanhXeHuyBoXeMenu = dieuHanhXeHuyBoXe.total,

            }, JsonRequestBehavior.AllowGet);
        }
    }
}