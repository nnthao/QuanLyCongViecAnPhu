using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Common
{
    public class IsAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        public string MenuKey { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.IsDefined(typeof(JwtAuthorizeAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(JwtAuthorizeAttribute), true))
            {
                return;
            }

            if (filterContext.ActionDescriptor.IsDefined(typeof(IsAuthorize), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(IsAuthorize), true))
            {
                if (HttpContext.Current.Session["User"] == null)
                {
                    filterContext.Result = new RedirectResult("/");
                }
                else
                {
                    if (HttpContext.Current.Session["IsSystemUser"] != null
                        && (bool)HttpContext.Current.Session["IsSystemUser"] == true)
                    {
                        return;
                    }
                    else if (MenuKey != null)
                    {
                        var phanQuyenArr = HttpContext.Current.Session["PhanQuyen"] as List<string>;

                        var MenuKeyList = MenuKey.Split(',');
                        foreach (var itemMenu in MenuKeyList)
                        {
                            if (phanQuyenArr?.FirstOrDefault(o => o == itemMenu) != null
                            || MenuKey == "Public")
                            {
                                return;
                            }
                        }

                        filterContext.Result = new RedirectResult("/Error");
                    }
                }
            }

            return;
        }
    }
}