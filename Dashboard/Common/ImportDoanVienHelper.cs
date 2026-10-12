using ClosedXML.Excel;
using Dashboard.Models.GridVo;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Dashboard.Common
{
    public class ImportDoanVienHelper
    {
        public static List<DoanVienImportGridVo> ReadDataExcel(IXLWorksheet workSheet)
        {
            var dataRows = new List<DoanVienImportGridVo>();
            var index = 1;
            foreach (IXLRow row in workSheet.RowsUsed())
            {
                if (row.RangeAddress.FirstAddress.RowNumber < 9)
                {
                    continue;
                }

                var item = new DoanVienImportGridVo
                {
                    Id = index
                };
                index++;

                foreach (IXLCell cell in row.Cells())
                {
                    var currentCell = UltiHelper.RemoveNumberInString(cell.Address.ToString());
                    var cellValue = UltiHelper.TrimText(cell.Value?.ToString() ?? "");

                    switch (currentCell)
                    {
                        case "A":
                            break;
                        case "B":
                            item.DonViDoan = cellValue;
                            break;
                        case "C":
                            item.HoTen = cellValue;
                            break;
                        case "D":
                            item.Nam = cellValue;
                            break;
                        case "E":
                            item.Nu = cellValue;
                            break;
                        case "F":
                            item.QueQuan = cellValue;
                            break;
                        case "G":
                            item.DanToc = cellValue;
                            break;
                        case "H":
                            item.ChuyenMonNghiepVu = cellValue;
                            break;
                        case "I":
                            item.LyLuanChinhTri = cellValue;
                            break;
                        case "J":
                            item.NgayVaoDoan = cellValue;
                            break;
                        case "K":
                            item.NgayVaoDang = cellValue;
                            break;
                        case "L":
                            item.ChucVu = cellValue;
                            break;
                        case "M":
                            item.GhiChu = cellValue;
                            break;
                        default:
                            continue;
                    }
                }

                if (!string.IsNullOrEmpty(item.HoTen))
                {
                    ProcessGioiTinhVaNgaySinh(item);
                    dataRows.Add(item);
                }
            }

            return dataRows;
        }

        private static void ProcessGioiTinhVaNgaySinh(DoanVienImportGridVo item)
        {
            var namValue = item.Nam?.Trim() ?? "";
            var nuValue = item.Nu?.Trim() ?? "";

            if (!string.IsNullOrEmpty(namValue))
            {
                item.GioiTinh = 1;
                item.NgaySinhStr = ParseNgaySinh(namValue);
            }
            else if (!string.IsNullOrEmpty(nuValue))
            {
                item.GioiTinh = 2;
                item.NgaySinhStr = ParseNgaySinh(nuValue);
            }
        }

        private static string ParseNgaySinh(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            value = value.Trim();

            if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateFull))
            {
                return dateFull.ToString("dd/MM/yyyy");
            }

            if (DateTime.TryParseExact(value, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateShort))
            {
                return dateShort.ToString("dd/MM/yyyy");
            }

            if (int.TryParse(value, out int year) && year >= 1900 && year <= 2100)
            {
                return $"01/01/{year}";
            }

            return null;
        }
    }
}

