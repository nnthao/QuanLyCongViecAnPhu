using Dashboard.Models.ViewModel;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dashboard.Common
{
    public class VnEduImportHelper
    {
        private const int SourceHeaderColumnIndex = 1;
        private const int TargetStartRowIndex = 9;
        private const string SourceHeaderText = "STT";
        private const string ApprovedStatusText = "Hồ sơ đã được xét duyệt";

        public VnEduImportResultViewModel ConvertToVnEduFile(string sourceFilePath, string templateFilePath, string outputFilePath, int khoi)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new ArgumentException("Không tìm thấy file Excel nguồn.");
            }

            if (!File.Exists(templateFilePath))
            {
                throw new ArgumentException("Không tìm thấy file mẫu vnEdu.");
            }

            using (var sourceWorkbook = new Workbook())
            using (var targetWorkbook = new Workbook())
            {
                sourceWorkbook.LoadFromFile(sourceFilePath);
                targetWorkbook.LoadFromFile(templateFilePath);

                var sourceWorksheet = sourceWorkbook.Worksheets[0];
                var targetWorksheet = targetWorkbook.Worksheets[0];
                var importSetting = GetImportSetting(khoi);
                var headerRowIndex = FindHeaderRowIndex(sourceWorksheet);
                var recordCount = WriteApprovedRows(sourceWorksheet, targetWorksheet, headerRowIndex, importSetting);

                if (recordCount == 0)
                {
                    throw new ArgumentException("Không có hồ sơ nào đã được xét duyệt để chuyển đổi.");
                }

                targetWorkbook.SaveToFile(outputFilePath, ExcelVersion.Version2013);
                return new VnEduImportResultViewModel
                {
                    Success = true,
                    FileName = Path.GetFileName(outputFilePath),
                    RecordCount = recordCount
                };
            }
        }

        private static VnEduGradeImportSetting GetImportSetting(int khoi)
        {
            var importSetting = GetImportSettings().FirstOrDefault(item => item.Khoi == khoi);
            if (importSetting == null)
            {
                throw new ArgumentException("Khối được chọn không hợp lệ.");
            }

            return importSetting;
        }

        private static int FindHeaderRowIndex(Worksheet worksheet)
        {
            for (var rowIndex = worksheet.FirstRow; rowIndex <= worksheet.LastDataRow; rowIndex++)
            {
                var cellText = GetCellText(worksheet, rowIndex, SourceHeaderColumnIndex);
                if (string.Equals(cellText, SourceHeaderText, StringComparison.Ordinal))
                {
                    return rowIndex;
                }
            }

            throw new ArgumentException("Không tìm thấy dòng tiêu đề có cột B là STT.");
        }

        private static int WriteApprovedRows(Worksheet sourceWorksheet, Worksheet targetWorksheet, int headerRowIndex, VnEduGradeImportSetting importSetting)
        {
            var targetRowIndex = TargetStartRowIndex;
            for (var sourceRowIndex = headerRowIndex + 1; sourceRowIndex <= sourceWorksheet.LastDataRow; sourceRowIndex++)
            {
                var approvalStatus = GetCellText(sourceWorksheet, sourceRowIndex, importSetting.StatusColumnIndex);
                if (!string.Equals(approvalStatus, ApprovedStatusText, StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (var columnMapping in importSetting.ColumnMappings)
                {
                    var sourceValue = GetCellText(sourceWorksheet, sourceRowIndex, columnMapping.SourceColumnIndex);
                    WriteTextCellValue(targetWorksheet, targetRowIndex, columnMapping.TargetColumnIndex, sourceValue);
                }

                targetRowIndex++;
            }

            return targetRowIndex - TargetStartRowIndex;
        }

        private static string GetCellText(Worksheet worksheet, int rowIndex, int columnIndex)
        {
            var cell = worksheet.Range[rowIndex, columnIndex];
            var displayedText = cell.DisplayedText;
            if (!string.IsNullOrWhiteSpace(displayedText))
            {
                return displayedText.Trim();
            }

            return (cell.Value ?? string.Empty).Trim();
        }

        private static void WriteTextCellValue(Worksheet worksheet, int rowIndex, int columnIndex, string sourceValue)
        {
            var targetCell = worksheet.Range[rowIndex, columnIndex];
            targetCell.NumberFormat = "@";
            targetCell.Value = ConvertToExcelTextValue(sourceValue);
        }

        private static string ConvertToExcelTextValue(string sourceValue)
        {
            if (string.IsNullOrEmpty(sourceValue))
            {
                return string.Empty;
            }

            return $"'{sourceValue}";
        }

        private static List<VnEduGradeImportSetting> GetImportSettings()
        {
            return new List<VnEduGradeImportSetting>
            {
                CreateGradeImportSetting(1, "AZ", GetKhoi1VaKhoi6ColumnMappings()),
                CreateGradeImportSetting(6, "CX", GetKhoi1VaKhoi6ColumnMappings()),
                CreateGradeImportSetting(10, "DN", GetKhoi10ColumnMappings())
            };
        }

        private static VnEduGradeImportSetting CreateGradeImportSetting(int khoi, string statusColumn, List<VnEduColumnMapping> columnMappings)
        {
            return new VnEduGradeImportSetting
            {
                Khoi = khoi,
                StatusColumn = statusColumn,
                StatusColumnIndex = ConvertColumnNameToIndex(statusColumn),
                ColumnMappings = columnMappings
            };
        }

        private static List<VnEduColumnMapping> GetKhoi1VaKhoi6ColumnMappings()
        {
            return new List<VnEduColumnMapping>
            {
                CreateColumnMapping("C", "G"),
                CreateColumnMapping("D", "J"),
                CreateColumnMapping("E", "H"),
                CreateColumnMapping("F", "V"),
                CreateColumnMapping("G", "T"),
                CreateColumnMapping("H", "Y"),
                CreateColumnMapping("I", "K"),
                CreateColumnMapping("J", "AE"),
                CreateColumnMapping("K", "AF"),
                CreateColumnMapping("M", "AU"),
                CreateColumnMapping("M", "AY"),
                CreateColumnMapping("P", "O"),
                CreateColumnMapping("Q", "N"),
                CreateColumnMapping("T", "L"),
                CreateColumnMapping("U", "S"),
                CreateColumnMapping("V", "R"),
                CreateColumnMapping("Y", "P"),
                CreateColumnMapping("Z", "AK"),
                CreateColumnMapping("AA", "AW"),
                CreateColumnMapping("AC", "AM"),
                CreateColumnMapping("AD", "AL"),
                CreateColumnMapping("AF", "AP"),
                CreateColumnMapping("AG", "AX"),
                CreateColumnMapping("AI", "AR"),
                CreateColumnMapping("AJ", "AQ"),
                CreateColumnMapping("AW", "E"),
                CreateColumnMapping("AX", "AB")
            };
        }

        private static List<VnEduColumnMapping> GetKhoi10ColumnMappings()
        {
            return new List<VnEduColumnMapping>
            {
                CreateColumnMapping("C", "E"),
                CreateColumnMapping("D", "AB"),
                CreateColumnMapping("E", "G"),
                CreateColumnMapping("F", "J"),
                CreateColumnMapping("G", "H"),
                CreateColumnMapping("H", "V"),
                CreateColumnMapping("I", "T"),
                CreateColumnMapping("J", "Y"),
                CreateColumnMapping("K", "K"),
                CreateColumnMapping("L", "AE"),
                CreateColumnMapping("M", "AF"),
                CreateColumnMapping("O", "AU"),
                CreateColumnMapping("O", "AY"),
                CreateColumnMapping("R", "O"),
                CreateColumnMapping("S", "N"),
                CreateColumnMapping("X", "L"),
                CreateColumnMapping("Y", "S"),
                CreateColumnMapping("Z", "R"),
                CreateColumnMapping("AE", "P"),
                CreateColumnMapping("AF", "AK"),
                CreateColumnMapping("AH", "AW"),
                CreateColumnMapping("AI", "AL"),
                CreateColumnMapping("AL", "AP"),
                CreateColumnMapping("AN", "AX"),
                CreateColumnMapping("AO", "AQ")
            };
        }

        private static VnEduColumnMapping CreateColumnMapping(string sourceColumn, string targetColumn)
        {
            return new VnEduColumnMapping
            {
                SourceColumn = sourceColumn,
                SourceColumnIndex = ConvertColumnNameToIndex(sourceColumn),
                TargetColumn = targetColumn,
                TargetColumnIndex = ConvertColumnNameToIndex(targetColumn)
            };
        }

        private static int ConvertColumnNameToIndex(string columnName)
        {
            var columnIndex = 0;
            foreach (var character in columnName.ToUpper())
            {
                columnIndex *= 26;
                columnIndex += character - 'A' + 1;
            }

            return columnIndex;
        }
    }

    public class VnEduGradeImportSetting
    {
        public int Khoi { get; set; }

        public string StatusColumn { get; set; }

        public int StatusColumnIndex { get; set; }

        public List<VnEduColumnMapping> ColumnMappings { get; set; }
    }

    public class VnEduColumnMapping
    {
        public string SourceColumn { get; set; }

        public int SourceColumnIndex { get; set; }

        public string TargetColumn { get; set; }

        public int TargetColumnIndex { get; set; }
    }
}
