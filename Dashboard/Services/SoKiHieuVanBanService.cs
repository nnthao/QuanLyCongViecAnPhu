using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System;

namespace Dashboard.Services
{
    public interface ISoKiHieuVanBanService
    {
        DataTableResult<SoKiHieuVanBanGridVo> GetAll(SoKiHieuVanBanQueryViewModel queryParams, int? currentUserId = null);
        dynamic LaySo(int userId, int nam, int hinhThucVanBanId);
        dynamic Xoa(int id, int userId);
        SoKiHieuVanBanGridVo LaySoGanNhat();
        dynamic KhoiTaoSoKiHieuBanDau(int soKiHieu, int? userId);
    }

    public class SoKiHieuVanBanService : ISoKiHieuVanBanService
    {
        private readonly ISoKiHieuVanBanRepository _soKiHieuVanBanRepository;

        public SoKiHieuVanBanService(ISoKiHieuVanBanRepository soKiHieuVanBanRepository)
        {
            _soKiHieuVanBanRepository = soKiHieuVanBanRepository;
        }

        public DataTableResult<SoKiHieuVanBanGridVo> GetAll(SoKiHieuVanBanQueryViewModel queryParams, int? currentUserId = null)
        {
            var data = _soKiHieuVanBanRepository.GetAll(queryParams);

            var soGanNhat = _soKiHieuVanBanRepository.LaySoGanNhat();

            if (data.rows != null && data.rows.Count > 0)
            {
                foreach (var item in data.rows)
                {
                    // Set isAllowDeleted: chỉ cho phép xóa nếu record là mới nhất và thuộc về user đang đăng nhập
                    if (soGanNhat != null &&
                        soGanNhat.Id == item.Id &&
                        currentUserId.GetValueOrDefault() == item.UserId.GetValueOrDefault() &&
                        currentUserId.GetValueOrDefault() == soGanNhat.UserId.GetValueOrDefault())
                    {
                        item.IsAllowDeleted = true;
                    }
                    else
                    {
                        item.IsAllowDeleted = false;
                    }
                }
            }

            return data;
        }

        public dynamic LaySo(int userId, int nam, int hinhThucVanBanId)
        {
            return _soKiHieuVanBanRepository.LaySo(userId, nam, hinhThucVanBanId);
        }

        public dynamic Xoa(int id, int userId)
        {
            var soGanNhat = _soKiHieuVanBanRepository.LaySoGanNhat();
            if (soGanNhat == null || soGanNhat.Id != id || soGanNhat.UserId.GetValueOrDefault() != userId)
            {
                return new { Errors = new[] { new { Name = "", Description = "Chỉ có thể xoá số gần nhất mà bạn đã lấy" } } };
            }

            var result = _soKiHieuVanBanRepository.Xoa(id, userId);
            return new { Success = result };
        }

        public SoKiHieuVanBanGridVo LaySoGanNhat()
        {
            return _soKiHieuVanBanRepository.LaySoGanNhat();
        }

        public dynamic KhoiTaoSoKiHieuBanDau(int soKiHieu, int? userId)
        {
            try
            {
                return _soKiHieuVanBanRepository.KhoiTaoSoKiHieuBanDau(soKiHieu, userId);
            }
            catch (Exception ex)
            {
                return new { Errors = new[] { new { Name = "", Description = ex.Message } } };
            }
        }
    }
}

