using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IDanhMucTraCuuThongTinValidate
    {
        List<ValidateError> Validate(DanhMucTraCuuThongTinViewModel DanhMucTraCuuThongTin);
    }

    public class DanhMucTraCuuThongTinValidate : IDanhMucTraCuuThongTinValidate
    {
        private readonly IContext _context;
        public DanhMucTraCuuThongTinValidate(IContext context)
        {
            _context = context;
        }
        public List<ValidateError> Validate(DanhMucTraCuuThongTinViewModel DanhMucTraCuuThongTin)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(DanhMucTraCuuThongTin.Ten) || string.IsNullOrEmpty(DanhMucTraCuuThongTin.Ten.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Ten",
                    Description = "Tên không được trống."
                });
            }
            else
            {
                var checkExist = _context.DanhMucTraCuuThongTins.AsNoTracking().FirstOrDefault(o => o.Ten.ToLower().Trim()
                                                == DanhMucTraCuuThongTin.Ten.ToLower().Trim());
                if (checkExist != null && checkExist.Id != DanhMucTraCuuThongTin.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "Ten",
                        Description = "Tên đã tồn tại."
                    });
                }
            }

            //if (DanhMucTraCuuThongTin.DanhMucTraCuuThongTinChaId.GetValueOrDefault() == 0)
            //{
            //    errors.Add(new ValidateError
            //    {
            //        Id = "DanhMucTraCuuThongTinChaId",
            //        Description = "Đơn vị cha không được trống."
            //    });
            //}

            return errors;
        }
    }
}