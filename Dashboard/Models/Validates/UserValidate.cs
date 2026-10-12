using Dashboard.Common;
using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IUserValidate
    {
        List<ValidateError> Validate(User user);
    }

    public class UserValidate : IUserValidate
    {
        private readonly IContext _context;

        public UserValidate(IContext context)
        {
            _context = context;
        }

        public List<ValidateError> Validate(User user)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Name.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Name",
                    Description = "Tên không được trống."
                });
            }

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Email.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Email",
                    Description = "Email không được trống."
                });
            }
            else if (!EmailValidateHelper.IsValidEmail(user.Email))
            {
                errors.Add(new ValidateError
                {
                    Id = "Email",
                    Description = "Email không hợp lệ."
                });
            }
            else
            {
                var emailTrim = user.Email.Trim();
                var checkExist = _context.Users.AsNoTracking().FirstOrDefault(o => o.Email != null && o.Email.Trim() == emailTrim);
                if (checkExist != null && checkExist.Id != user.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "Email",
                        Description = "Email đã tồn tại."
                    });
                }
            }

            if (!string.IsNullOrEmpty(user.Phone) && !string.IsNullOrEmpty(user.Phone.Trim()))
            {
                var phoneTrim = user.Phone.Trim();
                var checkExistPhone = _context.Users.AsNoTracking().FirstOrDefault(o => o.Phone != null && o.Phone.Trim() == phoneTrim);
                if (checkExistPhone != null && checkExistPhone.Id != user.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "Phone",
                        Description = "Số điện thoại đã tồn tại."
                    });
                }
            }

            if (user.Id == 0)
            {
                if ((string.IsNullOrEmpty(user.NewPassword) || string.IsNullOrEmpty(user.NewPassword.Trim())))
                {
                    errors.Add(new ValidateError
                    {
                        Id = "NewPassword",
                        Description = "Mật khẩu không được trống."
                    });
                }
            }

            if (!string.IsNullOrEmpty(user.NewPassword))
            {
                if (string.IsNullOrEmpty(user.RePassword) || string.IsNullOrEmpty(user.RePassword.Trim()))
                {
                    errors.Add(new ValidateError
                    {
                        Id = "RePassword",
                        Description = "Nhập lại Mật khẩu không được trống."
                    });
                }
                else if (user.NewPassword != user.RePassword)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "RePassword",
                        Description = "Nhập lại Mật khẩu không trùng khớp."
                    });
                }
            }

            if (user.ChucDanhId.GetValueOrDefault() == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "ChucDanhId",
                    Description = "Chức danh không được trống."
                });
            }

            if (user.VaiTroId == null || user.VaiTroId == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "VaiTroId",
                    Description = "Vai trò không được trống."
                });
            }

            if (user.PhongBanId == null || user.PhongBanId == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "PhongBanId",
                    Description = "Phòng ban không được trống."
                });
            }

            return errors;
        }
    }
}