using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IDanhSachCongTrinhValidate
    {
        List<ValidateError> Validate(DanhSachCongTrinhViewModel danhSachCongTrinh);
    }

    public class DanhSachCongTrinhValidate : IDanhSachCongTrinhValidate
    {
        private readonly IContext _context;
        public DanhSachCongTrinhValidate(IContext context)
        {
            _context = context;
        }
        public List<ValidateError> Validate(DanhSachCongTrinhViewModel danhSachCongTrinh)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(danhSachCongTrinh.TenCongTrinh) || string.IsNullOrEmpty(danhSachCongTrinh.TenCongTrinh.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "TenCongTrinh",
                    Description = "Tên công trình không được trống."
                });
            }
            else
            {
                var checkExist = _context.DanhSachCongTrinhs.AsNoTracking().FirstOrDefault(o => o.TenCongTrinh.ToLower().Trim()
                                                            == danhSachCongTrinh.TenCongTrinh.ToLower().Trim());
                if (checkExist != null && checkExist.Id != danhSachCongTrinh.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "TenCongTrinh",
                        Description = "Tên công trình đã tồn tại."
                    });
                }
            }
            return errors;
        }
    }
}
