using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IDoanVienValidate
    {
        List<ValidateError> Validate(DoanVienViewModel model);
    }

    public class DoanVienValidate : IDoanVienValidate
    {
        private readonly IContext _context;
        public DoanVienValidate(IContext context)
        {
            _context = context;
        }

        public List<ValidateError> Validate(DoanVienViewModel model)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(model.HoTen) || string.IsNullOrEmpty(model.HoTen.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "HoTen",
                    Description = "Họ tên không được trống."
                });
            }

            if (!model.DonViDoanId.HasValue || model.DonViDoanId.Value == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "DonViDoanId",
                    Description = "Đơn vị đoàn không được trống."
                });
            }
            else
            {
                var donViDoan = _context.DonViDoans.AsNoTracking()
                    .FirstOrDefault(o => o.Id == model.DonViDoanId.Value && o.IsDeleted != true);
                if (donViDoan == null)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "DonViDoanId",
                        Description = "Đơn vị đoàn không tồn tại hoặc đã bị xóa."
                    });
                }
            }

            if (!model.NgaySinh.HasValue)
            {
                errors.Add(new ValidateError
                {
                    Id = "NgaySinh",
                    Description = "Ngày sinh không được trống."
                });
            }

            if (!model.DanTocId.HasValue || model.DanTocId.Value == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "DanTocId",
                    Description = "Dân tộc không được trống."
                });
            }
            else
            {
                var danToc = _context.DoanVien_DanTocs.AsNoTracking()
                    .FirstOrDefault(o => o.Id == model.DanTocId.Value && o.IsDeleted != true);
                if (danToc == null)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "DanTocId",
                        Description = "Dân tộc không tồn tại hoặc đã bị xóa."
                    });
                }
            }

            if (!model.GioiTinh.HasValue || model.GioiTinh.Value == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "GioiTinh",
                    Description = "Giới tính không được trống."
                });
            }

            if (string.IsNullOrEmpty(model.ChuyenMonNghiepVu) || string.IsNullOrEmpty(model.ChuyenMonNghiepVu.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "ChuyenMonNghiepVu",
                    Description = "Trình độ chuyên môn không được trống."
                });
            }

            return errors;
        }
    }
}

