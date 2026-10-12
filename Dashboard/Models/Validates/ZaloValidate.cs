using Dashboard.Models.ViewModel;
using System.Collections.Generic;

namespace Dashboard.Models.Validates
{
    public interface IZaloValidate
    {
        List<ValidateError> Validate(ZaloTestMessageRequest request);
    }

    public class ZaloValidate : IZaloValidate
    {
        public List<ValidateError> Validate(ZaloTestMessageRequest request)
        {
            var errors = new List<ValidateError>();

            if (request == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "SoDienThoai",
                    Description = "Số điện thoại không được trống."
                });
                errors.Add(new ValidateError
                {
                    Id = "NoiDung",
                    Description = "Nội dung không được trống."
                });
                return errors;
            }

            if (string.IsNullOrWhiteSpace(request.SoDienThoai))
            {
                errors.Add(new ValidateError
                {
                    Id = "SoDienThoai",
                    Description = "Số điện thoại không được trống."
                });
            }

            if (string.IsNullOrWhiteSpace(request.NoiDung))
            {
                errors.Add(new ValidateError
                {
                    Id = "NoiDung",
                    Description = "Nội dung không được trống."
                });
            }

            return errors;
        }
    }
}
