using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface ILyDoRepository
    {
        dynamic CapNhat(LyDoViewModel itemViewModel);
        DataLookupViewModel LyDoLookup(int id = 0);
        DataLookupViewModel LyDoLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<LyDoGridVo> GetAll(QueryParamViewModel queryParams);
        LyDo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic Them(LyDoViewModel itemViewModel);
        bool Xoa(int id);
    }

    public class LyDoRepository : ILyDoRepository
    {
        private readonly IContext _context;
        private readonly ILyDoValidate _chucDanhValidate;
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<LyDoViewModel, LyDo>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }, NullLoggerFactory.Instance).CreateMapper();

        public LyDoRepository(IContext context, ILyDoValidate chucDanhValidate)
        {
            _context = context;
            _chucDanhValidate = chucDanhValidate;
        }

        private readonly string sortStringDefault = "Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten" };

        public DataTableResult<LyDoGridVo> GetAll(QueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.LyDos.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(cv => new LyDoGridVo
            {
                Id = cv.Id,
                Ten = cv.Ten,
                MaMau = cv.MaMau,
                IsDeleted = cv.IsDeleted
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<LyDoGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public LyDo GetById(int id)
        {
            return _context.LyDos.AsNoTracking().FirstOrDefault(o => o.Id == id);
        }

        public dynamic Them(LyDoViewModel itemViewModel)
        {
            var errors = _chucDanhValidate.Validate(itemViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var chucDanh = _iMapper.Map<LyDoViewModel, LyDo>(itemViewModel);

            _context.LyDos.Add(chucDanh);
            _context.SaveChanges();

            return new { Id = chucDanh.Id };
        }

        public dynamic CapNhat(LyDoViewModel itemViewModel)
        {
            var errors = _chucDanhValidate.Validate(itemViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var item = _context.LyDos.FirstOrDefault(o => o.Id == itemViewModel.Id);
            if (item == null)
            {
                return new { Id = 0 };
            }

            _iMapper.Map(itemViewModel, item);

            _context.SaveChanges();
            return new { Id = item.Id };
        }

        public bool Xoa(int id)
        {
            var item = _context.LyDos.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                item.IsDeleted = true;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool KhoiPhuc(int id)
        {
            var item = _context.LyDos.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public DataLookupViewModel LyDoLookup(int id = 0)
        {
            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn ---"
                    }
                };

            var chucDanhs = _context.LyDos.AsNoTracking().Where(x => x.IsDeleted != true && x.Id != id).ToList();

            var results = chucDanhs.Select(x => new LookupViewModel
            {
                id = x.Id,
                text = x.Ten
            }).OrderBy(o => o.text);

            return new DataLookupViewModel
            {
                Data = data.Concat(results).ToList()
            };
        }

        public DataLookupViewModel LyDoLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var vaiTros = CacheManagerHelper.GetLyDoCache();

            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn ---"
                    }
                };

            var query = vaiTros.AsQueryable();
            if (!lookupQueryViewModel.isGetAll)
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var result = query.Select(o => new LookupViewModel
            {
                id = o.Id,
                text = o.Ten
            }).ToList();

            if (lookupQueryViewModel.isHideOptionChonValueZero)
            {
                return new DataLookupViewModel
                {
                    Data = result.ToList()
                };
            }

            return new DataLookupViewModel
            {
                Data = data.Concat(result).ToList()
            };
        }
    }
}
