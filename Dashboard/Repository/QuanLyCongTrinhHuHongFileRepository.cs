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
    public interface IQuanLyCongTrinhHuHongFileRepository
    {
        DataTableResult<QuanLyCongTrinhHuHongFileGridVo> GetAll(QueryParamViewModel queryParams);
        QuanLyCongTrinhHuHongFileGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(QuanLyCongTrinhHuHongFileViewModel QuanLyCongTrinhHuHongFileViewModel);
        bool Xoa(int Id);
        List<QuanLyCongTrinhHuHongFileGridVo> GetByCongTrinhHuHongId(int congTrinhHuHongId);
    }

    public class QuanLyCongTrinhHuHongFileRepository : IQuanLyCongTrinhHuHongFileRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<QuanLyCongTrinhHuHongFileViewModel, CongTrinhHuHongFile>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.NgayXoa, opt => opt.Ignore())
                .ForMember(x => x.NguoiXoaId, opt => opt.Ignore())
                .ForMember(x => x.IsDeleted, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {
                    if (d.Id == 0)
                    {
                        d.IsDeleted = false;
                    }
                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IQuanLyCongTrinhHuHongFileValidate _quanLyCongTrinhHuHongFileValidate;
        public QuanLyCongTrinhHuHongFileRepository(IContext context, IQuanLyCongTrinhHuHongFileValidate quanLyCongTrinhHuHongFileValidate)
        {
            _context = context;
            _quanLyCongTrinhHuHongFileValidate = quanLyCongTrinhHuHongFileValidate;
        }
        //required
        private readonly string sortStringDefault = "TenFile asc";
        private readonly List<string> searchCol = new List<string> { "TenFile" };

        public DataTableResult<QuanLyCongTrinhHuHongFileGridVo> GetAll(QueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.CongTrinhHuHongFiles.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var dateTimeCurrent = DateTime.Now;
            var data = query.Select(cv => new QuanLyCongTrinhHuHongFileGridVo
            {
                Id = cv.Id,
                CongTrinhHuHongId = cv.CongTrinhHuHongId,
                TenFile = cv.TenFile,
                DuongDan = cv.DuongDan,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<QuanLyCongTrinhHuHongFileGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(QuanLyCongTrinhHuHongFileViewModel QuanLyCongTrinhHuHongFileViewModel)
        {
            var errors = _quanLyCongTrinhHuHongFileValidate.Validate(QuanLyCongTrinhHuHongFileViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (QuanLyCongTrinhHuHongFileViewModel.Id == 0)
            {
                var item = _iMapper.Map<QuanLyCongTrinhHuHongFileViewModel, CongTrinhHuHongFile>(QuanLyCongTrinhHuHongFileViewModel);
                _context.CongTrinhHuHongFiles.Add(item);
            }
            else
            {
                var item = _context.CongTrinhHuHongFiles.FirstOrDefault(o => o.Id == QuanLyCongTrinhHuHongFileViewModel.Id);
                if (item == null) throw new Exception("File công trình hư hỏng đã bị xóa");

                _iMapper.Map(QuanLyCongTrinhHuHongFileViewModel, item);
            }

            _context.SaveChanges();

            return true;
        }

        public bool Xoa(int Id)
        {
            var item = _context.CongTrinhHuHongFiles.FirstOrDefault(o => o.Id == Id);
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
            var item = _context.CongTrinhHuHongFiles.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public QuanLyCongTrinhHuHongFileGridVo GetById(int id)
        {
            var item = _context.CongTrinhHuHongFiles.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new QuanLyCongTrinhHuHongFileGridVo
                {
                    Id = item.Id,
                    CongTrinhHuHongId = item.CongTrinhHuHongId,
                    TenFile = item.TenFile,
                    DuongDan = item.DuongDan
                };
            }

            return new QuanLyCongTrinhHuHongFileGridVo();
        }

        public List<QuanLyCongTrinhHuHongFileGridVo> GetByCongTrinhHuHongId(int congTrinhHuHongId)
        {
            var items = _context.CongTrinhHuHongFiles.AsNoTracking()
                .Where(o => o.CongTrinhHuHongId == congTrinhHuHongId && o.IsDeleted != true)
                .Select(cv => new QuanLyCongTrinhHuHongFileGridVo
                {
                    Id = cv.Id,
                    CongTrinhHuHongId = cv.CongTrinhHuHongId,
                    TenFile = cv.TenFile,
                    DuongDan = cv.DuongDan,
                    IsDeleted = cv.IsDeleted,
                }).ToList();

            return items;
        }
    }
}
