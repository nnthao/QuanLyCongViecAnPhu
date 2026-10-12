using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IDuAnValidate
    {
        List<ValidateError> Validate(DuAnViewModel duAnViewModel);
    }

    public class DuAnValidate : IDuAnValidate
    {
        private readonly IContext _context;
        public DuAnValidate(IContext context)
        {
            _context = context;
        }

        public List<ValidateError> Validate(DuAnViewModel duAnViewModel)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(duAnViewModel.Ten) || string.IsNullOrEmpty(duAnViewModel.Ten.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Ten",
                    Description = "Tên dự án không được trống."
                });
            }
            else
            {
                var checkExist = _context.DuAns.AsNoTracking().FirstOrDefault(o => o.Ten.ToLower().Trim() == duAnViewModel.Ten.ToLower().Trim() && o.Id != duAnViewModel.Id && o.IsDeleted != true);
                if (checkExist != null)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "Ten",
                        Description = "Tên dự án đã tồn tại."
                    });
                }
            }

            if (string.IsNullOrEmpty(duAnViewModel.UserIds))
            {
                errors.Add(new ValidateError
                {
                    Id = "UserIds",
                    Description = "Vui lòng chọn ít nhất một thành viên tham gia."
                });
            }
            else
            {
                var userIntIds = duAnViewModel.UserIds.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => int.Parse(o.Trim())).ToList();
                
                // Kiểm tra các thành viên không được trùng nhau
                var duplicateUserIds = userIntIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                if (duplicateUserIds.Count > 0)
                {
                    var duplicateUserNames = _context.Users.AsNoTracking()
                        .Where(u => duplicateUserIds.Contains(u.Id))
                        .Select(u => u.Name)
                        .ToList();

                    var errorMessage = "Các thành viên sau đã được chọn trùng lặp: " + string.Join(", ", duplicateUserNames);
                    errors.Add(new ValidateError
                    {
                        Id = "UserIds",
                        Description = errorMessage
                    });
                }
            }

            if (string.IsNullOrEmpty(duAnViewModel.TruongNhomIds))
            {
                errors.Add(new ValidateError
                {
                    Id = "TruongNhomIds",
                    Description = "Vui lòng chọn ít nhất một trưởng nhóm."
                });
            }
            else
            {
                var truongNhomIntIds = duAnViewModel.TruongNhomIds.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => int.Parse(o.Trim())).ToList();
                
                if (truongNhomIntIds.Count == 0)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "TruongNhomIds",
                        Description = "Vui lòng chọn ít nhất một trưởng nhóm."
                    });
                }
            }

            return errors;
        }
    }
}
