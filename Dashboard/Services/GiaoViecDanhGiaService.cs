using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface IGiaoViecDanhGiaService
    {
        DataTableResult<GiaoViecThanhVienGridVo> GetListDanhGia(GiaoViecDanhGiaQueryViewModel queryParams);
        ChiTietDanhGiaItemVo GetChiTietDanhGia(int giaoViecThanhVienId);
        dynamic LuuDanhGia(LuuDanhGiaViewModel viewModel);
        int DemSoLuongCongViecChoDanhGia(int userId);
    }

    public class GiaoViecDanhGiaService : IGiaoViecDanhGiaService
    {
        private readonly IGiaoViecDanhGiaRepository _repository;

        public GiaoViecDanhGiaService(IGiaoViecDanhGiaRepository repository)
        {
            _repository = repository;
        }

        public DataTableResult<GiaoViecThanhVienGridVo> GetListDanhGia(GiaoViecDanhGiaQueryViewModel queryParams)
        {
            return _repository.GetListDanhGia(queryParams);
        }

        public ChiTietDanhGiaItemVo GetChiTietDanhGia(int giaoViecThanhVienId)
        {
            return _repository.GetChiTietDanhGia(giaoViecThanhVienId);
        }

        public dynamic LuuDanhGia(LuuDanhGiaViewModel viewModel)
        {
            return _repository.LuuDanhGia(viewModel);
        }

        public int DemSoLuongCongViecChoDanhGia(int userId)
        {
            return _repository.DemSoLuongCongViecChoDanhGia(userId);
        }
    }
}

