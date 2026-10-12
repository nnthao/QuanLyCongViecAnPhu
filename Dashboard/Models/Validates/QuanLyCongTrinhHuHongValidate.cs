using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IQuanLyCongTrinhHuHongValidate
    {
        List<ValidateError> Validate(QuanLyCongTrinhHuHongViewModel QuanLyCongTrinhHuHong);
    }

    public class QuanLyCongTrinhHuHongValidate : IQuanLyCongTrinhHuHongValidate
    {
        private readonly IContext _context;
        public QuanLyCongTrinhHuHongValidate(IContext context)
        {
            _context = context;
        }
        public List<ValidateError> Validate(QuanLyCongTrinhHuHongViewModel QuanLyCongTrinhHuHong)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(QuanLyCongTrinhHuHong.TenCongTrinh) || string.IsNullOrEmpty(QuanLyCongTrinhHuHong.TenCongTrinh.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "TenCongTrinh",
                    Description = "Tên công trình không được trống."
                });
            }
            else
            {
                var checkExist = _context.CongTrinhHuHongs.AsNoTracking().FirstOrDefault(o => o.TenCongTrinh.ToLower().Trim()
                                                == QuanLyCongTrinhHuHong.TenCongTrinh.ToLower().Trim());
                if (checkExist != null && checkExist.Id != QuanLyCongTrinhHuHong.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "TenCongTrinh",
                        Description = "Tên công trình đã tồn tại."
                    });
                }
            }

            if (string.IsNullOrEmpty(QuanLyCongTrinhHuHong.Diadiem) || string.IsNullOrEmpty(QuanLyCongTrinhHuHong.Diadiem.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Diadiem",
                    Description = "Địa điểm không được trống."
                });
            }

            if (QuanLyCongTrinhHuHong.LoaiCongTrinh.GetValueOrDefault() == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "LoaiCongTrinh",
                    Description = "Loại công trình không được trống."
                });
            }

            return errors;
        }
    }

    public interface IQuanLyCongTrinhHuHongChiTietValidate
    {
        List<ValidateError> Validate(QuanLyCongTrinhHuHongChiTietViewModel QuanLyCongTrinhHuHongChiTiet);
    }

    public class QuanLyCongTrinhHuHongChiTietValidate : IQuanLyCongTrinhHuHongChiTietValidate
    {
        private readonly IContext _context;
        public QuanLyCongTrinhHuHongChiTietValidate(IContext context)
        {
            _context = context;
        }
        public List<ValidateError> Validate(QuanLyCongTrinhHuHongChiTietViewModel QuanLyCongTrinhHuHongChiTiet)
        {
            var errors = new List<ValidateError>();

            if (QuanLyCongTrinhHuHongChiTiet.CongTrinhHuHongId.GetValueOrDefault() == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "CongTrinhHuHongId",
                    Description = "Công trình hư hỏng không được trống."
                });
            }

            if (string.IsNullOrEmpty(QuanLyCongTrinhHuHongChiTiet.TenCongTrinh) || string.IsNullOrEmpty(QuanLyCongTrinhHuHongChiTiet.TenCongTrinh.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "TenCongTrinh",
                    Description = "Tên công trình không được trống."
                });
            }

            if (string.IsNullOrEmpty(QuanLyCongTrinhHuHongChiTiet.Diadiem) || string.IsNullOrEmpty(QuanLyCongTrinhHuHongChiTiet.Diadiem.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Diadiem",
                    Description = "Địa điểm không được trống."
                });
            }

            return errors;
        }
    }

    public interface IQuanLyCongTrinhHuHongFileValidate
    {
        List<ValidateError> Validate(QuanLyCongTrinhHuHongFileViewModel QuanLyCongTrinhHuHongFile);
    }

    public class QuanLyCongTrinhHuHongFileValidate : IQuanLyCongTrinhHuHongFileValidate
    {
        private readonly IContext _context;
        public QuanLyCongTrinhHuHongFileValidate(IContext context)
        {
            _context = context;
        }
        public List<ValidateError> Validate(QuanLyCongTrinhHuHongFileViewModel QuanLyCongTrinhHuHongFile)
        {
            var errors = new List<ValidateError>();

            if (QuanLyCongTrinhHuHongFile.CongTrinhHuHongId.GetValueOrDefault() == 0)
            {
                errors.Add(new ValidateError
                {
                    Id = "CongTrinhHuHongId",
                    Description = "Công trình hư hỏng không được trống."
                });
            }

            if (string.IsNullOrEmpty(QuanLyCongTrinhHuHongFile.TenFile) || string.IsNullOrEmpty(QuanLyCongTrinhHuHongFile.TenFile.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "TenFile",
                    Description = "Tên file không được trống."
                });
            }

            if (string.IsNullOrEmpty(QuanLyCongTrinhHuHongFile.DuongDan) || string.IsNullOrEmpty(QuanLyCongTrinhHuHongFile.DuongDan.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "DuongDan",
                    Description = "Đường dẫn file không được trống."
                });
            }

            return errors;
        }
    }
}
