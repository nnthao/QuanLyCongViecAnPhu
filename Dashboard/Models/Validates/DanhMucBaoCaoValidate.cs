using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IDanhMucBaoCaoValidate
    {
        List<ValidateError> Validate(DanhMucBaoCaoViewModel danhMucBaoCao);
    }

    public class DanhMucBaoCaoValidate : IDanhMucBaoCaoValidate
    {
        private readonly IContext _context;
        public DanhMucBaoCaoValidate(IContext context)
        {
            _context = context;
        }
        public List<ValidateError> Validate(DanhMucBaoCaoViewModel danhMucBaoCao)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(danhMucBaoCao.Ten) || string.IsNullOrEmpty(danhMucBaoCao.Ten.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Ten",
                    Description = "Tên không được trống."
                });
            }
            else
            {
                var checkExist = _context.DanhMucBaoCaos.AsNoTracking().FirstOrDefault(o => o.Ten.ToLower().Trim()
                                                == danhMucBaoCao.Ten.ToLower().Trim());
                if (checkExist != null && checkExist.Id != danhMucBaoCao.Id)
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


