using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using Dashboard.Services;

namespace Dashboard.Controllers
{
    [JwtAuthorize("DanhSachCongTrinhHuHong")]
    public class DanhSachCongTrinhHuHongController : Controller
    {
        private readonly IDanhSachCongTrinhService _danhSachCongTrinhService;

        public DanhSachCongTrinhHuHongController(IDanhSachCongTrinhService danhSachCongTrinhService)
        {
            _danhSachCongTrinhService = danhSachCongTrinhService;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Danh sách công trình hư hỏng";
            return View();
        }

        public ActionResult GetAll(QueryParamViewModel queryParams)
        {
            var result = _danhSachCongTrinhService.GetAll(queryParams);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetById(int id)
        {
            var result = _danhSachCongTrinhService.GetByIdWithHuHong(id);
            return Json(new { Item = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ThemOrCapNhat(DanhSachCongTrinhViewModel danhSachCongTrinhViewModel)
        {
            try
            {
                var result = _danhSachCongTrinhService.CapNhatThongTinHuHong(danhSachCongTrinhViewModel);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Errors = new[] { new { Description = ex.Message } } });
            }
        }

        [HttpPost]
        public ActionResult Xoa(int id)
        {
            var result = _danhSachCongTrinhService.Xoa(id);
            return Json(new { Success = result });
        }

        [HttpPost]
        public ActionResult KhoiPhuc(int id)
        {
            var result = _danhSachCongTrinhService.KhoiPhuc(id);
            return Json(new { Success = result });
        }

        public ActionResult Form(int id = 0)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult DanhSachCongTrinhLookup(LookupQueryViewModel lookupQueryViewModel, int? loaiCongTrinh = null)
        {
            var result = _danhSachCongTrinhService.DanhSachCongTrinhLookup(lookupQueryViewModel, loaiCongTrinh);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
