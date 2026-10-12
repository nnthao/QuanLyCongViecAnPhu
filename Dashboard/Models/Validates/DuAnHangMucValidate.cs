using Dashboard.Models.Entities;
using System.Collections.Generic;

namespace Dashboard.Models.Validates
{
    public interface IDuAnHangMucValidate
    {
        List<ValidateError> Validate(DuAnHangMucInfo duAnHangMuc);
    }

    public class DuAnHangMucValidate : IDuAnHangMucValidate
    {
        public List<ValidateError> Validate(DuAnHangMucInfo duAnHangMuc)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(duAnHangMuc.TenHangMuc) || string.IsNullOrEmpty(duAnHangMuc.TenHangMuc.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "TenHangMuc",
                    Description = "Tên hạng mục không được trống."
                });
            }

            return errors;
        }
    }
}