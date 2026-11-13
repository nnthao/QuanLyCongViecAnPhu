using Dashboard.Models.Entities;
using System.Collections.Generic;

namespace Dashboard.Models.Validates
{
    public interface IGiaoViecValidate
    {
        List<ValidateError> Validate(GiaoViecViewModel viewModel);
    }

    public class GiaoViecValidate : IGiaoViecValidate
    {
        public List<ValidateError> Validate(GiaoViecViewModel viewModel)
        {
            var errors = new List<ValidateError>();
            if (string.IsNullOrWhiteSpace(viewModel.TenCongViec))
            {
                errors.Add(new ValidateError
                {
                    Id = "TenCongViec",
                    Description = "Tên công việc không được trống"
                });
            }

            if (viewModel.ThanhViens.Count == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "ThanhVien",
                    Description = "Phòng được giao việc không được trống"
                });
            }
            else
            {
                var errorThanhViens = new List<string>();
                foreach (var thanhVien in viewModel.ThanhViens)
                {
                    if (thanhVien.ThanhVienId.GetValueOrDefault() == 0)
                    {
                        errorThanhViens.Add("Phòng được giao việc chưa có người nhận hợp lệ");
                    }

                    if (string.IsNullOrWhiteSpace(thanhVien.NoiDungNhiemVu))
                    {
                        errorThanhViens.Add($@"{thanhVien.ThanhVien} - Nội dung thực hiện không được trống");
                    }

                    if (thanhVien.HanXuLy == null)
                    {
                        errorThanhViens.Add($@"{thanhVien.ThanhVien} - Hạn xử lý không được trống");
                    }
                }

                if (errorThanhViens.Count != 0)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "ThanhVien",
                        Description = string.Join("<br/>", errorThanhViens)
                    });
                }

            }
            return errors;
        }
    }
}

