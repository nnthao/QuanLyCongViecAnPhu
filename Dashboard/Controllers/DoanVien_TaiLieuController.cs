using Dashboard.Common;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DoanVien_TaiLieuController : Controller
    {
        private readonly IDoanVien_TaiLieuService _service;

        public DoanVien_TaiLieuController(IDoanVien_TaiLieuService service)
        {
            _service = service;
        }

        [JwtAuthorize("DoanVien_TaiLieu")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("DoanVien_TaiLieu")]
        public ActionResult GetAll(QueryParamViewModel queryParams)
        {
            var data = _service.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoanVien_TaiLieu")]
        public ActionResult ThemOrCapNhat()
        {
            var files = new List<HttpPostedFileBase>();
            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        files.Add(file);
                    }
                }
            }

            if (files.Count == 0)
            {
                return Json(new { Errors = new List<ValidateError> { new ValidateError { Id = "Files", Description = "Vui lòng chọn ít nhất một file." } } }, JsonRequestBehavior.AllowGet);
            }

            var data = _service.ThemOrCapNhat(files);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoanVien_TaiLieu")]
        public ActionResult Xoa(int Id)
        {
            var data = _service.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoanVien_TaiLieu")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _service.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DoanVien_TaiLieu")]
        public JsonResult GetById(int id)
        {
            var data = _service.GetById(id);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult Download(int id)
        {
            var item = _service.GetById(id);
            if (item == null || string.IsNullOrEmpty(item.DuongDan))
            {
                return HttpNotFound();
            }

            var filePath = Server.MapPath("~" + item.DuongDan);
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound();
            }

            return File(filePath, System.Net.Mime.MediaTypeNames.Application.Octet, item.TenFile);
        }
    }
}

