using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;
using System.Linq; // For debugging or if needed
using Dashboard.Common; // For UserClaimHelper if needed, but not used in snippet below yet

namespace Dashboard.Controllers
{
    public class CongViecChungController : Controller
    {
        private readonly ICongViecChungService _service;

        public CongViecChungController(ICongViecChungService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(string keyword, int offset = 0, int limit = 10)
        {
            // bootstrap-table sends offset (start index) and limit (page size).
            // page = (offset / limit) + 1
            int page = (offset / limit) + 1;
            var data = _service.GetAll(keyword, page, limit);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [HttpPost]
        public ActionResult Save(CongViecChungViewModel model)
        {
             if(model.Id == 0)
             {
                 var result = _service.Insert(model);
                 return Json(result);
             }
             else
             {
                 var result = _service.Update(model);
                 return Json(result);
             }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var result = _service.Delete(id);
            return Json(result);
        }

        public ActionResult GetById(int id)
        {
            var data = _service.GetById(id);
            // Also need User list for Dropdown?
            // Usually we return a composite object like in DuAnController
            
            var users = CacheManagerHelper.GetListUserFromCacheNotDeleted()
                .Select(o => new { id = o.Id, text = o.Name })
                .ToList();

            return Json(new
            {
                Item = data,
                User = new { Data = users }
            }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ThongKe()
        {
            ViewBag.PhongBans = CacheManagerHelper.GetPhongBanCache();
            return View();
        }

        public ActionResult GetThongKeData(CongViecChungThongKeQueryViewModel query)
        {
            var data = _service.ThongKeCongViec(query);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
