using Dashboard.Models.CommonModel;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface ISangKienCNTTService
    {
        DataTableResult<SangKienCNTTGridVo> GetAll(QueryParamViewModel queryParams);
        SangKienCNTTViewModel GetById(int id);
        int ThemOrCapNhat(SangKienCNTTViewModel viewModel);
        void Xoa(int id);
        void KhoiPhuc(int id);
        object GetThongKeTongSoSangKien();
        object GetThongKeXepLoai();
        object GetThongKeQuyTienThuong();
        object GetThongKeTongTienThuongThanhVien();
    }

    public class SangKienCNTTService : ISangKienCNTTService
    {
        private readonly ISangKienCNTTRepository _repository;

        public SangKienCNTTService(ISangKienCNTTRepository repository)
        {
            _repository = repository;
        }

        public DataTableResult<SangKienCNTTGridVo> GetAll(QueryParamViewModel queryParams)
        {
            return _repository.GetAll(queryParams);
        }

        public SangKienCNTTViewModel GetById(int id)
        {
            return _repository.GetById(id);
        }

        public int ThemOrCapNhat(SangKienCNTTViewModel viewModel)
        {
            return _repository.ThemOrCapNhat(viewModel);
        }

        public void Xoa(int id)
        {
            _repository.Xoa(id);
        }

        public void KhoiPhuc(int id)
        {
            _repository.KhoiPhuc(id);
        }

        public object GetThongKeTongSoSangKien()
        {
            return _repository.GetThongKeTongSoSangKien();
        }

        public object GetThongKeXepLoai()
        {
            return _repository.GetThongKeXepLoai();
        }

        public object GetThongKeQuyTienThuong()
        {
            return _repository.GetThongKeQuyTienThuong();
        }

        public object GetThongKeTongTienThuongThanhVien()
        {
            return _repository.GetThongKeTongTienThuongThanhVien();
        }
    }
}

