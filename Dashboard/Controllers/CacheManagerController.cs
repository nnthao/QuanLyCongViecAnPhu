using Dashboard.Common;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class CacheManagerController : Controller
    {
        [JwtAuthorize("CacheManager")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("CacheManager")]
        public ActionResult TaoLaiCache()
        {
            CacheManagerHelper.InitializeCache();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}