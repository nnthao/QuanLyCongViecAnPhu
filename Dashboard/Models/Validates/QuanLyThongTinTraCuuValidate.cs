using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IQuanLyThongTinTraCuuValidate
    {
        List<ValidateError> Validate(QuanLyThongTinTraCuuViewModel quanLyThongTinTraCuu);
    }

    public class QuanLyThongTinTraCuuValidate : IQuanLyThongTinTraCuuValidate
    {
        private readonly IContext _context;
        public QuanLyThongTinTraCuuValidate(IContext context)
        {
            _context = context;
        }
        public List<ValidateError> Validate(QuanLyThongTinTraCuuViewModel quanLyThongTinTraCuu)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(quanLyThongTinTraCuu.MaTraCuu) || string.IsNullOrEmpty(quanLyThongTinTraCuu.MaTraCuu.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "MaTraCuu",
                    Description = "Mã tra cứu không được trống."
                });
            }
            else
            {
                var checkExist = _context.QuanLyThongTinTraCuus.AsNoTracking().FirstOrDefault(o => o.MaTraCuu.ToLower().Trim()
                                                == quanLyThongTinTraCuu.MaTraCuu.ToLower().Trim());
                if (checkExist != null && checkExist.Id != quanLyThongTinTraCuu.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "MaTraCuu",
                        Description = "Mã tra cứu đã tồn tại."
                    });
                }
            }

            if (string.IsNullOrEmpty(quanLyThongTinTraCuu.TieuDe) || string.IsNullOrEmpty(quanLyThongTinTraCuu.TieuDe.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "TieuDe",
                    Description = "Tiêu đề không được trống."
                });
            }

            if (quanLyThongTinTraCuu.DanhMucTraCuuThongTinId.GetValueOrDefault() == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "DanhMucTraCuuThongTinId",
                    Description = "Danh mục không được trống."
                });
            }

            if (quanLyThongTinTraCuu.TrangThai.GetValueOrDefault() == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "TrangThai",
                    Description = "Trạng thái không được trống."
                });
            }

            return errors;
        }
    }
}
