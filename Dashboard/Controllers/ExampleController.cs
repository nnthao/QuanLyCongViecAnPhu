using Dashboard.Common;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class ExampleController : Controller
    {

        public ExampleController()
        {
        }

        [JwtAuthorize("Example")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
