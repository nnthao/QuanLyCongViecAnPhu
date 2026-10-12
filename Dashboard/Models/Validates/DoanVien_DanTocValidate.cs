using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IDoanVien_DanTocValidate
    {
        List<ValidateError> Validate(DoanVien_DanTocViewModel model);
    }

    public class DoanVien_DanTocValidate : IDoanVien_DanTocValidate
    {
        private readonly IContext _context;
        public DoanVien_DanTocValidate(IContext context)
        {
            _context = context;
        }

        public List<ValidateError> Validate(DoanVien_DanTocViewModel model)
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
                var checkExist = _context.DoanVien_DanTocs.AsNoTracking()
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

