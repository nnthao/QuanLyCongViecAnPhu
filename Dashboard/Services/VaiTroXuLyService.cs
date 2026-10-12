using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface IVaiTroXuLyService
    {
        DataTableResult<VaiTroXuLyGridVo> GetAll(QueryParamViewModel queryParams);
        VaiTroXuLyGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(VaiTroXuLyViewModel model);
        bool CapNhatThuTu(CapNhatThuTuVaiTroXuLyRequest request);
        bool CapNhatIsThongKeGiaoViec(CapNhatIsThongKeGiaoViecRequest request);
        bool CapNhatIsCanDinhKemFileKhiXacNhanHoanThanh(CapNhatIsCanDinhKemFileKhiXacNhanHoanThanhRequest request);
        bool Xoa(int id);
        DataLookupViewModel VaiTroXuLyLookup(LookupQueryViewModel lookupQueryViewModel);
    }

    public class VaiTroXuLyService : IVaiTroXuLyService
    {
        private readonly IVaiTroXuLyRepository _repository;

        public VaiTroXuLyService(IVaiTroXuLyRepository repository)
        {
            _repository = repository;
        }

        public DataTableResult<VaiTroXuLyGridVo> GetAll(QueryParamViewModel queryParams)
        {
            return _repository.GetAll(queryParams);
        }

        public VaiTroXuLyGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(VaiTroXuLyViewModel model)
        {
            return _repository.ThemOrCapNhat(model);
        }

        public bool CapNhatThuTu(CapNhatThuTuVaiTroXuLyRequest request)
        {
            return _repository.CapNhatThuTu(request);
        }

        public bool CapNhatIsThongKeGiaoViec(CapNhatIsThongKeGiaoViecRequest request)
        {
            return _repository.CapNhatIsThongKeGiaoViec(request);
        }

        public bool CapNhatIsCanDinhKemFileKhiXacNhanHoanThanh(CapNhatIsCanDinhKemFileKhiXacNhanHoanThanhRequest request)
        {
            return _repository.CapNhatIsCanDinhKemFileKhiXacNhanHoanThanh(request);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }

        public DataLookupViewModel VaiTroXuLyLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _repository.VaiTroXuLyLookup(lookupQueryViewModel);
        }
    }
}
