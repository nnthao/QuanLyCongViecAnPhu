using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IQuanLyBaoCaoChuyenNganhValidate
    {
        List<ValidateError> Validate(QuanLyBaoCaoChuyenNganhViewModel quanLyBaoCaoChuyenNganh);
    }

    public class QuanLyBaoCaoChuyenNganhValidate : IQuanLyBaoCaoChuyenNganhValidate
    {
        private readonly IContext _context;
        public QuanLyBaoCaoChuyenNganhValidate(IContext context)
        {
            _context = context;
        }
        public List<ValidateError> Validate(QuanLyBaoCaoChuyenNganhViewModel quanLyBaoCaoChuyenNganh)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(quanLyBaoCaoChuyenNganh.TenFile) || string.IsNullOrEmpty(quanLyBaoCaoChuyenNganh.TenFile.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "TenFile",
                    Description = "Tên file không được trống."
                });
            }
            else
            {
                var checkExist = _context.QuanLyBaoCaoChuyenNganhs.AsNoTracking().FirstOrDefault(o => o.TenFile.ToLower().Trim()
                                                == quanLyBaoCaoChuyenNganh.TenFile.ToLower().Trim());
                if (checkExist != null && checkExist.Id != quanLyBaoCaoChuyenNganh.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "TenFile",
                        Description = "Tên file đã tồn tại."
                    });
                }
            }

            if (quanLyBaoCaoChuyenNganh.DanhMucBaoCaoId == null || quanLyBaoCaoChuyenNganh.DanhMucBaoCaoId == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "DanhMucBaoCaoId",
                    Description = "Danh mục báo cáo không được trống."
                });
            }

            if (quanLyBaoCaoChuyenNganh.QuanLyBaoCaoChuyenNganhEnumId == null || quanLyBaoCaoChuyenNganh.QuanLyBaoCaoChuyenNganhEnumId == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "QuanLyBaoCaoChuyenNganhEnumId",
                    Description = "Loại báo cáo không được trống."
                });
            }

            return errors;
        }
    }
}
