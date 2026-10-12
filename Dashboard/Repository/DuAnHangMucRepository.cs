using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Dynamic;
using System;
using System.Linq;

namespace Dashboard.Repository
{
    public interface IDuAnHangMucRepository
    {
        dynamic CapNhat(DuAnHangMucInfo model);
        DataTableResult<DuAnHangMucGridVo> GetAll(QueryParamViewModel queryParams);
        DuAnHangMuc GetById(int id);
        List<DuAnHangMucGridVo> GetActiveList();
        dynamic Them(DuAnHangMucInfo model);
        bool Xoa(int id);
    }

    public class DuAnHangMucRepository : IDuAnHangMucRepository
    {
        private readonly IContext _context;
        private readonly IDuAnHangMucValidate _duAnHangMucValidate;
        private readonly string sortStringDefault = "ThuTu asc";
        private readonly List<string> searchCol = new List<string> { "TenHangMuc", "MoTa" };

        public DuAnHangMucRepository(IContext context, IDuAnHangMucValidate duAnHangMucValidate)
        {
            _context = context;
            _duAnHangMucValidate = duAnHangMucValidate;
        }

        public DataTableResult<DuAnHangMucGridVo> GetAll(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.DuAnHangMucs.AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(o => new DuAnHangMucGridVo
            {
                Id = o.Id,
                TenHangMuc = o.TenHangMuc,
                MoTa = o.MoTa,
                ThuTu = o.ThuTu
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<DuAnHangMucGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public List<DuAnHangMucGridVo> GetActiveList()
        {
            return _context.DuAnHangMucs
                .Where(x => x.IsDeleted != true)
                .OrderBy(x => x.ThuTu ?? int.MaxValue)
                .ThenBy(x => x.Id)
                .Select(x => new DuAnHangMucGridVo
                {
                    Id = x.Id,
                    TenHangMuc = x.TenHangMuc,
                    MoTa = x.MoTa,
                    ThuTu = x.ThuTu
                }).ToList();
        }

        public DuAnHangMuc GetById(int id)
        {
            return _context.DuAnHangMucs.AsNoTracking().FirstOrDefault(o => o.Id == id);
        }

        public dynamic Them(DuAnHangMucInfo model)
        {
            var errors = _duAnHangMucValidate.Validate(model);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            try
            {
                var duAnHangMuc = new DuAnHangMuc
                {
                    TenHangMuc = model.TenHangMuc,
                    MoTa = model.MoTa,
                    ThuTu = model.ThuTu,
                    IsDeleted = false
                };

                _context.DuAnHangMucs.Add(duAnHangMuc);
                _context.SaveChanges();
                return new { Id = duAnHangMuc.Id };
            }
            catch (Exception)
            {
                return new { Id = 0 };
            }
        }

        public dynamic CapNhat(DuAnHangMucInfo model)
        {
            var errors = _duAnHangMucValidate.Validate(model);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            try
            {
                var entity = _context.DuAnHangMucs.FirstOrDefault(x => x.Id == model.Id && x.IsDeleted != true);
                if (entity == null) return new { Id = 0 };

                entity.TenHangMuc = model.TenHangMuc;
                entity.MoTa = model.MoTa;
                entity.ThuTu = model.ThuTu;
                _context.SaveChanges();
                return new { Id = entity.Id };
            }
            catch (Exception)
            {
                return new { Id = 0 };
            }
        }

        public bool Xoa(int id)
        {
            var entity = _context.DuAnHangMucs.FirstOrDefault(x => x.Id == id && x.IsDeleted != true);
            if (entity == null) return false;

            entity.IsDeleted = true;
            _context.SaveChanges();
            return true;
        }
    }
}