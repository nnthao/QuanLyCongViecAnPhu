using ClosedXML.Excel;
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
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IVaiTroService _vaiTroService;
        private readonly IPhongBanService _phongBanService;
        private readonly IChucDanhService _chucDanhService;

        public UserController(IUserService userService, IVaiTroService vaiTroService,
            IPhongBanService phongBanService, IChucDanhService chucDanhService)
        {
            _userService = userService;
            _vaiTroService = vaiTroService;
            _phongBanService = phongBanService;
            _chucDanhService = chucDanhService;
        }

        [JwtAuthorize("User")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("User")]
        public ActionResult GetAll(UserQueryViewModel queryParams)
        {
            var data = _userService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("User")]
        public ActionResult ThemOrCapNhat(UserViewModel userViewModel)
        {
            dynamic data;
            if (userViewModel.Id == 0)
            {
                data = _userService.Them(userViewModel);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            data = _userService.CapNhat(userViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("User")]
        public ActionResult Xoa(int Id)
        {
            var data = _userService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("User")]
        public ActionResult XoaKhoiDatabase(int Id)
        {
            var data = _userService.XoaKhoiDatabase(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("User")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _userService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("User")]
        public ActionResult MoKhoaTaiKhoan(int Id)
        {
            var data = _userService.MoKhoaTaiKhoan(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("User")]
        public ActionResult ResetMatKhau(int Id)
        {
            var data = _userService.ResetMatKhau(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("User")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("User")]
        public ActionResult GetUserById(int id = 0)
        {
            var user = _userService.GetById(id);
            var roles = _vaiTroService.VaiTroLookup();
            var phongBan = _phongBanService.PhongBanLookup(new LookupQueryViewModel());
            var chucDanh = _chucDanhService.ChucDanhLookup(new LookupQueryViewModel());
            return Json(new { Item = user, Roles = roles, PhongBan = phongBan, ChucDanh = chucDanh }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("User")]
        public ActionResult ImportDanhSach()
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
                        return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                    }

                    var fileName = file.FileName;
                    var fullPath = System.Web.HttpContext.Current.Server.MapPath(BienDungChung.FolderSavePath);
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }

                    var extension = Path.GetExtension(fileName);
                    var fileNameTemp = $"ImportUser_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var filePathTemp = Path.Combine(fullPath, fileNameTemp);
                    file.SaveAs(filePathTemp);

                    if (System.IO.File.Exists(filePathTemp))
                    {
                        try
                        {
                            using (XLWorkbook workBook = new XLWorkbook(filePathTemp))
                            {
                                IXLWorksheet workSheet = workBook.Worksheet(1);
                                var danhSachUser = ImportUserHelper.ReadDataExcel(workSheet);
                                var ketQua = _userService.ImportDanhSach(danhSachUser);
                                return Json(ketQua, JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new
                            {
                                Errors = new List<ValidateError> {
                                    new ValidateError
                                    {
                                        Id = "ImportUser",
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
                Errors = new List<ValidateError> {
                    new ValidateError
                    {
                        Id = "ImportUser",
                        Description = "Vui lòng chọn file để import."
                    }
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("User")]
        public ActionResult TaiFileMau()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachUser");
                
                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Họ tên";
                worksheet.Cell(1, 3).Value = "Chức danh";
                worksheet.Cell(1, 4).Value = "Email";
                worksheet.Cell(1, 5).Value = "Số điện thoại";
                worksheet.Cell(1, 6).Value = "Phòng ban";
                worksheet.Cell(1, 7).Value = "Vai Trò";

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.BackgroundColor = XLColor.LightBlue;

                worksheet.Column(1).Width = 10;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 30;
                worksheet.Column(4).Width = 30;
                worksheet.Column(5).Width = 20;
                worksheet.Column(6).Width = 40;
                worksheet.Column(7).Width = 30;

                worksheet.Cell(2, 1).Value = 1;
                worksheet.Cell(2, 2).Value = "Nguyễn Văn A";
                worksheet.Cell(2, 3).Value = "Chuyên viên";
                worksheet.Cell(2, 4).Value = "nguyenvana@vnpt.vn";
                worksheet.Cell(2, 5).Value = "0912345678";
                worksheet.Cell(2, 6).Value = "Trung tâm Kinh doanh Giải pháp";
                worksheet.Cell(2, 7).Value = "Lãnh đạo phòng";

                var stream = new System.IO.MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Mau_Import_User.xlsx");
            }
        }

        [HttpPost]
        [JwtAuthorize("User")]
        public ActionResult CapNhatSoDienThoaiTuExcel()
        {
            if (Request.Files != null && Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["file"] ?? Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        FileHelper.ValidateFileExtension(file);
                    }
                    catch (ArgumentException ex)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = ex.Message,
                            Errors = new List<ValidateError>
                            {
                                new ValidateError
                                {
                                    Id = "ImportPhone",
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
                    var fileNameTemp = $"ImportPhone_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
                    var filePathTemp = Path.Combine(fullPath, fileNameTemp);
                    file.SaveAs(filePathTemp);

                    if (System.IO.File.Exists(filePathTemp))
                    {
                        try
                        {
                            using (XLWorkbook workBook = new XLWorkbook(filePathTemp))
                            {
                                IXLWorksheet workSheet = workBook.Worksheet(1);
                                var danhSach = ImportUserSoDienThoaiHelper.ReadDataExcel(workSheet);
                                var ketQua = _userService.CapNhatSoDienThoaiTuExcel(danhSach);
                                return Json(ketQua, JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "Lỗi đọc file Excel: " + ex.Message,
                                Errors = new List<ValidateError>
                                {
                                    new ValidateError
                                    {
                                        Id = "ImportPhone",
                                        Description = "Lỗi đọc file Excel: " + ex.Message
                                    }
                                }
                            }, JsonRequestBehavior.AllowGet);
                        }
                        finally
                        {
                            try
                            {
                                if (System.IO.File.Exists(filePathTemp))
                                {
                                    System.IO.File.Delete(filePathTemp);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }

            return Json(new
            {
                Success = false,
                Message = "Vui lòng chọn file Excel để cập nhật số điện thoại.",
                Errors = new List<ValidateError>
                {
                    new ValidateError
                    {
                        Id = "ImportPhone",
                        Description = "Vui lòng chọn file Excel để cập nhật số điện thoại."
                    }
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("User")]
        public ActionResult TaiFileMauCapNhatSoDienThoai()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("CapNhatSoDienThoai");

                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Họ và tên";
                worksheet.Cell(1, 3).Value = "Cơ quan, đơn vị công tác";
                worksheet.Cell(1, 4).Value = "Thư điện tử công vụ";
                worksheet.Cell(1, 5).Value = "Số điện thoại sử dụng Zalo";

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.BackgroundColor = XLColor.LightBlue;

                worksheet.Column(1).Width = 10;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 35;
                worksheet.Column(4).Width = 35;
                worksheet.Column(5).Width = 25;

                worksheet.Cell(2, 1).Value = 1;
                worksheet.Cell(2, 2).Value = "Nguyễn Văn A";
                worksheet.Cell(2, 3).Value = "Sở Văn hóa Thể thao và Du lịch";
                worksheet.Cell(2, 4).Value = "nguyenvana@quangngai.gov.vn";
                worksheet.Cell(2, 5).Value = "0912345678";

                var stream = new System.IO.MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Mau_Cap_Nhat_So_Dien_Thoai.xlsx");
            }
        }
    }
}