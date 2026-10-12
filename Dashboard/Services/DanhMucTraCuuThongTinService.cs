using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface IDanhMucTraCuuThongTinService
    {
        DataTableResult<DanhMucTraCuuThongTinGridVo> GetAll(QueryParamViewModel queryParams);
        DanhMucTraCuuThongTinGridVo GetById(int id);
        bool KhoiPhuc(int id);
        List<LookupViewModel> DanhMucTraCuuThongTinLookup(LookupQueryViewModel lookupQueryViewModel);
        dynamic ThemOrCapNhat(DanhMucTraCuuThongTinViewModel phongBanViewModel);
        bool Xoa(int id);
    }

    public class DanhMucTraCuuThongTinService : IDanhMucTraCuuThongTinService
    {
        private readonly IDanhMucTraCuuThongTinRepository _phongBanRepository;
        private readonly IUserRepository _userRepository;
        public DanhMucTraCuuThongTinService(IDanhMucTraCuuThongTinRepository phongBanRepository, IUserRepository userRepository)
        {
            _phongBanRepository = phongBanRepository;
            _userRepository = userRepository;
        }

        public List<LookupViewModel> DanhMucTraCuuThongTinLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _phongBanRepository.DanhMucTraCuuThongTinLookup(lookupQueryViewModel);
        }

        public DataTableResult<DanhMucTraCuuThongTinGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _phongBanRepository.GetAll(queryParams);
            return data;
        }

        public DanhMucTraCuuThongTinGridVo GetById(int id)
        {
            return _phongBanRepository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _phongBanRepository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(DanhMucTraCuuThongTinViewModel phongBanViewModel)
        {
            return _phongBanRepository.ThemOrCapNhat(phongBanViewModel);
        }

        public bool Xoa(int id)
        {
            return _phongBanRepository.Xoa(id);
        }
    }
}