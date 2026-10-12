using Dashboard.Common;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class ThongKeCongViecTheoCaNhanSoLuongController : Controller
    {
        private readonly IGiaoViecThongKeService _giaoViecThongKeService;

        public ThongKeCongViecTheoCaNhanSoLuongController(IGiaoViecThongKeService giaoViecThongKeService)
        {
            _giaoViecThongKeService = giaoViecThongKeService;
        }

        [JwtAuthorize("ThongKeCongViecTheoCaNhanSoLuong")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("ThongKeCongViecTheoCaNhanSoLuong")]
        [HttpPost]
        public JsonResult GetTop10ThanhVienCoNhieuCongViec(GiaoViecQueryViewModel queryViewModel)
        {
            var errors = new List<ValidateError>();
            if (string.IsNullOrWhiteSpace(queryViewModel.TuNgayStr))
            {
                errors.Add(new ValidateError
                {
                    Id = "TuNgay",
                    Description = "Từ ngày không được trống"
                });
            }
            if (string.IsNullOrWhiteSpace(queryViewModel.DenNgayStr))
            {
                errors.Add(new ValidateError
                {
                    Id = "DenNgay",
                    Description = "Đến ngày không được trống"
                });
            }

            if (errors.Count > 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Parse PhongBanIds từ string
                if (!string.IsNullOrWhiteSpace(queryViewModel.PhongBanIdsStr))
                {
                    queryViewModel.PhongBanIds = new List<int>();
                    var phongBanIdStrings = queryViewModel.PhongBanIdsStr.Split(',');
                    foreach (var idStr in phongBanIdStrings)
                    {
                        int id;
                        if (int.TryParse(idStr.Trim(), out id) && id > 0)
                        {
                            queryViewModel.PhongBanIds.Add(id);
                        }
                    }
                }

                var result = _giaoViecThongKeService.GetTop10ThanhVienCoNhieuCongViec(queryViewModel);
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [JwtAuthorize("ThongKeCongViecTheoCaNhanSoLuong")]
        [HttpPost]
        public JsonResult GetTop10ThanhVienCoItCongViec(GiaoViecQueryViewModel queryViewModel)
        {
            var errors = new List<ValidateError>();
            if (string.IsNullOrWhiteSpace(queryViewModel.TuNgayStr))
            {
                errors.Add(new ValidateError
                {
                    Id = "TuNgay",
                    Description = "Từ ngày không được trống"
                });
            }
            if (string.IsNullOrWhiteSpace(queryViewModel.DenNgayStr))
            {
                errors.Add(new ValidateError
                {
                    Id = "DenNgay",
                    Description = "Đến ngày không được trống"
                });
            }

            if (errors.Count > 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }

            try
            {

                if (!string.IsNullOrWhiteSpace(queryViewModel.PhongBanIdsStr))
                {
                    queryViewModel.PhongBanIds = new List<int>();
                    var phongBanIdStrings = queryViewModel.PhongBanIdsStr.Split(',');
                    foreach (var idStr in phongBanIdStrings)
                    {
                        int id;
                        if (int.TryParse(idStr.Trim(), out id) && id > 0)
                        {
                            queryViewModel.PhongBanIds.Add(id);
                        }
                    }
                }

                var result = _giaoViecThongKeService.GetTop10ThanhVienCoItCongViec(queryViewModel);
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [JwtAuthorize("ThongKeCongViecTheoCaNhanSoLuong")]
        [HttpPost]
        public JsonResult GetTop10ThanhVienCoNhieuCongViecSomHan(GiaoViecQueryViewModel queryViewModel)
        {
            var errors = new List<ValidateError>();
            if (string.IsNullOrWhiteSpace(queryViewModel.TuNgayStr))
            {
                errors.Add(new ValidateError
                {
                    Id = "TuNgay",
                    Description = "Từ ngày không được trống"
                });
            }
            if (string.IsNullOrWhiteSpace(queryViewModel.DenNgayStr))
            {
                errors.Add(new ValidateError
                {
                    Id = "DenNgay",
                    Description = "Đến ngày không được trống"
                });
            }

            if (errors.Count > 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(queryViewModel.PhongBanIdsStr))
                {
                    queryViewModel.PhongBanIds = new List<int>();
                    var phongBanIdStrings = queryViewModel.PhongBanIdsStr.Split(',');
                    foreach (var idStr in phongBanIdStrings)
                    {
                        int id;
                        if (int.TryParse(idStr.Trim(), out id) && id > 0)
                        {
                            queryViewModel.PhongBanIds.Add(id);
                        }
                    }
                }

                var result = _giaoViecThongKeService.GetTop10ThanhVienCoNhieuCongViecSomHan(queryViewModel);
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [JwtAuthorize("ThongKeCongViecTheoCaNhanSoLuong")]
        [HttpPost]
        public JsonResult GetTop10ThanhVienCoNhieuCongViecTreHan(GiaoViecQueryViewModel queryViewModel)
        {
            var errors = new List<ValidateError>();
            if (string.IsNullOrWhiteSpace(queryViewModel.TuNgayStr))
            {
                errors.Add(new ValidateError
                {
                    Id = "TuNgay",
                    Description = "Từ ngày không được trống"
                });
            }
            if (string.IsNullOrWhiteSpace(queryViewModel.DenNgayStr))
            {
                errors.Add(new ValidateError
                {
                    Id = "DenNgay",
                    Description = "Đến ngày không được trống"
                });
            }

            if (errors.Count > 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(queryViewModel.PhongBanIdsStr))
                {
                    queryViewModel.PhongBanIds = new List<int>();
                    var phongBanIdStrings = queryViewModel.PhongBanIdsStr.Split(',');
                    foreach (var idStr in phongBanIdStrings)
                    {
                        int id;
                        if (int.TryParse(idStr.Trim(), out id) && id > 0)
                        {
                            queryViewModel.PhongBanIds.Add(id);
                        }
                    }
                }

                var result = _giaoViecThongKeService.GetTop10ThanhVienCoNhieuCongViecTreHan(queryViewModel);
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}

