using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface IQuanLyBaoCaoChuyenNganhRepository
    {
        DataTableResult<QuanLyBaoCaoChuyenNganhGridVo> GetAll(BaoCaoChuyenNganhQueryViewModel queryParams);
        bool Xoa(int Id);
        int TaoBaoCaoLog(QuanLyBaoCaoChuyenNganhViewModel viewModel);
    }

    public class QuanLyBaoCaoChuyenNganhRepository : IQuanLyBaoCaoChuyenNganhRepository
    {
        private readonly IContext _context;
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<QuanLyBaoCaoChuyenNganhViewModel, QuanLyBaoCaoChuyenNganh>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(d => d.NguoiTaoId, opt => opt.MapFrom(s => s.CurrentUserId))
                .AfterMap((s, d) =>
                {
                    d.NgayTao = DateTime.Now;
                    d.IsDeleted = false;
                });
        }, NullLoggerFactory.Instance).CreateMapper();
        public QuanLyBaoCaoChuyenNganhRepository(IContext context)
        {
            _context = context;
        }
        private readonly string sortStringDefault = "TenFile asc";
        private readonly List<string> searchCol = new List<string> { "TenFile", "DuongDanFile" };

        public DataTableResult<QuanLyBaoCaoChuyenNganhGridVo> GetAll(BaoCaoChuyenNganhQueryViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.QuanLyBaoCaoChuyenNganhs.AsNoTracking().AsQueryable();

            if (queryParams.CurrentUserId.HasValue)
            {
                query = query.Where(o => o.NguoiTaoId == queryParams.CurrentUserId.Value);
            }

            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            // Bo loc theo ngay tao
            if (queryParams.TuNgay.HasValue)
            {
                query = query.Where(o => o.NgayTao >= queryParams.TuNgay.Value);
            }

            if (queryParams.DenNgay.HasValue)
            {
                query = query.Where(o => o.NgayTao <= queryParams.DenNgay.Value);
            }

            // Bo loc theo danh muc bao cao
            if (queryParams.DanhMucBaoCaoId.HasValue)
            {
                var dmId = queryParams.DanhMucBaoCaoId.Value;
                query = query.Where(o => (int?)o.DanhMucBaoCaoId == dmId);
            }

            // Bo loc theo loai bao cao chuyen nganh
            if (queryParams.QuanLyBaoCaoChuyenNganhEnumId.HasValue)
            {
                var loaiId = queryParams.QuanLyBaoCaoChuyenNganhEnumId.Value;
                query = query.Where(o => (int?)o.QuanLyBaoCaoChuyenNganhEnumId == loaiId);
            }

            // Bo loc theo phong ban (thong qua NguoiTaoId -> Users.PhongBanId)
            if (!string.IsNullOrEmpty(queryParams.PhongBanIdsStr))
            {
                var phongBanIds = queryParams.PhongBanIdsStr.Split(',')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => Convert.ToInt32(x))
                    .ToList();

                if (phongBanIds.Count != 0)
                {
                    query = query.Where(o => o.NguoiTaoId != null &&
                        _context.Users.Any(u => u.Id == o.NguoiTaoId && phongBanIds.Contains(u.PhongBanId ?? 0)));
                }
            }

            var data = query.Select(cv => new QuanLyBaoCaoChuyenNganhGridVo
            {
                Id = cv.Id,
                DanhMucBaoCaoId = cv.DanhMucBaoCaoId,
                TenFile = cv.TenFile,
                DuongDanFile = cv.DuongDanFile,
                QuanLyBaoCaoChuyenNganhEnumId = cv.QuanLyBaoCaoChuyenNganhEnumId,
                QueryInfo = cv.QueryInfo,
                NguoiTaoId = cv.NguoiTaoId,
                NgayTao = cv.NgayTao,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<QuanLyBaoCaoChuyenNganhGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }


        public bool Xoa(int Id)
        {
            var item = _context.QuanLyBaoCaoChuyenNganhs.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = true;
                _context.SaveChanges();
                return true;
            }

            return false;
        }


        public int TaoBaoCaoLog(QuanLyBaoCaoChuyenNganhViewModel viewModel)
        {
            var item = _iMapper.Map<QuanLyBaoCaoChuyenNganhViewModel, QuanLyBaoCaoChuyenNganh>(viewModel);
            _context.QuanLyBaoCaoChuyenNganhs.Add(item);
            _context.SaveChanges();
            return item.Id;
        }
    }
}
