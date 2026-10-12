using Dashboard.Common;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class QuanLyBaoCaoChuyenNganhController : Controller
    {
        private readonly IQuanLyBaoCaoChuyenNganhService _service;
        public QuanLyBaoCaoChuyenNganhController(IQuanLyBaoCaoChuyenNganhService service)
        {
            _service = service;
        }

        [JwtAuthorize("QuanLyBaoCaoChuyenNganh")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("QuanLyBaoCaoChuyenNganh")]
        public ActionResult GetListQuanLyBaoCaoChuyenNganh(BaoCaoChuyenNganhQueryViewModel queryParams)
        {
            queryParams.CurrentUserId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _service.GetAll(queryParams);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyBaoCaoChuyenNganh")]
        public ActionResult Xoa(int Id)
        {
            var data = _service.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
