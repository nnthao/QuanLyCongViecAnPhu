using ClosedXML.Excel;
using Dashboard.Models.GridVo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Dashboard.Common
{
    public static class ImportLichCuocHopHelper
    {
        public static List<CuocHopImportGridVo> ReadDataExcel(IXLWorksheet workSheet, int defaultLyDoId = 0)
        {
            var dataRows = new List<CuocHopImportGridVo>();
            var index = 1;
            var currentNgayStr = "";
            var currentCa = "";

            foreach (IXLRow row in workSheet.RowsUsed())
            {
                if (row.IsEmpty())
                {
                    continue;
                }

                var cellA = UltiHelper.TrimText(row.Cell(1).Value?.ToString() ?? "");
                var cellB = UltiHelper.TrimText(row.Cell(2).Value?.ToString() ?? "");
                var cellC = UltiHelper.TrimText(row.Cell(3).Value?.ToString() ?? "");
                var cellD = UltiHelper.TrimText(row.Cell(4).Value?.ToString() ?? "");
                var cellE = UltiHelper.TrimText(row.Cell(5).Value?.ToString() ?? "");
                var cellF = UltiHelper.TrimText(row.Cell(6).Value?.ToString() ?? "");
                var cellG = UltiHelper.TrimText(row.Cell(7).Value?.ToString() ?? "");

                var combinedRowText = $"{cellA} {cellB} {cellC} {cellD} {cellE} {cellF} {cellG}".Trim();
                if (string.IsNullOrEmpty(combinedRowText) || combinedRowText.StartsWith("*****"))
                {
                    continue;
                }

                if (cellB.IndexOf("Thời gian", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    cellC.IndexOf("Nội dung", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    combinedRowText.IndexOf("LỊCH TUẦN", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    combinedRowText.IndexOf("LỊCH CÔNG TÁC", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    continue;
                }

                var dateMatch = Regex.Match(combinedRowText, @"\b(\d{1,2})[\/\-\.](\d{1,2})[\/\-\.](\d{4})\b");
                var isDateRow = false;

                if (dateMatch.Success && (string.IsNullOrEmpty(cellC) ||
                    cellA.StartsWith("Thứ", StringComparison.OrdinalIgnoreCase) ||
                    cellA.StartsWith("Chủ", StringComparison.OrdinalIgnoreCase) ||
                    combinedRowText.StartsWith("Thứ", StringComparison.OrdinalIgnoreCase) ||
                    combinedRowText.StartsWith("Chủ", StringComparison.OrdinalIgnoreCase)))
                {
                    if (int.TryParse(dateMatch.Groups[1].Value, out int day) &&
                        int.TryParse(dateMatch.Groups[2].Value, out int month) &&
                        int.TryParse(dateMatch.Groups[3].Value, out int year))
                    {
                        if (day >= 1 && day <= 31 && month >= 1 && month <= 12 && year >= 2000 && year <= 2100)
                        {
                            currentNgayStr = $"{day:D2}/{month:D2}/{year:D4}";
                            currentCa = "";
                            isDateRow = true;
                        }
                    }
                }

                if (isDateRow)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(currentNgayStr))
                {
                    if (!string.IsNullOrEmpty(cellA))
                    {
                        if (cellA.IndexOf("Sáng", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            currentCa = "Sáng";
                        }
                        else if (cellA.IndexOf("Chiều", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            currentCa = "Chiều";
                        }
                        else
                        {
                            currentCa = cellA;
                        }
                    }

                    if (string.IsNullOrEmpty(cellC) && string.IsNullOrEmpty(cellB))
                    {
                        continue;
                    }

                    ParseThoiGian(cellB, currentCa, out string batDau, out string ketThuc);

                    var item = new CuocHopImportGridVo
                    {
                        Id = index++,
                        NgayStr = currentNgayStr,
                        Buoi = currentCa,
                        ThoiGian = cellB,
                        BatDauStr = batDau,
                        KetThucStr = ketThuc,
                        NoiDung = cellC,
                        ChuTri = cellD,
                        ThanhPhan = cellE,
                        DiaDiem = cellF,
                        GhiChu = cellG,
                        LyDoId = defaultLyDoId > 0 ? (int?)defaultLyDoId : null
                    };

                    if (!string.IsNullOrEmpty(item.NoiDung))
                    {
                        dataRows.Add(item);
                    }
                }
            }

            return dataRows;
        }

        public static void ParseThoiGian(string rawThoiGian, string ca, out string batDau, out string ketThuc)
        {
            batDau = "";
            ketThuc = "";

            if (!string.IsNullOrWhiteSpace(rawThoiGian))
            {
                var separators = new[] { "-", "–", "—", "->", "đến", "den" };
                string[] parts = null;
                foreach (var sep in separators)
                {
                    if (rawThoiGian.Contains(sep))
                    {
                        parts = rawThoiGian.Split(new[] { sep }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                    }
                }

                if (parts != null && parts.Length >= 2)
                {
                    batDau = ParseSingleTime(parts[0]);
                    ketThuc = ParseSingleTime(parts[1]);
                }
                else
                {
                    batDau = ParseSingleTime(rawThoiGian);
                }
            }

            if (string.IsNullOrEmpty(batDau))
            {
                batDau = (!string.IsNullOrEmpty(ca) && ca.IndexOf("Chiều", StringComparison.OrdinalIgnoreCase) >= 0) ? "14:00" : "08:00";
            }
        }

        private static string ParseSingleTime(string timeRaw)
        {
            if (string.IsNullOrWhiteSpace(timeRaw))
            {
                return "";
            }

            timeRaw = timeRaw.Trim().ToLowerInvariant();
            if (timeRaw.Contains("h"))
            {
                var parts = timeRaw.Split('h');
                var hourStr = parts[0].Trim();
                var minStr = parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]) ? parts[1].Trim() : "00";
                if (int.TryParse(hourStr, out int h) && int.TryParse(minStr, out int m))
                {
                    return $"{h:D2}:{m:D2}";
                }
            }

            if (timeRaw.Contains(":"))
            {
                var parts = timeRaw.Split(':');
                if (int.TryParse(parts[0].Trim(), out int h) && int.TryParse(parts[1].Trim(), out int m))
                {
                    return $"{h:D2}:{m:D2}";
                }
            }

            if (int.TryParse(timeRaw, out int hourOnly))
            {
                if (hourOnly >= 0 && hourOnly <= 23)
                {
                    return $"{hourOnly:D2}:00";
                }
            }

            return "";
        }
    }
}
