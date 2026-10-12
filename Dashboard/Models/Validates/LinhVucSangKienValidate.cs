using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface ILinhVucSangKienValidate
    {
        List<ValidateError> Validate(LinhVucSangKienViewModel LinhVucSangKien);
    }

    public class LinhVucSangKienValidate : ILinhVucSangKienValidate
    {
        private readonly IContext _context;
        public LinhVucSangKienValidate(IContext context)
        {
            _context = context;
        }
        public List<ValidateError> Validate(LinhVucSangKienViewModel LinhVucSangKien)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(LinhVucSangKien.TenLinhVuc) || string.IsNullOrEmpty(LinhVucSangKien.TenLinhVuc.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "TenLinhVuc",
                    Description = "Tên lĩnh vực không được trống."
                });
            }
            else
            {
                var checkExist = _context.LinhVucSangKiens.AsNoTracking().FirstOrDefault(o => o.TenLinhVuc.ToLower().Trim()
                                                == LinhVucSangKien.TenLinhVuc.ToLower().Trim());
                if (checkExist != null && checkExist.Id != LinhVucSangKien.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "TenLinhVuc",
                        Description = "Tên lĩnh vực đã tồn tại."
                    });
                }
            }

            return errors;
        }
    }
}

