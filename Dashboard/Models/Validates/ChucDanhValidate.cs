using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IChucDanhValidate
    {
        List<ValidateError> Validate(ChucDanhViewModel chucDanh);
    }

    public class ChucDanhValidate : IChucDanhValidate
    {
        private readonly IContext _context;
        public ChucDanhValidate(IContext context)
        {
            _context = context;
        }

        public List<ValidateError> Validate(ChucDanhViewModel chucDanh)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(chucDanh.Ten) || string.IsNullOrEmpty(chucDanh.Ten.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Ten",
                    Description = "Tên không được trống."
                });
            }
            else
            {
                var checkExist = _context.ChucDanhs.AsNoTracking().FirstOrDefault(o => o.Ten.Trim().ToLower() == chucDanh.Ten.Trim().ToLower());
                if (checkExist != null && checkExist.Id != chucDanh.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "Ten",
                        Description = "Tên đã tồn tại."
                    });
                }
            }

            return errors;
        }
    }
}