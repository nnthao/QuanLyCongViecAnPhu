using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface IDonViService
    {
        DataLookupViewModel Lookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<DonViGridVo> GetAll(QueryParamViewModel queryParams);
        DonViGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(DonViViewModel model);
        bool Xoa(int id);
    }

    public class DonViService : IDonViService
    {
        private readonly IDonViRepository _repository;
        public DonViService(IDonViRepository repository)
        {
            _repository = repository;
        }

        public DataLookupViewModel Lookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _repository.Lookup(lookupQueryViewModel);
        }

        public DataTableResult<DonViGridVo> GetAll(QueryParamViewModel queryParams)
        {
            return _repository.GetAll(queryParams);
        }

        public DonViGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(DonViViewModel model)
        {
            return _repository.ThemOrCapNhat(model);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }
    }
}
