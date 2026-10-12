
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Dashboard.AdoNetHelper;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface IDuAnRepository
    {
        DataTableResult<DuAnGridVo> GetAll(DuAnQueryViewModel queryParams);
        DuAnGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(DuAnViewModel duAnViewModel);
        bool Xoa(int Id);
        List<ThongKeThanhVienGridVo> ThongKeThanhVienThamGiaNhieuNhat();
        List<ThongKeThanhVienGridVo> ThongKeThanhVienThamGiaThuNghiemNhieuNhat();
        List<ThongKeThanhVienGridVo> ThongKeThanhVienMucDoHoTroCaoNhat();
        List<DuAnGridVo> GetListDuAnByUserId(int userId);
        List<DuAnGridVo> GetListDuAnThuNghiemByUserId(int userId);
        List<DuAnGridVo> GetListAllDuAnByUserId(int userId);
        List<DuAn> GetListAllDuAnActive();
        List<ThongKeDuAnGridVo> ThongKeDuAnTheoSoThanhVien();
        List<DuAnThanhVienInfo> GetListThanhVienByDuAnId(int duAnId);
        List<DuAn> GetListDuAnNotThuNghiem();
        List<MatranThanhVienDuAnInfo> GetMatranThanhVienDuAn();
        List<DuAnThanhVienThongKeGridVo> GetDuAnThanhVienThongKe();
        List<TruongNhomThongKeGridVo> GetTruongNhomThongKe();
    }

    public class DuAnRepository : IDuAnRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DuAnViewModel, DuAn>()
                .ForMember(x => x.Id, opt => opt.Ignore());

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IDuAnValidate _duAnValidate;
        public DuAnRepository(IContext context, IDuAnValidate duAnValidate)
        {
            _context = context;
            _duAnValidate = duAnValidate;
        }

        private readonly string sortStringDefault = "Ten asc";
        private readonly List<string> searchCol = new List<string> { "Ten", "MoTa" };

        public DataTableResult<DuAnGridVo> GetAll(DuAnQueryViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.DuAns.Include(o => o.DuAnThanhViens).AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            if (queryParams.IsThuNghiemSearch == true)
            {
                query = query.Where(o => o.IsThuNghiem == true);
            }


            var data = query.Select(cv => new DuAnGridVo
            {
                Id = cv.Id,
                Ten = cv.Ten,
                MoTa = cv.MoTa,
                IsDeleted = cv.IsDeleted,
                IsThuNghiem = cv.IsThuNghiem,
                ThanhViens = cv.DuAnThanhViens.Where(x => x.IsDeleted != true).Select(x => x.UserId).ToList(),
                TruongNhomIds = cv.DuAnThanhViens.Where(x => x.IsDeleted != true && x.IsTruongNhom == true).Select(x => x.UserId).ToList()
            }).Where(queryParams.searchString);

            var total = data.Select(o => o.Id).Count();


            var result = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();

            return new DataTableResult<DuAnGridVo>
            {
                rows = result,
                total = total
            };
        }

        public DuAnGridVo GetById(int id)
        {
            var item = _context.DuAns.Include(o => o.DuAnThanhViens).Include(o => o.DuAnFiles).AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                var danhSachThanhVien = item.DuAnThanhViens.Where(x => x.IsDeleted != true && x.UserId.HasValue)
                    .Select(x => new DuAnThanhVienInfo
                    {
                        UserId = x.UserId.Value,
                        TenUser = "",
                        MucDoHoTro = x.MucDoHoTro,
                        IsTruongNhom = x.IsTruongNhom == true
                    }).ToList();

                var danhSachFile = item.DuAnFiles.Where(x => x.IsDeleted != true)
                    .Select(x => new DuAnFileGridVo
                    {
                        Id = x.Id,
                        TenFile = x.TenFile,
                        DuongDan = x.DuongDan
                    }).ToList();

                var data = new DuAnGridVo
                {
                    Id = item.Id,
                    Ten = item.Ten,
                    MoTa = item.MoTa,
                    IsDeleted = item.IsDeleted,
                    IsThuNghiem = item.IsThuNghiem,
                    SoThanhVien = item.DuAnThanhViens.Count(x => x.IsDeleted != true),
                    SoTruongNhom = item.DuAnThanhViens.Count(x => x.IsDeleted != true && x.IsTruongNhom == true),
                    ThanhViens = item.DuAnThanhViens.Where(x => x.IsDeleted != true).Select(x => x.UserId).ToList(),
                    TruongNhomIds = item.DuAnThanhViens.Where(x => x.IsDeleted != true && x.IsTruongNhom == true).Select(x => x.UserId).ToList(),
                    DanhSachThanhVien = danhSachThanhVien,
                    DuAnFiles = danhSachFile
                };

                return data;
            }

            return new DuAnGridVo();
        }

        public bool KhoiPhuc(int Id)
        {
            var item = _context.DuAns.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public dynamic ThemOrCapNhat(DuAnViewModel duAnViewModel)
        {
            var errors = _duAnValidate.Validate(duAnViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (duAnViewModel.Id == 0)
            {
                var item = _iMapper.Map<DuAnViewModel, DuAn>(duAnViewModel);
                AddThanhVienVaoEntity(item, duAnViewModel);
                AddFileToEntity(item, duAnViewModel);

                _context.DuAns.Add(item);
                _context.SaveChanges();
            }
            else
            {
                var item = _context.DuAns.Include(o => o.DuAnThanhViens).Include(o => o.DuAnFiles).FirstOrDefault(o => o.Id == duAnViewModel.Id);
                if (item == null) throw new Exception("Dự án đã bị xóa");

                _iMapper.Map(duAnViewModel, item);
                AddThanhVienVaoEntity(item, duAnViewModel);
                AddFileToEntity(item, duAnViewModel);

                _context.SaveChanges();
            }

            return true;
        }

        private void AddThanhVienVaoEntity(DuAn item, DuAnViewModel itemViewModel)
        {
            var userIntIds = string.IsNullOrEmpty(itemViewModel.UserIds) ? new List<int>() :
                new List<int>(Array.ConvertAll(itemViewModel.UserIds.Split(','), int.Parse));

            int? truongNhomId = null;
            if (!string.IsNullOrEmpty(itemViewModel.TruongNhomIds) && int.TryParse(itemViewModel.TruongNhomIds, out int truongNhomIdValue))
            {
                truongNhomId = truongNhomIdValue;
            }

            var mucDoHoTroDict = new Dictionary<int, int?>();
            if (!string.IsNullOrEmpty(itemViewModel.ThanhVienMucDoHoTroJson))
            {
                try
                {
                    var mucDoHoTroList = JsonConvert.DeserializeObject<List<ThanhVienMucDoHoTroInfo>>(itemViewModel.ThanhVienMucDoHoTroJson);
                    if (mucDoHoTroList != null)
                    {
                        foreach (var mucDo in mucDoHoTroList)
                        {
                            mucDoHoTroDict[mucDo.UserId] = mucDo.MucDoHoTro;
                        }
                    }
                }
                catch { }
            }

            var oldIds = item.DuAnThanhViens.Where(x => x.IsDeleted != true && x.UserId.HasValue)
                .Select(x => x.UserId.Value).ToList();

            var removeIds = oldIds.Except(userIntIds).ToList();
            var addIds = userIntIds.Except(oldIds).ToList();

            foreach (var removeId in removeIds)
            {
                var removeItem = item.DuAnThanhViens.FirstOrDefault(x => x.UserId == removeId && x.IsDeleted != true);
                if (removeItem != null)
                {
                    removeItem.IsDeleted = true;
                }
            }

            foreach (var addId in addIds)
            {
                var addItem = new DuAnThanhVien
                {
                    UserId = addId,
                    IsTruongNhom = truongNhomId.HasValue && truongNhomId.Value == addId,
                    MucDoHoTro = mucDoHoTroDict.ContainsKey(addId) ? mucDoHoTroDict[addId] : null
                };
                item.DuAnThanhViens.Add(addItem);
            }

            foreach (var existingId in oldIds.Intersect(userIntIds))
            {
                var existingItem = item.DuAnThanhViens.FirstOrDefault(x => x.UserId == existingId && x.IsDeleted != true);
                if (existingItem != null)
                {
                    existingItem.IsTruongNhom = truongNhomId.HasValue && truongNhomId.Value == existingId;
                    if (mucDoHoTroDict.ContainsKey(existingId))
                    {
                        existingItem.MucDoHoTro = mucDoHoTroDict[existingId];
                    }
                }
            }
        }

        private void AddFileToEntity(DuAn item, DuAnViewModel itemViewModel)
        {
            var uploadPath = System.Web.HttpContext.Current.Server.MapPath(BienDungChung.UploadsPath);
            
            foreach (var file in item.DuAnFiles.Where(x => x.IsDeleted != true).ToList())
            {
                if (!itemViewModel.TaiLieuDinhKemIdInts.Contains(file.Id))
                {
                    file.IsDeleted = true;
                    file.NguoiXoaId = itemViewModel.CurrentUserId;
                    file.NgayXoa = DateTime.Now;
                }
            }

            if (itemViewModel.FileDinhKems == null || itemViewModel.FileDinhKems.Count == 0)
            {
                return;
            }

            foreach (var taiLieu in itemViewModel.FileDinhKems.Where(f => f != null))
            {
                try
                {
                    item.DuAnFiles.Add(new DuAnFile
                    {
                        TenFile = taiLieu.FileName,
                        DuongDan = FileHelper.SaveHttpFilesToUploads(uploadPath, taiLieu),
                        IsDeleted = false
                    });
                }
                catch (ArgumentException ex)
                {
                    throw new Exception($"Lỗi khi upload file '{taiLieu?.FileName}': {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lưu file '{taiLieu?.FileName}': {ex.Message}", ex);
                }
            }
        }

        public bool Xoa(int Id)
        {
            var item = _context.DuAns.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = true;

                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public List<ThongKeThanhVienGridVo> ThongKeThanhVienThamGiaNhieuNhat()
        {
            var query = from dt in _context.DuAnThanhViens
                        join d in _context.DuAns on dt.DuAnId equals d.Id
                        where dt.IsDeleted != true && dt.UserId.HasValue && d.IsDeleted != true && d.IsThuNghiem != true
                        group d.Ten by dt.UserId into g
                        select new ThongKeThanhVienGridVo
                        {
                            UserId = g.Key.Value,
                            TenUser = "",
                            SoNhomThamGia = g.Count(),
                            DanhSachTenDuAn = g.ToList()
                        };

            var result = MergeThongKeVoiNguoiDungCoQuyenThamGiaDuAn(query.ToList());
            return result.OrderBy(x => x.SoNhomThamGia).ThenBy(x => x.TenUser).ToList();
        }

        public List<ThongKeThanhVienGridVo> ThongKeThanhVienThamGiaThuNghiemNhieuNhat()
        {
            var query = from dt in _context.DuAnThanhViens
                        join d in _context.DuAns on dt.DuAnId equals d.Id
                        where dt.IsDeleted != true && dt.UserId.HasValue && d.IsDeleted != true && d.IsThuNghiem == true
                        group d.Ten by dt.UserId into g
                        select new ThongKeThanhVienGridVo
                        {
                            UserId = g.Key.Value,
                            TenUser = "",
                            SoNhomThamGia = g.Count(),
                            DanhSachTenDuAn = g.ToList()
                        };

            var result = MergeThongKeVoiNguoiDungCoQuyenThamGiaDuAn(query.ToList());
            return result.OrderBy(x => x.SoNhomThamGia).ThenBy(x => x.TenUser).ToList();
        }

        public List<ThongKeThanhVienGridVo> ThongKeThanhVienMucDoHoTroCaoNhat()
        {
            var query = from dt in _context.DuAnThanhViens
                        join d in _context.DuAns on dt.DuAnId equals d.Id
                        where dt.IsDeleted != true && dt.UserId.HasValue && d.IsDeleted != true && d.IsThuNghiem != true
                        group new { dt.MucDoHoTro, TenDuAn = d.Ten } by dt.UserId into g
                        select new ThongKeThanhVienGridVo
                        {
                            UserId = g.Key.Value,
                            TenUser = "",
                            SoNhomThamGia = g.Count(),
                            TongMucDoHoTro = g.Sum(x => x.MucDoHoTro ?? 0),
                            DanhSachTenDuAn = g.Select(x => x.TenDuAn).ToList()
                        };

            var result = MergeThongKeVoiNguoiDungCoQuyenThamGiaDuAn(query.ToList());
            return result.OrderBy(x => x.TongMucDoHoTro).ThenBy(x => x.TenUser).ToList();
        }

        private List<ThongKeThanhVienGridVo> MergeThongKeVoiNguoiDungCoQuyenThamGiaDuAn(List<ThongKeThanhVienGridVo> thongKes)
        {
            var thongKeTheoUserId = (thongKes ?? new List<ThongKeThanhVienGridVo>())
                .GroupBy(x => x.UserId)
                .ToDictionary(x => x.Key, x => x.First());

            return GetNguoiDungCoQuyenThamGiaDuAn()
                .Select(user =>
                {
                    ThongKeThanhVienGridVo thongKe;
                    if (thongKeTheoUserId.TryGetValue(user.Id, out thongKe))
                    {
                        thongKe.TenUser = user.Name;
                        return thongKe;
                    }

                    return new ThongKeThanhVienGridVo
                    {
                        UserId = user.Id,
                        TenUser = user.Name,
                        SoNhomThamGia = 0,
                        TongMucDoHoTro = 0,
                        DanhSachTenDuAn = new List<string>()
                    };
                })
                .ToList();
        }

        private List<UserGridVo> GetNguoiDungCoQuyenThamGiaDuAn()
        {
            var vaiTroIds = VaiTroAdoNetHelper.GetVaiTriIdCoQuyen(QuyenMoRong.QuyenThamGiaDuAn.ToString());
            if (vaiTroIds == null || vaiTroIds.Count == 0)
            {
                return new List<UserGridVo>();
            }

            return CacheManagerHelper.GetListUserFromCacheNotDeleted()
                .Where(user => user.IsSystemUser != true)
                .Where(user => (user.VaiTroId.HasValue && vaiTroIds.Contains(user.VaiTroId.Value))
                    || ParseIds(user.NhomVaiTroId).Intersect(vaiTroIds).Any())
                .OrderBy(user => user.Name)
                .ToList();
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

        public List<DuAnGridVo> GetListDuAnByUserId(int userId)
        {
            var query = from dt in _context.DuAnThanhViens
                        join d in _context.DuAns on dt.DuAnId equals d.Id
                        where dt.IsDeleted != true && dt.UserId == userId && d.IsDeleted != true && d.IsThuNghiem != true
                        select new DuAnGridVo
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                            MoTa = d.MoTa,
                            IsTruongNhom = dt.IsTruongNhom == true,
                            MucDoHoTro = dt.MucDoHoTro
                        };

            return query.OrderBy(x => x.Ten).ToList();
        }

        public List<DuAnGridVo> GetListDuAnThuNghiemByUserId(int userId)
        {
            var query = from dt in _context.DuAnThanhViens
                        join d in _context.DuAns on dt.DuAnId equals d.Id
                        where dt.IsDeleted != true && dt.UserId == userId && d.IsDeleted != true && d.IsThuNghiem == true
                        select new DuAnGridVo
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                            MoTa = d.MoTa,
                            IsTruongNhom = dt.IsTruongNhom == true,
                            MucDoHoTro = dt.MucDoHoTro
                        };

            return query.OrderBy(x => x.Ten).ToList();
        }

        public List<DuAnGridVo> GetListAllDuAnByUserId(int userId)
        {
            var query = from dt in _context.DuAnThanhViens
                        join d in _context.DuAns on dt.DuAnId equals d.Id
                        where dt.IsDeleted != true && dt.UserId == userId && d.IsDeleted != true
                        select new DuAnGridVo
                        {
                            Id = d.Id,
                            Ten = d.Ten,
                            MoTa = d.MoTa,
                            IsThuNghiem = d.IsThuNghiem,
                            IsTruongNhom = dt.IsTruongNhom == true,
                            MucDoHoTro = dt.MucDoHoTro
                        };

            return query.OrderBy(x => x.Ten).ToList();
        }

        public List<DuAn> GetListAllDuAnActive()
        {
            return _context.DuAns
                .Where(d => d.IsDeleted != true)
                .OrderBy(d => d.Ten)
                .ToList();
        }

        public List<ThongKeDuAnGridVo> ThongKeDuAnTheoSoThanhVien()
        {
            var query = from d in _context.DuAns
                        where d.IsDeleted != true && d.IsThuNghiem != true
                        select new ThongKeDuAnGridVo
                        {
                            DuAnId = d.Id,
                            TenDuAn = d.Ten,
                            MucDoHoTro = d.DuAnThanhViens
                                .Where(dt => dt.IsDeleted != true && dt.UserId.HasValue && dt.MucDoHoTro.HasValue)
                                .Sum(dt => (int?)dt.MucDoHoTro),
                            SoThanhVien = d.DuAnThanhViens.Count(dt => dt.IsDeleted != true && dt.UserId.HasValue),
                            DanhSachTenThanhVien = d.DuAnThanhViens
                                .Where(dt => dt.IsDeleted != true && dt.UserId.HasValue)
                                .Select(dt => dt.User != null ? dt.User.Name : "").ToList()
                        };

            return query.Where(x => x.SoThanhVien > 0).OrderBy(x => x.SoThanhVien).ToList();
        }

        public List<DuAnThanhVienInfo> GetListThanhVienByDuAnId(int duAnId)
        {
            var query = from dt in _context.DuAnThanhViens
                        where dt.IsDeleted != true && dt.DuAnId == duAnId && dt.UserId.HasValue
                        select new DuAnThanhVienInfo
                        {
                            UserId = dt.UserId.Value,
                            TenUser = "",
                            MucDoHoTro = dt.MucDoHoTro,
                            IsTruongNhom = dt.IsTruongNhom == true
                        };

            return query.ToList();
        }

        public List<DuAn> GetListDuAnNotThuNghiem()
        {
            return _context.DuAns.Where(d => d.IsDeleted != true && d.IsThuNghiem != true)
                .OrderBy(d => d.Ten)
                .ToList();
        }

        public List<MatranThanhVienDuAnInfo> GetMatranThanhVienDuAn()
        {
            var query = from dt in _context.DuAnThanhViens
                        join d in _context.DuAns on dt.DuAnId equals d.Id
                        where dt.IsDeleted != true && dt.UserId.HasValue && d.IsDeleted != true && d.IsThuNghiem != true
                        select new MatranThanhVienDuAnInfo
                        {
                            UserId = dt.UserId.Value,
                            DuAnId = (int)dt.DuAnId
                        };

            return query.Distinct().ToList();
        }

        public List<DuAnThanhVienThongKeGridVo> GetDuAnThanhVienThongKe()
        {
            var allThanhVienDuAn = from dt in _context.DuAnThanhViens
                                    join d in _context.DuAns on dt.DuAnId equals d.Id
                                    where dt.IsDeleted != true && dt.UserId.HasValue && d.IsDeleted != true
                                    select new
                                    {
                                        UserId = dt.UserId.Value,
                                        DuAnId = d.Id,
                                        IsThuNghiem = d.IsThuNghiem == true,
                                        IsTruongNhom = dt.IsTruongNhom == true
                                    };

            var groupedData = allThanhVienDuAn
                .GroupBy(x => x.UserId)
                .Select(g => new DuAnThanhVienThongKeGridVo
                {
                    UserId = g.Key,
                    TenUser = "",
                    SoNhomThamGia = g.Count(x => !x.IsThuNghiem),
                    SoDuAnThuNghiem = g.Count(x => x.IsThuNghiem),
                    SoDuAnTruongNhom = g.Count(x => x.IsTruongNhom),
                    DanhSachDuAnThamGia = g.Where(x => !x.IsThuNghiem).Select(x => x.DuAnId).Distinct().ToList(),
                    DanhSachDuAnThuNghiemThamGia = g.Where(x => x.IsThuNghiem).Select(x => x.DuAnId).Distinct().ToList()
                })
                .OrderBy(x => x.TenUser)
                .ToList();

            return groupedData;
        }

        public List<TruongNhomThongKeGridVo> GetTruongNhomThongKe()
        {
            var truongNhomQuery = from dt in _context.DuAnThanhViens
                                  join d in _context.DuAns on dt.DuAnId equals d.Id
                                  where dt.IsDeleted != true && dt.UserId.HasValue && d.IsDeleted != true && dt.IsTruongNhom == true
                                  select new
                                  {
                                      UserId = dt.UserId.Value,
                                      DuAnId = d.Id,
                                      TenDuAn = d.Ten,
                                      IsThuNghiem = d.IsThuNghiem == true,
                                      SoThanhVien = d.DuAnThanhViens.Count(x => x.IsDeleted != true && x.UserId.HasValue)
                                  };

            var groupedData = truongNhomQuery
                .GroupBy(x => x.UserId)
                .Select(g => new TruongNhomThongKeGridVo
                {
                    UserId = g.Key,
                    TenUser = "",
                    SoNhomLamTruongNhom = g.Count(),
                    DanhSachNhom = g.Select(x => new DuAnInfo
                    {
                        DuAnId = x.DuAnId,
                        TenDuAn = x.TenDuAn,
                        SoThanhVien = x.SoThanhVien
                    }).ToList()
                })
                .OrderByDescending(x => x.SoNhomLamTruongNhom)
                .ToList();

            var users = _context.Users.ToList();
            var allThanhViens = _context.DuAnThanhViens
                .Where(dt => dt.IsDeleted != true && dt.UserId.HasValue)
                .ToList();

            foreach (var item in groupedData)
            {
                var user = users.FirstOrDefault(u => u.Id == item.UserId);
                if (user != null)
                {
                    item.TenUser = user.Name;
                }

                foreach (var nhom in item.DanhSachNhom)
                {
                    var thanhVienKhacIds = allThanhViens
                        .Where(dt => dt.DuAnId == nhom.DuAnId && dt.UserId != item.UserId)
                        .Select(dt => dt.UserId.Value)
                        .ToList();

                    nhom.DanhSachThanhVien = thanhVienKhacIds
                        .Select(id =>
                        {
                            var u = users.FirstOrDefault(x => x.Id == id);
                            return new ThanhVienInfo
                            {
                                UserId = id,
                                TenUser = u?.Name ?? ""
                            };
                        })
                        .ToList();
                }
            }

            return groupedData;
        }
    }
}
