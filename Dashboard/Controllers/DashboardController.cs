using Dashboard.Common;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [JwtAuthorize("Public")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("Public")]
        [HttpGet]
        public JsonResult GetDashboardData(DashboardQueryViewModel query)
        {
            try
            {
                if (query == null)
                {
                    query = new DashboardQueryViewModel();
                }

                var currentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
                query.CurrentUserId = currentUserId;

                var data = _dashboardService.GetDashboardData(query);
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [JwtAuthorize("Public")]
        [HttpGet]
        public JsonResult GetListChiTietCongViecDashboard(DashboardDetailQueryViewModel query)
        {
            try
            {
                if (query == null)
                {
                    query = new DashboardDetailQueryViewModel();
                }

                var currentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
                query.CurrentUserId = currentUserId;

                var data = _dashboardService.GetListChiTietCongViecDashboard(query);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
