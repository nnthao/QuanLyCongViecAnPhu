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
    public interface IDonViDoanRepository
    {
        DataLookupViewModel DonViDoanLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<DonViDoanGridVo> GetAll(QueryParamViewModel queryParams);
        DonViDoanGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(DonViDoanViewModel model);
        bool Xoa(int Id);
    }

    public class DonViDoanRepository : IDonViDoanRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DonViDoanViewModel, DonViDoan>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IDonViDoanValidate _validate;
        public DonViDoanRepository(IContext context, IDonViDoanValidate validate)
        {
            _context = context;
            _validate = validate;
        }

        private readonly string sortStringDefault = "Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten", "TenTrim" };

        public DataTableResult<DonViDoanGridVo> GetAll(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.DonViDoans.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(cv => new DonViDoanGridVo
            {
                Id = cv.Id,
                Ten = cv.Ten,
                TenTrim = cv.TenTrim,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<DonViDoanGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(DonViDoanViewModel model)
        {
            var errors = _validate.Validate(model);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (model.Id == 0)
            {
                var item = _iMapper.Map<DonViDoanViewModel, DonViDoan>(model);
                _context.DonViDoans.Add(item);
            }
            else
            {
                var item = _context.DonViDoans.FirstOrDefault(o => o.Id == model.Id);
                if (item == null) throw new System.Exception("Dữ liệu đã bị xóa");

                _iMapper.Map(model, item);
            }

            _context.SaveChanges();

            return true;
        }

        public bool Xoa(int Id)
        {
            var item = _context.DonViDoans.FirstOrDefault(o => o.Id == Id);
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
            var item = _context.DonViDoans.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public DonViDoanGridVo GetById(int id)
        {
            var item = _context.DonViDoans.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new DonViDoanGridVo
                {
                    Id = item.Id,
                    Ten = item.Ten,
                    TenTrim = item.TenTrim
                };
            }

            return new DonViDoanGridVo();
        }

        public DataLookupViewModel DonViDoanLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var query = _context.DonViDoans.AsNoTracking().AsQueryable();
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

            var result = query.Select(o => new LookupViewModel
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

