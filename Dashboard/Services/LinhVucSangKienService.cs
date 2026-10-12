using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface ILinhVucSangKienService
    {
        DataTableResult<LinhVucSangKienGridVo> GetAll(QueryParamViewModel queryParams);
        LinhVucSangKienGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(LinhVucSangKienViewModel linhVucSangKienViewModel);
        bool Xoa(int id);
        List<LookupViewModel> LinhVucLookup();
    }

    public class LinhVucSangKienService : ILinhVucSangKienService
    {
        private readonly ILinhVucSangKienRepository _linhVucSangKienRepository;
        public LinhVucSangKienService(ILinhVucSangKienRepository linhVucSangKienRepository)
        {
            _linhVucSangKienRepository = linhVucSangKienRepository;
        }

        public DataTableResult<LinhVucSangKienGridVo> GetAll(QueryParamViewModel queryParams)
        {
            return _linhVucSangKienRepository.GetAll(queryParams);
        }

        public LinhVucSangKienGridVo GetById(int id)
        {
            return _linhVucSangKienRepository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _linhVucSangKienRepository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(LinhVucSangKienViewModel linhVucSangKienViewModel)
        {
            return _linhVucSangKienRepository.ThemOrCapNhat(linhVucSangKienViewModel);
        }

        public bool Xoa(int id)
        {
            return _linhVucSangKienRepository.Xoa(id);
        }

        public List<LookupViewModel> LinhVucLookup()
        {
            return _linhVucSangKienRepository.LinhVucLookup();
        }
    }
}

