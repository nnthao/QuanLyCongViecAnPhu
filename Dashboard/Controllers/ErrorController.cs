using System;
using System.Web;
using System.Web.Mvc;
using Dashboard.Common;
using Dashboard.Models.ViewModel;

namespace Dashboard.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult Message(ErrorViewModel errorViewModel)
        {
            return View(errorViewModel);
        }
    }
}