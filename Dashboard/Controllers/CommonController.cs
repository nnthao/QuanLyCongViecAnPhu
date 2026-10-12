using Dashboard.Models.ViewModel;
using log4net;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class CommonController : Controller
    {
        private static ILog log = log4net.LogManager.GetLogger(typeof(MvcApplication));

        public JsonResult NamLookup(int start, int end)
        {
            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn ---"
                    }
                };

            for (int i = end; i >= start; i--)
            {
                data.Add(new LookupViewModel
                {
                    id = i,
                    text = i.ToString()
                });
            }


            return Json(new { Data = data }, JsonRequestBehavior.AllowGet);
        }

    }
}