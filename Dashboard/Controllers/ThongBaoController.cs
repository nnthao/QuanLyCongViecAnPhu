using Dashboard.AdoNetHelper;
using Dashboard.Common;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class ThongBaoController : Controller
    {
        private readonly IThongBaoService _thongBaoService;
        public ThongBaoController(IThongBaoService thongBaoService)
        {
            _thongBaoService = thongBaoService;
        }

        [JwtAuthorize("Public")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("Public")]
        public ActionResult GetListThongBao(QueryParamViewModel queryParams)
        {
            queryParams.CurrentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            queryParams.IsSystemUser = UserClaimHelper.GetIsSystemUser((ClaimsIdentity)User.Identity);

            var data = _thongBaoService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public ActionResult GetListThongBaoBellIcon(QueryParamViewModel queryParams)
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);

            queryParams.limit = 10;
            queryParams.offset = 0;
            queryParams.sort = "NgayTao";
            queryParams.order = "desc";
            queryParams.CurrentUserId = currentUserId;

            var data = _thongBaoService.GetAll(queryParams);
            var chuaXem = ThongBaoAdoNetHelper.DemThongBaoChuaXem(currentUserId, BienDungChung.ConnectDBStr);
            return Json(new
            {
                Data = data,
                ChuaXem = chuaXem
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("Public")]
        public ActionResult Xoa(int Id)
        {
            ThongBaoAdoNetHelper.Xoa(Id, BienDungChung.ConnectDBStr);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("Public")]
        public ActionResult KhoiPhuc(int Id)
        {
            ThongBaoAdoNetHelper.KhoiPhuc(Id, BienDungChung.ConnectDBStr);
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [JwtAuthorize("Public")]
        public ActionResult DanhDauDaXem(int Id)
        {
            ThongBaoAdoNetHelper.DanhDauDaXem(Id, BienDungChung.ConnectDBStr);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("Public")]
        public ActionResult DanhDauDaXemNhieu(string Ids)
        {
            ThongBaoAdoNetHelper.DanhDauDaXemNhieu(Ids, BienDungChung.ConnectDBStr);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public ActionResult DanhDauDaXemTatCa()
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            ThongBaoAdoNetHelper.DanhDauDaXemTatCa(currentUserId, BienDungChung.ConnectDBStr);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public ActionResult TestGuiTelegramAsync()
        {
            var telegramId = UserClaimHelper.GetTelegramIdOfCurrentUser((ClaimsIdentity)User.Identity);
            Task.Run(async () => await TelegramHelper.SendMessageAsync(new TelegramSendMessageVm
            {
                ChatId = telegramId,
                Message = "Test gửi tin nhắn"
            }));

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}