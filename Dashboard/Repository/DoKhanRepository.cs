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
    public interface IDoKhanRepository
    {
        DataLookupViewModel DoKhanLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<DoKhanGridVo> GetAll(QueryParamViewModel queryParams);
        DoKhanGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(DoKhanViewModel model);
        bool Xoa(int Id);
    }

    public class DoKhanRepository : IDoKhanRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DoKhanViewModel, DoKhan>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {

                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IDoKhanValidate _validate;
        public DoKhanRepository(IContext context, IDoKhanValidate validate)
        {
            _context = context;
            _validate = validate;
        }
        //required
        private readonly string sortStringDefault = "Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten" };

        public DataTableResult<DoKhanGridVo> GetAll(QueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.DoKhans.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(cv => new DoKhanGridVo
            {
                Id = cv.Id,
                Ten = cv.Ten,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<DoKhanGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(DoKhanViewModel model)
        {
            var errors = _validate.Validate(model);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (model.Id == 0)
            {
                var item = _iMapper.Map<DoKhanViewModel, DoKhan>(model);
                _context.DoKhans.Add(item);
            }
            else
            {
                var item = _context.DoKhans.FirstOrDefault(o => o.Id == model.Id);
                if (item == null) throw new Exception("Dữ liệu đã bị xóa");

                _iMapper.Map(model, item);
            }

            _context.SaveChanges();

            return true;
        }

        public bool Xoa(int Id)
        {
            var item = _context.DoKhans.FirstOrDefault(o => o.Id == Id);
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
            var item = _context.DoKhans.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public DoKhanGridVo GetById(int id)
        {
            var item = _context.DoKhans.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new DoKhanGridVo
                {
                    Id = item.Id,
                    Ten = item.Ten
                };
            }

            return new DoKhanGridVo();
        }

        public DataLookupViewModel DoKhanLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var caches = CacheManagerHelper.GetDoKhanCache();

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


