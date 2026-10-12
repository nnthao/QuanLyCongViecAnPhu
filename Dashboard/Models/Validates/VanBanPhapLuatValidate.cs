using Dashboard.Models.Entities;
using System.Collections.Generic;

namespace Dashboard.Models.Validates
{
    public interface IVanBanPhapLuatValidate
    {
        List<ValidateError> Validate(VanBanPhapLuatViewModel model);
    }

    public class VanBanPhapLuatValidate : IVanBanPhapLuatValidate
    {
        private readonly IContext _context;
        public VanBanPhapLuatValidate(IContext context)
        {
            _context = context;
        }
        public List<ValidateError> Validate(VanBanPhapLuatViewModel model)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(model.SoKiHieu) || string.IsNullOrEmpty(model.SoKiHieu.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "SoKiHieu",
                    Description = "Số ký hiệu không được trống."
                });
            }

            if (string.IsNullOrEmpty(model.TrichYeu) || string.IsNullOrEmpty(model.TrichYeu.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "TrichYeu",
                    Description = "Trích yếu không được trống."
                });
            }

            if (model.HinhThucVanBanId.GetValueOrDefault() == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "HinhThucVanBanId",
                    Description = "Hình thức không được trống."
                });
            }

            if (model.HinhThucVanBanId.GetValueOrDefault() == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "CoQuanBanHanhId",
                    Description = "Cơ quan ban hành không được trống."
                });
            }

            if (model.HinhThucVanBanId.GetValueOrDefault() == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "DoKhanId",
                    Description = "Độ khẩn không được trống."
                });
            }

            if (model.NgayBanHanh == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "NgayBanHanhStr",
                    Description = "Ngày ban hành không được trống."
                });
            }

            if (model.TaiLieuDinhKemIdInts.Count == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "TaiLieuDinhKem",
                    Description = "File đính kèm không được trống."
                });
            }

            return errors;
        }
    }
}


