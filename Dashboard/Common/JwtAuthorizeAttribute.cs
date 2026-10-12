using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Common
{
    public class BearTokenHelper
    {
        public static LoginReturnViewModel GenerateJwtToken(UserGridVo nhanVienLoginInfo)
        {
            var key = new System.Text.UTF8Encoding().GetBytes(BienDungChung.JwtKeySecret);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userInfo = new LoginAuthenViewModel
            {
                Id = nhanVienLoginInfo.Id,
                VaiTroId = nhanVienLoginInfo.VaiTroId,
                PhongBanId = nhanVienLoginInfo.PhongBanId,
                Quyen = nhanVienLoginInfo.Quyen,
                IsSystemUser = nhanVienLoginInfo.IsSystemUser,
            };

            var userInfoJson = JsonConvert.SerializeObject(userInfo);

            var claims = new[]
            {
                new Claim(BienDungChung.JwtClaimInfoKey, EncryptionHelper.Encrypt(userInfoJson)),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(4),
                signingCredentials: credentials
            );

            return new LoginReturnViewModel
            {
                ExpTimeFormat = TimeZoneInfo.ConvertTimeFromUtc(token.ValidTo, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
    }

    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string _requiredClaimValue;

        public JwtAuthorizeAttribute(string requiredClaimValue)
        {
            _requiredClaimValue = requiredClaimValue;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string token = null;
            var cookie = httpContext.Request.Cookies["token"];
            if (cookie != null)
            {
                token = cookie.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.Default.GetBytes(BienDungChung.JwtKeySecret);

                    try
                    {
                        tokenHandler.ValidateToken(token, new TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        }, out SecurityToken validatedToken);

                        var jwtToken = (JwtSecurityToken)validatedToken;
                        var identity = new ClaimsIdentity(jwtToken.Claims, BienDungChung.JwtClaimInfoKey);
                        var principal = new ClaimsPrincipal(identity);
                        httpContext.User = principal;

                        var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
                        if (isSystemUser == true)
                        {
                            return true;
                        }

                        var vaiTroId = UserClaimHelper.GetCurrentVaiTroId(identity);
                        if (vaiTroId != 0)
                        {
                            var quyens = UserClaimHelper.GetCurrentQuyenOfUser(identity);
                            var quyenArr = quyens.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => o.Trim()).ToList();
                            quyenArr.Add("Public");

                            if (quyenArr.Contains(_requiredClaimValue))
                            {
                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Lỗi xác thực token: " + ex.Message);
                    }
                }
            }

            var expiredCookie = new HttpCookie("token");
            expiredCookie.Expires = DateTime.Now.AddDays(-1);
            httpContext.Response.Cookies.Add(expiredCookie);

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var isAjax = filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                filterContext.HttpContext.Response.ContentType = "application/json";
                filterContext.HttpContext.Response.StatusCode = 401;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.Result = new ContentResult
                {
                    Content = Newtonsoft.Json.JsonConvert.SerializeObject(new { message = "Token không hợp lệ hoặc không có quyền truy cập." }),
                    ContentType = "application/json"
                };
            }
            else
            {
                // Redirect về trang login nếu truy cập bằng URL thường
                // Lưu URL hiện tại vào returnUrl để redirect về sau khi đăng nhập
                var returnUrl = filterContext.HttpContext.Request.RawUrl;
                if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/"))
                {
                    filterContext.Result = new RedirectResult($"/dangnhap?returnUrl={HttpUtility.UrlEncode(returnUrl)}");
                }
                else
                {
                    filterContext.Result = new RedirectResult("/dangnhap");
                }
            }
        }
    }
}