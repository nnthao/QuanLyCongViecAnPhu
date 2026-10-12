using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface INhomNhanViecRepository
    {
        DataTableResult<NhomNhanViecGridVo> GetAll(NhomNhanViecQueryViewModel queryParams);
        NhomNhanViecGridVo GetById(int id);
        List<NhomNhanViecGridVo> GetListNhomNhanViecDeChon(int currentUserId);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(NhomNhanViecViewModel NhomNhanViecViewModel);
        bool Xoa(int Id);
    }

    public class NhomNhanViecRepository : INhomNhanViecRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<NhomNhanViecViewModel, NhomNhanViec>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {

                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly INhomNhanViecValidate _nhomNhanViecValidate;
        public NhomNhanViecRepository(IContext context, INhomNhanViecValidate nhomNhanViecValidate)
        {
            _context = context;
            _nhomNhanViecValidate = nhomNhanViecValidate;
        }

        private readonly string sortStringDefault = "Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten", "MoTa" };

        public DataTableResult<NhomNhanViecGridVo> GetAll(NhomNhanViecQueryViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.NhomNhanViecs.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            query = query.Where(o => o.NguoiTaoId == queryParams.CurrentUserId);

            var dateTimeCurrent = DateTime.Now;
            var data = query.Select(cv => new NhomNhanViecGridVo
            {
                Id = cv.Id,
                Ten = cv.Ten,
                MoTa = cv.MoTa,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            var result = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();

            return new DataTableResult<NhomNhanViecGridVo>
            {
                rows = result,
                total = total
            };
        }

        public NhomNhanViecGridVo GetById(int id)
        {
            var item = _context.NhomNhanViecs.Include(o => o.NhomNhanViecThanhViens).AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                var data = new NhomNhanViecGridVo
                {
                    Id = item.Id,
                    Ten = item.Ten,
                    MoTa = item.MoTa,
                    IsDeleted = item.IsDeleted,
                    ThanhViens = item.NhomNhanViecThanhViens.Where(o => o.IsDeleted != true).Select(o => o.UserId).ToList()
                };

                return data;
            }

            return new NhomNhanViecGridVo();
        }

        public bool KhoiPhuc(int Id)
        {
            var item = _context.NhomNhanViecs.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public dynamic ThemOrCapNhat(NhomNhanViecViewModel NhomNhanViecViewModel)
        {
            var errors = _nhomNhanViecValidate.Validate(NhomNhanViecViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (NhomNhanViecViewModel.Id == 0)
            {
                var item = _iMapper.Map<NhomNhanViecViewModel, NhomNhanViec>(NhomNhanViecViewModel);
                AddThanhVienVaoEntity(item, NhomNhanViecViewModel);

                _context.NhomNhanViecs.Add(item);
                _context.SaveChanges();
            }
            else
            {
                var item = _context.NhomNhanViecs.Include(o => o.NhomNhanViecThanhViens).FirstOrDefault(o => o.Id == NhomNhanViecViewModel.Id);
                if (item == null) throw new Exception("Nhóm nhân việc đã bị xóa");

                _iMapper.Map(NhomNhanViecViewModel, item);
                AddThanhVienVaoEntity(item, NhomNhanViecViewModel);

                _context.SaveChanges();
            }

            return true;
        }

        private void AddThanhVienVaoEntity(NhomNhanViec item, NhomNhanViecViewModel itemViewModel)
        {
            var oldIds = item.NhomNhanViecThanhViens.Where(x => x.IsDeleted != true)
                .Select(x => (int)x.UserId).ToList();

            var removeIds = oldIds.Except(itemViewModel.UserIntIds).ToList();
            var addIds = itemViewModel.UserIntIds.Except(oldIds).ToList();

            foreach (var removeId in removeIds)
            {
                var removeItem = item.NhomNhanViecThanhViens.FirstOrDefault(x => x.UserId == removeId && x.IsDeleted != true);
                if (removeItem != null)
                {
                    removeItem.IsDeleted = true;
                }
            }

            foreach (var addId in addIds)
            {
                var addItem = new NhomNhanViecThanhVien
                {
                    UserId = addId,
                };
                item.NhomNhanViecThanhViens.Add(addItem);
            }
        }

        public bool Xoa(int Id)
        {
            var item = _context.NhomNhanViecs.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = true;

                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public List<NhomNhanViecGridVo> GetListNhomNhanViecDeChon(int currentUserId)
        {
            var query = _context.NhomNhanViecs.Include(o => o.NhomNhanViecThanhViens).AsNoTracking()
            .Where(o => o.NguoiTaoId == currentUserId && o.IsDeleted != true);

            var data = query.Select(o => new NhomNhanViecGridVo
            {
                Id = o.Id,
                Ten = o.Ten,
                MoTa = o.MoTa,
                IsDeleted = o.IsDeleted,
                ThanhVienNhomNhanViecs = o.NhomNhanViecThanhViens.Where(s => o.IsDeleted != true).Select(p => new ThanhVienNhomNhanViecGridVo
                {
                    UserId = p.UserId,
                }).ToList()
            }).ToList();

            foreach (var item in data)
            {
                foreach (var usr in item.ThanhVienNhomNhanViecs)
                {
                    var userExist = CacheManagerHelper.GetUserByIdFromCache((int)usr.UserId);
                    if (userExist != null)
                    {
                        usr.Name = userExist.Name;
                    }
                }
            }
            return data;
        }
    }
}
