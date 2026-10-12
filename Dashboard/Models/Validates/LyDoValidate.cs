using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface ILyDoValidate
    {
        List<ValidateError> Validate(LyDoViewModel lyDo);
    }

    public class LyDoValidate : ILyDoValidate
    {
        private readonly IContext _context;
        public LyDoValidate(IContext context)
        {
            _context = context;
        }

        public List<ValidateError> Validate(LyDoViewModel lyDo)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(lyDo.Ten) || string.IsNullOrEmpty(lyDo.Ten.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Ten",
                    Description = "Tên không được trống."
                });
            }
            else
            {
                var checkExist = _context.LyDos.AsNoTracking().FirstOrDefault(o => o.Ten.Trim().ToLower() == lyDo.Ten.Trim().ToLower());
                if (checkExist != null && checkExist.Id != lyDo.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "Ten",
                        Description = "Tên đã tồn tại."
                    });
                }
            }

            if (string.IsNullOrEmpty(lyDo.MaMau) || string.IsNullOrEmpty(lyDo.MaMau.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "MaMau",
                    Description = "Mã màu không được trống."
                });
            }

            return errors;
        }
    }
}