using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IDonViDoanValidate
    {
        List<ValidateError> Validate(DonViDoanViewModel model);
    }

    public class DonViDoanValidate : IDonViDoanValidate
    {
        private readonly IContext _context;
        public DonViDoanValidate(IContext context)
        {
            _context = context;
        }

        public List<ValidateError> Validate(DonViDoanViewModel model)
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
                var checkExist = _context.DonViDoans.AsNoTracking()
                    .FirstOrDefault(o => o.Ten.ToLower().Trim() == model.Ten.ToLower().Trim() && o.IsDeleted != true);
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

