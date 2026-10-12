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
    public interface IHinhThucVanBanRepository
    {
        DataLookupViewModel HinhThucVanBanLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<HinhThucVanBanGridVo> GetAll(QueryParamViewModel queryParams);
        HinhThucVanBanGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(HinhThucVanBanViewModel model);
        bool Xoa(int Id);
    }

    public class HinhThucVanBanRepository : IHinhThucVanBanRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<HinhThucVanBanViewModel, HinhThucVanBan>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {

                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IHinhThucVanBanValidate _validate;
        public HinhThucVanBanRepository(IContext context, IHinhThucVanBanValidate validate)
        {
            _context = context;
            _validate = validate;
        }
        //required
        private readonly string sortStringDefault = "Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten" };

        public DataTableResult<HinhThucVanBanGridVo> GetAll(QueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.HinhThucVanBans.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(cv => new HinhThucVanBanGridVo
            {
                Id = cv.Id,
                Ten = cv.Ten,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<HinhThucVanBanGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(HinhThucVanBanViewModel model)
        {
            var errors = _validate.Validate(model);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (model.Id == 0)
            {
                var item = _iMapper.Map<HinhThucVanBanViewModel, HinhThucVanBan>(model);
                _context.HinhThucVanBans.Add(item);
            }
            else
            {
                var item = _context.HinhThucVanBans.FirstOrDefault(o => o.Id == model.Id);
                if (item == null) throw new Exception("Dữ liệu đã bị xóa");

                _iMapper.Map(model, item);
            }

            _context.SaveChanges();

            return true;
        }

        public bool Xoa(int Id)
        {
            var item = _context.HinhThucVanBans.FirstOrDefault(o => o.Id == Id);
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
            var item = _context.HinhThucVanBans.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public HinhThucVanBanGridVo GetById(int id)
        {
            var item = _context.HinhThucVanBans.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new HinhThucVanBanGridVo
                {
                    Id = item.Id,
                    Ten = item.Ten
                };
            }

            return new HinhThucVanBanGridVo();
        }

        public DataLookupViewModel HinhThucVanBanLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var caches = CacheManagerHelper.GetHinhThucVanBanCache();

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


