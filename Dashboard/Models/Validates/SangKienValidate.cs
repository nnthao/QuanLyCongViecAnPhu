using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface ISangKienValidate
    {
        List<ValidateError> Validate(SangKienViewModel sangKien);
    }

    public class SangKienValidate : ISangKienValidate
    {
        private readonly IContext _context;

        public SangKienValidate(IContext context)
        {
            _context = context;
        }

        public List<ValidateError> Validate(SangKienViewModel sangKien)
        {
            var errors = new List<ValidateError>();

            if (sangKien.DotXetDuyetId <= 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "DotXetDuyetId",
                    Description = "Vui lòng chọn đợt xét duyệt."
                });
            }

            if (!sangKien.LinhVucSangKienId.HasValue || sangKien.LinhVucSangKienId.Value <= 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "LinhVucSangKienId",
                    Description = "Vui lòng chọn lĩnh vực."
                });
            }
            else
            {
                var linhVucExist = _context.LinhVucSangKiens.AsNoTracking()
                    .Any(o => o.Id == sangKien.LinhVucSangKienId.Value && o.IsDeleted != true);
                if (!linhVucExist)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "LinhVucSangKienId",
                        Description = "Lĩnh vực không hợp lệ."
                    });
                }
            }

            if (string.IsNullOrWhiteSpace(sangKien.MoTaChiTiet))
            {
                errors.Add(new ValidateError
                {
                    Id = "MoTaChiTiet",
                    Description = "Mô tả chi tiết không được trống."
                });
            }
            else
            {
                var moTa = sangKien.MoTaChiTiet.Trim().ToLower();
                var duplicated = _context.SangKiens.AsNoTracking()
                    .FirstOrDefault(o =>
                        o.DotXetDuyetId == sangKien.DotXetDuyetId &&
                        o.MoTaChiTiet != null &&
                        o.MoTaChiTiet.Trim().ToLower() == moTa &&
                        o.Id != sangKien.Id);

                if (duplicated != null)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "MoTaChiTiet",
                        Description = "Sáng kiến đã tồn tại."
                    });
                }
            }

            return errors;
        }
    }
}

