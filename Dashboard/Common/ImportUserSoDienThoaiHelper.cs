using ClosedXML.Excel;
using Dashboard.Models.GridVo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.Common
{
    public static class ImportUserSoDienThoaiHelper
    {
        public static List<UserCapNhatSoDienThoaiImportVo> ReadDataExcel(IXLWorksheet workSheet)
        {
            var dataRows = new List<UserCapNhatSoDienThoaiImportVo>();
            if (workSheet == null)
            {
                return dataRows;
            }

            var usedRows = workSheet.RowsUsed().ToList();
            if (!usedRows.Any())
            {
                return dataRows;
            }

            int startRowNumber = 2;
            foreach (var row in usedRows)
            {
                var cellDVal = GetCellValueAsString(row.Cell(4)).ToLower();
                var cellEVal = GetCellValueAsString(row.Cell(5)).ToLower();

                if (cellDVal.Contains("thư điện tử") || cellDVal.Contains("email") ||
                    cellEVal.Contains("số điện thoại") || cellEVal.Contains("zalo") || cellEVal.Contains("sđt"))
                {
                    startRowNumber = row.RowNumber() + 1;
                    break;
                }
            }

            foreach (var row in usedRows)
            {
                var rowNumber = row.RowNumber();
                if (rowNumber < startRowNumber)
                {
                    continue;
                }

                var hoTen = GetCellValueAsString(row.Cell(2));
                var email = GetCellValueAsString(row.Cell(4));
                var phone = GetCellValueAsString(row.Cell(5));

                if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
                {
                    continue;
                }

                dataRows.Add(new UserCapNhatSoDienThoaiImportVo
                {
                    DongSo = rowNumber,
                    HoTen = hoTen,
                    Email = email,
                    SoDienThoaiGoc = phone
                });
            }

            return dataRows;
        }

        public static (bool IsValid, string NormalizedPhone, string ErrorMessage) ValidateAndNormalizePhoneNumber(string rawPhone)
        {
            if (string.IsNullOrWhiteSpace(rawPhone))
            {
                return (false, string.Empty, "Số điện thoại không được để trống.");
            }

            var phone = rawPhone.Trim();
            phone = phone.Replace(" ", "").Replace("\t", "").Replace(".", "").Replace("-", "");

            if (!phone.All(char.IsDigit))
            {
                return (false, phone, $"Số điện thoại '{rawPhone}' chứa ký tự không hợp lệ.");
            }

            if (!phone.StartsWith("0"))
            {
                phone = "0" + phone;
            }

            if (phone.Length != 10)
            {
                return (false, phone, $"Số điện thoại '{rawPhone}' sau khi chuẩn hóa ({phone}) có {phone.Length} chữ số (yêu cầu đúng 10 chữ số).");
            }

            return (true, phone, string.Empty);
        }

        private static string GetCellValueAsString(IXLCell cell)
        {
            if (cell == null || cell.IsEmpty())
            {
                return string.Empty;
            }

            var val = cell.Value?.ToString() ?? string.Empty;
            if (val.Contains(".") && double.TryParse(val, out double dVal))
            {
                if (dVal == Math.Floor(dVal))
                {
                    val = ((long)dVal).ToString();
                }
            }

            return UltiHelper.TrimText(val);
        }
    }
}
