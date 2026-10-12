using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Dashboard.Models.Validates
{
    public interface IVaiTroValidate
    {
        List<ValidateError> Validate(VaiTro vaiTro);
    }

    public class VaiTroValidate : IVaiTroValidate
    {
        private readonly IContext _context;
        public VaiTroValidate(IContext context)
        {
            _context = context;
        }

        public List<ValidateError> Validate(VaiTro vaiTro)
        {
            var errors = new List<ValidateError>();
            if (string.IsNullOrEmpty(vaiTro.Ten) || String.IsNullOrEmpty(vaiTro.Ten.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Ten",
                    Description = "Vai trò không được trống."
                });
            }
            else
            {
                var checkExist = _context.VaiTros.AsNoTracking().FirstOrDefault(o => o.Ten == vaiTro.Ten.Trim());
                if (checkExist != null && checkExist.Id != vaiTro.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "Ten",
                        Description = "Vai trò đã tồn tại."
                    });
                }
            }

            return errors;
        }
    }
}