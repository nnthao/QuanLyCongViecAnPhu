using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface IHinhThucVanBanService
    {
        DataTableResult<HinhThucVanBanGridVo> GetAll(QueryParamViewModel queryParams);
        HinhThucVanBanGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(HinhThucVanBanViewModel model);
        bool Xoa(int id);
        DataLookupViewModel HinhThucVanBanLookup(LookupQueryViewModel lookupQueryViewModel);
    }

    public class HinhThucVanBanService : IHinhThucVanBanService
    {
        private readonly IHinhThucVanBanRepository _repository;
        private readonly IUserRepository _userRepository;
        public HinhThucVanBanService(IHinhThucVanBanRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public DataTableResult<HinhThucVanBanGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _repository.GetAll(queryParams);
            return data;
        }

        public HinhThucVanBanGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(HinhThucVanBanViewModel model)
        {
            return _repository.ThemOrCapNhat(model);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }

        public DataLookupViewModel HinhThucVanBanLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _repository.HinhThucVanBanLookup(lookupQueryViewModel);
        }
    }
}


