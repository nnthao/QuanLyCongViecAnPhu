using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface IDanhSachCongTrinhService
    {
        DataTableResult<DanhSachCongTrinhGridVo> GetAll(QueryParamViewModel queryParams);
        DanhSachCongTrinhGridVo GetById(int id);
        DanhSachCongTrinhGridVo GetByIdWithHuHong(int id);
        bool KhoiPhuc(int id);
        List<LookupViewModel> DanhSachCongTrinhLookup(LookupQueryViewModel lookupQueryViewModel, int? loaiCongTrinh = null);
        dynamic ThemOrCapNhat(DanhSachCongTrinhViewModel danhSachCongTrinhViewModel);
        dynamic CapNhatThongTinHuHong(DanhSachCongTrinhViewModel danhSachCongTrinhViewModel);
        bool Xoa(int id);
    }

    public class DanhSachCongTrinhService : IDanhSachCongTrinhService
    {
        private readonly IDanhSachCongTrinhRepository _danhSachCongTrinhRepository;

        public DanhSachCongTrinhService(IDanhSachCongTrinhRepository danhSachCongTrinhRepository)
        {
            _danhSachCongTrinhRepository = danhSachCongTrinhRepository;
        }

        public DataTableResult<DanhSachCongTrinhGridVo> GetAll(QueryParamViewModel queryParams)
        {
            return _danhSachCongTrinhRepository.GetAll(queryParams);
        }

        public DanhSachCongTrinhGridVo GetById(int id)
        {
            return _danhSachCongTrinhRepository.GetById(id);
        }

        public DanhSachCongTrinhGridVo GetByIdWithHuHong(int id)
        {
            return _danhSachCongTrinhRepository.GetByIdWithHuHong(id);
        }

        public dynamic ThemOrCapNhat(DanhSachCongTrinhViewModel danhSachCongTrinhViewModel)
        {
            return _danhSachCongTrinhRepository.ThemOrCapNhat(danhSachCongTrinhViewModel);
        }

        public dynamic CapNhatThongTinHuHong(DanhSachCongTrinhViewModel danhSachCongTrinhViewModel)
        {
            return _danhSachCongTrinhRepository.CapNhatThongTinHuHong(danhSachCongTrinhViewModel);
        }

        public bool Xoa(int id)
        {
            return _danhSachCongTrinhRepository.Xoa(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _danhSachCongTrinhRepository.KhoiPhuc(id);
        }

        public List<LookupViewModel> DanhSachCongTrinhLookup(LookupQueryViewModel lookupQueryViewModel, int? loaiCongTrinh = null)
        {
            return _danhSachCongTrinhRepository.DanhSachCongTrinhLookup(lookupQueryViewModel, loaiCongTrinh);
        }
    }
}
