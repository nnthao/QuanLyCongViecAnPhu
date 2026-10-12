using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IDonViValidate
    {
        List<ValidateError> Validate(DonViViewModel model);
    }

    public class DonViValidate : IDonViValidate
    {
        private readonly IContext _context;
        public DonViValidate(IContext context)
        {
            _context = context;
        }

        public List<ValidateError> Validate(DonViViewModel model)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(model.Ten) || string.IsNullOrEmpty(model.Ten.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Ten",
                    Description = "Tên đơn vị không được trống."
                });
            }
            else
            {
                var checkExist = _context.DonVis.AsNoTracking()
                    .FirstOrDefault(o => o.Ten.ToLower().Trim() == model.Ten.ToLower().Trim() && o.IsDeleted != true);
                if (checkExist != null && checkExist.Id != model.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "Ten",
                        Description = "Tên đơn vị đã tồn tại."
                    });
                }
            }

            return errors;
        }
    }
}
