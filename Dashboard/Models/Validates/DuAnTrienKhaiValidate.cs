using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IDuAnTrienKhaiValidate
    {
        List<ValidateError> Validate(DuAnTrienKhaiViewModel model);
    }

    public class DuAnTrienKhaiValidate : IDuAnTrienKhaiValidate
    {
        public List<ValidateError> Validate(DuAnTrienKhaiViewModel model)
        {
            var errors = new List<ValidateError>();

            if (!model.DuAnId.HasValue || model.DuAnId.Value <= 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "DuAnId",
                    Description = "Dự án không được để trống."
                });
            }

            if (!model.DonViId.HasValue || model.DonViId.Value <= 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "DonViId",
                    Description = "Đơn vị không được để trống."
                });
            }

            if (string.IsNullOrWhiteSpace(model.Ten))
            {
                errors.Add(new ValidateError
                {
                    Id = "Ten",
                    Description = "Tên dự án triển khai không được để trống."
                });
            }

            if (model.NgayBatDau.HasValue && model.NgayKetThuc.HasValue && model.NgayKetThuc < model.NgayBatDau)
            {
                errors.Add(new ValidateError
                {
                    Id = "NgayKetThucStr",
                    Description = "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu."
                });
            }

            var hangMucs = (model.HangMucs ?? new List<DuAnTrienKhaiHangMucInfo>())
                .Where(x => x.IsDeleted != true)
                .ToList();

            if (hangMucs.Count == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "HangMucs",
                    Description = "Phải có ít nhất một hạng mục triển khai."
                });
            }

            for (var i = 0; i < hangMucs.Count; i++)
            {
                var item = hangMucs[i];
                if (string.IsNullOrWhiteSpace(item.TenHangMuc))
                {
                    errors.Add(new ValidateError
                    {
                        Id = "HangMucs",
                        Description = "Tên hạng mục không được để trống."
                    });
                    break;
                }

                if (item.NgayBatDau.HasValue && item.NgayKetThuc.HasValue && item.NgayKetThuc < item.NgayBatDau)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "HangMucs",
                        Description = "Ngày kết thúc của hạng mục phải lớn hơn hoặc bằng ngày bắt đầu."
                    });
                    break;
                }

                if (item.TienDo.HasValue && (item.TienDo.Value < 0 || item.TienDo.Value > 100))
                {
                    errors.Add(new ValidateError
                    {
                        Id = "HangMucs",
                        Description = "Tiến độ hạng mục phải từ 0 đến 100."
                    });
                    break;
                }
            }

            return errors;
        }
    }
}
