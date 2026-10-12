using ClosedXML.Excel;
using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DoanVienController : Controller
    {
        private readonly IDoanVienService _service;
        private readonly IDonViDoanService _donViDoanService;
        private readonly IDoanVien_DanTocService _doanVien_DanTocService;
        public DoanVienController(IDoanVienService service, IDonViDoanService donViDoanService, IDoanVien_DanTocService doanVien_DanTocService)
        {
            _service = service;
            _donViDoanService = donViDoanService;
            _doanVien_DanTocService = doanVien_DanTocService;
        }

        [JwtAuthorize("DoanVien")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("DoanVien")]
        public ActionResult GetAll(QueryParamViewModel queryParams)
        {
            var data = _service.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoanVien")]
        public ActionResult ThemOrCapNhat(DoanVienViewModel model)
        {
            var data = _service.ThemOrCapNhat(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoanVien")]
        public ActionResult Xoa(int Id)
        {
            var data = _service.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoanVien")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _service.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DoanVien")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("DoanVien")]
        public JsonResult GetById(int id)
        {
            var donViDoan = _donViDoanService.DonViDoanLookup(new LookupQueryViewModel());
            var danToc = _doanVien_DanTocService.DoanVien_DanTocLookup(new LookupQueryViewModel());
            var data = _service.GetById(id);
            return Json(new { Item = data, DonViDoan = donViDoan, DanToc = danToc }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoanVien")]
        public ActionResult ImportDanhSach()
        {
            if (Request.Files != null && Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["file"] ?? Request.Files[0];
                if (file != null)
                {
                    var extension = Path.GetExtension(file.FileName)?.ToLower();
                    if (extension != ".xlsx")
                    {
                        return Json(new
                        {
                            Errors = new List<Models.Validates.ValidateError> {
                                new Models.Validates.ValidateError
                                {
                                    Id = "ImportDoanVien",
                                    Description = "Chỉ cho phép file Excel (.xlsx)."
                                }
                            }
                        }, JsonRequestBehavior.AllowGet);
                    }

                    try
                    {
                        FileHelper.ValidateFileExtension(file);
                    }
                    catch (ArgumentException ex)
                    {
                        return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                    }

                    var fileName = file.FileName;
                    var fullPath = System.Web.HttpContext.Current.Server.MapPath(BienDungChung.FolderSavePath);
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }

                    extension = Path.GetExtension(fileName);
                    var fileNameTemp = $"ImportDoanVien_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var filePathTemp = Path.Combine(fullPath, fileNameTemp);
                    file.SaveAs(filePathTemp);

                    if (System.IO.File.Exists(filePathTemp))
                    {
                        try
                        {
                            using (XLWorkbook workBook = new XLWorkbook(filePathTemp))
                            {
                                IXLWorksheet workSheet = workBook.Worksheet(1);
                                var danhSachDoanVien = ImportDoanVienHelper.ReadDataExcel(workSheet);
                                var ketQua = _service.ImportDanhSach(danhSachDoanVien);
                                return Json(ketQua, JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new
                            {
                                Errors = new List<Models.Validates.ValidateError> {
                                    new Models.Validates.ValidateError
                                    {
                                        Id = "ImportDoanVien",
                                        Description = "Lỗi import file: " + ex.Message
                                    }
                                }
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

            return Json(new
            {
                Errors = new List<Models.Validates.ValidateError> {
                    new Models.Validates.ValidateError
                    {
                        Id = "ImportDoanVien",
                        Description = "Vui lòng chọn file để import."
                    }
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("DoanVien")]
        public ActionResult TaiFileMau()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachDoanVien");

                worksheet.Cell(6, 1).Value = "STT";
                worksheet.Cell(6, 2).Value = "Đơn vị";
                worksheet.Cell(6, 3).Value = "Họ và tên";
                worksheet.Range(6, 4, 6, 5).Merge();
                worksheet.Cell(6, 4).Value = "Ngày tháng năm sinh";
                worksheet.Cell(7, 4).Value = "Nam";
                worksheet.Cell(7, 5).Value = "Nữ";
                worksheet.Cell(6, 6).Value = "Quê quán";
                worksheet.Cell(6, 7).Value = "Dân tộc";
                worksheet.Range(6, 8, 6, 9).Merge();
                worksheet.Cell(6, 8).Value = "Trình độ";
                worksheet.Cell(7, 8).Value = "Chuyên môn, nghiệp vụ";
                worksheet.Cell(7, 9).Value = "Lý luận chính trị";
                worksheet.Cell(6, 10).Value = "Ngày vào Đoàn";
                worksheet.Cell(6, 11).Value = "Ngày vào Đảng";
                worksheet.Cell(6, 12).Value = "Chức vụ, đơn vị công tác";
                worksheet.Cell(7, 12).Value = "(lưu ý ghi rõ chức vụ Đảng (nếu có))";
                worksheet.Cell(6, 13).Value = "Ghi chú";
                worksheet.Cell(7, 13).Value = "(Ghi chú Trưởng đoàn, kèm sđt)";

                worksheet.Range(6, 1, 7, 13).Style.Font.Bold = true;
                worksheet.Range(6, 1, 7, 13).Style.Fill.BackgroundColor = XLColor.LightBlue;
                worksheet.Range(6, 1, 7, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(6, 1, 7, 13).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range(6, 1, 7, 13).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(6, 1, 7, 13).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                worksheet.Column(1).Width = 8;
                worksheet.Column(2).Width = 25;
                worksheet.Column(3).Width = 25;
                worksheet.Column(4).Width = 12;
                worksheet.Column(5).Width = 12;
                worksheet.Column(6).Width = 25;
                worksheet.Column(7).Width = 15;
                worksheet.Column(8).Width = 30;
                worksheet.Column(9).Width = 25;
                worksheet.Column(10).Width = 18;
                worksheet.Column(11).Width = 18;
                worksheet.Column(12).Width = 50;
                worksheet.Column(13).Width = 30;

                worksheet.Cell(8, 1).Value = 1;
                worksheet.Cell(8, 2).Value = "Phường Cẩm Thành";
                worksheet.Cell(8, 3).Value = "An Việt Đạt";
                worksheet.Cell(8, 4).Value = 1993;
                worksheet.Cell(8, 5).Value = "";
                worksheet.Cell(8, 6).Value = "Hợp Đức, Hưng Yên";
                worksheet.Cell(8, 7).Value = "Kinh";
                worksheet.Cell(8, 8).Value = "Đại học CNTT";
                worksheet.Cell(8, 9).Value = "Trung cấp";
                worksheet.Cell(8, 10).Value = "26/03/2009";
                worksheet.Cell(8, 11).Value = "12/06/2017";
                worksheet.Cell(8, 12).Value = "Đảng ủy viên, Phó Chủ tịch Ủy ban MTTQ Việt Nam phường, Bí thư Đoàn phường Cẩm Thành";
                worksheet.Cell(8, 13).Value = "Trưởng đoàn 0909390552";

                worksheet.Cell(9, 1).Value = 2;
                worksheet.Cell(9, 2).Value = "Phường Cẩm Thành";
                worksheet.Cell(9, 3).Value = "Đặng Trương Ngọc Tú";
                worksheet.Cell(9, 4).Value = 1995;
                worksheet.Cell(9, 5).Value = "";
                worksheet.Cell(9, 6).Value = "Trà Giang, Quảng Ngãi";
                worksheet.Cell(9, 7).Value = "Kinh";
                worksheet.Cell(9, 8).Value = "Đại học CSND";
                worksheet.Cell(9, 9).Value = "Trung cấp";
                worksheet.Cell(9, 10).Value = "26/03/2011";
                worksheet.Cell(9, 11).Value = "05/12/2017";
                worksheet.Cell(9, 12).Value = "Phó Bí thư Đoàn phường, Bí thư Chi đoàn Công an phường Cẩm Thành";
                worksheet.Cell(9, 13).Value = "";

                var ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;
                return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MauImportDoanVien.xlsx");
            }
        }

        [HttpPost]
        [JwtAuthorize("DoanVien")]
        public ActionResult ResetDiemDanh()
        {
            var data = _service.ResetDiemDanh();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("DoanVien")]
        public ActionResult DiemDanhById(int Id)
        {
            var data = _service.DiemDanhById(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}

