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
    public interface IQuanLyCongTrinhHuHongChiTietRepository
    {
        DataTableResult<QuanLyCongTrinhHuHongChiTietGridVo> GetAll(QueryParamViewModel queryParams);
        QuanLyCongTrinhHuHongChiTietGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(QuanLyCongTrinhHuHongChiTietViewModel QuanLyCongTrinhHuHongChiTietViewModel);
        bool Xoa(int Id);
        List<QuanLyCongTrinhHuHongChiTietGridVo> GetByCongTrinhHuHongId(int congTrinhHuHongId);
    }

    public class QuanLyCongTrinhHuHongChiTietRepository : IQuanLyCongTrinhHuHongChiTietRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<QuanLyCongTrinhHuHongChiTietViewModel, CongTrinhHuHongChiTiet>()
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
        private readonly IQuanLyCongTrinhHuHongChiTietValidate _quanLyCongTrinhHuHongChiTietValidate;
        public QuanLyCongTrinhHuHongChiTietRepository(IContext context, IQuanLyCongTrinhHuHongChiTietValidate quanLyCongTrinhHuHongChiTietValidate)
        {
            _context = context;
            _quanLyCongTrinhHuHongChiTietValidate = quanLyCongTrinhHuHongChiTietValidate;
        }
        //required
        private readonly string sortStringDefault = "TenCongTrinh asc";
        private readonly List<string> searchCol = new List<string> { "TenCongTrinh", "Diadiem", "ChuDauTu" };

        public DataTableResult<QuanLyCongTrinhHuHongChiTietGridVo> GetAll(QueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.CongTrinhHuHongChiTiets.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var dateTimeCurrent = DateTime.Now;
            var data = query.Select(cv => new QuanLyCongTrinhHuHongChiTietGridVo
            {
                Id = cv.Id,
                CongTrinhHuHongId = cv.CongTrinhHuHongId,
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

            return new DataTableResult<QuanLyCongTrinhHuHongChiTietGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(QuanLyCongTrinhHuHongChiTietViewModel QuanLyCongTrinhHuHongChiTietViewModel)
        {
            var errors = _quanLyCongTrinhHuHongChiTietValidate.Validate(QuanLyCongTrinhHuHongChiTietViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (QuanLyCongTrinhHuHongChiTietViewModel.Id == 0)
            {
                var item = _iMapper.Map<QuanLyCongTrinhHuHongChiTietViewModel, CongTrinhHuHongChiTiet>(QuanLyCongTrinhHuHongChiTietViewModel);
                _context.CongTrinhHuHongChiTiets.Add(item);
            }
            else
            {
                var item = _context.CongTrinhHuHongChiTiets.FirstOrDefault(o => o.Id == QuanLyCongTrinhHuHongChiTietViewModel.Id);
                if (item == null) throw new Exception("Chi tiết công trình hư hỏng đã bị xóa");

                _iMapper.Map(QuanLyCongTrinhHuHongChiTietViewModel, item);
            }

            _context.SaveChanges();

            return true;
        }

        public bool Xoa(int Id)
        {
            var item = _context.CongTrinhHuHongChiTiets.FirstOrDefault(o => o.Id == Id);
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
            var item = _context.CongTrinhHuHongChiTiets.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public QuanLyCongTrinhHuHongChiTietGridVo GetById(int id)
        {
            var item = _context.CongTrinhHuHongChiTiets.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new QuanLyCongTrinhHuHongChiTietGridVo
                {
                    Id = item.Id,
                    CongTrinhHuHongId = item.CongTrinhHuHongId,
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

            return new QuanLyCongTrinhHuHongChiTietGridVo();
        }

        public List<QuanLyCongTrinhHuHongChiTietGridVo> GetByCongTrinhHuHongId(int congTrinhHuHongId)
        {
            var items = _context.CongTrinhHuHongChiTiets.AsNoTracking()
                .Where(o => o.CongTrinhHuHongId == congTrinhHuHongId && o.IsDeleted != true)
                .Select(cv => new QuanLyCongTrinhHuHongChiTietGridVo
                {
                    Id = cv.Id,
                    CongTrinhHuHongId = cv.CongTrinhHuHongId,
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
                }).ToList();

            return items;
        }
    }
}
