using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface IDotXetDuyetService
    {
        DataTableResult<DotXetDuyetGridVo> GetAll(QueryParamViewModel queryParams);
        DotXetDuyetGridVo GetById(int id);
        bool KhoiPhuc(int id);
        List<LookupViewModel> DotXetDuyetLookup(LookupQueryViewModel lookupQueryViewModel);
        dynamic ThemOrCapNhat(DotXetDuyetViewModel dotXetDuyetViewModel);
        bool Xoa(int id);
    }

    public class DotXetDuyetService : IDotXetDuyetService
    {
        private readonly IDotXetDuyetRepository _dotXetDuyetRepository;
        private readonly IUserRepository _userRepository;
        public DotXetDuyetService(IDotXetDuyetRepository dotXetDuyetRepository, IUserRepository userRepository)
        {
            _dotXetDuyetRepository = dotXetDuyetRepository;
            _userRepository = userRepository;
        }

        public List<LookupViewModel> DotXetDuyetLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _dotXetDuyetRepository.DotXetDuyetLookup(lookupQueryViewModel);
        }

        public DataTableResult<DotXetDuyetGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _dotXetDuyetRepository.GetAll(queryParams);
            return data;
        }

        public DotXetDuyetGridVo GetById(int id)
        {
            return _dotXetDuyetRepository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _dotXetDuyetRepository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(DotXetDuyetViewModel dotXetDuyetViewModel)
        {
            return _dotXetDuyetRepository.ThemOrCapNhat(dotXetDuyetViewModel);
        }

        public bool Xoa(int id)
        {
            return _dotXetDuyetRepository.Xoa(id);
        }
    }
}

