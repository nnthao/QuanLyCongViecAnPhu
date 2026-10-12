using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface IPhongHopService
    {
        DataTableResult<PhongHopGridVo> GetAll(QueryParamViewModel queryParams);
        PhongHopGridVo GetById(int id);
        bool KhoiPhuc(int id);
        DataLookupViewModel PhongHopLookup(LookupQueryViewModel lookupQueryViewModel);
        dynamic ThemOrCapNhat(PhongHopViewModel phongHopViewModel);
        bool Xoa(int id);
    }

    public class PhongHopService : IPhongHopService
    {
        private readonly IPhongHopRepository _phongHopRepository;
        public PhongHopService(IPhongHopRepository phongHopRepository)
        {
            _phongHopRepository = phongHopRepository;
        }

        public DataTableResult<PhongHopGridVo> GetAll(QueryParamViewModel queryParams)
        {
            return _phongHopRepository.GetAll(queryParams);
        }

        public PhongHopGridVo GetById(int id)
        {
            return _phongHopRepository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _phongHopRepository.KhoiPhuc(id);
        }

        public DataLookupViewModel PhongHopLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _phongHopRepository.PhongHopLookup(lookupQueryViewModel);
        }

        public dynamic ThemOrCapNhat(PhongHopViewModel phongHopViewModel)
        {
            return _phongHopRepository.ThemOrCapNhat(phongHopViewModel);
        }

        public bool Xoa(int id)
        {
            return _phongHopRepository.Xoa(id);
        }
    }
}