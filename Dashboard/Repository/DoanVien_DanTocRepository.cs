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
    public interface IDoanVien_DanTocRepository
    {
        DataLookupViewModel DoanVien_DanTocLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<DoanVien_DanTocGridVo> GetAll(QueryParamViewModel queryParams);
        DoanVien_DanTocGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(DoanVien_DanTocViewModel model);
        bool Xoa(int Id);
    }

    public class DoanVien_DanTocRepository : IDoanVien_DanTocRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DoanVien_DanTocViewModel, DoanVien_DanToc>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IDoanVien_DanTocValidate _validate;
        public DoanVien_DanTocRepository(IContext context, IDoanVien_DanTocValidate validate)
        {
            _context = context;
            _validate = validate;
        }

        private readonly string sortStringDefault = "Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten" };

        public DataTableResult<DoanVien_DanTocGridVo> GetAll(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.DoanVien_DanTocs.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(cv => new DoanVien_DanTocGridVo
            {
                Id = cv.Id,
                Ten = cv.Ten,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<DoanVien_DanTocGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(DoanVien_DanTocViewModel model)
        {
            var errors = _validate.Validate(model);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (model.Id == 0)
            {
                var item = _iMapper.Map<DoanVien_DanTocViewModel, DoanVien_DanToc>(model);
                _context.DoanVien_DanTocs.Add(item);
            }
            else
            {
                var item = _context.DoanVien_DanTocs.FirstOrDefault(o => o.Id == model.Id);
                if (item == null) throw new System.Exception("Dữ liệu đã bị xóa");

                _iMapper.Map(model, item);
            }

            _context.SaveChanges();

            return true;
        }

        public bool Xoa(int Id)
        {
            var item = _context.DoanVien_DanTocs.FirstOrDefault(o => o.Id == Id);
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
            var item = _context.DoanVien_DanTocs.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public DoanVien_DanTocGridVo GetById(int id)
        {
            var item = _context.DoanVien_DanTocs.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new DoanVien_DanTocGridVo
                {
                    Id = item.Id,
                    Ten = item.Ten
                };
            }

            return new DoanVien_DanTocGridVo();
        }

        public DataLookupViewModel DoanVien_DanTocLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var query = _context.DoanVien_DanTocs.AsNoTracking().AsQueryable();
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

