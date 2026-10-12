using Dashboard.Models;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Repository
{
    public interface ISangKienPhanThuongRepository
    {
        List<SangKienPhanThuongGridVo> GetByDotXetDuyet(int dotXetDuyetId);
        SangKienPhanThuongGridVo GetById(int id);
        bool Them(SangKienPhanThuongViewModel viewModel);
        bool CapNhat(SangKienPhanThuongViewModel viewModel);
        bool Xoa(int id, int? nguoiXoaId);
        bool LuuDanhSachPhanThuong(int dotXetDuyetId, List<SangKienPhanThuongViewModel> danhSach, int? nguoiXoaId);
    }

    public class SangKienPhanThuongRepository : ISangKienPhanThuongRepository
    {
        private readonly IContext _context;

        public SangKienPhanThuongRepository(IContext context)
        {
            _context = context;
        }

        public List<SangKienPhanThuongGridVo> GetByDotXetDuyet(int dotXetDuyetId)
        {
            var query = _context.SangKienPhanThuongs.AsNoTracking()
                .Where(o => o.DotXetDuyetId == dotXetDuyetId && (o.IsDeleted == null || o.IsDeleted != true))
                .OrderBy(o => o.GiaiThuongXepLoai)
                .Select(o => new SangKienPhanThuongGridVo
                {
                    Id = o.Id,
                    DotXetDuyetId = o.DotXetDuyetId,
                    GiaiThuongXepLoai = o.GiaiThuongXepLoai,
                    TienThuongXepLoai = o.TienThuongXepLoai
                });

            return query.ToList();
        }

        public SangKienPhanThuongGridVo GetById(int id)
        {
            var entity = _context.SangKienPhanThuongs.AsNoTracking()
                .FirstOrDefault(o => o.Id == id && (o.IsDeleted == null || o.IsDeleted != true));

            if (entity == null) return null;

            return new SangKienPhanThuongGridVo
            {
                Id = entity.Id,
                DotXetDuyetId = entity.DotXetDuyetId,
                GiaiThuongXepLoai = entity.GiaiThuongXepLoai,
                TienThuongXepLoai = entity.TienThuongXepLoai
            };
        }

        public bool Them(SangKienPhanThuongViewModel viewModel)
        {
            var exists = _context.SangKienPhanThuongs
                .Any(o => o.DotXetDuyetId == viewModel.DotXetDuyetId
                    && o.GiaiThuongXepLoai == viewModel.GiaiThuongXepLoai
                    && (o.IsDeleted == null || o.IsDeleted != true));

            if (exists)
            {
                return false;
            }

            var entity = new SangKienPhanThuong
            {
                DotXetDuyetId = viewModel.DotXetDuyetId,
                GiaiThuongXepLoai = viewModel.GiaiThuongXepLoai,
                TienThuongXepLoai = viewModel.TienThuongXepLoai,
                NgayTao = DateTime.Now,
                IsDeleted = false
            };

            _context.SangKienPhanThuongs.Add(entity);
            _context.SaveChanges();
            return true;
        }

        public bool CapNhat(SangKienPhanThuongViewModel viewModel)
        {
            var entity = _context.SangKienPhanThuongs.FirstOrDefault(o => o.Id == viewModel.Id);
            if (entity == null) return false;

            entity.GiaiThuongXepLoai = viewModel.GiaiThuongXepLoai;
            entity.TienThuongXepLoai = viewModel.TienThuongXepLoai;

            _context.SaveChanges();
            return true;
        }

        public bool Xoa(int id, int? nguoiXoaId)
        {
            var entity = _context.SangKienPhanThuongs.FirstOrDefault(o => o.Id == id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.NgayXoa = DateTime.Now;
            entity.NguoiXoaId = nguoiXoaId;

            _context.SaveChanges();
            return true;
        }

        public bool LuuDanhSachPhanThuong(int dotXetDuyetId, List<SangKienPhanThuongViewModel> danhSach, int? nguoiXoaId)
        {
            var existingIds = danhSach.Where(o => o.Id > 0).Select(o => o.Id).ToList();

            var toDelete = _context.SangKienPhanThuongs
                .Where(o => o.DotXetDuyetId == dotXetDuyetId
                    && !existingIds.Contains(o.Id)
                    && (o.IsDeleted == null || o.IsDeleted != true))
                .ToList();

            foreach (var entity in toDelete)
            {
                entity.IsDeleted = true;
                entity.NgayXoa = DateTime.Now;
                entity.NguoiXoaId = nguoiXoaId;
            }

            foreach (var item in danhSach)
            {
                if (item.Id > 0)
                {
                    var entity = _context.SangKienPhanThuongs.FirstOrDefault(o => o.Id == item.Id);
                    if (entity != null)
                    {
                        entity.GiaiThuongXepLoai = item.GiaiThuongXepLoai;
                        entity.TienThuongXepLoai = item.TienThuongXepLoai;
                    }
                }
                else
                {
                    var exists = _context.SangKienPhanThuongs
                        .Any(o => o.DotXetDuyetId == dotXetDuyetId
                            && o.GiaiThuongXepLoai == item.GiaiThuongXepLoai
                            && (o.IsDeleted == null || o.IsDeleted != true));

                    if (!exists)
                    {
                        _context.SangKienPhanThuongs.Add(new SangKienPhanThuong
                        {
                            DotXetDuyetId = dotXetDuyetId,
                            GiaiThuongXepLoai = item.GiaiThuongXepLoai,
                            TienThuongXepLoai = item.TienThuongXepLoai,
                            NgayTao = DateTime.Now,
                            IsDeleted = false
                        });
                    }
                }
            }

            _context.SaveChanges();
            return true;
        }
    }
}

