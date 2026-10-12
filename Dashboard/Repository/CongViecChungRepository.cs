using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Repository
{
    public interface ICongViecChungRepository
    {
        DataTableResult<CongViecChungGridVo> GetAll(string keyword, int page, int pageSize);
        CongViecChungGridVo GetById(int id);
        bool Insert(CongViecChungViewModel model);
        bool Update(CongViecChungViewModel model);
        bool Delete(int id);
        List<CongViecChungThongKeGridVo> ThongKeCongViec(CongViecChungThongKeQueryViewModel query);
    }

    public class CongViecChungRepository : ICongViecChungRepository
    {
        private readonly IContext _context;

        public CongViecChungRepository(IContext context)
        {
            _context = context;
        }

        public DataTableResult<CongViecChungGridVo> GetAll(string keyword, int page, int pageSize)
        {
            var query = _context.CongViecChungs
                .Include(x => x.CongViecChungThanhViens)
                .Include(x => x.CongViecChungThanhViens.Select(t => t.User))
                .Where(x => x.IsDeleted != true);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Ten.Contains(keyword));
            }

            var total = query.Count();

            var queryResult = query.OrderByDescending(x => x.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var data = queryResult.Select(x => new CongViecChungGridVo
            {
                Id = x.Id,
                Ten = x.Ten,
                NgayTao = x.NgayTao,
                SoThanhVien = x.CongViecChungThanhViens.Count(t => t.IsDeleted != true),
                ThanhVienNames = string.Join(", ", x.CongViecChungThanhViens
                        .Where(t => t.IsDeleted != true && t.User != null)
                        .Select(t => t.User.Name))
            }).ToList();

            return new DataTableResult<CongViecChungGridVo>
            {
                rows = data,
                total = total
            };
        }

        public CongViecChungGridVo GetById(int id)
        {
            var entity = _context.CongViecChungs
                .Include(x => x.CongViecChungThanhViens)
                .FirstOrDefault(x => x.Id == id && x.IsDeleted != true);

            if (entity == null) return null;

            return new CongViecChungGridVo
            {
                Id = entity.Id,
                Ten = entity.Ten,
                NgayTao = entity.NgayTao,
                SelectedUserIds = entity.CongViecChungThanhViens
                    .Where(t => t.IsDeleted != true && t.UserId.HasValue)
                    .Select(t => t.UserId.Value).ToList()
            };
        }

        public bool Insert(CongViecChungViewModel model)
        {
            try
            {
                var entity = new CongViecChung
                {
                    Ten = model.Ten,
                    NgayTao = model.NgayTao ?? DateTime.Now,
                    IsDeleted = false
                };

                _context.CongViecChungs.Add(entity);
                _context.SaveChanges();

                if (model.SelectedUserIds != null && model.SelectedUserIds.Any())
                {
                    foreach (var userId in model.SelectedUserIds)
                    {
                        var member = new CongViecChungThanhVien
                        {
                            CongViecChungId = entity.Id,
                            UserId = userId,
                            IsDeleted = false
                        };
                        _context.CongViecChungThanhViens.Add(member);
                    }
                    _context.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update(CongViecChungViewModel model)
        {
            try
            {
                var entity = _context.CongViecChungs
                    .Include(x => x.CongViecChungThanhViens)
                    .FirstOrDefault(x => x.Id == model.Id && x.IsDeleted != true);

                if (entity == null) return false;

                entity.Ten = model.Ten;
                if (model.NgayTao.HasValue)
                {
                    entity.NgayTao = model.NgayTao.Value;
                }

                var currentMembers = entity.CongViecChungThanhViens.Where(t => t.IsDeleted != true).ToList();
                var currentMemberIds = currentMembers.Where(t => t.UserId.HasValue).Select(t => t.UserId.Value).ToList();

                var newMemberIds = model.SelectedUserIds ?? new List<int>();

                // Delete
                var toDeleteUsers = currentMembers.Where(m => m.UserId.HasValue && !newMemberIds.Contains(m.UserId.Value)).ToList();
                foreach (var member in toDeleteUsers)
                {
                    member.IsDeleted = true;
                }

                // Add
                foreach (var userId in newMemberIds)
                {
                    if (!currentMemberIds.Contains(userId))
                    {
                        // Check if exists but deleted (though we filtered IsDeleted!=true above, checking DB for completely new row vs reactured row)
                        // But entity.CongViecChungThanhViens has all loaded? Should include deleted?
                        // The Include loads all associated? No, filtered?
                        // Usually Include loads all unless filtered.
                        // Let's assume generic Include loads EVERYTHING related.
                        // Wait, in EF6 Include loads all related entities unless explicit query. 
                        // But here we are iterating `entity.CongViecChungThanhViens`.

                        // Ideally we should check if there is a soft-deleted one to re-activate.
                        var existing = entity.CongViecChungThanhViens.FirstOrDefault(t => t.UserId == userId);
                        if (existing != null)
                        {
                            existing.IsDeleted = false;
                        }
                        else
                        {
                            entity.CongViecChungThanhViens.Add(new CongViecChungThanhVien
                            {
                                CongViecChungId = entity.Id,
                                UserId = userId,
                                IsDeleted = false
                            });
                        }
                    }
                }

                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            var entity = _context.CongViecChungs.FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<CongViecChungThongKeGridVo> ThongKeCongViec(CongViecChungThongKeQueryViewModel queryModel)
        {
            var query = _context.Users.AsQueryable();

            if (queryModel.PhongBanId > 0)
            {
                query = query.Where(u => u.PhongBanId == queryModel.PhongBanId);
            }

            var tuNgay = queryModel.TuNgay;
            var denNgay = queryModel.DenNgay;

            // Filtering CongViecChung by date range
            var congViecQuery = _context.CongViecChungs.Where(cv => cv.IsDeleted != true);

            if (tuNgay.HasValue)
            {
                var fromDate = tuNgay.Value.Date;
                congViecQuery = congViecQuery.Where(cv => cv.NgayTao >= fromDate);
            }

            if (denNgay.HasValue)
            {
                var toDate = denNgay.Value.Date.AddDays(1).AddTicks(-1); // End of day
                congViecQuery = congViecQuery.Where(cv => cv.NgayTao <= toDate);
            }

            var result = query
                .Where(u => u.IsDeleted != true)
                .GroupJoin(
                    _context.CongViecChungThanhViens.Where(tv => tv.IsDeleted != true),
                    u => u.Id,
                    tv => tv.UserId,
                    (u, tvs) => new { User = u, Tasks = tvs }
                )
                .SelectMany(
                    x => x.Tasks.DefaultIfEmpty(),
                    (x, tv) => new { x.User, Task = tv }
                )
                .GroupJoin(
                    congViecQuery, // Use filtered query
                    x => x.Task.CongViecChungId,
                    cv => cv.Id,
                    (x, cvs) => new { x.User, x.Task, Works = cvs }
                )
                .SelectMany(
                    x => x.Works.DefaultIfEmpty(), // If Works is empty (because filtered out by date), Work becomes null
                    (x, cv) => new { x.User, Work = cv }
                )
                .GroupBy(x => new { x.User.Id, x.User.Name })
                .Select(g => new CongViecChungThongKeGridVo
                {
                    TenUser = g.Key.Name,
                    SoLuongCongViec = g.Count(x => x.Work != null),
                    SoLuongThanhVien = 0
                })
                .OrderBy(x => x.SoLuongCongViec)
                .ToList();

            return result;
        }
    }
}
