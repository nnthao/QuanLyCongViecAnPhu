using Dashboard.Common;
using Dashboard.Services;
using Dashboard.Models.Entities;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DuAnThongKeController : Controller
    {
        private readonly IDuAnService _duAnService;

        public DuAnThongKeController(IDuAnService duAnService)
        {
            _duAnService = duAnService;
        }

        [JwtAuthorize("DuAnThongKe")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("DuAnThongKe")]
        public JsonResult GetThongKeThanhVienThamGiaNhieuNhat()
        {
            var data = _duAnService.ThongKeThanhVienThamGiaNhieuNhat();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnThongKe")]
        public JsonResult GetThongKeThanhVienThamGiaThuNghiemNhieuNhat()
        {
            var data = _duAnService.ThongKeThanhVienThamGiaThuNghiemNhieuNhat();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnThongKe")]
        public JsonResult GetThongKeThanhVienMucDoHoTroCaoNhat()
        {
            var data = _duAnService.ThongKeThanhVienMucDoHoTroCaoNhat();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnThongKe")]
        public JsonResult GetListDuAnByUserId(int userId)
        {
            var data = _duAnService.GetListDuAnByUserId(userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnThongKe")]
        public JsonResult GetListDuAnThuNghiemByUserId(int userId)
        {
            var data = _duAnService.GetListDuAnThuNghiemByUserId(userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnThongKe")]
        public JsonResult GetListAllDuAnByUserId(int userId)
        {
            var data = _duAnService.GetListAllDuAnByUserId(userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnThongKe")]
        public JsonResult GetThongKeDuAnTheoSoThanhVien()
        {
            var data = _duAnService.ThongKeDuAnTheoSoThanhVien();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnThongKe")]
        public JsonResult GetListThanhVienByDuAnId(int duAnId)
        {
            var data = _duAnService.GetListThanhVienByDuAnId(duAnId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnThongKe")]
        public ActionResult DuAnThanhVienThongKe()
        {
            return View();
        }

        [JwtAuthorize("DuAnThongKe")]
        public ActionResult DuAnTruongNhomThongKe()
        {
            return View();
        }

        [JwtAuthorize("DuAnThongKe")]
        public JsonResult GetDuAnThanhVienThongKe()
        {
            var data = _duAnService.GetDuAnThanhVienThongKe();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnThongKe")]
        public JsonResult GetListAllDuAnActive()
        {
            var data = _duAnService.GetListAllDuAnActive()
                .Select(x => new
                {
                    x.Id,
                    x.Ten,
                    x.MoTa,
                    x.IsThuNghiem
                })
                .ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnThongKe")]
        public JsonResult GetListDuAnNotThuNghiem()
        {
            var data = _duAnService.GetListDuAnNotThuNghiem()
                .Select(x => new
                {
                    x.Id,
                    x.Ten,
                    x.MoTa,
                    x.IsThuNghiem
                })
                .ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnThongKe")]
        public JsonResult GetTruongNhomThongKe()
        {
            var data = _duAnService.GetTruongNhomThongKe();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DuAnThongKe")]
        public ActionResult XuatExcelThongKeDuAn()
        {
            try
            {
                var thanhViens = _duAnService.ThongKeThanhVienThamGiaNhieuNhat();
                var duAns = _duAnService.GetListDuAnNotThuNghiem();
                var matranList = _duAnService.GetMatranThanhVienDuAn();

                if (thanhViens == null || thanhViens.Count == 0 || duAns == null || duAns.Count == 0)
                {
                    return Json(new { success = false, message = "Không có dữ liệu để xuất Excel" }, JsonRequestBehavior.AllowGet);
                }

                var fileName = $"ThongKeDuAn_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var uploadsPath = Server.MapPath("~/Uploads");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var filePath = Path.Combine(uploadsPath, fileName);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Thống kê dự án");

                    // Header row 1: Tên dự án (merge cells)
                    worksheet.Cell(1, 1).Value = "Họ tên";
                    worksheet.Range(1, 1, 2, 1).Merge();

                    worksheet.Cell(1, 2).Value = "Số nhóm tham gia";
                    worksheet.Range(1, 2, 2, 2).Merge();

                    int colIndex = 3;
                    foreach (var duAn in duAns)
                    {
                        worksheet.Cell(1, colIndex).Value = duAn.Ten;
                        worksheet.Range(1, colIndex, 2, colIndex).Merge();
                        colIndex++;
                    }

                    // Header row 2: Số thành viên cho mỗi dự án
                    colIndex = 3;
                    foreach (var duAn in duAns)
                    {
                        var soThanhVien = matranList.Count(m => m.DuAnId == duAn.Id);
                        worksheet.Cell(2, colIndex).Value = soThanhVien;
                        if (soThanhVien >= 4)
                        {
                            worksheet.Cell(2, colIndex).Style.Fill.BackgroundColor = XLColor.LightGreen;
                        }
                        else
                        {
                            worksheet.Cell(2, colIndex).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        }
                        colIndex++;
                    }

                    // Data rows
                    int rowIndex = 3;
                    foreach (var thanhVien in thanhViens)
                    {
                        worksheet.Cell(rowIndex, 1).Value = thanhVien.TenUser;
                        worksheet.Cell(rowIndex, 2).Value = thanhVien.SoNhomThamGia;

                        colIndex = 3;
                        foreach (var duAn in duAns)
                        {
                            var isThamGia = matranList.Any(m => m.UserId == thanhVien.UserId && m.DuAnId == duAn.Id);
                            if (isThamGia)
                            {
                                worksheet.Cell(rowIndex, colIndex).Value = "X";
                            }
                            colIndex++;
                        }

                        rowIndex++;
                    }

                    // Style headers
                    worksheet.Range(1, 1, 2, colIndex - 1).Style.Font.Bold = true;
                    worksheet.Range(1, 1, 2, colIndex - 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range(1, 1, 2, colIndex - 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    workbook.SaveAs(filePath);
                }

                var downloadUrl = Url.Content($"~/Uploads/{fileName}");
                return Json(new { success = true, url = downloadUrl }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã có lỗi xảy ra: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
