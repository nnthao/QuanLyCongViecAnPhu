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
    public interface IDotXetDuyetRepository
    {
        List<LookupViewModel> DotXetDuyetLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<DotXetDuyetGridVo> GetAll(QueryParamViewModel queryParams);
        DotXetDuyetGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(DotXetDuyetViewModel DotXetDuyetViewModel);
        bool Xoa(int Id);
    }

    public class DotXetDuyetRepository : IDotXetDuyetRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DotXetDuyetViewModel, DotXetDuyet>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {

                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IDotXetDuyetValidate _dotXetDuyetValidate;
        public DotXetDuyetRepository(IContext context, IDotXetDuyetValidate dotXetDuyetValidate)
        {
            _context = context;
            _dotXetDuyetValidate = dotXetDuyetValidate;
        }

        private readonly string sortStringDefault = "TenDotXetDuyet asc";
        private readonly List<string> searchCol = new List<string> { "TenDotXetDuyet", "NamXetDuyet" };

        public DataTableResult<DotXetDuyetGridVo> GetAll(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.DotXetDuyets.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var dateTimeCurrent = DateTime.Now;
            var data = query.Select(cv => new DotXetDuyetGridVo
            {
                Id = cv.Id,
                TenDotXetDuyet = cv.TenDotXetDuyet,
                NamXetDuyet = cv.NamXetDuyet,
                GhiChu = cv.GhiChu,
                TuNgay = cv.TuNgay,
                DenNgay = cv.DenNgay,
                NguoiXoaId = cv.NguoiXoaId,
                NgayTao = cv.NgayTao,
                NgayXoa = cv.NgayXoa,
                IsDeleted = cv.IsDeleted,
                TrangThai = cv.TrangThai,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<DotXetDuyetGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(DotXetDuyetViewModel DotXetDuyetViewModel)
        {
            var errors = _dotXetDuyetValidate.Validate(DotXetDuyetViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            int dotXetDuyetId = DotXetDuyetViewModel.Id;

            if (DotXetDuyetViewModel.Id == 0)
            {
                var item = _iMapper.Map<DotXetDuyetViewModel, DotXetDuyet>(DotXetDuyetViewModel);
                item.NgayTao = DateTime.Now;
                item.IsDeleted = false;
                _context.DotXetDuyets.Add(item);
                _context.SaveChanges();
                dotXetDuyetId = item.Id;
            }
            else
            {
                var item = _context.DotXetDuyets.FirstOrDefault(o => o.Id == DotXetDuyetViewModel.Id);
                if (item == null) throw new Exception("Đợt xét duyệt đã bị xóa");

                _iMapper.Map(DotXetDuyetViewModel, item);
                _context.SaveChanges();
            }

            return new { DotXetDuyetId = dotXetDuyetId };
        }

        public bool Xoa(int Id)
        {
            var item = _context.DotXetDuyets.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = true;
                item.NgayXoa = DateTime.Now;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool KhoiPhuc(int Id)
        {
            var item = _context.DotXetDuyets.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                item.NgayXoa = null;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public DotXetDuyetGridVo GetById(int id)
        {
            var item = _context.DotXetDuyets.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new DotXetDuyetGridVo
                {
                    Id = item.Id,
                    TenDotXetDuyet = item.TenDotXetDuyet,
                    NamXetDuyet = item.NamXetDuyet,
                    GhiChu = item.GhiChu,
                    TuNgay = item.TuNgay,
                    DenNgay = item.DenNgay,
                    NguoiXoaId = item.NguoiXoaId,
                    NgayTao = item.NgayTao,
                    NgayXoa = item.NgayXoa,
                    TrangThai = item.TrangThai,
                };
            }

            return new DotXetDuyetGridVo();
        }

        public List<LookupViewModel> DotXetDuyetLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var dotXetDuyets = _context.DotXetDuyets.AsNoTracking().AsQueryable();

            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn ---"
                    }
                };

            var query = dotXetDuyets.AsQueryable();
            if (!lookupQueryViewModel.isGetAll)
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            if (lookupQueryViewModel.TuNgayFilter.HasValue && lookupQueryViewModel.DenNgayFilter.HasValue)
            {
                var tuNgay = lookupQueryViewModel.TuNgayFilter.Value.Date;
                var denNgay = lookupQueryViewModel.DenNgayFilter.Value.Date.AddDays(1).AddTicks(-1);

                query = query.Where(o => o.TuNgay.HasValue && o.DenNgay.HasValue &&
                    o.TuNgay.Value <= denNgay && o.DenNgay.Value >= tuNgay);
            }
            else
            {
                var dateTimeNow = DateTime.Now;
                query = query.Where(o => o.TuNgay.HasValue && o.DenNgay.HasValue &&
                    o.TuNgay.Value <= dateTimeNow && o.DenNgay.Value >= dateTimeNow);
            }

            var result = query.Select(o => new LookupViewModel
            {
                id = o.Id,
                text = o.TenDotXetDuyet
            }).ToList();

            if (lookupQueryViewModel.isHideOptionChonValueZero)
            {
                return result.ToList();
            }

            return data.Concat(result).ToList();
        }
    }
}

