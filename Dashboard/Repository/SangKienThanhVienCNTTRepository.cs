using Dashboard.Models;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Repository
{
    public interface ISangKienThanhVienCNTTRepository
    {
        List<SangKienThanhVienCNTTGridVo> GetBySangKienCNTT(int sangKienCNTTId);
        SangKienThanhVienCNTTGridVo GetById(int id);
        bool Them(SangKienThanhVienCNTTViewModel viewModel);
        bool CapNhat(SangKienThanhVienCNTTViewModel viewModel);
        bool Xoa(int id, int? nguoiXoaId);
        bool LuuDanhSachThanhVien(int sangKienCNTTId, List<SangKienThanhVienCNTTViewModel> danhSach, int? nguoiXoaId);
        decimal TinhTongTiLeDongGop(int sangKienCNTTId, int? excludeId = null);
    }

    public class SangKienThanhVienCNTTRepository : ISangKienThanhVienCNTTRepository
    {
        private readonly IContext _context;

        public SangKienThanhVienCNTTRepository(IContext context)
        {
            _context = context;
        }

        public List<SangKienThanhVienCNTTGridVo> GetBySangKienCNTT(int sangKienCNTTId)
        {
            var query = from tv in _context.SangKienThanhVienCNTTs.AsNoTracking()
                        join u in _context.Users.AsNoTracking() on tv.UserId equals u.Id into userJoin
                        from u in userJoin.DefaultIfEmpty()
                        where tv.SangKienCNTTId == sangKienCNTTId && (tv.IsDeleted == null || tv.IsDeleted != true)
                        orderby tv.TiLeDongGop descending
                        select new SangKienThanhVienCNTTGridVo
                        {
                            Id = tv.Id,
                            SangKienCNTTId = tv.SangKienCNTTId,
                            UserId = tv.UserId,
                            TiLeDongGop = tv.TiLeDongGop,
                            VaiTro = tv.VaiTro,
                            GhiChu = tv.GhiChu,
                            TenThanhVien = u != null ? u.Name : ""
                        };

            return query.ToList();
        }

        public SangKienThanhVienCNTTGridVo GetById(int id)
        {
            var query = from tv in _context.SangKienThanhVienCNTTs.AsNoTracking()
                        join u in _context.Users.AsNoTracking() on tv.UserId equals u.Id into userJoin
                        from u in userJoin.DefaultIfEmpty()
                        where tv.Id == id && (tv.IsDeleted == null || tv.IsDeleted != true)
                        select new SangKienThanhVienCNTTGridVo
                        {
                            Id = tv.Id,
                            SangKienCNTTId = tv.SangKienCNTTId,
                            UserId = tv.UserId,
                            TiLeDongGop = tv.TiLeDongGop,
                            VaiTro = tv.VaiTro,
                            GhiChu = tv.GhiChu,
                            TenThanhVien = u != null ? u.Name : ""
                        };

            return query.FirstOrDefault();
        }

        public bool Them(SangKienThanhVienCNTTViewModel viewModel)
        {
            var exists = _context.SangKienThanhVienCNTTs
                .Any(o => o.SangKienCNTTId == viewModel.SangKienCNTTId
                    && o.UserId == viewModel.UserId
                    && (o.IsDeleted == null || o.IsDeleted != true));

            if (exists)
            {
                return false;
            }

            var entity = new SangKienThanhVienCNTT
            {
                SangKienCNTTId = viewModel.SangKienCNTTId,
                UserId = viewModel.UserId,
                TiLeDongGop = viewModel.TiLeDongGop,
                VaiTro = viewModel.VaiTro,
                GhiChu = viewModel.GhiChu,
                NgayTao = DateTime.Now,
                IsDeleted = false
            };

            _context.SangKienThanhVienCNTTs.Add(entity);
            _context.SaveChanges();
            return true;
        }

        public bool CapNhat(SangKienThanhVienCNTTViewModel viewModel)
        {
            var entity = _context.SangKienThanhVienCNTTs.FirstOrDefault(o => o.Id == viewModel.Id);
            if (entity == null) return false;

            entity.TiLeDongGop = viewModel.TiLeDongGop;
            entity.VaiTro = viewModel.VaiTro;
            entity.GhiChu = viewModel.GhiChu;

            _context.SaveChanges();
            return true;
        }

        public bool Xoa(int id, int? nguoiXoaId)
        {
            var entity = _context.SangKienThanhVienCNTTs.FirstOrDefault(o => o.Id == id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.NgayXoa = DateTime.Now;
            entity.NguoiXoaId = nguoiXoaId;

            _context.SaveChanges();
            return true;
        }

        public bool LuuDanhSachThanhVien(int sangKienCNTTId, List<SangKienThanhVienCNTTViewModel> danhSach, int? nguoiXoaId)
        {
            var existingIds = danhSach.Where(o => o.Id > 0).Select(o => o.Id).ToList();

            var toDelete = _context.SangKienThanhVienCNTTs
                .Where(o => o.SangKienCNTTId == sangKienCNTTId
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
                    var entity = _context.SangKienThanhVienCNTTs.FirstOrDefault(o => o.Id == item.Id);
                    if (entity != null)
                    {
                        entity.TiLeDongGop = item.TiLeDongGop;
                        entity.VaiTro = item.VaiTro;
                        entity.GhiChu = item.GhiChu;
                    }
                }
                else
                {
                    var exists = _context.SangKienThanhVienCNTTs
                        .Any(o => o.SangKienCNTTId == sangKienCNTTId
                            && o.UserId == item.UserId
                            && (o.IsDeleted == null || o.IsDeleted != true));

                    if (!exists)
                    {
                        _context.SangKienThanhVienCNTTs.Add(new SangKienThanhVienCNTT
                        {
                            SangKienCNTTId = sangKienCNTTId,
                            UserId = item.UserId,
                            TiLeDongGop = item.TiLeDongGop,
                            VaiTro = item.VaiTro,
                            GhiChu = item.GhiChu,
                            NgayTao = DateTime.Now,
                            IsDeleted = false
                        });
                    }
                }
            }

            _context.SaveChanges();
            return true;
        }

        public decimal TinhTongTiLeDongGop(int sangKienCNTTId, int? excludeId = null)
        {
            var query = _context.SangKienThanhVienCNTTs
                .Where(o => o.SangKienCNTTId == sangKienCNTTId && (o.IsDeleted == null || o.IsDeleted != true));

            if (excludeId.HasValue)
            {
                query = query.Where(o => o.Id != excludeId.Value);
            }

            return query.Sum(o => (decimal?)o.TiLeDongGop) ?? 0;
        }
    }
}

