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
    public interface IChucDanhRepository
    {
        dynamic CapNhat(ChucDanhViewModel itemViewModel);
        bool CapNhatThuTu(CapNhatThuTuChucDanhRequest request);
        DataLookupViewModel ChucDanhLookup(int id = 0);
        DataLookupViewModel ChucDanhLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<ChucDanhGridVo> GetAll(QueryParamViewModel queryParams);
        ChucDanh GetById(int id);
        bool KhoiPhuc(int id);
        dynamic Them(ChucDanhViewModel itemViewModel);
        bool Xoa(int id);
    }

    public class ChucDanhRepository : IChucDanhRepository
    {
        private readonly IContext _context;
        private readonly IChucDanhValidate _chucDanhValidate;
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ChucDanhViewModel, ChucDanh>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }, NullLoggerFactory.Instance).CreateMapper();

        public ChucDanhRepository(IContext context, IChucDanhValidate chucDanhValidate)
        {
            _context = context;
            _chucDanhValidate = chucDanhValidate;
        }

        private readonly string sortStringDefault = "ThuTu desc, Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten" };

        public DataTableResult<ChucDanhGridVo> GetAll(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.ChucDanhs.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(cv => new ChucDanhGridVo
            {
                Id = cv.Id,
                Ten = cv.Ten,
                ThuTu = cv.ThuTu,
                IsDeleted = cv.IsDeleted
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<ChucDanhGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public ChucDanh GetById(int id)
        {
            return _context.ChucDanhs.AsNoTracking().FirstOrDefault(o => o.Id == id);
        }

        public dynamic Them(ChucDanhViewModel itemViewModel)
        {
            var errors = _chucDanhValidate.Validate(itemViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var chucDanh = _iMapper.Map<ChucDanhViewModel, ChucDanh>(itemViewModel);

            _context.ChucDanhs.Add(chucDanh);
            _context.SaveChanges();
            CacheManagerHelper.InitChucDanhCache();

            return new { Id = chucDanh.Id };
        }

        public dynamic CapNhat(ChucDanhViewModel itemViewModel)
        {
            var errors = _chucDanhValidate.Validate(itemViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var item = _context.ChucDanhs.FirstOrDefault(o => o.Id == itemViewModel.Id);
            if (item == null)
            {
                return new { Id = 0 };
            }

            _iMapper.Map(itemViewModel, item);

            _context.SaveChanges();
            CacheManagerHelper.InitChucDanhCache();
            return new { Id = item.Id };
        }

        public bool CapNhatThuTu(CapNhatThuTuChucDanhRequest request)
        {
            if (request == null || request.Id <= 0)
            {
                return false;
            }

            var item = _context.ChucDanhs.FirstOrDefault(o => o.Id == request.Id);
            if (item == null)
            {
                return false;
            }

            item.ThuTu = request.ThuTu;
            _context.SaveChanges();
            CacheManagerHelper.InitChucDanhCache();
            return true;
        }

        public bool Xoa(int id)
        {
            var item = _context.ChucDanhs.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                item.IsDeleted = true;
                _context.SaveChanges();
                CacheManagerHelper.InitChucDanhCache();
                return true;
            }
            return false;
        }

        public bool KhoiPhuc(int id)
        {
            var item = _context.ChucDanhs.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                CacheManagerHelper.InitChucDanhCache();
                return true;
            }
            return false;
        }

        public DataLookupViewModel ChucDanhLookup(int id = 0)
        {
            var data = new List<LookupViewModel>
            {
                new LookupViewModel
                {
                    id = 0,
                    text = "--- Chọn ---"
                }
            };

            var chucDanhs = _context.ChucDanhs.AsNoTracking().Where(x => x.IsDeleted != true && x.Id != id).ToList();

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

        public DataLookupViewModel ChucDanhLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var vaiTros = CacheManagerHelper.GetChucDanhCache();

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
