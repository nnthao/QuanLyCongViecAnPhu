using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dashboard.Services
{
    public interface IGiaoViecThongKeService
    {
        DataTableResult<GiaoViecThanhVienGridVo> GetListChiTietHoanThanhCongViecPhongBan(GiaoViecQueryViewModel queryViewModel);
        ThongKeCongViecTheoPhongBanResponse GetThongKeCongViecTheoPhongBan(GiaoViecQueryViewModel queryViewModel);
        byte[] XuatExcelPhuLuc03(GiaoViecQueryViewModel queryViewModel);
        byte[] XuatExcelPhuLuc05(GiaoViecQueryViewModel queryViewModel);
        byte[] XuatExcelPhuLuc345(GiaoViecQueryViewModel queryViewModel);
        string XuatExcelChiTietCongViecTheoTrangThaiToFile(List<GiaoViecThanhVienGridVo> data, string filePath);
        List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoNhieuCongViec(GiaoViecQueryViewModel queryViewModel);
        List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoItCongViec(GiaoViecQueryViewModel queryViewModel);
        List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoNhieuCongViecSomHan(GiaoViecQueryViewModel queryViewModel);
        List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoNhieuCongViecTreHan(GiaoViecQueryViewModel queryViewModel);
    }

    public class GiaoViecThongKeService : IGiaoViecThongKeService
    {
        private readonly IGiaoViecThongKeRepository _repository;
        public GiaoViecThongKeService(IGiaoViecThongKeRepository repository)
        {
            _repository = repository;
        }

        public DataTableResult<GiaoViecThanhVienGridVo> GetListChiTietHoanThanhCongViecPhongBan(GiaoViecQueryViewModel queryViewModel)
        {
            var data = _repository.GetListChiTietHoanThanhCongViecPhongBan(queryViewModel);
            foreach (var item in data.rows)
            {
                if (item.PhongBanId != 0)
                {
                    item.PhongBan = CacheManagerHelper.GetPhongBanByIdFromCache(item.PhongBanId)?.Ten;
                }
            }
            return data;
        }

        public ThongKeCongViecTheoPhongBanResponse GetThongKeCongViecTheoPhongBan(GiaoViecQueryViewModel queryViewModel)
        {
            var data = _repository.GetThongKeCongViecTheoPhongBan(queryViewModel);
            var phongBanCache = CacheManagerHelper.GetPhongBanCache();

            // Sắp xếp: phòng ban cha trước, sau đó các phòng ban con theo ThuTu
            var sortedData = data.OrderBy(item =>
            {
                var phongBan = phongBanCache.FirstOrDefault(pb => pb.Id == item.PhongBanId);
                return phongBan != null && phongBan.PhongBanChaId.HasValue ? 1 : 0;
            })
            .ThenBy(item =>
            {
                var phongBan = phongBanCache.FirstOrDefault(pb => pb.Id == item.PhongBanId);
                return phongBan != null && phongBan.ThuTu.HasValue ? phongBan.ThuTu.Value : int.MaxValue;
            })
            .ThenBy(item => item.TenPhongBan)
            .ToList();

            // Tính toán dữ liệu thống kê theo trạng thái
            var trangThaiData = new ThongKeCongViecTheoTrangThaiResponse
            {
                HoanThanhSomHonHan = sortedData.Sum(x => x.SoCongViecHoanThanhSomHonHan),
                HoanThanhDungHan = sortedData.Sum(x => x.SoCongViecHoanThanhDungHan),
                HoanThanhTreHan = sortedData.Sum(x => x.SoCongViecHoanThanhTreHan),
                ChuaHoanThanhConHan = sortedData.Sum(x => x.SoCongViecChuaHoanThanhConHan),
                ChuaHoanThanhTreHan = sortedData.Sum(x => x.SoCongViecChuaHoanThanhTreHan)
            };

            var response = new ThongKeCongViecTheoPhongBanResponse
            {
                Data = sortedData,
                Labels = sortedData.Select(x => x.TenPhongBan).ToList(),
                HoanThanhData = sortedData.Select(x => x.SoCongViecHoanThanh).ToList(),
                ChuaHoanThanhData = sortedData.Select(x => x.SoCongViecChuaHoanThanh).ToList(),
                TrangThaiData = trangThaiData
            };

            return response;
        }

        public byte[] XuatExcelPhuLuc03(GiaoViecQueryViewModel queryViewModel)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("PHỤ LỤC 03");
                BuildWorksheetPhuLuc03(ws, queryViewModel);
                return package.GetAsByteArray();
            }
        }

        public byte[] XuatExcelPhuLuc05(GiaoViecQueryViewModel queryViewModel)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("PHỤ LỤC 05");
                BuildWorksheetChiTietNhiemVu(ws, queryViewModel, "PHỤ LỤC 05", "BÁO CÁO NHIỆM VỤ ĐÃ HOÀN THÀNH TRỄ HẠN", new List<int> { 5 });
                return package.GetAsByteArray();
            }
        }

        public byte[] XuatExcelPhuLuc345(GiaoViecQueryViewModel queryViewModel)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var ws3 = package.Workbook.Worksheets.Add("PHỤ LỤC 03");
                BuildWorksheetPhuLuc03(ws3, queryViewModel);

                var ws4 = package.Workbook.Worksheets.Add("PHỤ LỤC 04");
                BuildWorksheetChiTietNhiemVu(ws4, queryViewModel, "PHỤ LỤC 04", "BÁO CÁO NHIỆM VỤ CHƯA HOÀN THÀNH TRỄ HẠN", new List<int> { 4 }, false);

                var ws5 = package.Workbook.Worksheets.Add("PHỤ LỤC 05");
                BuildWorksheetChiTietNhiemVu(ws5, queryViewModel, "PHỤ LỤC 05", "BÁO CÁO NHIỆM VỤ ĐÃ HOÀN THÀNH TRỄ HẠN", new List<int> { 5 });

                return package.GetAsByteArray();
            }
        }

        private void BuildWorksheetPhuLuc03(ExcelWorksheet ws, GiaoViecQueryViewModel queryViewModel)
        {
            var response = GetThongKeCongViecTheoPhongBan(queryViewModel);
            var data = response.Data ?? new List<ThongKeCongViecTheoPhongBanGridVo>();

            var tuNgayStr = queryViewModel.TuNgayStr ?? "";
            var denNgayStr = queryViewModel.DenNgayStr ?? "";

            ws.Cells.Style.Font.Name = "Times New Roman";
            ws.Cells.Style.Font.Size = 11;

            // Row 1: PHỤ LỤC 03
            ws.Cells["A1:E1"].Merge = true;
            ws.Cells["A1"].Value = "PHỤ LỤC 03";
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A1"].Style.Font.Size = 12;
            ws.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // Row 2: BÁO CÁO KẾT QUẢ THỰC HIỆN NHIỆM VỤ PHÁT SINH QUAN TRỌNG
            ws.Cells["A2:E2"].Merge = true;
            ws.Cells["A2"].Value = "BÁO CÁO KẾT QUẢ THỰC HIỆN NHIỆM VỤ PHÁT SINH QUAN TRỌNG";
            ws.Cells["A2"].Style.Font.Bold = true;
            ws.Cells["A2"].Style.Font.Size = 13;
            ws.Cells["A2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // Row 3: DO UBND TỈNH, CHỦ TỊCH UBND TỈNH GIAO
            ws.Cells["A3:E3"].Merge = true;
            ws.Cells["A3"].Value = "DO UBND TỈNH, CHỦ TỊCH UBND TỈNH GIAO";
            ws.Cells["A3"].Style.Font.Bold = true;
            ws.Cells["A3"].Style.Font.Size = 13;
            ws.Cells["A3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // Row 4: Thời hạn hoàn thành từ ngày...
            ws.Cells["A4:E4"].Merge = true;
            ws.Cells["A4"].Value = $"Thời hạn hoàn thành từ ngày: {tuNgayStr} đến ngày: {denNgayStr}";
            ws.Cells["A4"].Style.Font.Italic = true;
            ws.Cells["A4"].Style.Font.Size = 11;
            ws.Cells["A4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // Row 5 & 6: Header
            ws.Cells["A5:A6"].Merge = true;
            ws.Cells["A5"].Value = "Tên phòng ban";
            ws.Cells["A5"].Style.Font.Bold = true;
            ws.Cells["A5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["A5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            ws.Cells["B5:B6"].Merge = true;
            ws.Cells["B5"].Value = "Tổng";
            ws.Cells["B5"].Style.Font.Bold = true;
            ws.Cells["B5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["B5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            ws.Cells["C5:C6"].Merge = true;
            ws.Cells["C5"].Value = "Chưa hoàn thành trễ hạn";
            ws.Cells["C5"].Style.Font.Bold = true;
            ws.Cells["C5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["C5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            ws.Cells["C5"].Style.WrapText = true;

            ws.Cells["D5:E5"].Merge = true;
            ws.Cells["D5"].Value = "Hoàn thành";
            ws.Cells["D5"].Style.Font.Bold = true;
            ws.Cells["D5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["D5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            ws.Cells["D6"].Value = "Đúng hạn";
            ws.Cells["D6"].Style.Font.Bold = true;
            ws.Cells["D6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["D6"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            ws.Cells["E6"].Value = "Trễ hạn";
            ws.Cells["E6"].Style.Font.Bold = true;
            ws.Cells["E6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["E6"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            // Total Row: Row 7
            int sumTong = data.Sum(x => x.TongCongViec);
            int sumChuaHoanThanhTre = data.Sum(x => x.SoCongViecChuaHoanThanhTreHan);
            int sumHoanThanhDung = data.Sum(x => x.SoCongViecHoanThanhDungHan + x.SoCongViecHoanThanhSomHonHan);
            int sumHoanThanhTre = data.Sum(x => x.SoCongViecHoanThanhTreHan);

            ws.Cells["A7"].Value = "Tổng";
            ws.Cells["A7"].Style.Font.Bold = true;
            ws.Cells["A7"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            ws.Cells["B7"].Value = sumTong;
            ws.Cells["B7"].Style.Font.Bold = true;
            ws.Cells["B7"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            ws.Cells["C7"].Value = sumChuaHoanThanhTre;
            ws.Cells["C7"].Style.Font.Bold = true;
            ws.Cells["C7"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            ws.Cells["D7"].Value = sumHoanThanhDung;
            ws.Cells["D7"].Style.Font.Bold = true;
            ws.Cells["D7"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            ws.Cells["E7"].Value = sumHoanThanhTre;
            ws.Cells["E7"].Style.Font.Bold = true;
            ws.Cells["E7"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // Data Rows
            int currentRow = 8;
            foreach (var item in data)
            {
                ws.Cells[currentRow, 1].Value = item.TenPhongBan ?? "";
                ws.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                ws.Cells[currentRow, 2].Value = item.TongCongViec;
                ws.Cells[currentRow, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                ws.Cells[currentRow, 3].Value = item.SoCongViecChuaHoanThanhTreHan;
                ws.Cells[currentRow, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                ws.Cells[currentRow, 4].Value = item.SoCongViecHoanThanhDungHan + item.SoCongViecHoanThanhSomHonHan;
                ws.Cells[currentRow, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                ws.Cells[currentRow, 5].Value = item.SoCongViecHoanThanhTreHan;
                ws.Cells[currentRow, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                currentRow++;
            }

            int lastRow = currentRow - 1;
            var range = ws.Cells[5, 1, lastRow, 5];
            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            ws.Column(1).Width = 45;
            ws.Column(2).Width = 12;
            ws.Column(3).Width = 20;
            ws.Column(4).Width = 15;
            ws.Column(5).Width = 15;
        }

        private void BuildWorksheetChiTietNhiemVu(ExcelWorksheet ws, GiaoViecQueryViewModel queryViewModel, string phuLucTitle, string mainTitle, List<int> statusTypes, bool hasNoiDungBaoCao = true)
        {
            var query = new GiaoViecQueryViewModel
            {
                TuNgayStr = queryViewModel.TuNgayStr,
                DenNgayStr = queryViewModel.DenNgayStr,
                PhongBanId = queryViewModel.PhongBanId,
                PhongBanIds = queryViewModel.PhongBanIds,
                IsHoanThanhTypeList = statusTypes,
                limit = 0
            };

            var result = _repository.GetListChiTietHoanThanhCongViecPhongBan(query);
            var listData = result.rows ?? new List<GiaoViecThanhVienGridVo>();

            foreach (var item in listData)
            {
                if (string.IsNullOrEmpty(item.PhongBan) && item.PhongBanId != 0)
                {
                    item.PhongBan = CacheManagerHelper.GetPhongBanByIdFromCache(item.PhongBanId)?.Ten;
                }
            }

            var groupedData = listData.GroupBy(o => string.IsNullOrWhiteSpace(o.PhongBan) ? "Khác" : o.PhongBan).ToList();

            var tuNgayStr = queryViewModel.TuNgayStr ?? "";
            var denNgayStr = queryViewModel.DenNgayStr ?? "";

            ws.Cells.Style.Font.Name = "Times New Roman";
            ws.Cells.Style.Font.Size = 11;

            int totalCols = hasNoiDungBaoCao ? 6 : 5;
            string endColLetter = hasNoiDungBaoCao ? "F" : "E";

            // Row 1: Title phụ lục
            ws.Cells[$"A1:{endColLetter}1"].Merge = true;
            ws.Cells["A1"].Value = phuLucTitle;
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A1"].Style.Font.Size = 12;
            ws.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // Row 2: Title chính
            ws.Cells[$"A2:{endColLetter}2"].Merge = true;
            ws.Cells["A2"].Value = mainTitle;
            ws.Cells["A2"].Style.Font.Bold = true;
            ws.Cells["A2"].Style.Font.Size = 13;
            ws.Cells["A2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // Row 3: Thời hạn hoàn thành từ ngày...
            ws.Cells[$"A3:{endColLetter}3"].Merge = true;
            ws.Cells["A3"].Value = $"Thời hạn hoàn thành từ ngày: {tuNgayStr} đến ngày: {denNgayStr}";
            ws.Cells["A3"].Style.Font.Italic = true;
            ws.Cells["A3"].Style.Font.Size = 11;
            ws.Cells["A3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // Row 4: Header
            List<string> headers = hasNoiDungBaoCao
                ? new List<string> { "STT", "Tên công việc/Trích yếu văn bản", "Nội dung", "Nội dung báo cáo", "Ngày hết hạn", "Ghi chú" }
                : new List<string> { "STT", "Tên công việc/Trích yếu văn bản", "Nội dung", "Ngày hết hạn", "Ghi chú" };

            for (int i = 0; i < headers.Count; i++)
            {
                var cell = ws.Cells[4, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cell.Style.WrapText = true;
            }

            int currentRow = 5;
            foreach (var group in groupedData)
            {
                var groupHeaderCell = ws.Cells[currentRow, 1, currentRow, totalCols];
                groupHeaderCell.Merge = true;
                groupHeaderCell.Value = $"{group.Key} ({group.Count()})";
                groupHeaderCell.Style.Font.Bold = true;
                groupHeaderCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                currentRow++;

                int stt = 1;
                foreach (var item in group)
                {
                    ws.Cells[currentRow, 1].Value = stt++;
                    ws.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    ws.Cells[currentRow, 2].Value = item.TenCongViec ?? "";
                    ws.Cells[currentRow, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    ws.Cells[currentRow, 2].Style.WrapText = true;

                    var noiDung = !string.IsNullOrWhiteSpace(item.NoiDung)
                        ? item.NoiDung
                        : (!string.IsNullOrWhiteSpace(item.NoiDungNhiemVu) ? item.NoiDungNhiemVu : "");
                    ws.Cells[currentRow, 3].Value = noiDung;
                    ws.Cells[currentRow, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    ws.Cells[currentRow, 3].Style.WrapText = true;

                    if (hasNoiDungBaoCao)
                    {
                        ws.Cells[currentRow, 4].Value = item.NoiDungBaoCao ?? "";
                        ws.Cells[currentRow, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        ws.Cells[currentRow, 4].Style.WrapText = true;

                        ws.Cells[currentRow, 5].Value = item.HanXuLyStr ?? "";
                        ws.Cells[currentRow, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        ws.Cells[currentRow, 6].Value = item.GhiChu ?? "";
                        ws.Cells[currentRow, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        ws.Cells[currentRow, 6].Style.WrapText = true;
                    }
                    else
                    {
                        ws.Cells[currentRow, 4].Value = item.HanXuLyStr ?? "";
                        ws.Cells[currentRow, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        ws.Cells[currentRow, 5].Value = item.GhiChu ?? "";
                        ws.Cells[currentRow, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        ws.Cells[currentRow, 5].Style.WrapText = true;
                    }

                    currentRow++;
                }
            }

            int lastRow = currentRow - 1;
            if (lastRow >= 4)
            {
                var range = ws.Cells[4, 1, lastRow, totalCols];
                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }

            if (hasNoiDungBaoCao)
            {
                ws.Column(1).Width = 8;
                ws.Column(2).Width = 35;
                ws.Column(3).Width = 30;
                ws.Column(4).Width = 30;
                ws.Column(5).Width = 15;
                ws.Column(6).Width = 20;
            }
            else
            {
                ws.Column(1).Width = 8;
                ws.Column(2).Width = 35;
                ws.Column(3).Width = 35;
                ws.Column(4).Width = 15;
                ws.Column(5).Width = 20;
            }
        }



        public string XuatExcelChiTietCongViecTheoTrangThaiToFile(List<GiaoViecThanhVienGridVo> data, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Chi tiết công việc");

                // Header row
                var headers = new string[]
                {
                    "STT", "Phòng ban", "Người thực hiện", "Tên công việc",
                    "Ngày giao việc", "Ngày hoàn thành", "Hạn xử lý", "Trạng thái"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Data rows
                for (int i = 0; i < data.Count; i++)
                {
                    var item = data[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = i + 1; // STT
                    worksheet.Cells[row, 2].Value = item.PhongBan ?? ""; // Phòng ban
                    worksheet.Cells[row, 3].Value = item.ThanhVien ?? ""; // Người thực hiện
                    worksheet.Cells[row, 4].Value = item.TenCongViec ?? ""; // Tên công việc
                    worksheet.Cells[row, 5].Value = item.NgayGiaoViecStr ?? ""; // Ngày giao việc
                    worksheet.Cells[row, 6].Value = item.NgayHoanThanhStr ?? ""; // Ngày hoàn thành
                    worksheet.Cells[row, 7].Value = item.HanXuLyStr ?? ""; // Hạn xử lý
                    worksheet.Cells[row, 8].Value = item.TrangThaiHoanThanh ?? ""; // Trạng thái
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Add borders
                var range = worksheet.Cells[1, 1, data.Count + 1, headers.Length];
                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                // Save file directly
                var fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);

                return filePath;
            }
        }

        public List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoNhieuCongViec(GiaoViecQueryViewModel queryViewModel)
        {
            return _repository.GetTop10ThanhVienCoNhieuCongViec(queryViewModel);
        }

        public List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoItCongViec(GiaoViecQueryViewModel queryViewModel)
        {
            return _repository.GetTop10ThanhVienCoItCongViec(queryViewModel);
        }

        public List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoNhieuCongViecSomHan(GiaoViecQueryViewModel queryViewModel)
        {
            return _repository.GetTop10ThanhVienCoNhieuCongViecSomHan(queryViewModel);
        }

        public List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoNhieuCongViecTreHan(GiaoViecQueryViewModel queryViewModel)
        {
            return _repository.GetTop10ThanhVienCoNhieuCongViecTreHan(queryViewModel);
        }
    }
}


