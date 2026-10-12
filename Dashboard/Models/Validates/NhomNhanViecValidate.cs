using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface INhomNhanViecValidate
    {
        List<ValidateError> Validate(NhomNhanViecViewModel nhomNhanViecViewModel);
    }

    public class NhomNhanViecValidate : INhomNhanViecValidate
    {
        private readonly IContext _context;
        public NhomNhanViecValidate(IContext context)
        {
            _context = context;
        }
        public List<ValidateError> Validate(NhomNhanViecViewModel nhomNhanViecViewModel)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(nhomNhanViecViewModel.Ten) || string.IsNullOrEmpty(nhomNhanViecViewModel.Ten.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Ten",
                    Description = "Tên không được trống."
                });
            }
            else
            {
                var checkExist = _context.NhomNhanViecs.AsNoTracking().FirstOrDefault(o => o.Ten.ToLower().Trim() == nhomNhanViecViewModel.Ten.ToLower().Trim() && o.NguoiTaoId != nhomNhanViecViewModel.NguoiTaoId);
                if (checkExist != null && checkExist.Id != nhomNhanViecViewModel.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "Ten",
                        Description = "Tên đã tồn tại."
                    });
                }
            }

            if (string.IsNullOrEmpty(nhomNhanViecViewModel.UserIds))
            {
                errors.Add(new ValidateError
                {
                    Id = "UserIds",
                    Description = "Chọn ít nhất 1 người nhận."
                });
            }

            return errors;
        }
    }
}
