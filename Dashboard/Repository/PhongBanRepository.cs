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
    public interface IPhongBanRepository
    {
        bool CapNhatThuTu(CapNhatThuTuPhongBanRequest request);
        bool CapNhatIsPhongNhanViec(CapNhatIsPhongNhanViecRequest request);
        DataLookupViewModel PhongBanLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<PhongBanGridVo> GetAll(QueryParamViewModel queryParams);
        PhongBanGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(PhongBanViewModel PhongBanViewModel);
        bool Xoa(int Id);
    }

    public class PhongBanRepository : IPhongBanRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<PhongBanViewModel, PhongBan>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IPhongBanValidate _phongBanValidate;
        public PhongBanRepository(IContext context, IPhongBanValidate phongBanValidate)
        {
            _context = context;
            _phongBanValidate = phongBanValidate;
        }

        private readonly string sortStringDefault = "ThuTu asc, Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten" };

        public DataTableResult<PhongBanGridVo> GetAll(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var isDeletedSearch = queryParams.DaXoaSearch == true;
            var allPhongBans = CacheManagerHelper.GetPhongBanCache();

            var filteredList = allPhongBans
                .Where(o => isDeletedSearch ? o.IsDeleted == true : o.IsDeleted != true)
                .ToList();

            if (queryParams.PhongBanChaId.GetValueOrDefault() != 0)
            {
                var phongBanChaId = queryParams.PhongBanChaId.GetValueOrDefault();
                filteredList = filteredList.Where(o => o.PhongBanChaId == phongBanChaId).ToList();
            }

            var treeSortedList = GetPhongBanTreeSortedList(filteredList);

            var query = treeSortedList.AsQueryable().Where(queryParams.searchString);
            var resultList = query.ToList();

            return new DataTableResult<PhongBanGridVo>
            {
                rows = resultList,
                total = resultList.Count
            };
        }

        private List<PhongBanGridVo> GetPhongBanTreeSortedList(List<PhongBanGridVo> list)
        {
            var result = new List<PhongBanGridVo>();
            var roots = list
                .Where(pb => !pb.PhongBanChaId.HasValue || pb.PhongBanChaId.Value == 0 || !list.Any(p => p.Id == pb.PhongBanChaId.Value))
                .OrderBy(pb => pb.ThuTu ?? int.MaxValue)
                .ThenBy(pb => pb.Ten)
                .ToList();

            foreach (var root in roots)
            {
                root.Level = 0;
                result.Add(root);
                AddChildren(root.Id, list, result, 1);
            }

            return result;
        }

        private void AddChildren(int parentId, List<PhongBanGridVo> list, List<PhongBanGridVo> result, int level)
        {
            var children = list
                .Where(pb => pb.PhongBanChaId == parentId)
                .OrderBy(pb => pb.ThuTu ?? int.MaxValue)
                .ThenBy(pb => pb.Ten)
                .ToList();

            foreach (var child in children)
            {
                child.Level = level;
                result.Add(child);
                AddChildren(child.Id, list, result, level + 1);
            }
        }

        public dynamic ThemOrCapNhat(PhongBanViewModel PhongBanViewModel)
        {
            var errors = _phongBanValidate.Validate(PhongBanViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (PhongBanViewModel.Id == 0)
            {
                var item = _iMapper.Map<PhongBanViewModel, PhongBan>(PhongBanViewModel);
                _context.PhongBans.Add(item);
            }
            else
            {
                var item = _context.PhongBans.FirstOrDefault(o => o.Id == PhongBanViewModel.Id);
                if (item == null) throw new Exception("Phòng bàn không tồn tại");

                _iMapper.Map(PhongBanViewModel, item);
            }

            _context.SaveChanges();
            CacheManagerHelper.InitPhongBanCache();

            return true;
        }

        public bool CapNhatThuTu(CapNhatThuTuPhongBanRequest request)
        {
            if (request == null || request.Id <= 0)
            {
                return false;
            }

            var item = _context.PhongBans.FirstOrDefault(o => o.Id == request.Id);
            if (item == null)
            {
                return false;
            }

            item.ThuTu = request.ThuTu;
            _context.SaveChanges();
            CacheManagerHelper.InitPhongBanCache();
            return true;
        }

        public bool CapNhatIsPhongNhanViec(CapNhatIsPhongNhanViecRequest request)
        {
            if (request == null || request.Id <= 0)
            {
                return false;
            }

            var item = _context.PhongBans.FirstOrDefault(o => o.Id == request.Id);
            if (item == null)
            {
                return false;
            }

            item.IsPhongNhanViec = request.IsPhongNhanViec;
            _context.SaveChanges();
            CacheManagerHelper.InitPhongBanCache();
            return true;
        }

        public bool Xoa(int Id)
        {
            var item = _context.PhongBans.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = true;
                _context.SaveChanges();
                CacheManagerHelper.InitPhongBanCache();
                return true;
            }

            return false;
        }

        public bool KhoiPhuc(int Id)
        {
            var item = _context.PhongBans.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                CacheManagerHelper.InitPhongBanCache();
                return true;
            }

            return false;
        }

        public PhongBanGridVo GetById(int id)
        {
            var item = _context.PhongBans.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new PhongBanGridVo
                {
                    Id = item.Id,
                    Ten = item.Ten,
                    PhongBanChaId = item.PhongBanChaId,
                    ThuTu = item.ThuTu,
                    IsPhongNhanViec = item.IsPhongNhanViec,
                    NguoiTheoDoiChiDaoId = item.NguoiTheoDoiChiDaoId
                };
            }

            return new PhongBanGridVo();
        }

        public DataLookupViewModel PhongBanLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var vaiTros = CacheManagerHelper.GetPhongBanCache();

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

            if (lookupQueryViewModel.phongBanLoaiTruId != 0)
            {
                query = query.Where(o => o.Id != lookupQueryViewModel.phongBanLoaiTruId);
            }

            query = query.OrderBy(o => o.ThuTu ?? int.MaxValue).ThenBy(o => o.Ten);

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
