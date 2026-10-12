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
    public interface ICoQuanBanHanhRepository
    {
        DataLookupViewModel CoQuanBanHanhLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<CoQuanBanHanhGridVo> GetAll(QueryParamViewModel queryParams);
        CoQuanBanHanhGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(CoQuanBanHanhViewModel model);
        bool Xoa(int Id);
    }

    public class CoQuanBanHanhRepository : ICoQuanBanHanhRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CoQuanBanHanhViewModel, CoQuanBanHanh>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {

                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly ICoQuanBanHanhValidate _validate;
        public CoQuanBanHanhRepository(IContext context, ICoQuanBanHanhValidate validate)
        {
            _context = context;
            _validate = validate;
        }
        //required
        private readonly string sortStringDefault = "Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten" };

        public DataTableResult<CoQuanBanHanhGridVo> GetAll(QueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.CoQuanBanHanhs.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(cv => new CoQuanBanHanhGridVo
            {
                Id = cv.Id,
                Ten = cv.Ten,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<CoQuanBanHanhGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(CoQuanBanHanhViewModel model)
        {
            var errors = _validate.Validate(model);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (model.Id == 0)
            {
                var item = _iMapper.Map<CoQuanBanHanhViewModel, CoQuanBanHanh>(model);
                _context.CoQuanBanHanhs.Add(item);
            }
            else
            {
                var item = _context.CoQuanBanHanhs.FirstOrDefault(o => o.Id == model.Id);
                if (item == null) throw new Exception("Dữ liệu đã bị xóa");

                _iMapper.Map(model, item);
            }

            _context.SaveChanges();

            return true;
        }

        public bool Xoa(int Id)
        {
            var item = _context.CoQuanBanHanhs.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = true;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool KhoiPhuc(int Id)
        {
            var item = _context.CoQuanBanHanhs.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public CoQuanBanHanhGridVo GetById(int id)
        {
            var item = _context.CoQuanBanHanhs.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new CoQuanBanHanhGridVo
                {
                    Id = item.Id,
                    Ten = item.Ten
                };
            }

            return new CoQuanBanHanhGridVo();
        }

        public DataLookupViewModel CoQuanBanHanhLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var caches = CacheManagerHelper.GetCoQuanBanHanhCache();

            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn ---"
                    }
                };

            var query = caches.AsQueryable();
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


