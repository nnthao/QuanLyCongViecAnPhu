using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface IChucDanhService
    {
        dynamic CapNhat(ChucDanhViewModel chucDanhViewModel);
        bool CapNhatThuTu(CapNhatThuTuChucDanhRequest request);
        DataLookupViewModel ChucDanhLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<ChucDanhGridVo> GetAll(QueryParamViewModel queryParams);
        ChucDanh GetById(int id);
        bool KhoiPhuc(int id);
        dynamic Them(ChucDanhViewModel chucDanhViewModel);
        bool Xoa(int id);
    }

    public class ChucDanhService : IChucDanhService
    {
        private readonly IChucDanhRepository _chucDanhRepository;
        public ChucDanhService(IChucDanhRepository chucDanhRepository)
        {
            _chucDanhRepository = chucDanhRepository;
        }

        public dynamic CapNhat(ChucDanhViewModel chucDanhViewModel)
        {
            return _chucDanhRepository.CapNhat(chucDanhViewModel);
        }

        public bool CapNhatThuTu(CapNhatThuTuChucDanhRequest request)
        {
            return _chucDanhRepository.CapNhatThuTu(request);
        }

        public DataLookupViewModel ChucDanhLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _chucDanhRepository.ChucDanhLookup(lookupQueryViewModel);
        }

        public DataTableResult<ChucDanhGridVo> GetAll(QueryParamViewModel queryParams)
        {
            return _chucDanhRepository.GetAll(queryParams);
        }

        public ChucDanh GetById(int id)
        {
            return _chucDanhRepository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _chucDanhRepository.KhoiPhuc(id);
        }

        public dynamic Them(ChucDanhViewModel chucDanhViewModel)
        {
            return _chucDanhRepository.Them(chucDanhViewModel);
        }

        public bool Xoa(int id)
        {
            return _chucDanhRepository.Xoa(id);
        }
    }
}