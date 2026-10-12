using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Linq;

namespace Dashboard.Services
{
    public interface IPhongBanService
    {
        bool CapNhatThuTu(CapNhatThuTuPhongBanRequest request);
        bool CapNhatIsPhongNhanViec(CapNhatIsPhongNhanViecRequest request);
        DataTableResult<PhongBanGridVo> GetAll(QueryParamViewModel queryParams);
        PhongBanGridVo GetById(int id);
        bool KhoiPhuc(int id);
        DataLookupViewModel PhongBanLookup(LookupQueryViewModel lookupQueryViewModel);
        DataLookupViewModel UserLookup(LookupQueryViewModel lookupQueryViewModel);
        dynamic ThemOrCapNhat(PhongBanViewModel phongBanViewModel);
        bool Xoa(int id);
    }

    public class PhongBanService : IPhongBanService
    {
        private readonly IPhongBanRepository _phongBanRepository;
        private readonly IUserRepository _userRepository;
        public PhongBanService(IPhongBanRepository phongBanRepository, IUserRepository userRepository)
        {
            _phongBanRepository = phongBanRepository;
            _userRepository = userRepository;
        }

        public bool CapNhatThuTu(CapNhatThuTuPhongBanRequest request)
        {
            return _phongBanRepository.CapNhatThuTu(request);
        }

        public bool CapNhatIsPhongNhanViec(CapNhatIsPhongNhanViecRequest request)
        {
            return _phongBanRepository.CapNhatIsPhongNhanViec(request);
        }

        public DataLookupViewModel PhongBanLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _phongBanRepository.PhongBanLookup(lookupQueryViewModel);
        }

        public DataLookupViewModel UserLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _userRepository.UserLookup(lookupQueryViewModel);
        }

        public DataTableResult<PhongBanGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _phongBanRepository.GetAll(queryParams);
            var phongBanCache = CacheManagerHelper.GetPhongBanCache();
            var userCache = CacheManagerHelper.GetUserCache();

            if (data.rows != null && data.rows.Any())
            {
                foreach (var item in data.rows)
                {
                    if (item.PhongBanChaId.HasValue)
                    {
                        var phongBanCha = phongBanCache.FirstOrDefault(p => p.Id == item.PhongBanChaId.Value);
                        item.TenPhongBanCha = phongBanCha != null ? phongBanCha.Ten : null;
                    }

                    if (item.NguoiTheoDoiChiDaoId.HasValue)
                    {
                        var user = userCache.FirstOrDefault(u => u.Id == item.NguoiTheoDoiChiDaoId.Value);
                        item.TenNguoiTheoDoiChiDao = user != null ? user.Name : null;
                    }
                }
            }

            return data;
        }

        public PhongBanGridVo GetById(int id)
        {
            return _phongBanRepository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _phongBanRepository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(PhongBanViewModel phongBanViewModel)
        {
            return _phongBanRepository.ThemOrCapNhat(phongBanViewModel);
        }

        public bool Xoa(int id)
        {
            return _phongBanRepository.Xoa(id);
        }
    }
}