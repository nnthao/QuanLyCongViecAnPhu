using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Models.Validates
{
    public interface IMatKhauValidate
    {
        List<ValidateError> Validate(UserViewModel userViewModel);
    }

    public class MatKhauValidate : IMatKhauValidate
    {
        private readonly IContext _context;
        public MatKhauValidate(IContext context)
        {
            _context = context;
        }

        public List<ValidateError> Validate(UserViewModel userViewModel)
        {
            var errors = new List<ValidateError>();

            var user = new User();
            using (var db = new ConnectDB())
            {
                user = db.Users.FirstOrDefault(o => o.Id == userViewModel.Id);
            }

            if (string.IsNullOrEmpty(userViewModel.CurrentPassword))
            {
                errors.Add(new ValidateError
                {
                    Id = "CurrentPassword",
                    Description = "Mật khẩu hiện tại không được trống."
                });
            }
            else
            {
                var currentPassInput = PasswordHelper.HashString(user.Guid, userViewModel.CurrentPassword);
                if (currentPassInput != user.Password)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "CurrentPassword",
                        Description = "Mật khẩu hiện tại không đúng."
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(userViewModel.CurrentPassword)
                        && !string.IsNullOrEmpty(userViewModel.NewPassword))
                    {
                        if (userViewModel.CurrentPassword == userViewModel.NewPassword)
                        {
                            errors.Add(new ValidateError
                            {
                                Id = "CurrentPassword",
                                Description = "Mật khẩu mới không được trùng mật khẩu hiện tại."
                            });
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(userViewModel.NewPassword))
            {
                errors.Add(new ValidateError
                {
                    Id = "NewPassword",
                    Description = "Mật khẩu mới không được trống."
                });
            }

            if (string.IsNullOrEmpty(userViewModel.RePassword))
            {
                errors.Add(new ValidateError
                {
                    Id = "RePassword",
                    Description = "Nhập lại Mật khẩu không được trống."
                });
            }
            else if (userViewModel.NewPassword != userViewModel.RePassword)
            {
                errors.Add(new ValidateError
                {
                    Id = "RePassword",
                    Description = "Nhập lại Mật khẩu không trùng khớp."
                });
            }

            return errors;
        }
    }
}