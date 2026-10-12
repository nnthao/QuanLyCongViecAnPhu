using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface IDonViDoanService
    {
        DataTableResult<DonViDoanGridVo> GetAll(QueryParamViewModel queryParams);
        DonViDoanGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(DonViDoanViewModel model);
        bool Xoa(int id);
        DataLookupViewModel DonViDoanLookup(LookupQueryViewModel lookupQueryViewModel);
    }

    public class DonViDoanService : IDonViDoanService
    {
        private readonly IDonViDoanRepository _repository;
        public DonViDoanService(IDonViDoanRepository repository)
        {
            _repository = repository;
        }

        public DataTableResult<DonViDoanGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _repository.GetAll(queryParams);
            return data;
        }

        public DonViDoanGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(DonViDoanViewModel model)
        {
            return _repository.ThemOrCapNhat(model);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }

        public DataLookupViewModel DonViDoanLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _repository.DonViDoanLookup(lookupQueryViewModel);
        }
    }
}

