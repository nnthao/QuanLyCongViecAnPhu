using Dashboard.Models.Entities;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface ISangKienThanhVienService
    {
        List<SangKienThanhVienGridVo> GetBySangKien(int sangKienId);
        SangKienThanhVienGridVo GetById(int id);
        bool Them(SangKienThanhVienViewModel viewModel);
        bool CapNhat(SangKienThanhVienViewModel viewModel);
        bool Xoa(int id, int? nguoiXoaId);
        bool LuuDanhSachThanhVien(int sangKienId, List<SangKienThanhVienViewModel> danhSach, int? nguoiXoaId);
        decimal TinhTongTiLeDongGop(int sangKienId, int? excludeId = null);
    }

    public class SangKienThanhVienService : ISangKienThanhVienService
    {
        private readonly ISangKienThanhVienRepository _repository;

        public SangKienThanhVienService(ISangKienThanhVienRepository repository)
        {
            _repository = repository;
        }

        public List<SangKienThanhVienGridVo> GetBySangKien(int sangKienId)
        {
            return _repository.GetBySangKien(sangKienId);
        }

        public SangKienThanhVienGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Them(SangKienThanhVienViewModel viewModel)
        {
            return _repository.Them(viewModel);
        }

        public bool CapNhat(SangKienThanhVienViewModel viewModel)
        {
            return _repository.CapNhat(viewModel);
        }

        public bool Xoa(int id, int? nguoiXoaId)
        {
            return _repository.Xoa(id, nguoiXoaId);
        }

        public bool LuuDanhSachThanhVien(int sangKienId, List<SangKienThanhVienViewModel> danhSach, int? nguoiXoaId)
        {
            return _repository.LuuDanhSachThanhVien(sangKienId, danhSach, nguoiXoaId);
        }

        public decimal TinhTongTiLeDongGop(int sangKienId, int? excludeId = null)
        {
            return _repository.TinhTongTiLeDongGop(sangKienId, excludeId);
        }
    }
}


