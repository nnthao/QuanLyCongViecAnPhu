using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface IDonViRepository
    {
        DataLookupViewModel Lookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<DonViGridVo> GetAll(QueryParamViewModel queryParams);
        DonViGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(DonViViewModel model);
        bool Xoa(int id);
    }

    public class DonViRepository : IDonViRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DonViViewModel, DonVi>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IDonViValidate _validate;
        public DonViRepository(IContext context, IDonViValidate validate)
        {
            _context = context;
            _validate = validate;
        }

        private readonly string sortStringDefault = "Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten", "TenTrim" };

        public DataTableResult<DonViGridVo> GetAll(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.DonVis.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(o => new DonViGridVo
            {
                Id = o.Id,
                Ten = o.Ten,
                TenTrim = o.TenTrim,
                IsDeleted = o.IsDeleted
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<DonViGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(DonViViewModel model)
        {
            var errors = _validate.Validate(model);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            model.TenTrim = UltiHelper.CreateTenTrim(model.Ten);

            if (model.Id == 0)
            {
                var item = _iMapper.Map<DonViViewModel, DonVi>(model);
                _context.DonVis.Add(item);
            }
            else
            {
                var item = _context.DonVis.FirstOrDefault(o => o.Id == model.Id);
                if (item == null) throw new System.Exception("Dữ liệu đã bị xóa");

                _iMapper.Map(model, item);
            }

            _context.SaveChanges();

            return true;
        }

        public bool Xoa(int id)
        {
            var item = _context.DonVis.FirstOrDefault(o => o.Id == id);
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
            var item = _context.DonVis.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public DonViGridVo GetById(int id)
        {
            var item = _context.DonVis.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new DonViGridVo
                {
                    Id = item.Id,
                    Ten = item.Ten,
                    TenTrim = item.TenTrim
                };
            }

            return new DonViGridVo();
        }

        public DataLookupViewModel Lookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var query = _context.DonVis.AsNoTracking().AsQueryable();
            if (!lookupQueryViewModel.isGetAll)
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = new List<LookupViewModel>();
            if (!lookupQueryViewModel.isHideOptionChonValueZero)
            {
                data.Add(new LookupViewModel
                {
                    id = 0,
                    text = "--- Chọn ---"
                });
            }

            var result = query.OrderBy(o => o.Ten).Select(o => new LookupViewModel
            {
                id = o.Id,
                text = o.Ten
            }).ToList();

            return new DataLookupViewModel
            {
                Data = data.Concat(result).ToList()
            };
        }
    }
}
