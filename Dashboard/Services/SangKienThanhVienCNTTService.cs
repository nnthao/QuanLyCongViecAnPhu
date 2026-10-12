using Dashboard.Models.Entities;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface ISangKienThanhVienCNTTService
    {
        List<SangKienThanhVienCNTTGridVo> GetBySangKienCNTT(int sangKienCNTTId);
        SangKienThanhVienCNTTGridVo GetById(int id);
        bool Them(SangKienThanhVienCNTTViewModel viewModel);
        bool CapNhat(SangKienThanhVienCNTTViewModel viewModel);
        bool Xoa(int id, int? nguoiXoaId);
        bool LuuDanhSachThanhVien(int sangKienCNTTId, List<SangKienThanhVienCNTTViewModel> danhSach, int? nguoiXoaId);
        decimal TinhTongTiLeDongGop(int sangKienCNTTId, int? excludeId = null);
    }

    public class SangKienThanhVienCNTTService : ISangKienThanhVienCNTTService
    {
        private readonly ISangKienThanhVienCNTTRepository _repository;

        public SangKienThanhVienCNTTService(ISangKienThanhVienCNTTRepository repository)
        {
            _repository = repository;
        }

        public List<SangKienThanhVienCNTTGridVo> GetBySangKienCNTT(int sangKienCNTTId)
        {
            return _repository.GetBySangKienCNTT(sangKienCNTTId);
        }

        public SangKienThanhVienCNTTGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Them(SangKienThanhVienCNTTViewModel viewModel)
        {
            return _repository.Them(viewModel);
        }

        public bool CapNhat(SangKienThanhVienCNTTViewModel viewModel)
        {
            return _repository.CapNhat(viewModel);
        }

        public bool Xoa(int id, int? nguoiXoaId)
        {
            return _repository.Xoa(id, nguoiXoaId);
        }

        public bool LuuDanhSachThanhVien(int sangKienCNTTId, List<SangKienThanhVienCNTTViewModel> danhSach, int? nguoiXoaId)
        {
            return _repository.LuuDanhSachThanhVien(sangKienCNTTId, danhSach, nguoiXoaId);
        }

        public decimal TinhTongTiLeDongGop(int sangKienCNTTId, int? excludeId = null)
        {
            return _repository.TinhTongTiLeDongGop(sangKienCNTTId, excludeId);
        }
    }
}

