using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IVaiTroXuLyValidate
    {
        List<ValidateError> Validate(VaiTroXuLyViewModel model);
    }

    public class VaiTroXuLyValidate : IVaiTroXuLyValidate
    {
        private readonly IContext _context;

        public VaiTroXuLyValidate(IContext context)
        {
            _context = context;
        }

        public List<ValidateError> Validate(VaiTroXuLyViewModel model)
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
                var checkExist = _context.VaiTroXuLys.AsNoTracking()
                    .FirstOrDefault(o => o.Ten.ToLower().Trim() == model.Ten.ToLower().Trim());
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
