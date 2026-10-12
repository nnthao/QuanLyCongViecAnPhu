using AutoMapper;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface IVaiTroXuLyRepository
    {
        DataLookupViewModel VaiTroXuLyLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<VaiTroXuLyGridVo> GetAll(QueryParamViewModel queryParams);
        VaiTroXuLyGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(VaiTroXuLyViewModel model);
        bool CapNhatThuTu(CapNhatThuTuVaiTroXuLyRequest request);
        bool CapNhatIsThongKeGiaoViec(CapNhatIsThongKeGiaoViecRequest request);
        bool CapNhatIsCanDinhKemFileKhiXacNhanHoanThanh(CapNhatIsCanDinhKemFileKhiXacNhanHoanThanhRequest request);
        bool Xoa(int id);
    }

    public class VaiTroXuLyRepository : IVaiTroXuLyRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<VaiTroXuLyViewModel, VaiTroXuLy>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IVaiTroXuLyValidate _validate;

        public VaiTroXuLyRepository(IContext context, IVaiTroXuLyValidate validate)
        {
            _context = context;
            _validate = validate;
        }

        private readonly string sortStringDefault = "ThuTu asc, Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten" };

        public DataTableResult<VaiTroXuLyGridVo> GetAll(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.VaiTroXuLys.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(cv => new VaiTroXuLyGridVo
            {
                Id = cv.Id,
                Ten = cv.Ten,
                ThuTu = cv.ThuTu,
                IsDeleted = cv.IsDeleted,
                IsThongKeGiaoViec = cv.IsThongKeGiaoViec,
                IsCanDinhKemFileKhiXacNhanHoanThanh = cv.IsCanDinhKemFileKhiXacNhanHoanThanh,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<VaiTroXuLyGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(VaiTroXuLyViewModel model)
        {
            var errors = _validate.Validate(model);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (model.Id == 0)
            {
                var item = _iMapper.Map<VaiTroXuLyViewModel, VaiTroXuLy>(model);
                _context.VaiTroXuLys.Add(item);
            }
            else
            {
                var item = _context.VaiTroXuLys.FirstOrDefault(o => o.Id == model.Id);
                if (item == null) throw new Exception("Dữ liệu đã bị xóa");

                _iMapper.Map(model, item);
            }

            _context.SaveChanges();
            CacheManagerHelper.InitVaiTroXuLyCache();

            return true;
        }

        public bool CapNhatThuTu(CapNhatThuTuVaiTroXuLyRequest request)
        {
            if (request == null || request.Id <= 0)
            {
                return false;
            }

            var item = _context.VaiTroXuLys.FirstOrDefault(o => o.Id == request.Id);
            if (item == null)
            {
                return false;
            }

            item.ThuTu = request.ThuTu;
            _context.SaveChanges();
            CacheManagerHelper.InitVaiTroXuLyCache();
            return true;
        }

        public bool CapNhatIsThongKeGiaoViec(CapNhatIsThongKeGiaoViecRequest request)
        {
            if (request == null || request.Id <= 0)
            {
                return false;
            }

            var item = _context.VaiTroXuLys.FirstOrDefault(o => o.Id == request.Id);
            if (item == null)
            {
                return false;
            }

            item.IsThongKeGiaoViec = request.IsThongKeGiaoViec;
            _context.SaveChanges();
            CacheManagerHelper.InitVaiTroXuLyCache();
            return true;
        }

        public bool CapNhatIsCanDinhKemFileKhiXacNhanHoanThanh(CapNhatIsCanDinhKemFileKhiXacNhanHoanThanhRequest request)
        {
            if (request == null || request.Id <= 0)
            {
                return false;
            }

            var item = _context.VaiTroXuLys.FirstOrDefault(o => o.Id == request.Id);
            if (item == null)
            {
                return false;
            }

            item.IsCanDinhKemFileKhiXacNhanHoanThanh = request.IsCanDinhKemFileKhiXacNhanHoanThanh;
            _context.SaveChanges();
            CacheManagerHelper.InitVaiTroXuLyCache();
            return true;
        }

        public bool Xoa(int id)
        {
            var item = _context.VaiTroXuLys.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                item.IsDeleted = true;
                _context.SaveChanges();
                CacheManagerHelper.InitVaiTroXuLyCache();
                return true;
            }

            return false;
        }

        public bool KhoiPhuc(int id)
        {
            var item = _context.VaiTroXuLys.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                CacheManagerHelper.InitVaiTroXuLyCache();
                return true;
            }

            return false;
        }

        public VaiTroXuLyGridVo GetById(int id)
        {
            var item = _context.VaiTroXuLys.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new VaiTroXuLyGridVo
                {
                    Id = item.Id,
                    Ten = item.Ten,
                    ThuTu = item.ThuTu,
                    IsThongKeGiaoViec = item.IsThongKeGiaoViec,
                    IsCanDinhKemFileKhiXacNhanHoanThanh = item.IsCanDinhKemFileKhiXacNhanHoanThanh
                };
            }

            return new VaiTroXuLyGridVo();
        }

        public DataLookupViewModel VaiTroXuLyLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var caches = CacheManagerHelper.GetVaiTroXuLyCache();

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
