using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface IDanhMucBaoCaoService
    {
        DataTableResult<DanhMucBaoCaoGridVo> GetAll(QueryParamViewModel queryParams);
        DanhMucBaoCaoGridVo GetById(int id);
        bool KhoiPhuc(int id);
        List<LookupViewModel> DanhMucBaoCaoLookup(LookupQueryViewModel lookupQueryViewModel);
        dynamic ThemOrCapNhat(DanhMucBaoCaoViewModel danhMucBaoCaoViewModel);
        bool Xoa(int id);
    }

    public class DanhMucBaoCaoService : IDanhMucBaoCaoService
    {
        private readonly IDanhMucBaoCaoRepository _repository;
        private readonly IUserRepository _userRepository;
        public DanhMucBaoCaoService(IDanhMucBaoCaoRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public DataTableResult<DanhMucBaoCaoGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _repository.GetAll(queryParams);
            return data;
        }

        public DanhMucBaoCaoGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(DanhMucBaoCaoViewModel danhMucBaoCaoViewModel)
        {
            return _repository.ThemOrCapNhat(danhMucBaoCaoViewModel);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }

        public List<LookupViewModel> DanhMucBaoCaoLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _repository.DanhMucBaoCaoLookup(lookupQueryViewModel);
        }
    }
}


