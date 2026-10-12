using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface IDoKhanService
    {
        DataTableResult<DoKhanGridVo> GetAll(QueryParamViewModel queryParams);
        DoKhanGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(DoKhanViewModel model);
        bool Xoa(int id);
        DataLookupViewModel DoKhanLookup(LookupQueryViewModel lookupQueryViewModel);
    }

    public class DoKhanService : IDoKhanService
    {
        private readonly IDoKhanRepository _repository;
        private readonly IUserRepository _userRepository;
        public DoKhanService(IDoKhanRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public DataTableResult<DoKhanGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _repository.GetAll(queryParams);
            return data;
        }

        public DoKhanGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(DoKhanViewModel model)
        {
            return _repository.ThemOrCapNhat(model);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }

        public DataLookupViewModel DoKhanLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _repository.DoKhanLookup(lookupQueryViewModel);
        }
    }
}


