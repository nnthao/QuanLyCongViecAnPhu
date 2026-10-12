using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface ICoQuanBanHanhService
    {
        DataTableResult<CoQuanBanHanhGridVo> GetAll(QueryParamViewModel queryParams);
        CoQuanBanHanhGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(CoQuanBanHanhViewModel model);
        bool Xoa(int id);
        DataLookupViewModel CoQuanBanHanhLookup(LookupQueryViewModel lookupQueryViewModel);
    }

    public class CoQuanBanHanhService : ICoQuanBanHanhService
    {
        private readonly ICoQuanBanHanhRepository _repository;
        private readonly IUserRepository _userRepository;
        public CoQuanBanHanhService(ICoQuanBanHanhRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public DataTableResult<CoQuanBanHanhGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _repository.GetAll(queryParams);
            return data;
        }

        public CoQuanBanHanhGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(CoQuanBanHanhViewModel model)
        {
            return _repository.ThemOrCapNhat(model);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }

        public DataLookupViewModel CoQuanBanHanhLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _repository.CoQuanBanHanhLookup(lookupQueryViewModel);
        }
    }
}


