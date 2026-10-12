using Dashboard.Models.Entities;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface ISangKienPhanThuongService
    {
        List<SangKienPhanThuongGridVo> GetByDotXetDuyet(int dotXetDuyetId);
        SangKienPhanThuongGridVo GetById(int id);
        bool Them(SangKienPhanThuongViewModel viewModel);
        bool CapNhat(SangKienPhanThuongViewModel viewModel);
        bool Xoa(int id, int? nguoiXoaId);
        bool LuuDanhSachPhanThuong(int dotXetDuyetId, List<SangKienPhanThuongViewModel> danhSach, int? nguoiXoaId);
    }

    public class SangKienPhanThuongService : ISangKienPhanThuongService
    {
        private readonly ISangKienPhanThuongRepository _repository;

        public SangKienPhanThuongService(ISangKienPhanThuongRepository repository)
        {
            _repository = repository;
        }

        public List<SangKienPhanThuongGridVo> GetByDotXetDuyet(int dotXetDuyetId)
        {
            return _repository.GetByDotXetDuyet(dotXetDuyetId);
        }

        public SangKienPhanThuongGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Them(SangKienPhanThuongViewModel viewModel)
        {
            return _repository.Them(viewModel);
        }

        public bool CapNhat(SangKienPhanThuongViewModel viewModel)
        {
            return _repository.CapNhat(viewModel);
        }

        public bool Xoa(int id, int? nguoiXoaId)
        {
            return _repository.Xoa(id, nguoiXoaId);
        }

        public bool LuuDanhSachPhanThuong(int dotXetDuyetId, List<SangKienPhanThuongViewModel> danhSach, int? nguoiXoaId)
        {
            return _repository.LuuDanhSachPhanThuong(dotXetDuyetId, danhSach, nguoiXoaId);
        }
    }
}


