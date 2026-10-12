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
    public interface IDanhMucTraCuuThongTinRepository
    {
        List<LookupViewModel> DanhMucTraCuuThongTinLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<DanhMucTraCuuThongTinGridVo> GetAll(QueryParamViewModel queryParams);
        DanhMucTraCuuThongTinGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(DanhMucTraCuuThongTinViewModel DanhMucTraCuuThongTinViewModel);
        bool Xoa(int Id);
    }

    public class DanhMucTraCuuThongTinRepository : IDanhMucTraCuuThongTinRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DanhMucTraCuuThongTinViewModel, DanhMucTraCuuThongTin>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {

                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IDanhMucTraCuuThongTinValidate _phongBanValidate;
        public DanhMucTraCuuThongTinRepository(IContext context, IDanhMucTraCuuThongTinValidate phongBanValidate)
        {
            _context = context;
            _phongBanValidate = phongBanValidate;
        }
        //required
        private readonly string sortStringDefault = "Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten" };

        public DataTableResult<DanhMucTraCuuThongTinGridVo> GetAll(QueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.DanhMucTraCuuThongTins.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var dateTimeCurrent = DateTime.Now;
            var data = query.Select(cv => new DanhMucTraCuuThongTinGridVo
            {
                Id = cv.Id,
                Ten = cv.Ten,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<DanhMucTraCuuThongTinGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(DanhMucTraCuuThongTinViewModel DanhMucTraCuuThongTinViewModel)
        {
            var errors = _phongBanValidate.Validate(DanhMucTraCuuThongTinViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (DanhMucTraCuuThongTinViewModel.Id == 0)
            {
                var item = _iMapper.Map<DanhMucTraCuuThongTinViewModel, DanhMucTraCuuThongTin>(DanhMucTraCuuThongTinViewModel);
                _context.DanhMucTraCuuThongTins.Add(item);
            }
            else
            {
                var item = _context.DanhMucTraCuuThongTins.FirstOrDefault(o => o.Id == DanhMucTraCuuThongTinViewModel.Id);
                if (item == null) throw new Exception("Phòng họp đã bị xóa");

                _iMapper.Map(DanhMucTraCuuThongTinViewModel, item);
            }

            _context.SaveChanges();

            return true;
        }


        public bool Xoa(int Id)
        {
            var item = _context.DanhMucTraCuuThongTins.FirstOrDefault(o => o.Id == Id);
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
            var item = _context.DanhMucTraCuuThongTins.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public DanhMucTraCuuThongTinGridVo GetById(int id)
        {
            var item = _context.DanhMucTraCuuThongTins.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new DanhMucTraCuuThongTinGridVo
                {
                    Id = item.Id,
                    Ten = item.Ten
                };
            }

            return new DanhMucTraCuuThongTinGridVo();
        }



        public List<LookupViewModel> DanhMucTraCuuThongTinLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var vaiTros = CacheManagerHelper.GetDanhMucTraCuuThongTinCache();

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
                return result.ToList();
            }

            return data.Concat(result).ToList();
        }
    }
}
