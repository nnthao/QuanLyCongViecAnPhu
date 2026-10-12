using ClosedXML.Excel;
using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class SangKienController : Controller
    {
        private readonly ISangKienService _sangKienService;
        private readonly IPhanCongChamService _phanCongChamService;
        private readonly IUserService _userService;
        private readonly ILinhVucSangKienService _linhVucSangKienService;
        private readonly ISangKienThanhVienService _sangKienThanhVienService;

        public SangKienController(ISangKienService sangKienService,
            IPhanCongChamService phanCongChamService,
            IUserService userService,
            ILinhVucSangKienService linhVucSangKienService,
            ISangKienThanhVienService sangKienThanhVienService)
        {
            _sangKienService = sangKienService;
            _phanCongChamService = phanCongChamService;
            _userService = userService;
            _linhVucSangKienService = linhVucSangKienService;
            _sangKienThanhVienService = sangKienThanhVienService;
        }

        [JwtAuthorize("SangKien")]
        public ActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            ViewBag.IsAdmin = UserClaimHelper.GetIsSystemUser(identity);
            ViewBag.CurrentUserId = UserClaimHelper.GetCurrentUserId(identity);
            return View();
        }

        [JwtAuthorize("SangKien")]
        public ActionResult GetListSangKien(QueryParamViewModel queryParams)
        {
            var data = _sangKienService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public ActionResult ThemOrCapNhat(SangKienViewModel sangKienViewModel)
        {
            var identity = (ClaimsIdentity)User.Identity;
            sangKienViewModel.CurrentUserId = UserClaimHelper.GetCurrentUserId(identity);
            sangKienViewModel.IsAdmin = UserClaimHelper.GetIsSystemUser(identity);

            if (sangKienViewModel.Id != 0)
            {
                var existingSangKien = _sangKienService.GetById(sangKienViewModel.Id);
                if (existingSangKien == null || existingSangKien.Id == 0)
                {
                    return Json(new { Success = false, Error = "Sáng kiến không tồn tại." }, JsonRequestBehavior.AllowGet);
                }
                if (!sangKienViewModel.IsAdmin && existingSangKien.NguoiNopId != sangKienViewModel.CurrentUserId)
                {
                    return Json(new { Success = false, Error = "Bạn không có quyền chỉnh sửa sáng kiến này." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                sangKienViewModel.NguoiNopId = sangKienViewModel.CurrentUserId;
                sangKienViewModel.TrangThai = "Chờ duyệt";
            }

            var data = _sangKienService.ThemOrCapNhat(sangKienViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public ActionResult Xoa(int Id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var isAdmin = UserClaimHelper.GetIsSystemUser(identity);

            var sangKien = _sangKienService.GetById(Id);
            if (sangKien == null)
            {
                return Json(new { Success = false, Error = "Sáng kiến không tồn tại." }, JsonRequestBehavior.AllowGet);
            }

            if (!isAdmin && sangKien.NguoiNopId != currentUserId)
            {
                return Json(new { Success = false, Error = "Bạn không có quyền xóa sáng kiến này." }, JsonRequestBehavior.AllowGet);
            }

            var data = _sangKienService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public ActionResult KhoiPhuc(int Id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var isAdmin = UserClaimHelper.GetIsSystemUser(identity);

            var sangKien = _sangKienService.GetById(Id);
            if (sangKien == null)
            {
                return Json(new { Success = false, Error = "Sáng kiến không tồn tại." }, JsonRequestBehavior.AllowGet);
            }

            if (!isAdmin && sangKien.NguoiNopId != currentUserId)
            {
                return Json(new { Success = false, Error = "Bạn không có quyền khôi phục sáng kiến này." }, JsonRequestBehavior.AllowGet);
            }

            var data = _sangKienService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("SangKien")]
        public ActionResult Form(int id = 0)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var isAdmin = UserClaimHelper.GetIsSystemUser(identity);
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);

            ViewBag.IsAdmin = isAdmin;

            if (id > 0)
            {
                var sangKien = _sangKienService.GetById(id);
                if (sangKien == null || sangKien.Id == 0)
                {
                    return RedirectToAction("Index");
                }
                if (!isAdmin && sangKien.NguoiNopId != currentUserId)
                {
                    TempData["Error"] = "Bạn không có quyền chỉnh sửa sáng kiến này.";
                    return RedirectToAction("Index");
                }
            }

            return View(id);
        }

        [JwtAuthorize("SangKien")]
        public JsonResult GetById(int id)
        {
            var data = _sangKienService.GetById(id);
            return Json(new
            {
                Item = data
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult SangKienLookup()
        {
            var data = _sangKienService.SangKienLookup(new LookupQueryViewModel());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("SangKien")]
        public JsonResult GetFileDinhKemById(int id)
        {
            var data = _sangKienService.GetFileDinhKemById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("SangKien")]
        public JsonResult GetLinhVucSangKienLookup()
        {
            var data = _linhVucSangKienService.LinhVucLookup();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("SangKien")]
        public JsonResult GetNguoiChamLookup()
        {
            var data = _userService.UserLookup(new LookupQueryViewModel { isGetAll = true });
            return Json(data.Data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("SangKien")]
        public JsonResult GetPhanCongBySangKien(int sangKienId)
        {
            if (!UserClaimHelper.GetIsSystemUser((ClaimsIdentity)User.Identity))
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            var data = _phanCongChamService.GetBySangKien(sangKienId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public JsonResult ThemPhanCong(int sangKienId, string ghiChu, int[] userIds)
        {
            if (!UserClaimHelper.GetIsSystemUser((ClaimsIdentity)User.Identity))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            if (userIds == null || userIds.Length == 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            foreach (var userId in userIds)
            {
                _phanCongChamService.Them(new PhanCongChamViewModel
                {
                    SangKienId = sangKienId,
                    UserId = userId,
                    GhiChu = ghiChu
                });
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public JsonResult XoaPhanCong(int id)
        {
            if (!UserClaimHelper.GetIsSystemUser((ClaimsIdentity)User.Identity))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var data = _phanCongChamService.Xoa(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public JsonResult CapNhatTrangThai(int id, string trangThai, string ghiChu, decimal? tongDiem = null)
        {
            if (!UserClaimHelper.GetIsSystemUser((ClaimsIdentity)User.Identity))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var data = _sangKienService.CapNhatTrangThai(id, trangThai, ghiChu, tongDiem);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("SangKien")]
        public JsonResult GetThanhVienBySangKien(int sangKienId)
        {
            var data = _sangKienThanhVienService.GetBySangKien(sangKienId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult GetThanhVienLookup()
        {
            var data = _userService.UserLookup(new LookupQueryViewModel { isHideOptionChonValueZero = true });
            return Json(data.Data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public JsonResult ThemThanhVien(SangKienThanhVienViewModel viewModel)
        {
            var tongHienTai = _sangKienThanhVienService.TinhTongTiLeDongGop(viewModel.SangKienId);
            if (tongHienTai + viewModel.TiLeDongGop > 100)
            {
                return Json(new { Success = false, Error = "Tổng tỉ lệ đóng góp không được vượt quá 100%" }, JsonRequestBehavior.AllowGet);
            }

            var result = _sangKienThanhVienService.Them(viewModel);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public JsonResult CapNhatThanhVien(SangKienThanhVienViewModel viewModel)
        {
            var tongHienTai = _sangKienThanhVienService.TinhTongTiLeDongGop(viewModel.SangKienId, viewModel.Id);
            if (tongHienTai + viewModel.TiLeDongGop > 100)
            {
                return Json(new { Success = false, Error = "Tổng tỉ lệ đóng góp không được vượt quá 100%" }, JsonRequestBehavior.AllowGet);
            }

            var result = _sangKienThanhVienService.CapNhat(viewModel);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public JsonResult XoaThanhVien(int id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var userId = UserClaimHelper.GetCurrentUserId(identity);
            var result = _sangKienThanhVienService.Xoa(id, userId);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public JsonResult LuuDanhSachThanhVien(int sangKienId, List<SangKienThanhVienViewModel> danhSach)
        {
            if (danhSach == null)
            {
                danhSach = new List<SangKienThanhVienViewModel>();
            }

            decimal tongTiLe = 0;
            foreach (var item in danhSach)
            {
                tongTiLe += item.TiLeDongGop;
            }

            if (tongTiLe > 100)
            {
                return Json(new { Success = false, Error = "Tổng tỉ lệ đóng góp không được vượt quá 100%" }, JsonRequestBehavior.AllowGet);
            }

            var identity = (ClaimsIdentity)User.Identity;
            var userId = UserClaimHelper.GetCurrentUserId(identity);
            var result = _sangKienThanhVienService.LuuDanhSachThanhVien(sangKienId, danhSach, userId);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("SangKien")]
        public ActionResult XuatBaoCao(int? dotXetDuyetId, int? linhVucSangKienId, string namXetDuyet, string trangThaiDuyet, bool? daXoa, string search)
        {
            var queryParams = new QueryParamViewModel
            {
                limit = int.MaxValue,
                offset = 0,
                DotXetDuyetIdFilter = dotXetDuyetId ?? 0,
                LinhVucSangKienIdFilter = linhVucSangKienId ?? 0,
                NamXetDuyetFilter = namXetDuyet ?? "",
                TrangThaiDuyetFilter = trangThaiDuyet ?? "",
                DaXoaSearch = daXoa ?? false
            };

            if (!string.IsNullOrEmpty(search))
            {
                queryParams.search = search;
            }

            var result = _sangKienService.GetAll(queryParams);
            var data = result.rows ?? new List<SangKienGridVo>();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachSangKien");

                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Đợt xét duyệt";
                worksheet.Cell(1, 3).Value = "Năm xét duyệt";
                worksheet.Cell(1, 4).Value = "Lĩnh vực";
                worksheet.Cell(1, 5).Value = "Người nộp";
                worksheet.Cell(1, 6).Value = "Mô tả chi tiết";
                worksheet.Cell(1, 7).Value = "File Sáng Kiến";
                worksheet.Cell(1, 8).Value = "Giám khảo được phân công chấm";
                worksheet.Cell(1, 9).Value = "Điểm trung bình";
                worksheet.Cell(1, 10).Value = "Trạng Thái Chấm Điểm";
                worksheet.Cell(1, 11).Value = "Trạng thái sáng kiến";

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                worksheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Column(1).Width = 8;
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 25;
                worksheet.Column(5).Width = 25;
                worksheet.Column(6).Width = 50;
                worksheet.Column(7).Width = 40;
                worksheet.Column(8).Width = 40;
                worksheet.Column(9).Width = 15;
                worksheet.Column(10).Width = 25;
                worksheet.Column(11).Width = 25;

                int stt = 1;
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = stt;
                    worksheet.Cell(row, 2).Value = item.TenDotXetDuyet ?? "";
                    worksheet.Cell(row, 3).Value = item.NamXetDuyet ?? "";
                    worksheet.Cell(row, 4).Value = item.TenLinhVuc ?? "";
                    worksheet.Cell(row, 5).Value = item.TenNguoiNop ?? "";
                    worksheet.Cell(row, 6).Value = item.MoTaChiTiet ?? "";

                    var danhSachFile = "";
                    if (item.DanhSachFile != null && item.DanhSachFile.Count > 0)
                    {
                        danhSachFile = string.Join("; ", item.DanhSachFile.Select(f => f.TenFile ?? ""));
                    }
                    worksheet.Cell(row, 7).Value = danhSachFile;

                    var danhSachGiamKhao = "";
                    if (item.DanhSachGiamKhao != null && item.DanhSachGiamKhao.Count > 0)
                    {
                        danhSachGiamKhao = string.Join("; ", item.DanhSachGiamKhao.Select(g => g.TenNguoiCham ?? ""));
                    }
                    worksheet.Cell(row, 8).Value = danhSachGiamKhao;

                    var diemTrungBinh = "";
                    if (item.DiemTrungBinhTong.HasValue)
                    {
                        diemTrungBinh = item.DiemTrungBinhTong.Value.ToString("F2");
                    }
                    worksheet.Cell(row, 9).Value = diemTrungBinh;

                    var trangThaiChamDiem = "";
                    if (item.TrangThaiChamDiem == "ChuaCham")
                    {
                        trangThaiChamDiem = "Chưa chấm";
                    }
                    else if (item.TrangThaiChamDiem == "DangCham")
                    {
                        trangThaiChamDiem = "Đang chấm";
                    }
                    else if (item.TrangThaiChamDiem == "DaCham")
                    {
                        trangThaiChamDiem = "Đã chấm";
                    }
                    worksheet.Cell(row, 10).Value = trangThaiChamDiem;

                    worksheet.Cell(row, 11).Value = item.TrangThai ?? "";

                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    stt++;
                    row++;
                }

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"BaoCaoSangKien_{System.DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}

