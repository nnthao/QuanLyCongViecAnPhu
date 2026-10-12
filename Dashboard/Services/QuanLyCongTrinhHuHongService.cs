using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface IQuanLyCongTrinhHuHongService
    {
        DataTableResult<QuanLyCongTrinhHuHongGridVo> GetAll(QueryParamViewModel queryParams);
        QuanLyCongTrinhHuHongGridVo GetById(int id);
        bool KhoiPhuc(int id);
        List<LookupViewModel> QuanLyCongTrinhHuHongLookup(LookupQueryViewModel lookupQueryViewModel);
        dynamic ThemOrCapNhat(QuanLyCongTrinhHuHongViewModel quanLyCongTrinhHuHongViewModel);
        bool Xoa(int id);
    }

    public class QuanLyCongTrinhHuHongService : IQuanLyCongTrinhHuHongService
    {
        private readonly IQuanLyCongTrinhHuHongRepository _quanLyCongTrinhHuHongRepository;
        private readonly IUserRepository _userRepository;
        public QuanLyCongTrinhHuHongService(IQuanLyCongTrinhHuHongRepository quanLyCongTrinhHuHongRepository, IUserRepository userRepository)
        {
            _quanLyCongTrinhHuHongRepository = quanLyCongTrinhHuHongRepository;
            _userRepository = userRepository;
        }

        public List<LookupViewModel> QuanLyCongTrinhHuHongLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _quanLyCongTrinhHuHongRepository.QuanLyCongTrinhHuHongLookup(lookupQueryViewModel);
        }

        public DataTableResult<QuanLyCongTrinhHuHongGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _quanLyCongTrinhHuHongRepository.GetAll(queryParams);
            return data;
        }

        public QuanLyCongTrinhHuHongGridVo GetById(int id)
        {
            return _quanLyCongTrinhHuHongRepository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _quanLyCongTrinhHuHongRepository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(QuanLyCongTrinhHuHongViewModel quanLyCongTrinhHuHongViewModel)
        {
            return _quanLyCongTrinhHuHongRepository.ThemOrCapNhat(quanLyCongTrinhHuHongViewModel);
        }

        public bool Xoa(int id)
        {
            return _quanLyCongTrinhHuHongRepository.Xoa(id);
        }
    }
}
