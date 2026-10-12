using Dashboard.AdoNetHelper;
using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Dashboard.Services
{
    public interface ICuocHopService
    {
        dynamic CapNhat(CuocHopViewModel cuocHopViewModel);
        dynamic CapNhatThongTinPhongThoiGian(CuocHopViewModel cuocHopViewModel);
        FileDinhKemGridVo DownloadTaiLieuCuocHop(int id);
        dynamic DuyetPhieu(CuocHopViewModel cuocHopViewModel);
        DataTableResult<CuocHopGridVo> GetAll(CuocHopQueryParamViewModel queryParams);
        CuocHopGridVo GetCuocHopInfoFromId(int id);
        List<CuocHopGridVo> GetCuocHopTrongDanhSachIds(List<int> cuocHopIds);
        List<NgaySscheduler> GetLichHopTheoThang(QueryParamViewModel queryParams);
        List<NgaySscheduler> GetLichHopTheoTuan(QueryParamViewModel queryParams);
        dynamic GetTaiLieuCuocHop(int id);
        dynamic GuiLaiPhieu(CuocHopViewModel cuocHopViewModel);
        dynamic ImportDanhSachLich(ImportLichCuocHopRequestViewModel request);
        bool KhoiPhuc(int id);
        List<ValidateError> KiemTraLoiKhiDieuPhoiPhong(CuocHopViewModel cuocHopViewModel);
        bool KiemTraThoiGianThayDoiKhiDieuPhoiPhong(CuocHopViewModel cuocHopViewModel);
        dynamic Them(CuocHopViewModel cuocHopViewModel);
        dynamic TraPhieu(CuocHopViewModel cuocHopViewModel);
        bool ThongBaoLaiLichHop(int cuocHopId);
        bool Xoa(int id);
        dynamic XoaTaiLieuCuocHop(int id);
        FileDinhKemGridVo XuatFile(FileExportVm cuocHopExport);
        FileDinhKemGridVo XuatFilePdf(string fileExcel);
    }

    public class CuocHopService : ICuocHopService
    {
        private readonly ICuocHopRepository _cuocHopRepository;

        public CuocHopService(ICuocHopRepository cuocHopRepository)
        {
            _cuocHopRepository = cuocHopRepository;
        }

        public DataTableResult<CuocHopGridVo> GetAll(CuocHopQueryParamViewModel queryParams)
        {
            var data = _cuocHopRepository.GetAll(queryParams);
            MapTenNguoiDungVaoThongTinCuocHop(data.rows);
            MapThongTinCacCuocHopBiTrung(data.rows, queryParams);
            MapTaiLieuCuocHop(data.rows);

            return data;
        }

        private void MapTaiLieuCuocHop(List<CuocHopGridVo> results)
        {
            var cuocHopIds = results.Select(o => o.Id).ToList();
            if (cuocHopIds.Count > 0)
            {
                var taiLieus = _cuocHopRepository.GetTaiLieuCuocHopByCuocHopIds(cuocHopIds);
                var taiLieuDict = taiLieus.GroupBy(o => o.CuocHopId).ToDictionary(g => g.Key, g => g.ToList());

                foreach (var item in results)
                {
                    if (taiLieuDict.ContainsKey(item.Id))
                    {
                        item.TaiLieuCuocHops = taiLieuDict[item.Id];
                    }
                    else
                    {
                        item.TaiLieuCuocHops = new List<TaiLieuCuocHopGridVo>();
                    }
                }
            }
        }

        private void MapThongTinCacCuocHopBiTrung(List<CuocHopGridVo> datas, CuocHopQueryParamViewModel queryParams)
        {
            if (queryParams.IsDieuPhieuCuocHopQuery == true)
            {
                var cuocHopTrungIdStrs = datas.Where(o => !string.IsNullOrEmpty(o.CuocHopTrungId))
                    .Select(o => o.CuocHopTrungId.Trim());

                var cuocHopTrungIds = cuocHopTrungIdStrs.Where(o => !string.IsNullOrEmpty(o))
                    .SelectMany(s => s.Split(',').Select(int.Parse)).Distinct().ToList();

                var danhSachCuocHopTrungInfo = _cuocHopRepository.LayCuocHopTuDanhDachIds(cuocHopTrungIds);

                foreach (var item in datas)
                {
                    if (!string.IsNullOrEmpty(item.CuocHopTrungId))
                    {
                        var itemCuocHopTrungIds = item.CuocHopTrungId.Split(',').Select(o => int.Parse(o)).ToList();
                        item.CuocHopTrung = danhSachCuocHopTrungInfo.Where(o => itemCuocHopTrungIds.Contains(o.Id))
                            .Select(s => new CuocHopGridVo
                            {
                                Id = s.Id,
                                TenCuocHop = s.TenCuocHop,
                            }).ToList();
                    }
                }
            }
        }

        public List<NgaySscheduler> GetLichHopTheoTuan(QueryParamViewModel queryParams)
        {
            var data = _cuocHopRepository.GetAllCuocHops(queryParams);
            MapTenNguoiDungVaoThongTinCuocHopChoLichHopSangChieu(data);
            return data;
        }

        private void MapTenNguoiDungVaoThongTinCuocHopChoLichHopSangChieu(List<NgaySscheduler> data)
        {
            foreach (var item in data)
            {
                MapTenNguoiDungVaoThongTinCuocHop(item.Sangs);
                MapTenNguoiDungVaoThongTinCuocHop(item.Chieus);
            }
        }

        public List<NgaySscheduler> GetLichHopTheoThang(QueryParamViewModel queryParams)
        {
            var thangHienTai = DateTime.Now.ToString("MM/yyyy");
            if (!string.IsNullOrEmpty(queryParams.ThangHienTai))
            {
                thangHienTai = queryParams.ThangHienTai.Trim();
            }

            DateTime startDate;
            if (!DateTime.TryParseExact(thangHienTai, new[] { "MM/yyyy", "M/yyyy", "yyyy-MM", "MM-yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            else
            {
                startDate = new DateTime(startDate.Year, startDate.Month, 1);
            }

            string startStr = startDate.ToString("dd/MM/yyyy");
            var endDate = startDate.AddMonths(1).AddDays(-1);
            string endStr = endDate.ToString("dd/MM/yyyy");

            queryParams.TuanTimKiem = null;
            queryParams.TuanHienTai = $"{startStr} - {endStr}";

            var data = _cuocHopRepository.GetAllCuocHops(queryParams);
            MapTenNguoiDungVaoThongTinCuocHopChoLichHopSangChieu(data);
            return data;
        }

        public FileDinhKemGridVo XuatFile(FileExportVm cuocHopExport)
        {
            var fileName = $"LichHop_{DateTime.Now.ToFileTime()}.xlsx";
            var filePath = Path.Combine(cuocHopExport.FolderSave, fileName);

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(cuocHopExport.FileTemplate)))
                {
                    var ws = package.Workbook.Worksheets[0];

                    int row = 4;
                    foreach (var ngay in cuocHopExport.Lichs)
                    {
                        ws.Cells[row, 1].Value = $"{ngay.Thu}, {ngay.NgayStr}";
                        ws.Cells[row, 1, row, 7].Merge = true;
                        ws.Cells[row, 1].Style.Font.Bold = true;
                        ws.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ECECEC"));

                        var mergedRange = ws.Cells[row, 1, row, 7];
                        mergedRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        mergedRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        mergedRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        mergedRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        row++;

                        row = GhiCa(ws, row, "Sáng", ngay.Sangs, true);
                        row = GhiCa(ws, row, "Chiều", ngay.Chieus, false);
                    }

                    string tuanThangOld = ws.Cells[1, 1].Text;
                    string textKhoangThoiGianOld = ws.Cells[2, 1].Text;
                    var itemFirst = cuocHopExport.Lichs.FirstOrDefault();
                    var itemLast = cuocHopExport.Lichs.LastOrDefault();

                    if (cuocHopExport.IsTuan)
                    {
                        ws.Cells[1, 1].Value = tuanThangOld.Replace("{{tuanThang}}", $"TUẦN {DateTimeHelper.TuanTrongNam(itemFirst.Ngay)}");
                    }
                    else
                    {
                        ws.Cells[1, 1].Value = tuanThangOld.Replace("{{tuanThang}}", $"THÁNG {itemFirst.Ngay.Month}");
                    }

                    ws.Cells[2, 1].Value = textKhoangThoiGianOld.Replace("{{khoangThoiGian}}", $"Từ {itemFirst.NgayStr} đến {itemLast.NgayStr}");

                    package.SaveAs(new FileInfo(filePath));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting file: {ex.Message}");
            }
            return new FileDinhKemGridVo
            {
                DuongDan = filePath
            };
        }

        private int GhiCa(ExcelWorksheet ws, int row, string tenCa, List<CuocHopGridVo> cuocHops, bool? isSang = true)
        {
            int soDong = (cuocHops != null && cuocHops.Count > 0) ? cuocHops.Count : 1;

            ws.Cells[row, 1, row + soDong - 1, 1].Merge = true;
            ws.Cells[row, 1].Value = tenCa;
            ws.Cells[row, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            ws.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(isSang == true ? "#DEEAF6" : "#FEF2CB"));

            var mergedRange = ws.Cells[row, 1, row + soDong - 1, 1];
            mergedRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            mergedRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            mergedRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            mergedRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            if (cuocHops != null && cuocHops.Count > 0)
            {
                foreach (var hop in cuocHops)
                {
                    ws.Cells[row, 2].Value = $"{hop.BatDauStr} - {hop.KetThucStr}";
                    ws.Cells[row, 3].Value = hop.TenCuocHop;
                    ws.Cells[row, 4].Value = string.Join("; ", new[] { hop.NguoiThamGia, hop.NguoiThamGiaKhac, hop.PhongBanThamGia }
                        .Where(o => !string.IsNullOrWhiteSpace(o)));
                    ws.Cells[row, 5].Value = hop.PhongHopTxt;
                    ws.Cells[row, 6].Value = hop.GhiChu;

                    for (int col = 2; col <= 7; col++)
                    {
                        ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    row++;
                }
            }
            else
            {
                for (int col = 2; col <= 7; col++)
                {
                    ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
                row++;
            }

            return row;
        }

        public FileDinhKemGridVo XuatFilePdf(string fileExcel)
        {
            string pdfPath = Path.ChangeExtension(fileExcel, ".pdf");

            try
            {
                Workbook workbook = new Workbook();
                workbook.LoadFromFile(fileExcel);

                Worksheet sheet = workbook.Worksheets[0];

                sheet.PageSetup.Orientation = PageOrientationType.Landscape;
                sheet.PageSetup.FitToPagesWide = 1;
                sheet.PageSetup.FitToPagesTall = 0;

                double marginInch = 0.5 / 2.54;
                sheet.PageSetup.TopMargin = marginInch;
                sheet.PageSetup.BottomMargin = marginInch;
                sheet.PageSetup.LeftMargin = marginInch;
                sheet.PageSetup.RightMargin = marginInch;

                workbook.SaveToFile(pdfPath, Spire.Xls.FileFormat.PDF);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to PDF: {ex.Message}", ex);
            }

            return new FileDinhKemGridVo
            {
                DuongDan = pdfPath
            };
        }

        public CuocHopGridVo GetCuocHopInfoFromId(int id)
        {
            var item = _cuocHopRepository.GetCuocHopInfoFromId(id);
            if (item != null && item.Id > 0)
            {
                var users = CacheManagerHelper.GetUserCache();
                item.NguoiThamGiaStr = UltiHelper.TaoTenNguoiDungTuIds(item.NguoiThamGia, users);
                item.PhongBanThamGia = LayTenPhongBanThamGia(item.PhongBanThamGiaCuocHops);
                item.CoQuanChuTri = "";
                item.CoQuanThamGia = "";

                var phongHop = new List<string>();
                if (item.PhongHopId.GetValueOrDefault() != 0)
                {
                    var ph = CacheManagerHelper.GetPhongHopByIdFromCache(item.PhongHopId.GetValueOrDefault());
                    if (ph != null)
                    {
                        phongHop.Add(ph.TenPhong);
                    }
                }
                if (!string.IsNullOrEmpty(item.PhongHopTxt))
                {
                    phongHop.Add(item.PhongHopTxt);
                }

                item.PhongHopTongTxt = string.Join("; ", phongHop);

                var dhx = DieuHanhXeAdoNetHelper.GetDieuHanhXeTrongDanhSachCuocHopIds(new List<int> { item.Id }, BienDungChung.ConnectDBStr);
                if (dhx.Count != 0)
                {
                    item.DieuHanhXeId = dhx.FirstOrDefault().Id;
                }
                if (item.LyDoId.GetValueOrDefault() != 0)
                {
                    var lyDos = CacheManagerHelper.GetLyDoByIdFromCache(item.LyDoId.GetValueOrDefault());
                    if (lyDos != null)
                    {
                        item.LyDoTxt = lyDos.Ten;
                        item.LyDoMaMau = lyDos.MaMau;
                    }
                }
            }

            return item;
        }

        private void MapTenNguoiDungVaoThongTinCuocHop(List<CuocHopGridVo> results)
        {
            var users = CacheManagerHelper.GetUserCache();
            var phongHops = CacheManagerHelper.GetPhongHopCache();

            var lichIds = results.Select(o => o.Id).ToList();
            var lichXeCoLichHops = DieuHanhXeAdoNetHelper.GetDieuHanhXeTrongDanhSachCuocHopIds(lichIds, BienDungChung.ConnectDBStr);

            foreach (var item in results)
            {
                var nguoiThamGiaIds = "";
                if (item.NguoiThamGias != null && item.NguoiThamGias.Count > 0)
                {
                    var userIds = item.NguoiThamGias.Where(o => o.UserId.HasValue).Select(o => o.UserId.Value).ToList();
                    nguoiThamGiaIds = userIds.Count > 0 ? string.Join(",", userIds) : "";
                }
                item.NguoiThamGia = UltiHelper.TaoTenNguoiDungTuIds(nguoiThamGiaIds, users);
                item.NguoiThamGiaStr = UltiHelper.TaoTenNguoiDungTuIds(nguoiThamGiaIds, users);

                item.PhongBanThamGia = LayTenPhongBanThamGia(item.PhongBanThamGiaCuocHops);

                if (item.NguoiTaoId.GetValueOrDefault() != 0)
                {
                    var user = users.FirstOrDefault(o => o.Id == item.NguoiTaoId);
                    if (user != null)
                    {
                        item.NguoiTao = user.Name;
                    }
                }

                if (item.NguoiDuyetId.GetValueOrDefault() != 0)
                {
                    var user = users.FirstOrDefault(o => o.Id == item.NguoiDuyetId);
                    if (user != null)
                    {
                        item.NguoiDuyet = user.Name;
                    }
                }

                item.CoQuanChuTri = "";
                item.CoQuanThamGia = "";

                if (item.LyDoId.GetValueOrDefault() != 0)
                {
                    var lyDo = CacheManagerHelper.GetLyDoByIdFromCache(item.LyDoId.GetValueOrDefault());
                    if (lyDo != null)
                    {
                        item.LyDoTxt = lyDo.Ten;
                        item.LyDoMaMau = lyDo.MaMau;
                    }
                }

                var phongHop = new List<string>();
                if (item.PhongHopId.GetValueOrDefault() != 0)
                {
                    var ph = phongHops.FirstOrDefault(o => o.Id == item.PhongHopId);
                    if (ph != null)
                    {
                        phongHop.Add(ph.TenPhong);
                    }
                }
                if (!string.IsNullOrEmpty(item.PhongHopTxt))
                {
                    phongHop.Add(item.PhongHopTxt);
                }

                item.PhongHopTxt = string.Join("; ", phongHop);

                var dhx = lichXeCoLichHops.Where(o => o.CuocHopId == item.Id).FirstOrDefault();
                if (dhx != null)
                {
                    item.DieuHanhXeId = dhx.Id;
                }
            }
        }

        private static string LayTenPhongBanThamGia(List<PhongBanThamGiaCuocHopGridVo> phongBanThamGias)
        {
            if (phongBanThamGias == null || phongBanThamGias.Count == 0)
            {
                return "";
            }

            var phongBanIds = phongBanThamGias
                .Where(o => o.PhongBanId.HasValue)
                .Select(o => o.PhongBanId.Value)
                .Distinct()
                .ToList();

            return string.Join(", ", CacheManagerHelper.GetPhongBanCache()
                .Where(o => phongBanIds.Contains(o.Id) && o.IsDeleted != true)
                .OrderBy(o => o.ThuTu.GetValueOrDefault() == 0 ? int.MaxValue : o.ThuTu.Value)
                .ThenBy(o => o.Ten)
                .Select(o => o.Ten));
        }

        public dynamic Them(CuocHopViewModel cuocHopViewModel)
        {
            return _cuocHopRepository.Them(cuocHopViewModel);
        }

        public dynamic ImportDanhSachLich(ImportLichCuocHopRequestViewModel request)
        {
            return _cuocHopRepository.ImportDanhSachLich(request);
        }

        public dynamic CapNhat(CuocHopViewModel cuocHopViewModel)
        {
            return _cuocHopRepository.CapNhat(cuocHopViewModel);
        }

        public FileDinhKemGridVo DownloadTaiLieuCuocHop(int id)
        {
            return _cuocHopRepository.DownloadTaiLieuCuocHop(id);
        }

        public dynamic GetTaiLieuCuocHop(int id)
        {
            return _cuocHopRepository.GetTaiLieuCuocHop(id);
        }

        public dynamic GuiLaiPhieu(CuocHopViewModel cuocHopViewModel)
        {
            return _cuocHopRepository.GuiLaiPhieu(cuocHopViewModel);
        }

        public bool KhoiPhuc(int id)
        {
            return _cuocHopRepository.KhoiPhuc(id);
        }

        public bool Xoa(int id)
        {
            return _cuocHopRepository.Xoa(id);
        }

        public dynamic XoaTaiLieuCuocHop(int id)
        {
            return _cuocHopRepository.XoaTaiLieuCuocHop(id);
        }

        public dynamic DuyetPhieu(CuocHopViewModel cuocHopViewModel)
        {
            return _cuocHopRepository.DuyetPhieu(cuocHopViewModel);
        }

        public dynamic TraPhieu(CuocHopViewModel cuocHopViewModel)
        {
            return _cuocHopRepository.TraPhieu(cuocHopViewModel);
        }

        public dynamic CapNhatThongTinPhongThoiGian(CuocHopViewModel cuocHopViewModel)
        {
            return _cuocHopRepository.CapNhatThongTinPhongThoiGian(cuocHopViewModel);
        }

        public List<ValidateError> KiemTraLoiKhiDieuPhoiPhong(CuocHopViewModel cuocHopViewModel)
        {
            return _cuocHopRepository.KiemTraLoiKhiDieuPhoiPhong(cuocHopViewModel);
        }

        public bool KiemTraThoiGianThayDoiKhiDieuPhoiPhong(CuocHopViewModel cuocHopViewModel)
        {
            return _cuocHopRepository.KiemTraThoiGianThayDoiKhiDieuPhoiPhong(cuocHopViewModel);
        }

        public List<CuocHopGridVo> GetCuocHopTrongDanhSachIds(List<int> cuocHopIds)
        {
            return _cuocHopRepository.LayCuocHopTuDanhDachIds(cuocHopIds);
        }

        public bool ThongBaoLaiLichHop(int cuocHopId)
        {
            return _cuocHopRepository.ThongBaoLaiLichHop(cuocHopId);
        }
    }
}
