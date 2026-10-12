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
    public interface IDanhMucBaoCaoRepository
    {
        List<LookupViewModel> DanhMucBaoCaoLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<DanhMucBaoCaoGridVo> GetAll(QueryParamViewModel queryParams);
        DanhMucBaoCaoGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(DanhMucBaoCaoViewModel danhMucBaoCaoViewModel);
        bool Xoa(int Id);
    }

    public class DanhMucBaoCaoRepository : IDanhMucBaoCaoRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DanhMucBaoCaoViewModel, DanhMucBaoCao>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {

                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IDanhMucBaoCaoValidate _validate;
        public DanhMucBaoCaoRepository(IContext context, IDanhMucBaoCaoValidate validate)
        {
            _context = context;
            _validate = validate;
        }
        private readonly string sortStringDefault = "Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten" };

        public DataTableResult<DanhMucBaoCaoGridVo> GetAll(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.DanhMucBaoCaos.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(cv => new DanhMucBaoCaoGridVo
            {
                Id = cv.Id,
                Ten = cv.Ten,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<DanhMucBaoCaoGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(DanhMucBaoCaoViewModel danhMucBaoCaoViewModel)
        {
            var errors = _validate.Validate(danhMucBaoCaoViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (danhMucBaoCaoViewModel.Id == 0)
            {
                var item = _iMapper.Map<DanhMucBaoCaoViewModel, DanhMucBaoCao>(danhMucBaoCaoViewModel);
                _context.DanhMucBaoCaos.Add(item);
            }
            else
            {
                var item = _context.DanhMucBaoCaos.FirstOrDefault(o => o.Id == danhMucBaoCaoViewModel.Id);
                if (item == null) throw new Exception("Dữ liệu đã bị xóa");

                _iMapper.Map(danhMucBaoCaoViewModel, item);
            }

            _context.SaveChanges();

            return true;
        }

        public bool Xoa(int Id)
        {
            var item = _context.DanhMucBaoCaos.FirstOrDefault(o => o.Id == Id);
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
            var item = _context.DanhMucBaoCaos.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public DanhMucBaoCaoGridVo GetById(int id)
        {
            var item = _context.DanhMucBaoCaos.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new DanhMucBaoCaoGridVo
                {
                    Id = item.Id,
                    Ten = item.Ten
                };
            }

            return new DanhMucBaoCaoGridVo();
        }

        public List<LookupViewModel> DanhMucBaoCaoLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn ---"
                    }
                };

            var query = _context.DanhMucBaoCaos.AsNoTracking().AsQueryable();
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
                return result.ToList();
            }

            return data.Concat(result).ToList();
        }
    }
}


