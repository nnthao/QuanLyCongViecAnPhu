using ClosedXML.Excel;
using Dashboard.Models.GridVo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Common
{
    public class ImportNoiDungCuocHopHelper
    {
        public static List<NoiDungCuocHopImportGridVo> ReadDataExcel(IXLWorksheet workSheet)
        {
            var dataRows = new List<NoiDungCuocHopImportGridVo>();
            var index = 1;
            foreach (IXLRow row in workSheet.RowsUsed())
            {
                if (row.RangeAddress.FirstAddress.RowNumber < 2)
                {
                    continue;
                }


                var item = new NoiDungCuocHopImportGridVo
                {
                    Id = index
                };
                index++;

                foreach (IXLCell cell in row.Cells())
                {
                    var currentCell = UltiHelper.RemoveNumberInString(cell.Address.ToString());
                    var cellValue = UltiHelper.TrimText(cell.Value.ToString());
                    switch (currentCell)
                    {
                        case "B":
                            item.ThoiGian = cellValue;
                            break;
                        case "C":
                            item.NoiDung = cellValue;
                            break;
                        case "D":
                            item.NguoiTrinhBay = cellValue;
                            break;
                        default:
                            continue;
                    }
                }

                if (!string.IsNullOrEmpty(item.ThoiGian) && !string.IsNullOrEmpty(item.NoiDung))
                {
                    dataRows.Add(item);
                }
            }

            return dataRows;
        }
    }
}