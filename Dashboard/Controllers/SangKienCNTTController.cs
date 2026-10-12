using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using ClosedXML.Excel;
using Dashboard.Common;
using Dashboard.Models.CommonModel;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;

namespace Dashboard.Controllers
{
    public class SangKienCNTTController : Controller
    {
        private readonly ISangKienCNTTService _sangKienCNTTService;
        private readonly ISangKienThanhVienCNTTService _sangKienThanhVienCNTTService;

        public SangKienCNTTController(ISangKienCNTTService sangKienCNTTService,
            ISangKienThanhVienCNTTService sangKienThanhVienCNTTService)
        {
            _sangKienCNTTService = sangKienCNTTService;
            _sangKienThanhVienCNTTService = sangKienThanhVienCNTTService;
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
        public ActionResult GetListSangKienCNTT(QueryParamViewModel queryParams)
        {
            var data = _sangKienCNTTService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public ActionResult ThemOrCapNhat(SangKienCNTTViewModel viewModel)
        {
            var identity = (ClaimsIdentity)User.Identity;
            viewModel.CurrentUserId = UserClaimHelper.GetCurrentUserId(identity);
            viewModel.IsAdmin = UserClaimHelper.GetIsSystemUser(identity);

            if (viewModel.Id != 0)
            {
                var existingItem = _sangKienCNTTService.GetById(viewModel.Id);
                if (existingItem == null || existingItem.Id == 0)
                {
                    return Json(new { Success = false, Error = "Sáng kiến CNTT không tồn tại." }, JsonRequestBehavior.AllowGet);
                }
                if (!viewModel.IsAdmin && existingItem.NguoiNopId != viewModel.CurrentUserId)
                {
                    return Json(new { Success = false, Error = "Bạn không có quyền chỉnh sửa sáng kiến này." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                viewModel.NguoiNopId = viewModel.CurrentUserId;
                viewModel.TrangThai = "Chờ duyệt";
            }

            var sangKienCNTTId = _sangKienCNTTService.ThemOrCapNhat(viewModel);
            return Json(new { Success = true, SangKienId = sangKienCNTTId }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public ActionResult Xoa(int Id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var isAdmin = UserClaimHelper.GetIsSystemUser(identity);

            var item = _sangKienCNTTService.GetById(Id);
            if (item == null)
            {
                return Json(new { Success = false, Error = "Sáng kiến CNTT không tồn tại." }, JsonRequestBehavior.AllowGet);
            }

            if (!isAdmin && item.NguoiNopId != currentUserId)
            {
                return Json(new { Success = false, Error = "Bạn không có quyền xóa sáng kiến này." }, JsonRequestBehavior.AllowGet);
            }

            _sangKienCNTTService.Xoa(Id);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public ActionResult KhoiPhuc(int Id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var isAdmin = UserClaimHelper.GetIsSystemUser(identity);

            var item = _sangKienCNTTService.GetById(Id);
            if (item == null)
            {
                return Json(new { Success = false, Error = "Sáng kiến CNTT không tồn tại." }, JsonRequestBehavior.AllowGet);
            }

            if (!isAdmin && item.NguoiNopId != currentUserId)
            {
                return Json(new { Success = false, Error = "Bạn không có quyền khôi phục sáng kiến này." }, JsonRequestBehavior.AllowGet);
            }

            _sangKienCNTTService.KhoiPhuc(Id);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
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
                var item = _sangKienCNTTService.GetById(id);
                if (item == null || item.Id == 0)
                {
                    return RedirectToAction("Index");
                }
                if (!isAdmin && item.NguoiNopId != currentUserId)
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
            var data = _sangKienCNTTService.GetById(id);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public ActionResult CapNhatTrangThai(int id, string trangThai, string ghiChu = "")
        {
            var identity = (ClaimsIdentity)User.Identity;
            var isAdmin = UserClaimHelper.GetIsSystemUser(identity);

            if (!isAdmin)
            {
                return Json(new { Success = false, Error = "Bạn không có quyền thực hiện thao tác này." }, JsonRequestBehavior.AllowGet);
            }

            var item = _sangKienCNTTService.GetById(id);
            if (item == null || item.Id == 0)
            {
                return Json(new { Success = false, Error = "Sáng kiến CNTT không tồn tại." }, JsonRequestBehavior.AllowGet);
            }

            item.TrangThai = trangThai;
            if (!string.IsNullOrEmpty(ghiChu))
            {
                item.GhiChu = ghiChu;
            }

            _sangKienCNTTService.ThemOrCapNhat(item);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("SangKien")]
        public JsonResult GetFileBySangKienCNTT(int sangKienCNTTId)
        {
            var item = _sangKienCNTTService.GetById(sangKienCNTTId);
            return Json(new { Item = item }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("SangKien")]
        public JsonResult GetThanhVienBySangKienCNTT(int sangKienCNTTId)
        {
            var data = _sangKienThanhVienCNTTService.GetBySangKienCNTT(sangKienCNTTId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("SangKien")]
        public JsonResult LuuDanhSachThanhVien(int sangKienCNTTId, List<SangKienThanhVienCNTTViewModel> danhSach)
        {
            if (danhSach == null)
            {
                danhSach = new List<SangKienThanhVienCNTTViewModel>();
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
            var result = _sangKienThanhVienCNTTService.LuuDanhSachThanhVien(sangKienCNTTId, danhSach, userId);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("SangKien")]
        public ActionResult XuatBaoCao(string namSangKien, string xepLoaiSangKien, bool? daXoa, string search)
        {
            var queryParams = new QueryParamViewModel
            {
                limit = int.MaxValue,
                offset = 0,
                NamSangKienFilter = namSangKien ?? "",
                XepLoaiSangKienFilter = xepLoaiSangKien ?? "",
                DaXoaSearch = daXoa ?? false
            };

            if (!string.IsNullOrEmpty(search))
            {
                queryParams.search = search;
            }

            var result = _sangKienCNTTService.GetAll(queryParams);
            var data = result.rows ?? new List<SangKienCNTTGridVo>();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachSangKienCNTT");

                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Tên sáng kiến";
                worksheet.Cell(1, 3).Value = "Năm sáng kiến";
                worksheet.Cell(1, 4).Value = "Người nộp";
                worksheet.Cell(1, 5).Value = "Mô tả chi tiết";
                worksheet.Cell(1, 6).Value = "File đính kèm";
                worksheet.Cell(1, 7).Value = "Xếp loại sáng kiến";

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                worksheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Column(1).Width = 8;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 25;
                worksheet.Column(5).Width = 50;
                worksheet.Column(6).Width = 40;
                worksheet.Column(7).Width = 25;

                int stt = 1;
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = stt;
                    worksheet.Cell(row, 2).Value = item.TenSangKien ?? "";
                    worksheet.Cell(row, 3).Value = item.NamSangKien ?? "";
                    worksheet.Cell(row, 4).Value = item.TenNguoiNop ?? "";
                    worksheet.Cell(row, 5).Value = item.MoTaChiTiet ?? "";

                    var danhSachFile = "";
                    if (item.DanhSachFile != null && item.DanhSachFile.Count > 0)
                    {
                        danhSachFile = string.Join("; ", item.DanhSachFile.Select(f => f.TenFile ?? ""));
                    }
                    worksheet.Cell(row, 6).Value = danhSachFile;
                    worksheet.Cell(row, 7).Value = item.XepLoaiSangKien ?? "";

                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    stt++;
                    row++;
                }

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"BaoCaoSangKienCNTT_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}

