using Dashboard.AdoNetHelper;
using Dashboard.Common;
using Dashboard.Models.ViewModel;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class GiaoViecTiViController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
		
        public JsonResult GetThongTinTiVi(GiaoViecTiViQueryViewModel queryParams)
        {
            if (queryParams == null)
            {
                queryParams = new GiaoViecTiViQueryViewModel();
            }

            var result = GiaoViecTiViAdoNetHelper.GetThongTinDataTiVi(queryParams);
            return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
        }
    }
}
