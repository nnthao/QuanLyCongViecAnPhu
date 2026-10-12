using Dashboard.Models;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.Repository
{
    public interface IPhanCongChamRepository
    {
        List<PhanCongChamViewModel> GetBySangKien(int sangKienId);
        bool Them(PhanCongChamViewModel viewModel);
        bool Xoa(int id);
        bool Exists(int sangKienId, int userId);
    }

    public class PhanCongChamRepository : IPhanCongChamRepository
    {
        private readonly IContext _context;
        public PhanCongChamRepository(IContext context)
        {
            _context = context;
        }

        public List<PhanCongChamViewModel> GetBySangKien(int sangKienId)
        {
            var query = from pc in _context.PhanCongChams.AsQueryable()
                        join u in _context.Users.AsQueryable() on pc.UserId equals u.Id
                        where pc.SangKienId == sangKienId && (pc.IsDeleted == null || pc.IsDeleted != true)
                        select new PhanCongChamViewModel
                        {
                            Id = pc.Id,
                            SangKienId = pc.SangKienId,
                            UserId = pc.UserId,
                            NgayPhanCong = pc.NgayTao,
                            GhiChu = pc.TrangThai,
                            TenNguoiCham = u.Name
                        };

            return query.OrderByDescending(x => x.NgayPhanCong).ToList();
        }

        public bool Exists(int sangKienId, int userId)
        {
            return _context.PhanCongChams.Any(o => o.SangKienId == sangKienId && o.UserId == userId && (o.IsDeleted == null || o.IsDeleted != true));
        }

        public bool Them(PhanCongChamViewModel viewModel)
        {
            if (Exists(viewModel.SangKienId, viewModel.UserId))
            {
                return true;
            }

            var entity = new PhanCongCham
            {
                SangKienId = viewModel.SangKienId,
                UserId = viewModel.UserId,
                NgayTao = viewModel.NgayPhanCong ?? DateTime.Now,
                TrangThai = viewModel.GhiChu,
                IsDeleted = false
            };

            _context.PhanCongChams.Add(entity);
            _context.SaveChanges();
            return true;
        }

        public bool Xoa(int id)
        {
            var item = _context.PhanCongChams.FirstOrDefault(o => o.Id == id);
            if (item == null) return false;
            item.IsDeleted = true;
            item.NgayXoa = DateTime.Now;
            _context.SaveChanges();
            return true;
        }
    }
}

