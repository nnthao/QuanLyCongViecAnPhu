using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public class VnEduImportValidate
    {
        private static readonly List<int> ValidGrades = new List<int> { 1, 6, 10 };

        public List<ValidateError> Validate(VnEduImportRequestViewModel request)
        {
            var errors = new List<ValidateError>();
            ValidateFile(request, errors);
            ValidateKhoi(request, errors);
            return errors;
        }

        private static void ValidateFile(VnEduImportRequestViewModel request, List<ValidateError> errors)
        {
            if (request == null || request.FileImport == null)
            {
                errors.Add(CreateError("FileImport", "Vui lòng chọn file Excel cần chuyển đổi."));
                return;
            }

            if (request.FileImport.ContentLength == 0)
            {
                errors.Add(CreateError("FileImport", "File Excel không được để trống."));
                return;
            }

            var extension = Path.GetExtension(request.FileImport.FileName);
            if (!string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(CreateError("FileImport", "Chỉ cho phép chọn file Excel định dạng .xlsx."));
            }
        }

        private static void ValidateKhoi(VnEduImportRequestViewModel request, List<ValidateError> errors)
        {
            if (request == null || !request.Khoi.HasValue || request.Khoi.GetValueOrDefault() == 0)
            {
                errors.Add(CreateError("Khoi", "Vui lòng chọn khối."));
                return;
            }

            if (!ValidGrades.Contains(request.Khoi.GetValueOrDefault()))
            {
                errors.Add(CreateError("Khoi", "Khối được chọn không hợp lệ."));
            }
        }

        private static ValidateError CreateError(string id, string description)
        {
            return new ValidateError
            {
                Id = id,
                Description = description
            };
        }
    }
}
