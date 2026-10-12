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
    public interface IQuanLyCongTrinhHuHongRepository
    {
        List<LookupViewModel> QuanLyCongTrinhHuHongLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<QuanLyCongTrinhHuHongGridVo> GetAll(QueryParamViewModel queryParams);
        QuanLyCongTrinhHuHongGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(QuanLyCongTrinhHuHongViewModel QuanLyCongTrinhHuHongViewModel);
        bool Xoa(int Id);
    }

    public class QuanLyCongTrinhHuHongRepository : IQuanLyCongTrinhHuHongRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<QuanLyCongTrinhHuHongViewModel, CongTrinhHuHong>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.NgayTao, opt => opt.Ignore())
                .ForMember(x => x.NgayXoa, opt => opt.Ignore())
                .ForMember(x => x.NguoiXoaId, opt => opt.Ignore())
                .ForMember(x => x.IsDeleted, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {
                    if (d.Id == 0)
                    {
                        d.NgayTao = DateTime.Now;
                    }
                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IQuanLyCongTrinhHuHongValidate _quanLyCongTrinhHuHongValidate;
        public QuanLyCongTrinhHuHongRepository(IContext context, IQuanLyCongTrinhHuHongValidate quanLyCongTrinhHuHongValidate)
        {
            _context = context;
            _quanLyCongTrinhHuHongValidate = quanLyCongTrinhHuHongValidate;
        }
        //required
        private readonly string sortStringDefault = "TenCongTrinh asc";
        private readonly List<string> searchCol = new List<string> { "TenCongTrinh", "Diadiem", "ChuDauTu", "QuyMo", "DienTich", "ThongSoKiThuat" };

        public DataTableResult<QuanLyCongTrinhHuHongGridVo> GetAll(QueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.CongTrinhHuHongs.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var dateTimeCurrent = DateTime.Now;
            var data = query.Select(cv => new QuanLyCongTrinhHuHongGridVo
            {
                Id = cv.Id,
                TenCongTrinh = cv.TenCongTrinh,
                Diadiem = cv.Diadiem,
                LoaiCongTrinh = cv.LoaiCongTrinh,
                NamXayDung = cv.NamXayDung,
                ChuDauTu = cv.ChuDauTu,
                QuyMo = cv.QuyMo,
                DienTich = cv.DienTich,
                ThongSoKiThuat = cv.ThongSoKiThuat,
                GhiChu = cv.GhiChu,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<QuanLyCongTrinhHuHongGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(QuanLyCongTrinhHuHongViewModel QuanLyCongTrinhHuHongViewModel)
        {
            var errors = _quanLyCongTrinhHuHongValidate.Validate(QuanLyCongTrinhHuHongViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (QuanLyCongTrinhHuHongViewModel.Id == 0)
            {
                var item = _iMapper.Map<QuanLyCongTrinhHuHongViewModel, CongTrinhHuHong>(QuanLyCongTrinhHuHongViewModel);
                _context.CongTrinhHuHongs.Add(item);
            }
            else
            {
                var item = _context.CongTrinhHuHongs.FirstOrDefault(o => o.Id == QuanLyCongTrinhHuHongViewModel.Id);
                if (item == null) throw new Exception("Công trình hư hỏng đã bị xóa");

                _iMapper.Map(QuanLyCongTrinhHuHongViewModel, item);
            }

            _context.SaveChanges();

            return true;
        }

        public bool Xoa(int Id)
        {
            var item = _context.CongTrinhHuHongs.FirstOrDefault(o => o.Id == Id);
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
            var item = _context.CongTrinhHuHongs.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public QuanLyCongTrinhHuHongGridVo GetById(int id)
        {
            var item = _context.CongTrinhHuHongs.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new QuanLyCongTrinhHuHongGridVo
                {
                    Id = item.Id,
                    TenCongTrinh = item.TenCongTrinh,
                    Diadiem = item.Diadiem,
                    LoaiCongTrinh = item.LoaiCongTrinh,
                    NamXayDung = item.NamXayDung,
                    ChuDauTu = item.ChuDauTu,
                    QuyMo = item.QuyMo,
                    DienTich = item.DienTich,
                    ThongSoKiThuat = item.ThongSoKiThuat,
                    GhiChu = item.GhiChu
                };
            }

            return new QuanLyCongTrinhHuHongGridVo();
        }

        public List<LookupViewModel> QuanLyCongTrinhHuHongLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn ---"
                    }
                };

            var query = _context.CongTrinhHuHongs.AsNoTracking().AsQueryable();
            if (!lookupQueryViewModel.isGetAll)
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var result = query.Select(o => new LookupViewModel
            {
                id = o.Id,
                text = o.TenCongTrinh
            }).ToList();

            if (lookupQueryViewModel.isHideOptionChonValueZero)
            {
                return result.ToList();
            }

            return data.Concat(result).ToList();
        }
    }
}
