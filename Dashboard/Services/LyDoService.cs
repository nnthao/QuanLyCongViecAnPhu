using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface ILyDoService
    {
        dynamic CapNhat(LyDoViewModel lyDoViewModel);
        DataLookupViewModel LyDoLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<LyDoGridVo> GetAll(QueryParamViewModel queryParams);
        LyDo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic Them(LyDoViewModel lyDoViewModel);
        bool Xoa(int id);
    }

    public class LyDoService : ILyDoService
    {
        private readonly ILyDoRepository _lyDoRepository;
        public LyDoService(ILyDoRepository lyDoRepository)
        {
            _lyDoRepository = lyDoRepository;
        }

        public dynamic CapNhat(LyDoViewModel lyDoViewModel)
        {
            return _lyDoRepository.CapNhat(lyDoViewModel);
        }

        public DataLookupViewModel LyDoLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _lyDoRepository.LyDoLookup(lookupQueryViewModel);
        }

        public DataTableResult<LyDoGridVo> GetAll(QueryParamViewModel queryParams)
        {
            return _lyDoRepository.GetAll(queryParams);
        }

        public LyDo GetById(int id)
        {
            return _lyDoRepository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _lyDoRepository.KhoiPhuc(id);
        }

        public dynamic Them(LyDoViewModel lyDoViewModel)
        {
            return _lyDoRepository.Them(lyDoViewModel);
        }

        public bool Xoa(int id)
        {
            return _lyDoRepository.Xoa(id);
        }
    }
}