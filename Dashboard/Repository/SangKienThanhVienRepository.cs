using Dashboard.Models;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Repository
{
    public interface ISangKienThanhVienRepository
    {
        List<SangKienThanhVienGridVo> GetBySangKien(int sangKienId);
        SangKienThanhVienGridVo GetById(int id);
        bool Them(SangKienThanhVienViewModel viewModel);
        bool CapNhat(SangKienThanhVienViewModel viewModel);
        bool Xoa(int id, int? nguoiXoaId);
        bool XoaTheoDanhSach(int sangKienId, List<int> thanhVienIds, int? nguoiXoaId);
        bool LuuDanhSachThanhVien(int sangKienId, List<SangKienThanhVienViewModel> danhSach, int? nguoiXoaId);
        decimal TinhTongTiLeDongGop(int sangKienId, int? excludeId = null);
    }

    public class SangKienThanhVienRepository : ISangKienThanhVienRepository
    {
        private readonly IContext _context;

        public SangKienThanhVienRepository(IContext context)
        {
            _context = context;
        }

        public List<SangKienThanhVienGridVo> GetBySangKien(int sangKienId)
        {
            var query = from tv in _context.SangKienThanhViens.AsNoTracking()
                        join u in _context.Users.AsNoTracking() on tv.UserId equals u.Id into userJoin
                        from u in userJoin.DefaultIfEmpty()
                        where tv.SangKienId == sangKienId && (tv.IsDeleted == null || tv.IsDeleted != true)
                        orderby tv.TiLeDongGop descending
                        select new SangKienThanhVienGridVo
                        {
                            Id = tv.Id,
                            SangKienId = tv.SangKienId,
                            UserId = tv.UserId,
                            TiLeDongGop = tv.TiLeDongGop,
                            VaiTro = tv.VaiTro,
                            GhiChu = tv.GhiChu,
                            TenThanhVien = u != null ? u.Name : ""
                        };

            return query.ToList();
        }

        public SangKienThanhVienGridVo GetById(int id)
        {
            var query = from tv in _context.SangKienThanhViens.AsNoTracking()
                        join u in _context.Users.AsNoTracking() on tv.UserId equals u.Id into userJoin
                        from u in userJoin.DefaultIfEmpty()
                        where tv.Id == id && (tv.IsDeleted == null || tv.IsDeleted != true)
                        select new SangKienThanhVienGridVo
                        {
                            Id = tv.Id,
                            SangKienId = tv.SangKienId,
                            UserId = tv.UserId,
                            TiLeDongGop = tv.TiLeDongGop,
                            VaiTro = tv.VaiTro,
                            GhiChu = tv.GhiChu,
                            TenThanhVien = u != null ? u.Name : ""
                        };

            return query.FirstOrDefault();
        }

        public bool Them(SangKienThanhVienViewModel viewModel)
        {
            var exists = _context.SangKienThanhViens
                .Any(o => o.SangKienId == viewModel.SangKienId
                    && o.UserId == viewModel.UserId
                    && (o.IsDeleted == null || o.IsDeleted != true));

            if (exists)
            {
                return false;
            }

            var entity = new SangKienThanhVien
            {
                SangKienId = viewModel.SangKienId,
                UserId = viewModel.UserId,
                TiLeDongGop = viewModel.TiLeDongGop,
                VaiTro = viewModel.VaiTro,
                GhiChu = viewModel.GhiChu,
                NgayTao = DateTime.Now,
                IsDeleted = false
            };

            _context.SangKienThanhViens.Add(entity);
            _context.SaveChanges();
            return true;
        }

        public bool CapNhat(SangKienThanhVienViewModel viewModel)
        {
            var entity = _context.SangKienThanhViens.FirstOrDefault(o => o.Id == viewModel.Id);
            if (entity == null) return false;

            entity.TiLeDongGop = viewModel.TiLeDongGop;
            entity.VaiTro = viewModel.VaiTro;
            entity.GhiChu = viewModel.GhiChu;

            _context.SaveChanges();
            return true;
        }

        public bool Xoa(int id, int? nguoiXoaId)
        {
            var entity = _context.SangKienThanhViens.FirstOrDefault(o => o.Id == id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.NgayXoa = DateTime.Now;
            entity.NguoiXoaId = nguoiXoaId;

            _context.SaveChanges();
            return true;
        }

        public bool XoaTheoDanhSach(int sangKienId, List<int> thanhVienIds, int? nguoiXoaId)
        {
            var entities = _context.SangKienThanhViens
                .Where(o => o.SangKienId == sangKienId
                    && !thanhVienIds.Contains(o.Id)
                    && (o.IsDeleted == null || o.IsDeleted != true))
                .ToList();

            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
                entity.NgayXoa = DateTime.Now;
                entity.NguoiXoaId = nguoiXoaId;
            }

            _context.SaveChanges();
            return true;
        }

        public bool LuuDanhSachThanhVien(int sangKienId, List<SangKienThanhVienViewModel> danhSach, int? nguoiXoaId)
        {
            var existingIds = danhSach.Where(o => o.Id > 0).Select(o => o.Id).ToList();

            var toDelete = _context.SangKienThanhViens
                .Where(o => o.SangKienId == sangKienId
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
                    var entity = _context.SangKienThanhViens.FirstOrDefault(o => o.Id == item.Id);
                    if (entity != null)
                    {
                        entity.TiLeDongGop = item.TiLeDongGop;
                        entity.VaiTro = item.VaiTro;
                        entity.GhiChu = item.GhiChu;
                    }
                }
                else
                {
                    var exists = _context.SangKienThanhViens
                        .Any(o => o.SangKienId == sangKienId
                            && o.UserId == item.UserId
                            && (o.IsDeleted == null || o.IsDeleted != true));

                    if (!exists)
                    {
                        _context.SangKienThanhViens.Add(new SangKienThanhVien
                        {
                            SangKienId = sangKienId,
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

        public decimal TinhTongTiLeDongGop(int sangKienId, int? excludeId = null)
        {
            var query = _context.SangKienThanhViens
                .Where(o => o.SangKienId == sangKienId && (o.IsDeleted == null || o.IsDeleted != true));

            if (excludeId.HasValue)
            {
                query = query.Where(o => o.Id != excludeId.Value);
            }

            return query.Sum(o => (decimal?)o.TiLeDongGop) ?? 0;
        }
    }
}


