using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IPhongBanValidate
    {
        List<ValidateError> Validate(PhongBanViewModel PhongBan);
    }

    public class PhongBanValidate : IPhongBanValidate
    {
        private readonly IContext _context;
        public PhongBanValidate(IContext context)
        {
            _context = context;
        }
        public List<ValidateError> Validate(PhongBanViewModel PhongBan)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(PhongBan.Ten) || string.IsNullOrEmpty(PhongBan.Ten.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Ten",
                    Description = "Tên phòng ban không được trống."
                });
            }
            else
            {
                var checkExist = _context.PhongBans.AsNoTracking().FirstOrDefault(o => o.Ten.ToLower().Trim()
                                                == PhongBan.Ten.ToLower().Trim());
                if (checkExist != null && checkExist.Id != PhongBan.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "Ten",
                        Description = "Tên phòng ban đã tồn tại."
                    });
                }
            }

            //if (PhongBan.PhongBanChaId.GetValueOrDefault() == 0)
            //{
            //    errors.Add(new ValidateError
            //    {
            //        Id = "PhongBanChaId",
            //        Description = "Đơn vị cha không được trống."
            //    });
            //}

            return errors;
        }
    }
}