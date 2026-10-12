using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IHinhThucVanBanValidate
    {
        List<ValidateError> Validate(HinhThucVanBanViewModel model);
    }

    public class HinhThucVanBanValidate : IHinhThucVanBanValidate
    {
        private readonly IContext _context;
        public HinhThucVanBanValidate(IContext context)
        {
            _context = context;
        }
        public List<ValidateError> Validate(HinhThucVanBanViewModel model)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(model.Ten) || string.IsNullOrEmpty(model.Ten.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Ten",
                    Description = "Tên không được trống."
                });
            }
            else
            {
                var checkExist = _context.HinhThucVanBans.AsNoTracking().FirstOrDefault(o => o.Ten.ToLower().Trim() == model.Ten.ToLower().Trim());
                if (checkExist != null && checkExist.Id != model.Id)
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


