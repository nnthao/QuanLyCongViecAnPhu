using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface IQuanLyCongTrinhHuHongFileService
    {
        DataTableResult<QuanLyCongTrinhHuHongFileGridVo> GetAll(QueryParamViewModel queryParams);
        QuanLyCongTrinhHuHongFileGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(QuanLyCongTrinhHuHongFileViewModel quanLyCongTrinhHuHongFileViewModel);
        bool Xoa(int id);
        List<QuanLyCongTrinhHuHongFileGridVo> GetByCongTrinhHuHongId(int congTrinhHuHongId);
    }

    public class QuanLyCongTrinhHuHongFileService : IQuanLyCongTrinhHuHongFileService
    {
        private readonly IQuanLyCongTrinhHuHongFileRepository _quanLyCongTrinhHuHongFileRepository;
        private readonly IUserRepository _userRepository;
        public QuanLyCongTrinhHuHongFileService(IQuanLyCongTrinhHuHongFileRepository quanLyCongTrinhHuHongFileRepository, IUserRepository userRepository)
        {
            _quanLyCongTrinhHuHongFileRepository = quanLyCongTrinhHuHongFileRepository;
            _userRepository = userRepository;
        }

        public DataTableResult<QuanLyCongTrinhHuHongFileGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _quanLyCongTrinhHuHongFileRepository.GetAll(queryParams);
            return data;
        }

        public QuanLyCongTrinhHuHongFileGridVo GetById(int id)
        {
            return _quanLyCongTrinhHuHongFileRepository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _quanLyCongTrinhHuHongFileRepository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(QuanLyCongTrinhHuHongFileViewModel quanLyCongTrinhHuHongFileViewModel)
        {
            return _quanLyCongTrinhHuHongFileRepository.ThemOrCapNhat(quanLyCongTrinhHuHongFileViewModel);
        }

        public bool Xoa(int id)
        {
            return _quanLyCongTrinhHuHongFileRepository.Xoa(id);
        }

        public List<QuanLyCongTrinhHuHongFileGridVo> GetByCongTrinhHuHongId(int congTrinhHuHongId)
        {
            return _quanLyCongTrinhHuHongFileRepository.GetByCongTrinhHuHongId(congTrinhHuHongId);
        }
    }
}
