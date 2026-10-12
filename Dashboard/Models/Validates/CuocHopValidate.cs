using Dashboard.Common;
using Dashboard.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface ICuocHopValidate
    {
        List<ValidateError> Validate(CuocHopViewModel cuocHop);
    }

    public class CuocHopValidate : ICuocHopValidate
    {
        public List<ValidateError> Validate(CuocHopViewModel cuocHop)
        {
            var errors = new List<ValidateError>();
            if (cuocHop.IsAddOrUpdate)
            {
                if (string.IsNullOrEmpty(cuocHop.TenCuocHop))
                {
                    errors.Add(new ValidateError
                    {
                        Id = "TenCuocHop",
                        Description = "Tên cuộc họp không được trống."
                    });
                }

                if (string.IsNullOrEmpty(cuocHop.NgayDienRaStr))
                {
                    errors.Add(new ValidateError
                    {
                        Id = "NgayDienRaStr",
                        Description = "Ngày diễn ra không được trống."
                    });
                }

                if (string.IsNullOrEmpty(cuocHop.BatDauStr))
                {
                    errors.Add(new ValidateError
                    {
                        Id = "BatDauStr",
                        Description = "Bắt đầu không được trống."
                    });
                }

                if (cuocHop.ThoiGianBatDau != null && cuocHop.ThoiGianKetThuc != null && cuocHop.ThoiGianBatDau.GetValueOrDefault() >= cuocHop.ThoiGianKetThuc.GetValueOrDefault())
                {
                    errors.Add(new ValidateError
                    {
                        Id = "KetThucStr",
                        Description = "Thời gian kết thúc phải lớn hơn thời gian bắt đầu."
                    });
                }

                var nguoiThamGia = !string.IsNullOrEmpty(cuocHop.NguoiThamGia) ?
                   cuocHop.NguoiThamGia.Split(',').Where(o => !string.IsNullOrEmpty(o) && o != "0").ToList() : new List<string>();
                var phongBanThamGiaValues = !string.IsNullOrEmpty(cuocHop.PhongBanThamGiaIds) ?
                    cuocHop.PhongBanThamGiaIds.Split(',').Where(o => !string.IsNullOrEmpty(o) && o != "0").ToList() : new List<string>();
                var phongBanThamGiaIds = new List<int>();
                var phongBanThamGiaKhongHopLe = false;
                foreach (var phongBanIdValue in phongBanThamGiaValues)
                {
                    int phongBanId;
                    if (int.TryParse(phongBanIdValue, out phongBanId) && phongBanId > 0)
                    {
                        phongBanThamGiaIds.Add(phongBanId);
                    }
                    else
                    {
                        phongBanThamGiaKhongHopLe = true;
                    }
                }

                if (nguoiThamGia.Count == 0 && phongBanThamGiaIds.Count == 0 && string.IsNullOrWhiteSpace(cuocHop.NguoiThamGiaKhac))
                {
                    errors.Add(new ValidateError
                    {
                        Id = "NguoiThamGia",
                        Description = "Phải chọn ít nhất một người, phòng ban hoặc nhập người tham gia khác."
                    });
                }

                var phongBanIdsHopLe = CacheManagerHelper.GetPhongBanCache()
                    .Where(o => o.IsDeleted != true && phongBanThamGiaIds.Contains(o.Id))
                    .Select(o => o.Id)
                    .Distinct()
                    .ToList();
                if (phongBanThamGiaKhongHopLe || phongBanIdsHopLe.Count != phongBanThamGiaIds.Distinct().Count())
                {
                    errors.Add(new ValidateError
                    {
                        Id = "PhongBanThamGiaIds",
                        Description = "Danh sách phòng ban tham gia không hợp lệ."
                    });
                }

                var donViChuTri = !string.IsNullOrEmpty(cuocHop.CoQuanChuTriId) ?
                    cuocHop.CoQuanChuTriId.Split(',').Where(o => !string.IsNullOrEmpty(o)).ToList()
                    : new List<string>();

                var donViThamGia = !string.IsNullOrEmpty(cuocHop.CoQuanThamGiaId) ?
                    cuocHop.CoQuanThamGiaId.Split(',').Where(o => !string.IsNullOrEmpty(o)).ToList() :
                    new List<string>();

                if (donViThamGia.Intersect(donViChuTri).Count() != 0)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "CoQuanThamGiaId",
                        Description = "Đơn vị tham gia đã tồn tại trong chủ trì."
                    });
                }
            }

            return errors;
        }
    }
}
