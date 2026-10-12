using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;
using System.Web;

namespace Dashboard.Services
{
    public interface IDoanVien_TaiLieuService
    {
        DataTableResult<DoanVien_TaiLieuGridVo> GetAll(QueryParamViewModel queryParams);
        DoanVien_TaiLieuGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(List<HttpPostedFileBase> files);
        bool Xoa(int id);
    }

    public class DoanVien_TaiLieuService : IDoanVien_TaiLieuService
    {
        private readonly IDoanVien_TaiLieuRepository _repository;

        public DoanVien_TaiLieuService(IDoanVien_TaiLieuRepository repository)
        {
            _repository = repository;
        }

        public DataTableResult<DoanVien_TaiLieuGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _repository.GetAll(queryParams);
            return data;
        }

        public DoanVien_TaiLieuGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(List<HttpPostedFileBase> files)
        {
            return _repository.ThemOrCapNhat(files);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }
    }
}

