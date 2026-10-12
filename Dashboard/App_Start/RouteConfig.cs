using System.Web.Mvc;
using System.Web.Routing;

namespace Dashboard
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ChamDiem",
                url: "ChamDiem",
                defaults: new { controller = "KetQua", action = "ChamDiem" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "DangNhap", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
