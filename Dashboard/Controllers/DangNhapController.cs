using Dashboard.AdoNetHelper;
using Dashboard.Common;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using log4net;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DangNhapController : Controller
    {
        private static ILog _log = log4net.LogManager.GetLogger(typeof(MvcApplication));

        private readonly IVaiTroService _vaiTroService;
        private readonly IUserService _userService;
        private readonly string connStr;

        public DangNhapController(IVaiTroService vaiTroService,
            IUserService userService)
        {
            _vaiTroService = vaiTroService;
            _userService = userService;
            connStr = ConfigurationManager.ConnectionStrings["ConnectDB"].ConnectionString;
        }

        public ActionResult Index(string returnUrl = null)
        {
            var token = HttpContext.Request.Cookies["token"];
            if (token != null)
            {
                // Nếu đã đăng nhập và có returnUrl hợp lệ, redirect về đó
                if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/"))
                {
                    return Redirect(returnUrl);
                }
                return Redirect("/Dashboard");
            }

            // Validate returnUrl để tránh open redirect vulnerability
            if (!string.IsNullOrEmpty(returnUrl) && !returnUrl.StartsWith("/"))
            {
                returnUrl = null;
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult LogOut()
        {
            var token = HttpContext.Request.Cookies["token"];
            if (token != null)
            {
                var cookie = new HttpCookie("token")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie);
            }

            return Redirect("/DangNhap");
        }

        [HttpPost]
        public ActionResult Auth(LoginViewModel loginViewModel)
        {
            if (string.IsNullOrEmpty(loginViewModel.Email) || string.IsNullOrEmpty(loginViewModel.Password))
            {
                return Json(new LoginReturnViewModel
                {
                    IsError = true,
                    Msg = "Tên đăng nhập hoặc Mật khẩu không được trống."
                });
            }
            _log.Info("Kiểm tra đăng nhập tài khoản: " + loginViewModel.Email);

            var checkUser = _userService.GetUserDangNhap(loginViewModel.Email);
            if (checkUser.Id != 0)
            {
                if (checkUser.LoginFail.GetValueOrDefault() < 5)
                {
                    var passwordHash = PasswordHelper.HashString(checkUser.Guid, loginViewModel.Password);
                    if (passwordHash.Equals(checkUser.Password))
                    {
                        checkUser.Quyen = string.Join(",", _vaiTroService.VaiTroPhanQuyenStr(checkUser.VaiTroId.GetValueOrDefault(), checkUser.NhomVaiTroId));

                        var token = BearTokenHelper.GenerateJwtToken(checkUser);
                        if (token != null)
                        {
                            Task.Run(async () => await UserAdoNetHelper.ResetLoginFail(checkUser.Id, connStr));

                            return Json(new LoginReturnViewModel
                            {
                                IsError = false,
                                Msg = "Đăng nhập thành công.",
                                Token = token.Token,
                                ExpTimeFormat = token.ExpTimeFormat
                            });
                        }
                    }
                    else
                    {
                        Task.Run(async () => await UserAdoNetHelper.TangLanLoginFailAsync(checkUser.Id, connStr));
                    }
                }
            }

            return Json(new LoginReturnViewModel
            {
                IsError = true,
                Msg = "Tên đăng nhập hoặc Mật khẩu không đúng."
            });
        }
    }
}