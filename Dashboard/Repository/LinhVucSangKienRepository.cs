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
    public interface ILinhVucSangKienRepository
    {
        DataTableResult<LinhVucSangKienGridVo> GetAll(QueryParamViewModel queryParams);
        LinhVucSangKienGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(LinhVucSangKienViewModel LinhVucSangKienViewModel);
        bool Xoa(int Id);
        List<LookupViewModel> LinhVucLookup();
    }

    public class LinhVucSangKienRepository : ILinhVucSangKienRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<LinhVucSangKienViewModel, LinhVucSangKien>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {

                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly ILinhVucSangKienValidate _linhVucSangKienValidate;
        public LinhVucSangKienRepository(IContext context, ILinhVucSangKienValidate linhVucSangKienValidate)
        {
            _context = context;
            _linhVucSangKienValidate = linhVucSangKienValidate;
        }
        //required
        private readonly string sortStringDefault = "TenLinhVuc asc";
        private readonly List<string> searchCol = new List<string> { "TenLinhVuc" };

        public DataTableResult<LinhVucSangKienGridVo> GetAll(QueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.LinhVucSangKiens.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(cv => new LinhVucSangKienGridVo
            {
                Id = cv.Id,
                TenLinhVuc = cv.TenLinhVuc,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<LinhVucSangKienGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(LinhVucSangKienViewModel LinhVucSangKienViewModel)
        {
            var errors = _linhVucSangKienValidate.Validate(LinhVucSangKienViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (LinhVucSangKienViewModel.Id == 0)
            {
                var item = _iMapper.Map<LinhVucSangKienViewModel, LinhVucSangKien>(LinhVucSangKienViewModel);
                item.IsDeleted = false;
                _context.LinhVucSangKiens.Add(item);
            }
            else
            {
                var item = _context.LinhVucSangKiens.FirstOrDefault(o => o.Id == LinhVucSangKienViewModel.Id);
                if (item == null) throw new Exception("Lĩnh vực sáng kiến đã bị xóa");

                _iMapper.Map(LinhVucSangKienViewModel, item);
            }

            _context.SaveChanges();

            return true;
        }


        public bool Xoa(int Id)
        {
            var item = _context.LinhVucSangKiens.FirstOrDefault(o => o.Id == Id);
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
            var item = _context.LinhVucSangKiens.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public LinhVucSangKienGridVo GetById(int id)
        {
            var item = _context.LinhVucSangKiens.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new LinhVucSangKienGridVo
                {
                    Id = item.Id,
                    TenLinhVuc = item.TenLinhVuc
                };
            }

            return new LinhVucSangKienGridVo();
        }

        public List<LookupViewModel> LinhVucLookup()
        {
            var data = new List<LookupViewModel>
            {
                new LookupViewModel
                {
                    id = 0,
                    text = "--- Chọn ---"
                }
            };

            var linhVucActive = _context.LinhVucSangKiens.AsNoTracking()
                .Where(o => o.IsDeleted != true)
                .OrderBy(o => o.TenLinhVuc ?? string.Empty)
                .Select(o => new LookupViewModel
                {
                    id = o.Id,
                    text = o.TenLinhVuc
                }).ToList();

            data.AddRange(linhVucActive);
            return data;
        }
    }
}

