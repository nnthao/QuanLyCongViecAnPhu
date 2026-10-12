using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface IVaiTroRepository
    {
        dynamic CapNhat(VaiTroViewModel vaiTroViewModel);
        dynamic GanVaiTroKhacChoPhongBan(GanVaiTroKhacViewModel viewModel);
        DataTableResult<VaiTroGridVo> GetAll(QueryParamViewModel queryParams);
        bool KhoiPhuc(int Id);
        dynamic Them(VaiTroViewModel vaiTroViewModel);
        List<string> VaiTroPhanQuyenStr(int vaitroId, string nhomVaiTroId);
        bool Xoa(int Id);
    }

    public class VaiTroRepository : IVaiTroRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<VaiTroViewModel, VaiTro>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {

                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IVaiTroValidate _vaiTroValidate;

        public VaiTroRepository(IContext context, IVaiTroValidate vaiTroValidate)
        {
            _context = context;
            _vaiTroValidate = vaiTroValidate;
        }

        //required
        private readonly string sortStringDefault = "Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten" };

        public DataTableResult<VaiTroGridVo> GetAll(QueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.VaiTros.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }
            var data = query.Select(cv => new VaiTroGridVo
            {
                Id = cv.Id,
                Ten = cv.Ten,
                PhanQuyenKeys = cv.PhanQuyenKeys,
                IsDeleted = cv.IsDeleted
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<VaiTroGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic Them(VaiTroViewModel vaiTroViewModel)
        {
            var item = _iMapper.Map<VaiTroViewModel, VaiTro>(vaiTroViewModel);
            if (!string.IsNullOrEmpty(vaiTroViewModel.PhanQuyenStr))
            {
                var phanQuyenMenus = JsonConvert.DeserializeObject<List<string>>(vaiTroViewModel.PhanQuyenStr);
                if (phanQuyenMenus != null && phanQuyenMenus.Count > 0)
                {
                    var phanQuyenKeys = phanQuyenMenus.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                    item.PhanQuyenKeys = string.Join(",", phanQuyenKeys);
                }
            }

            var errors = _vaiTroValidate.Validate(item);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            _context.VaiTros.Add(item);
            _context.SaveChanges();
            CacheManagerHelper.InitVaiTroCache();

            return new { Id = item.Id };
        }

        public dynamic CapNhat(VaiTroViewModel vaiTroViewModel)
        {
            var item = _context.VaiTros.FirstOrDefault(o => o.Id == vaiTroViewModel.Id);
            if (item == null) throw new Exception("Vai trò đã bị xóa");

            _iMapper.Map(vaiTroViewModel, item);
            if (!string.IsNullOrEmpty(vaiTroViewModel.PhanQuyenStr))
            {
                var phanQuyenMenus = JsonConvert.DeserializeObject<List<string>>(vaiTroViewModel.PhanQuyenStr);
                if (phanQuyenMenus != null && phanQuyenMenus.Count > 0)
                {
                    var phanQuyenKeys = phanQuyenMenus.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                    item.PhanQuyenKeys = string.Join(",", phanQuyenKeys);
                }
                else
                {
                    item.PhanQuyenKeys = "";
                }
            }

            var errors = _vaiTroValidate.Validate(item);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            _context.SaveChanges();
            CacheManagerHelper.InitVaiTroCache();
            return new { Id = item.Id };
        }

        public bool Xoa(int Id)
        {
            var item = _context.VaiTros.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = true;
                _context.SaveChanges();
                CacheManagerHelper.InitVaiTroCache();
                return true;
            }

            return false;
        }

        public bool KhoiPhuc(int Id)
        {
            var item = _context.VaiTros.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                CacheManagerHelper.InitVaiTroCache();
                return true;
            }

            return false;
        }

        public dynamic GanVaiTroKhacChoPhongBan(GanVaiTroKhacViewModel viewModel)
        {
            var vaiTro = _context.VaiTros.AsNoTracking().FirstOrDefault(o => o.Id == viewModel.VaiTroId && o.IsDeleted != true);
            if (vaiTro == null)
            {
                return new { Success = false, Message = "Vai trò không tồn tại hoặc đã bị xóa." };
            }

            var phongBanIds = ParseIds(viewModel.PhongBanIdsStr);
            if (phongBanIds.Count == 0)
            {
                return new { Success = false, Message = "Vui lòng chọn ít nhất một phòng ban." };
            }

            var users = _context.Users
                .Where(o => o.IsSystemUser != true
                    && o.IsDeleted != true
                    && o.PhongBanId.HasValue
                    && phongBanIds.Contains(o.PhongBanId.Value))
                .ToList();

            var soNguoiDungCapNhat = 0;
            foreach (var user in users)
            {
                var nhomVaiTroIds = ParseIds(user.NhomVaiTroId);
                if (user.VaiTroId.GetValueOrDefault() == viewModel.VaiTroId || nhomVaiTroIds.Contains(viewModel.VaiTroId))
                {
                    continue;
                }

                nhomVaiTroIds.Add(viewModel.VaiTroId);
                user.NhomVaiTroId = string.Join(",", nhomVaiTroIds.Distinct());
                soNguoiDungCapNhat++;
            }

            if (soNguoiDungCapNhat > 0)
            {
                _context.SaveChanges();
                CacheManagerHelper.InitUserCache();
            }

            return new
            {
                Success = true,
                SoNguoiDung = users.Count,
                SoNguoiDungCapNhat = soNguoiDungCapNhat,
                Message = $"Đã gán vai trò khác cho {soNguoiDungCapNhat}/{users.Count} người dùng."
            };
        }

        public List<string> VaiTroPhanQuyenStr(int vaitroId, string nhomVaiTroId)
        {
            var phanQuyens = new List<string>();

            var vaiTroIds = new List<int> { vaitroId };
            if (!string.IsNullOrEmpty(nhomVaiTroId))
            {
                vaiTroIds.AddRange(nhomVaiTroId.Split(',').Where(o => !string.IsNullOrEmpty(o))
                .Select(o => Convert.ToInt32(o)));
            }

            var datas = _context.VaiTros.AsNoTracking().Where(o => vaiTroIds.Contains(o.Id));
            foreach (var item in datas)
            {
                phanQuyens.AddRange(item.PhanQuyenKeys.Split(',').Where(o => !string.IsNullOrEmpty(o))
                    .Select(o => o.Trim()).ToList());
            }
            return phanQuyens.Distinct().ToList();
        }

        private static List<int> ParseIds(string idsStr)
        {
            if (string.IsNullOrWhiteSpace(idsStr))
            {
                return new List<int>();
            }

            return idsStr.Split(',')
                .Select(o =>
                {
                    int id;
                    return int.TryParse(o, out id) ? id : 0;
                })
                .Where(o => o > 0)
                .Distinct()
                .ToList();
        }
    }
}
