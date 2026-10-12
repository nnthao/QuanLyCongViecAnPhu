using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface IDoanVien_DanTocService
    {
        DataTableResult<DoanVien_DanTocGridVo> GetAll(QueryParamViewModel queryParams);
        DoanVien_DanTocGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(DoanVien_DanTocViewModel model);
        bool Xoa(int id);
        DataLookupViewModel DoanVien_DanTocLookup(LookupQueryViewModel lookupQueryViewModel);
    }

    public class DoanVien_DanTocService : IDoanVien_DanTocService
    {
        private readonly IDoanVien_DanTocRepository _repository;
        public DoanVien_DanTocService(IDoanVien_DanTocRepository repository)
        {
            _repository = repository;
        }

        public DataTableResult<DoanVien_DanTocGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _repository.GetAll(queryParams);
            return data;
        }

        public DoanVien_DanTocGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(DoanVien_DanTocViewModel model)
        {
            return _repository.ThemOrCapNhat(model);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }

        public DataLookupViewModel DoanVien_DanTocLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _repository.DoanVien_DanTocLookup(lookupQueryViewModel);
        }
    }
}

