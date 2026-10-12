using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class GiaoViecThongKeController : Controller
    {
        private readonly IGiaoViecThongKeService _service;
        private readonly IQuanLyBaoCaoChuyenNganhService _quanLyBaoCaoChuyenNganhService;
        public GiaoViecThongKeController(IGiaoViecThongKeService service, IQuanLyBaoCaoChuyenNganhService quanLyBaoCaoChuyenNganhService)
        {
            _service = service;
            _quanLyBaoCaoChuyenNganhService = quanLyBaoCaoChuyenNganhService;
        }

        [JwtAuthorize("GiaoViecThongKe")]
        public ActionResult ThongKeCongViecTheoPhongBanBieuDo()
        {
            return View();
        }

        [JwtAuthorize("GiaoViecThongKe")]
        [HttpPost]
        public JsonResult GetThongKeCongViecTheoPhongBan(GiaoViecQueryViewModel queryViewModel)
        {
            var errors = new List<ValidateError>();
            if (queryViewModel.TuNgay == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "TuNgay",
                    Description = "Từ ngày không được trống"

                });
            }
            if (queryViewModel.DenNgay == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "DenNgay",
                    Description = "Đến ngày không được trống"

                });
            }

            if (errors.Count != 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var phongBanCurrentId = UserClaimHelper.GetPhongBanId(identity);
                var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
                var isQuyenGiaoViecTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenGiaoViecTatCaPhongBan.ToString());

                if (!isSystemUser && !isQuyenGiaoViecTatCaPhongBan && phongBanCurrentId != 0)
                {
                    var phongBanIds = new List<int> { phongBanCurrentId };
                    phongBanIds.AddRange(UltiHelper.GetPhongBanChildrenIds(phongBanCurrentId));
                    queryViewModel.PhongBanIds = phongBanIds;
                }

                var result = _service.GetThongKeCongViecTheoPhongBan(queryViewModel);
                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [JwtAuthorize("GiaoViecThongKe")]
        public JsonResult GetListChiTietHoanThanhCongViecPhongBan(GiaoViecQueryViewModel queryViewModel)
        {
            var errors = KiemTraThongSoTimKiem(queryViewModel);
            if (errors.Count != 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                if (queryViewModel.PhongBanId.HasValue && queryViewModel.PhongBanId.Value > 0)
                {
                    if (queryViewModel.PhongBanIds == null)
                    {
                        queryViewModel.PhongBanIds = new List<int>();
                    }
                    if (!queryViewModel.PhongBanIds.Contains(queryViewModel.PhongBanId.Value))
                    {
                        queryViewModel.PhongBanIds.Add(queryViewModel.PhongBanId.Value);
                    }
                }

                // Nếu IsHoanThanhType = 0, lấy tất cả các trạng thái (1, 2, 3, 4, 5)
                if (queryViewModel.IsHoanThanhType == 0)
                {
                    if (queryViewModel.IsHoanThanhTypeList == null)
                    {
                        queryViewModel.IsHoanThanhTypeList = new List<int>();
                    }
                    queryViewModel.IsHoanThanhTypeList.Clear();
                    queryViewModel.IsHoanThanhTypeList.AddRange(new List<int> { 1, 2, 3, 4, 5 });
                }
                else if (queryViewModel.IsHoanThanhType > 0)
                {
                    if (queryViewModel.IsHoanThanhTypeList == null)
                    {
                        queryViewModel.IsHoanThanhTypeList = new List<int>();
                    }
                    if (!queryViewModel.IsHoanThanhTypeList.Contains(queryViewModel.IsHoanThanhType))
                    {
                        queryViewModel.IsHoanThanhTypeList.Add(queryViewModel.IsHoanThanhType);
                    }
                }

                var data = _service.GetListChiTietHoanThanhCongViecPhongBan(queryViewModel);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [JwtAuthorize("GiaoViecThongKe")]
        public ActionResult ThongKeCongViecTheoTrangThai()
        {
            return View();
        }

        [JwtAuthorize("GiaoViecThongKe")]
        public JsonResult GetListChiTietHoanThanhCongViecTheoTrangThai(GiaoViecQueryViewModel queryViewModel)
        {
            if (queryViewModel.TuNgay == null || queryViewModel.DenNgay == null)
            {
                return Json(new DataTableResult<GiaoViecThanhVienGridVo>(), JsonRequestBehavior.AllowGet);
            }

            try
            {
                if (!string.IsNullOrEmpty(queryViewModel.PhongBanIdsStr))
                {
                    queryViewModel.PhongBanIds = queryViewModel.PhongBanIdsStr.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => Convert.ToInt32(o)).ToList();
                }

                if (!string.IsNullOrEmpty(queryViewModel.IsHoanThanhTypeListStr))
                {
                    queryViewModel.IsHoanThanhTypeList = queryViewModel.IsHoanThanhTypeListStr.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => Convert.ToInt32(o)).ToList();
                }

                if (!string.IsNullOrEmpty(queryViewModel.ThanhVienIdListStr))
                {
                    queryViewModel.ThanhVienIdList = queryViewModel.ThanhVienIdListStr.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => Convert.ToInt32(o)).ToList();
                }

                var data = _service.GetListChiTietHoanThanhCongViecPhongBan(queryViewModel);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [JwtAuthorize("GiaoViecThongKe")]
        public ActionResult XuatExcelChiTietCongViecTheoTrangThai(GiaoViecQueryViewModel queryViewModel)
        {
            var errors = new List<ValidateError>();
            if (queryViewModel.TuNgay == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "TuNgay",
                    Description = "Từ ngày không được trống"

                });
            }
            if (queryViewModel.DenNgay == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "DenNgay",
                    Description = "Đến ngày không được trống"

                });
            }

            if (errors.Count != 0)
            {
                return Json(new { success = false, Errors = errors }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Parse PhongBanIds from string
                if (!string.IsNullOrEmpty(queryViewModel.PhongBanIdsStr))
                {
                    queryViewModel.PhongBanIds = queryViewModel.PhongBanIdsStr.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => Convert.ToInt32(o)).ToList();
                }

                // Parse IsHoanThanhTypeList from string
                if (!string.IsNullOrEmpty(queryViewModel.IsHoanThanhTypeListStr))
                {
                    queryViewModel.IsHoanThanhTypeList = queryViewModel.IsHoanThanhTypeListStr.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => Convert.ToInt32(o)).ToList();
                }

                if (!string.IsNullOrEmpty(queryViewModel.ThanhVienIdListStr))
                {
                    queryViewModel.ThanhVienIdList = queryViewModel.ThanhVienIdListStr.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => Convert.ToInt32(o)).ToList();
                }

                // Set limit = 0 to get all records
                queryViewModel.limit = 0;

                var data = _service.GetListChiTietHoanThanhCongViecPhongBan(queryViewModel);

                // Check if there's data to export
                if (data.rows == null || data.rows.Count == 0)
                {
                    return Json(new { success = false, message = "Không có dữ liệu để xuất Excel" }, JsonRequestBehavior.AllowGet);
                }

                // Generate unique filename
                var fileName = $"ChiTietCongViecTheoTrangThai_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                // Get uploads folder path
                var uploadsPath = Server.MapPath("~/Uploads");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Full file path
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file directly using service method
                var savedFilePath = _service.XuatExcelChiTietCongViecTheoTrangThaiToFile(data.rows, filePath);

                // Return download URL
                var downloadUrl = Url.Content($"~/Uploads/{fileName}");

                // Validate saved file path exists
                if (System.IO.File.Exists(savedFilePath))
                {
                    var queryInfo = new BaoCaoChuyenNganhQueryInfo
                    {
                        TuNgay = queryViewModel.TuNgay,
                        DenNgay = queryViewModel.DenNgay,
                        PhongBanIdsStr = queryViewModel.PhongBanIdsStr,
                        IsHoanThanhTypeListStr = queryViewModel.IsHoanThanhTypeListStr,
                        ThanhVienIdListStr = queryViewModel.ThanhVienIdListStr
                    };

                    var logVm = new QuanLyBaoCaoChuyenNganhViewModel
                    {
                        DanhMucBaoCaoId = DanhMucBaoCaoEnum.CongViec,
                        TenFile = fileName,
                        DuongDanFile = downloadUrl,
                        QuanLyBaoCaoChuyenNganhEnumId = QuanLyBaoCaoChuyenNganhEnum.BaoCaoCongViec,
                        QueryInfo = JsonConvert.SerializeObject(queryInfo),
                        CurrentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity)
                    };

                    _quanLyBaoCaoChuyenNganhService.TaoBaoCaoLog(logVm);
                }

                return Json(new
                {
                    downloadUrl = downloadUrl,
                    fileName = fileName,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [JwtAuthorize("GiaoViecThongKe")]
        public ActionResult ThongKeCongViecTheoCaNhan()
        {
            return View();
        }

        private List<ValidateError> KiemTraThongSoTimKiem(GiaoViecQueryViewModel queryViewModel)
        {
            var errors = new List<ValidateError>();
            if (queryViewModel.TuNgay == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "TuNgay",
                    Description = "Từ ngày không được trống"

                });
            }
            if (queryViewModel.DenNgay == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "DenNgay",
                    Description = "Đến ngày không được trống"

                });
            }

            if (queryViewModel.PhongBanId.GetValueOrDefault() == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "PhongBanId",
                    Description = "Phòng ban không được trống"

                });
            }

            //if (queryViewModel.IsHoanThanhType == 0)
            //{
            //    errors.Add(new ValidateError
            //    {
            //        Id = "IsHoanThanhType",
            //        Description = "Trạng thái không được trống"

            //    });
            //}


            return errors;
        }

        [JwtAuthorize("GiaoViecThongKe")]
        public ActionResult XuatExcelPhuLuc03(GiaoViecQueryViewModel queryViewModel)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var phongBanCurrentId = UserClaimHelper.GetPhongBanId(identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenGiaoViecTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenGiaoViecTatCaPhongBan.ToString());

            if (!isSystemUser && !isQuyenGiaoViecTatCaPhongBan && phongBanCurrentId != 0)
            {
                var phongBanIds = new List<int> { phongBanCurrentId };
                phongBanIds.AddRange(UltiHelper.GetPhongBanChildrenIds(phongBanCurrentId));
                queryViewModel.PhongBanIds = phongBanIds;
            }

            var fileBytes = _service.XuatExcelPhuLuc03(queryViewModel);
            var fileName = $"PhuLuc03_BaoCaoThongKePhongBan_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [JwtAuthorize("GiaoViecThongKe")]
        public ActionResult XuatExcelPhuLuc05(GiaoViecQueryViewModel queryViewModel)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var phongBanCurrentId = UserClaimHelper.GetPhongBanId(identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenGiaoViecTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenGiaoViecTatCaPhongBan.ToString());

            if (!isSystemUser && !isQuyenGiaoViecTatCaPhongBan && phongBanCurrentId != 0)
            {
                var phongBanIds = new List<int> { phongBanCurrentId };
                phongBanIds.AddRange(UltiHelper.GetPhongBanChildrenIds(phongBanCurrentId));
                queryViewModel.PhongBanIds = phongBanIds;
            }

            var fileBytes = _service.XuatExcelPhuLuc05(queryViewModel);
            var fileName = $"PhuLuc05_NhiemVuHoanThanhTreHan_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [JwtAuthorize("GiaoViecThongKe")]
        public ActionResult XuatExcelPhuLuc345(GiaoViecQueryViewModel queryViewModel)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var phongBanCurrentId = UserClaimHelper.GetPhongBanId(identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenGiaoViecTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenGiaoViecTatCaPhongBan.ToString());

            if (!isSystemUser && !isQuyenGiaoViecTatCaPhongBan && phongBanCurrentId != 0)
            {
                var phongBanIds = new List<int> { phongBanCurrentId };
                phongBanIds.AddRange(UltiHelper.GetPhongBanChildrenIds(phongBanCurrentId));
                queryViewModel.PhongBanIds = phongBanIds;
            }

            var fileBytes = _service.XuatExcelPhuLuc345(queryViewModel);
            var fileName = $"BaoCaoPhuLuc_03_04_05_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}

