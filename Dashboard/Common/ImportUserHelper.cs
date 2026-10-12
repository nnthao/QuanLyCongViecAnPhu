using ClosedXML.Excel;
using Dashboard.Models.GridVo;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.Common
{
    public class ImportUserHelper
    {
        public static List<UserImportGridVo> ReadDataExcel(IXLWorksheet workSheet)
        {
            var dataRows = new List<UserImportGridVo>();
            var index = 1;
            foreach (IXLRow row in workSheet.RowsUsed())
            {
                if (row.RangeAddress.FirstAddress.RowNumber < 2)
                {
                    continue;
                }

                var item = new UserImportGridVo
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
                            // STT - bỏ qua
                            break;
                        case "B":
                            item.HoTen = cellValue;
                            break;
                        case "C":
                            item.ChucDanh = cellValue;
                            break;
                        case "D":
                            item.Email = cellValue;
                            break;
                        case "E":
                            item.SoDienThoai = cellValue;
                            break;
                        case "F":
                            item.PhongBan = cellValue;
                            break;
                        case "G":
                            item.VaiTro = cellValue;
                            break;
                        default:
                            continue;
                    }
                }

                if (!string.IsNullOrEmpty(item.HoTen) || !string.IsNullOrEmpty(item.Email))
                {
                    dataRows.Add(item);
                }
            }

            return dataRows;
        }
    }
}

