using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface ISangKienService
    {
        DataTableResult<SangKienGridVo> GetAll(QueryParamViewModel queryParams);
        DataTableResult<SangKienGridVo> GetKetQua(QueryParamViewModel queryParams);
        SangKienViewModel GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(SangKienViewModel sangKienViewModel);
        bool Xoa(int id);
        List<LookupViewModel> SangKienLookup(LookupQueryViewModel lookupQueryViewModel);
        List<FileDinhKemGridVo> GetFileDinhKemById(int id);
        bool CapNhatTrangThai(int id, string trangThai, string lyDo, decimal? tongDiem);
        bool CapNhatTongDiemVaTrangThai(int sangKienId, decimal? tongDiem);
    }

    public class SangKienService : ISangKienService
    {
        private readonly ISangKienRepository _sangKienRepository;

        public SangKienService(ISangKienRepository sangKienRepository)
        {
            _sangKienRepository = sangKienRepository;
        }

        public DataTableResult<SangKienGridVo> GetAll(QueryParamViewModel queryParams)
        {
            return _sangKienRepository.GetAll(queryParams);
        }

        public DataTableResult<SangKienGridVo> GetKetQua(QueryParamViewModel queryParams)
        {
            return _sangKienRepository.GetKetQua(queryParams);
        }

        public SangKienViewModel GetById(int id)
        {
            return _sangKienRepository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _sangKienRepository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(SangKienViewModel sangKienViewModel)
        {
            return _sangKienRepository.ThemOrCapNhat(sangKienViewModel);
        }

        public bool Xoa(int id)
        {
            return _sangKienRepository.Xoa(id);
        }

        public List<LookupViewModel> SangKienLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _sangKienRepository.SangKienLookup(lookupQueryViewModel);
        }

        public List<FileDinhKemGridVo> GetFileDinhKemById(int id)
        {
            return _sangKienRepository.GetFileDinhKemById(id);
        }

        public bool CapNhatTrangThai(int id, string trangThai, string lyDo, decimal? tongDiem)
        {
            return _sangKienRepository.CapNhatTrangThai(id, trangThai, lyDo, tongDiem);
        }

        public bool CapNhatTongDiemVaTrangThai(int sangKienId, decimal? tongDiem)
        {
            return _sangKienRepository.CapNhatTongDiemVaTrangThai(sangKienId, tongDiem);
        }
    }
}

