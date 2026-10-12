using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface IDuAnTrienKhaiService
    {
        DataTableResult<DuAnTrienKhaiGridVo> GetAll(DuAnTrienKhaiQueryViewModel queryParams);
        DuAnTrienKhaiGridVo GetById(int id);
        dynamic ThemOrCapNhat(DuAnTrienKhaiViewModel model);
        bool Xoa(int id);
        bool KhoiPhuc(int id);
        List<DuAnTrienKhaiHangMucGridVo> GetHangMucMau(int duAnId);
    }

    public class DuAnTrienKhaiService : IDuAnTrienKhaiService
    {
        private readonly IDuAnTrienKhaiRepository _repository;

        public DuAnTrienKhaiService(IDuAnTrienKhaiRepository repository)
        {
            _repository = repository;
        }

        public DataTableResult<DuAnTrienKhaiGridVo> GetAll(DuAnTrienKhaiQueryViewModel queryParams)
        {
            return _repository.GetAll(queryParams);
        }

        public DuAnTrienKhaiGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public dynamic ThemOrCapNhat(DuAnTrienKhaiViewModel model)
        {
            return _repository.ThemOrCapNhat(model);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public List<DuAnTrienKhaiHangMucGridVo> GetHangMucMau(int duAnId)
        {
            return _repository.GetHangMucMau(duAnId);
        }
    }
}
