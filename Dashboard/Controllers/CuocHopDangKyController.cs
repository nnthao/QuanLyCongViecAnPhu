using ClosedXML.Excel;
using Dashboard.AdoNetHelper;
using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class CuocHopDangKyController : Controller
    {
        private readonly ICuocHopService _cuocHopService;
        private readonly IUserService _userService;
        private readonly ILyDoService _lyDoService;
        private readonly IPhongBanService _phongBanService;

        public CuocHopDangKyController(ICuocHopService cuocHopService,
            IUserService userService, ILyDoService lyDoService, IPhongBanService phongBanService)
        {
            _cuocHopService = cuocHopService;
            _userService = userService;
            _lyDoService = lyDoService;
            _phongBanService = phongBanService;
        }

        [JwtAuthorize("CuocHopDangKy")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("CuocHopDangKy")]
        public ActionResult GetAll(CuocHopQueryParamViewModel queryParams)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenCRUDTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenCRUDTatCaPhongBan.ToString());

            if (!isSystemUser && !isQuyenCRUDTatCaPhongBan)
            {
                queryParams.UserId = UserClaimHelper.GetCurrentUserId(identity);
            }

            var data = _cuocHopService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("CuocHopDangKy")]
        public ActionResult ThemOrCapNhat(CuocHopViewModel cuocHopViewModel)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenCRUDTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenCRUDTatCaPhongBan.ToString());

            cuocHopViewModel.NguoiTaoId = currentUserId;
            cuocHopViewModel.CoQuanId = UserClaimHelper.GetPhongBanId(identity);
            cuocHopViewModel.IsAddOrUpdate = true;

            if (cuocHopViewModel.Id == 0)
            {
                var dataAdd = _cuocHopService.Them(cuocHopViewModel);
                return Json(dataAdd, JsonRequestBehavior.AllowGet);
            }

            var cuocHop = _cuocHopService.GetCuocHopInfoFromId(cuocHopViewModel.Id);
            if (cuocHop == null || cuocHop.Id == 0)
            {
                return Json(new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "Id",
                            Description = "Cuộc họp không tồn tại."
                        }
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            if (!isSystemUser && !isQuyenCRUDTatCaPhongBan && cuocHop.NguoiTaoId.GetValueOrDefault() != currentUserId)
            {
                return Json(new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "Permission",
                            Description = "Bạn không có quyền cập nhật cuộc họp này. Chỉ người tạo cuộc họp mới được phép cập nhật."
                        }
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            var data = _cuocHopService.CapNhat(cuocHopViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("CuocHopDangKy")]
        public ActionResult Xoa(int Id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenCRUDTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenCRUDTatCaPhongBan.ToString());

            var cuocHop = _cuocHopService.GetCuocHopInfoFromId(Id);
            if (cuocHop == null || cuocHop.Id == 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (!isSystemUser && !isQuyenCRUDTatCaPhongBan && cuocHop.NguoiTaoId.GetValueOrDefault() != currentUserId)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var data = _cuocHopService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("CuocHopDangKy")]
        public ActionResult KhoiPhuc(int Id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenCRUDTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenCRUDTatCaPhongBan.ToString());

            var cuocHop = _cuocHopService.GetCuocHopInfoFromId(Id);
            if (cuocHop == null || cuocHop.Id == 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (!isSystemUser && !isQuyenCRUDTatCaPhongBan && cuocHop.NguoiTaoId.GetValueOrDefault() != currentUserId)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var data = _cuocHopService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("CuocHopDangKy")]
        public ActionResult GuiLaiPhieu(CuocHopViewModel cuocHopViewModel)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenCRUDTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenCRUDTatCaPhongBan.ToString());

            var cuocHop = _cuocHopService.GetCuocHopInfoFromId(cuocHopViewModel.Id);
            if (cuocHop == null || cuocHop.Id == 0)
            {
                return Json(new { Error = "Cuộc họp không tồn tại." }, JsonRequestBehavior.AllowGet);
            }

            if (!isSystemUser && !isQuyenCRUDTatCaPhongBan && cuocHop.NguoiTaoId.GetValueOrDefault() != currentUserId)
            {
                return Json(new { Error = "Bạn không có quyền gửi lại phiếu cuộc họp này." }, JsonRequestBehavior.AllowGet);
            }

            cuocHopViewModel.CurrentUserId = currentUserId;
            var data = _cuocHopService.GuiLaiPhieu(cuocHopViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("CuocHopDangKy")]
        public JsonResult GetTaiLieuCuocHop(int id)
        {
            var data = _cuocHopService.GetTaiLieuCuocHop(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("CuocHopDangKy")]
        public ActionResult DownloadTaiLieuCuocHop(int id)
        {
            var data = _cuocHopService.DownloadTaiLieuCuocHop(id);
            if (data != null)
            {
                return Json(data.DuongDan, JsonRequestBehavior.AllowGet);
            }

            throw new Exception("File đã bị xóa");
        }

        [JwtAuthorize("CuocHopDangKy")]
        public JsonResult XoaTaiLieuCuocHop(int id)
        {
            return Json(_cuocHopService.XoaTaiLieuCuocHop(id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("CuocHopDangKy")]
        public JsonResult ImportFileNoiDungCuocHop()
        {
            if (Request.Files != null && Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file != null)
                {
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

                    var extension = Path.GetExtension(fileName);
                    var fileNameTemp = $"ImportNoiDung_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var filePathTemp = Path.Combine(fullPath, fileNameTemp);
                    file.SaveAs(filePathTemp);

                    if (System.IO.File.Exists(filePathTemp))
                    {
                        try
                        {
                            using (XLWorkbook workBook = new XLWorkbook(filePathTemp))
                            {
                                IXLWorksheet workSheet = workBook.Worksheet(1);
                                return Json(ImportNoiDungCuocHopHelper.ReadDataExcel(workSheet), JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch (Exception)
                        {
                            return Json(new
                            {
                                Errors = new List<ValidateError>
                                {
                                    new ValidateError
                                    {
                                        Id = "NoiDungCuocHop",
                                        Description = "Lỗi import file."
                                    }
                                }
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("CuocHopDangKy")]
        public ActionResult ImportLichTuExcel(int? lyDoId)
        {
            if (Request.Files != null && Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["file"] ?? Request.Files[0];
                if (file != null)
                {
                    try
                    {
                        FileHelper.ValidateFileExtension(file);
                    }
                    catch (ArgumentException ex)
                    {
                        return Json(new
                        {
                            Errors = new List<ValidateError>
                            {
                                new ValidateError
                                {
                                    Id = "ImportLich",
                                    Description = ex.Message
                                }
                            }
                        }, JsonRequestBehavior.AllowGet);
                    }

                    var fileName = file.FileName;
                    var fullPath = System.Web.HttpContext.Current.Server.MapPath(BienDungChung.FolderSavePath);
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }

                    var extension = Path.GetExtension(fileName);
                    var fileNameTemp = $"ImportLichHop_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var filePathTemp = Path.Combine(fullPath, fileNameTemp);
                    file.SaveAs(filePathTemp);

                    if (System.IO.File.Exists(filePathTemp))
                    {
                        try
                        {
                            using (XLWorkbook workBook = new XLWorkbook(filePathTemp))
                            {
                                IXLWorksheet workSheet = workBook.Worksheet(1);
                                var defaultLyDo = lyDoId.GetValueOrDefault();
                                var danhSachCuocHop = ImportLichCuocHopHelper.ReadDataExcel(workSheet, defaultLyDo);

                                if (danhSachCuocHop == null || danhSachCuocHop.Count == 0)
                                {
                                    return Json(new
                                    {
                                        Errors = new List<ValidateError>
                                        {
                                            new ValidateError
                                            {
                                                Id = "ImportLich",
                                                Description = "Không tìm thấy dữ liệu lịch họp trong file. Vui lòng kiểm tra lại cấu trúc file mẫu."
                                            }
                                        }
                                    }, JsonRequestBehavior.AllowGet);
                                }

                                var identity = (ClaimsIdentity)User.Identity;
                                var request = new ImportLichCuocHopRequestViewModel
                                {
                                    LyDoId = lyDoId,
                                    UserId = UserClaimHelper.GetCurrentUserId(identity),
                                    PhongBanId = UserClaimHelper.GetPhongBanId(identity),
                                    DanhSachCuocHop = danhSachCuocHop
                                };

                                var ketQua = _cuocHopService.ImportDanhSachLich(request);
                                return Json(ketQua, JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new
                            {
                                Errors = new List<ValidateError>
                                {
                                    new ValidateError
                                    {
                                        Id = "ImportLich",
                                        Description = "Lỗi đọc file Excel: " + ex.Message
                                    }
                                }
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

            return Json(new
            {
                Errors = new List<ValidateError>
                {
                    new ValidateError
                    {
                        Id = "ImportLich",
                        Description = "Vui lòng chọn file để import."
                    }
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("CuocHopDangKy")]
        public ActionResult TaiFileMau()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("LichTuan");

                worksheet.Cell(1, 1).Value = "LỊCH TUẦN - VNPT Quảng Ngãi (28/09/2026-04/10/2026)";
                worksheet.Range(1, 1, 1, 7).Merge();
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Font.FontSize = 13;
                worksheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Row(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Row(1).Height = 28;

                worksheet.Cell(2, 1).Value = "";
                worksheet.Cell(2, 2).Value = "Thời gian";
                worksheet.Cell(2, 3).Value = "Nội dung";
                worksheet.Cell(2, 4).Value = "Chủ trì";
                worksheet.Cell(2, 5).Value = "Thành phần";
                worksheet.Cell(2, 6).Value = "Địa điểm";
                worksheet.Cell(2, 7).Value = "Ghi chú";

                var headerRange = worksheet.Range(2, 1, 2, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F81BD");
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Row(2).Height = 24;

                worksheet.Cell(3, 1).Value = "*****";
                worksheet.Range(3, 1, 3, 7).Merge();
                worksheet.Row(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(4, 1).Value = "Thứ 2, 28/09/2026";
                worksheet.Range(4, 1, 4, 7).Merge();
                worksheet.Row(4).Style.Font.Bold = true;
                worksheet.Row(4).Style.Fill.BackgroundColor = XLColor.FromHtml("#E7EEF8");

                worksheet.Cell(5, 1).Value = "Sáng";
                worksheet.Cell(5, 2).Value = "08:00 - 11:00";
                worksheet.Cell(5, 3).Value = "Họp triển khai công tác chuyển đổi số và ứng dụng CNTT tháng 10/2026";
                worksheet.Cell(5, 4).Value = "";
                worksheet.Cell(5, 5).Value = "Ban Giám đốc và lãnh đạo các phòng ban";
                worksheet.Cell(5, 6).Value = "Phòng họp số 1 VNPT Quảng Ngãi";
                worksheet.Cell(5, 7).Value = "";

                worksheet.Cell(6, 1).Value = "";
                worksheet.Cell(6, 2).Value = "09:00 - 11:00";
                worksheet.Cell(6, 3).Value = "Giao ban kỹ thuật mạng lưới";
                worksheet.Cell(6, 4).Value = "";
                worksheet.Cell(6, 5).Value = "Phòng Kỹ thuật - Đầu tư";
                worksheet.Cell(6, 6).Value = "Phòng họp số 2 VNPT Quảng Ngãi";
                worksheet.Cell(6, 7).Value = "";

                worksheet.Cell(7, 1).Value = "Chiều";
                worksheet.Cell(7, 2).Value = "14:00 - 17:00";
                worksheet.Cell(7, 3).Value = "Hội nghị trực tuyến triển khai kế hoạch sản xuất kinh doanh";
                worksheet.Cell(7, 4).Value = "";
                worksheet.Cell(7, 5).Value = "Lãnh đạo đơn vị và cán bộ chủ chốt";
                worksheet.Cell(7, 6).Value = "Phòng họp trực tuyến VNPT Quảng Ngãi";
                worksheet.Cell(7, 7).Value = "";

                worksheet.Cell(8, 1).Value = "Thứ 3, 29/09/2026";
                worksheet.Range(8, 1, 8, 7).Merge();
                worksheet.Row(8).Style.Font.Bold = true;
                worksheet.Row(8).Style.Fill.BackgroundColor = XLColor.FromHtml("#E7EEF8");

                worksheet.Cell(9, 1).Value = "Sáng";
                worksheet.Cell(9, 2).Value = "08:00 - 11:00";
                worksheet.Cell(9, 3).Value = "Họp Đảng ủy VNPT Quảng Ngãi, giao ban Quý III, 9 tháng đầu năm, triển khai nhiệm vụ Quý IV";
                worksheet.Cell(9, 4).Value = "Bí thư Đảng ủy, Giám đốc VNPT Quảng Ngãi";
                worksheet.Cell(9, 5).Value = "Các đ/c Phó Giám đốc; Trưởng phó các phòng ban, trung tâm";
                worksheet.Cell(9, 6).Value = "Hội trường tầng 3 VNPT Quảng Ngãi";
                worksheet.Cell(9, 7).Value = "";

                var allData = worksheet.Range(2, 1, 9, 7);
                allData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                allData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Column(1).Width = 10;
                worksheet.Column(2).Width = 18;
                worksheet.Column(3).Width = 45;
                worksheet.Column(4).Width = 30;
                worksheet.Column(5).Width = 35;
                worksheet.Column(6).Width = 30;
                worksheet.Column(7).Width = 20;

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Mau_Import_LichHop.xlsx");
            }
        }

        [JwtAuthorize("CuocHopDangKy")]
        public ActionResult Form(int id = 0)
        {
            return View(new ParamControllerToView
            {
                Id = id,
                UserCurrentId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity),
            });
        }

        [JwtAuthorize("CuocHopDangKy")]
        public ActionResult GetById(int id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var currentPhongBanId = UserClaimHelper.GetPhongBanId(identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenGiaoViecTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenGiaoViecTatCaPhongBan.ToString());
            var isQuyenCRUDTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenCRUDTatCaPhongBan.ToString());
            CuocHopGridVo cuocHop = null;

            if (id != 0)
            {
                cuocHop = _cuocHopService.GetCuocHopInfoFromId(id);

                if (cuocHop == null || cuocHop.Id == 0)
                {
                    return Json(new
                    {
                        Error = "Cuộc họp không tồn tại."
                    }, JsonRequestBehavior.AllowGet);
                }

                if (!isSystemUser && !isQuyenCRUDTatCaPhongBan && cuocHop.NguoiTaoId.GetValueOrDefault() != currentUserId && currentUserId != 0)
                {
                    return Json(new
                    {
                        Error = "Bạn không có quyền xem/cập nhật cuộc họp này. Chỉ người tạo cuộc họp mới được phép."
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            List<int> phongBanIds = null;
            if (!isSystemUser && !isQuyenGiaoViecTatCaPhongBan && !isQuyenCRUDTatCaPhongBan && currentPhongBanId != 0)
            {
                phongBanIds = new List<int> { currentPhongBanId };
                phongBanIds.AddRange(UltiHelper.GetPhongBanChildrenIds(currentPhongBanId));
            }

            var userAlls = _userService.UserLookup(new LookupQueryViewModel
            {
                tenKemTen = true,
                phongBanIds = phongBanIds
            });
            var phongBans = _phongBanService.PhongBanLookup(new LookupQueryViewModel
            {
                isHideOptionChonValueZero = true
            });

            if (phongBanIds != null)
            {
                phongBans.Data = phongBans.Data.Where(o => phongBanIds.Contains(o.id)).ToList();
            }
            var lyDos = _lyDoService.LyDoLookup(new LookupQueryViewModel());

            return Json(new
            {
                Item = cuocHop,
                User = userAlls,
                PhongBan = phongBans,
                LyDo = lyDos,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
