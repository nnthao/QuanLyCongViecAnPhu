using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface IKhoNhiemVuHoanThanhService
    {
        DataTableResult<GiaoViecThanhVienGridVo> GetListHoanThanh(KhoNhiemVuHoanThanhQueryViewModel queryParams);
        dynamic HuyDanhGia(HuyDanhGiaViewModel viewModel);
        int DemSoLuongHoanThanh(int currentUserId);
    }

    public class KhoNhiemVuHoanThanhService : IKhoNhiemVuHoanThanhService
    {
        private readonly IKhoNhiemVuHoanThanhRepository _repository;

        public KhoNhiemVuHoanThanhService(IKhoNhiemVuHoanThanhRepository repository)
        {
            _repository = repository;
        }

        public DataTableResult<GiaoViecThanhVienGridVo> GetListHoanThanh(KhoNhiemVuHoanThanhQueryViewModel queryParams)
        {
            return _repository.GetListHoanThanh(queryParams);
        }

        public dynamic HuyDanhGia(HuyDanhGiaViewModel viewModel)
        {
            return _repository.HuyDanhGia(viewModel);
        }

        public int DemSoLuongHoanThanh(int currentUserId)
        {
            return _repository.DemSoLuongHoanThanh(currentUserId);
        }
    }
}

