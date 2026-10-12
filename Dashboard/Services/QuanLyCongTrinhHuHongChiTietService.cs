using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface IQuanLyCongTrinhHuHongChiTietService
    {
        DataTableResult<QuanLyCongTrinhHuHongChiTietGridVo> GetAll(QueryParamViewModel queryParams);
        QuanLyCongTrinhHuHongChiTietGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(QuanLyCongTrinhHuHongChiTietViewModel quanLyCongTrinhHuHongChiTietViewModel);
        bool Xoa(int id);
        List<QuanLyCongTrinhHuHongChiTietGridVo> GetByCongTrinhHuHongId(int congTrinhHuHongId);
    }

    public class QuanLyCongTrinhHuHongChiTietService : IQuanLyCongTrinhHuHongChiTietService
    {
        private readonly IQuanLyCongTrinhHuHongChiTietRepository _quanLyCongTrinhHuHongChiTietRepository;
        private readonly IUserRepository _userRepository;
        public QuanLyCongTrinhHuHongChiTietService(IQuanLyCongTrinhHuHongChiTietRepository quanLyCongTrinhHuHongChiTietRepository, IUserRepository userRepository)
        {
            _quanLyCongTrinhHuHongChiTietRepository = quanLyCongTrinhHuHongChiTietRepository;
            _userRepository = userRepository;
        }

        public DataTableResult<QuanLyCongTrinhHuHongChiTietGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _quanLyCongTrinhHuHongChiTietRepository.GetAll(queryParams);
            return data;
        }

        public QuanLyCongTrinhHuHongChiTietGridVo GetById(int id)
        {
            return _quanLyCongTrinhHuHongChiTietRepository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _quanLyCongTrinhHuHongChiTietRepository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(QuanLyCongTrinhHuHongChiTietViewModel quanLyCongTrinhHuHongChiTietViewModel)
        {
            return _quanLyCongTrinhHuHongChiTietRepository.ThemOrCapNhat(quanLyCongTrinhHuHongChiTietViewModel);
        }

        public bool Xoa(int id)
        {
            return _quanLyCongTrinhHuHongChiTietRepository.Xoa(id);
        }

        public List<QuanLyCongTrinhHuHongChiTietGridVo> GetByCongTrinhHuHongId(int congTrinhHuHongId)
        {
            return _quanLyCongTrinhHuHongChiTietRepository.GetByCongTrinhHuHongId(congTrinhHuHongId);
        }
    }
}
