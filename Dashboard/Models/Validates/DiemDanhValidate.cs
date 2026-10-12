using Dashboard.Models.Entities;
using System.Collections.Generic;

namespace Dashboard.Models.Validates
{
    public interface IDiemDanhValidate
    {
        List<ValidateError> Validate(DiemDanhViewModel model);
    }

    public class DiemDanhValidate : IDiemDanhValidate
    {
        public List<ValidateError> Validate(DiemDanhViewModel model)
        {
            var errors = new List<ValidateError>();

            if (model.UserId == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "UserId",
                    Description = "Vui lòng chọn nhân viên."
                });
            }

            if (model.TuNgay == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "TuNgayStr",
                    Description = "Vui lòng chọn từ ngày."
                });
            }

            if (model.DenNgay == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "DenNgayStr",
                    Description = "Vui lòng chọn đến ngày."
                });
            }

            if (model.TuNgay.HasValue && model.DenNgay.HasValue && model.TuNgay.Value.Date > model.DenNgay.Value.Date)
            {
                errors.Add(new ValidateError
                {
                    Id = "DenNgayStr",
                    Description = "Đến ngày phải lớn hơn hoặc bằng từ ngày."
                });
            }

            if (model.LoaiNghi == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "LoaiNghi",
                    Description = "Vui lòng chọn loại nghỉ."
                });
            }

            return errors;
        }
    }
}
