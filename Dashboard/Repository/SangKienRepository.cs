using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;

namespace Dashboard.Repository
{
    public interface ISangKienRepository
    {
        DataTableResult<SangKienGridVo> GetAll(QueryParamViewModel queryParams);
        DataTableResult<SangKienGridVo> GetKetQua(QueryParamViewModel queryParams);
        SangKienViewModel GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(SangKienViewModel sangKienViewModel);
        bool Xoa(int id);
        List<LookupViewModel> SangKienLookup(LookupQueryViewModel lookupQueryViewModel);
        List<FileDinhKemGridVo> GetFileDinhKemById(int id);
        bool CapNhatTrangThai(int id, string trangThai, string lyDo, decimal? tongDiem);
        bool CapNhatTongDiemVaTrangThai(int sangKienId, decimal? tongDiem);
    }

    public class SangKienRepository : ISangKienRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<SangKienViewModel, SangKien>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly ISangKienValidate _sangKienValidate;

        public SangKienRepository(IContext context, ISangKienValidate sangKienValidate)
        {
            _context = context;
            _sangKienValidate = sangKienValidate;
        }

        private readonly string sortStringDefault = "NgayTao desc, Id desc";
        private readonly List<string> searchCol = new List<string> { "TenDotXetDuyet", "MoTaChiTiet", "NamXetDuyet", "TrangThai", "TenNguoiNop" };

        public DataTableResult<SangKienGridVo> GetAll(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.SangKiens.AsNoTracking()
                .GroupJoin(_context.DotXetDuyets.AsNoTracking(),
                    sk => sk.DotXetDuyetId,
                    dot => dot.Id,
                    (sk, dots) => new
                    {
                        SangKien = sk,
                        DotXetDuyet = dots.FirstOrDefault()
                    })
                .GroupJoin(_context.Users.AsNoTracking(),
                    sk => sk.SangKien.NguoiNopId,
                    user => user.Id,
                    (sk, users) => new
                    {
                        sk.SangKien,
                        sk.DotXetDuyet,
                        NguoiNop = users.FirstOrDefault()
                    })
                .GroupJoin(_context.LinhVucSangKiens.AsNoTracking(),
                    sk => sk.SangKien.LinhVucSangKienId,
                    linh => linh.Id,
                    (sk, linhVucs) => new
                    {
                        sk.SangKien,
                        sk.DotXetDuyet,
                        sk.NguoiNop,
                        LinhVuc = linhVucs.FirstOrDefault(),
                        TenNguoiNop = sk.NguoiNop != null ? sk.NguoiNop.Name : string.Empty,
                        TenDotXetDuyet = sk.DotXetDuyet != null ? sk.DotXetDuyet.TenDotXetDuyet : string.Empty,
                        MoTaChiTiet = sk.SangKien.MoTaChiTiet ?? string.Empty,
                        NamXetDuyet = !string.IsNullOrEmpty(sk.SangKien.NamXetDuyet) ? sk.SangKien.NamXetDuyet : (sk.DotXetDuyet != null ? sk.DotXetDuyet.NamXetDuyet : string.Empty),
                        TrangThai = sk.SangKien.TrangThai ?? string.Empty
                    })
                .AsQueryable();

            if (queryParams.DotXetDuyetIdFilter.HasValue && queryParams.DotXetDuyetIdFilter.Value > 0)
            {
                var dotFilter = queryParams.DotXetDuyetIdFilter.Value;
                query = query.Where(o => o.SangKien.DotXetDuyetId == dotFilter);
            }

            if (queryParams.LinhVucSangKienIdFilter.HasValue && queryParams.LinhVucSangKienIdFilter.Value > 0)
            {
                var linhVucFilter = queryParams.LinhVucSangKienIdFilter.Value;
                query = query.Where(o => o.SangKien.LinhVucSangKienId == linhVucFilter);
            }

            if (!string.IsNullOrWhiteSpace(queryParams.NamXetDuyetFilter))
            {
                var namFilter = queryParams.NamXetDuyetFilter.Trim();
                query = query.Where(o => o.SangKien.NamXetDuyet != null && o.SangKien.NamXetDuyet.Contains(namFilter));
            }

            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.SangKien.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.SangKien.IsDeleted != true);
            }

            var queryForSearch = query.Select(o => new
            {
                o.SangKien,
                o.DotXetDuyet,
                o.NguoiNop,
                o.LinhVuc,
                TenNguoiNop = o.NguoiNop != null ? o.NguoiNop.Name : string.Empty,
                TenDotXetDuyet = o.DotXetDuyet != null ? o.DotXetDuyet.TenDotXetDuyet : string.Empty,
                MoTaChiTiet = o.SangKien.MoTaChiTiet ?? string.Empty,
                NamXetDuyet = !string.IsNullOrEmpty(o.SangKien.NamXetDuyet) ? o.SangKien.NamXetDuyet : (o.DotXetDuyet != null ? o.DotXetDuyet.NamXetDuyet : string.Empty),
                TrangThai = o.SangKien.TrangThai ?? string.Empty
            }).AsQueryable();

            var queryData = queryForSearch.Where(queryParams.searchString).ToList();

            if (!string.IsNullOrWhiteSpace(queryParams.TrangThaiDuyetFilter))
            {
                var filterKey = queryParams.TrangThaiDuyetFilter.ToLower();
                if (filterKey == "chuaduyet")
                {
                    filterKey = "choduyet";
                }

                queryData = queryData
                    .Where(o => GetTrangThaiKey(o.SangKien.TrangThai) == filterKey)
                    .ToList();
            }

            var allResults = queryData.Select(o => new SangKienGridVo
            {
                Id = o.SangKien.Id,
                DotXetDuyetId = o.SangKien.DotXetDuyetId,
                TenDotXetDuyet = o.DotXetDuyet != null ? o.DotXetDuyet.TenDotXetDuyet : string.Empty,
                NamXetDuyet = !string.IsNullOrEmpty(o.SangKien.NamXetDuyet) ? o.SangKien.NamXetDuyet : (o.DotXetDuyet != null ? o.DotXetDuyet.NamXetDuyet : string.Empty),
                NguoiNopId = o.SangKien.NguoiNopId,
                TenNguoiNop = o.NguoiNop != null ? o.NguoiNop.Name : string.Empty,
                LinhVucSangKienId = o.SangKien.LinhVucSangKienId,
                TenLinhVuc = o.LinhVuc != null ? o.LinhVuc.TenLinhVuc : string.Empty,
                MoTaChiTiet = o.SangKien.MoTaChiTiet,
                GhiChu = o.SangKien.GhiChu,
                TrangThai = o.SangKien.TrangThai,
                TongDiem = o.SangKien.TongDiemDouble.HasValue ? (decimal?)Convert.ToDecimal(o.SangKien.TongDiemDouble.Value) : null,
                DiemTrungBinhTong = TinhDiemTrungBinhTuTongDiem(o.SangKien.TongDiemDouble),
                IsDeleted = o.SangKien.IsDeleted,
                NgayTao = o.SangKien.NgayTao,
                NgayXoa = o.SangKien.NgayXoa,
                DanhSachGiamKhao = new List<GiamKhaoPhanCongInfo>()
            }).ToList();

            foreach (var item in allResults)
            {
                item.DiemDanhGia = item.DiemTrungBinhTong;
            }

            var total = allResults.Count;

            var result = allResults.AsQueryable().OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();

            if (result.Any())
            {
                var sangKienIds = result.Select(o => o.Id).ToList();
                var allFiles = _context.SangKienFiles.AsNoTracking()
                    .Where(o => sangKienIds.Contains(o.SangKienId) && o.IsDeleted != true)
                    .ToList();

                var fileDict = allFiles.GroupBy(o => o.SangKienId)
                    .ToDictionary(g => g.Key, g => g.Select(f => new FileDinhKemGridVo
                    {
                        Id = f.Id,
                        TenFile = f.TenFile,
                        DuongDan = f.DuongDan
                    }).ToList());

                foreach (var item in result)
                {
                    item.DanhSachFile = fileDict.ContainsKey(item.Id) ? fileDict[item.Id] : new List<FileDinhKemGridVo>();
                }

                var allPhanCong = _context.PhanCongChams.AsNoTracking()
                    .Where(o => sangKienIds.Contains(o.SangKienId) && (o.IsDeleted == null || o.IsDeleted != true))
                    .ToList();

                var allChamDiem = _context.ChamSangKiens.AsNoTracking()
                    .Where(o => sangKienIds.Contains(o.SangKienId))
                    .ToList();

                var phanCongDict = allPhanCong.GroupBy(o => o.SangKienId)
                    .ToDictionary(g => g.Key, g => g.Select(pc => new { pc.UserId, pc.SangKienId }).ToList());

                var chamDiemDict = allChamDiem.GroupBy(o => o.SangKienId)
                    .ToDictionary(g => g.Key, g => g.Select(cd => cd.UserId).ToList());

                var userIds = allPhanCong.Select(o => o.UserId).Distinct().ToList();
                var users = _context.Users.AsNoTracking()
                    .Where(o => userIds.Contains(o.Id))
                    .ToDictionary(u => u.Id, u => u.Name);

                var giamKhaoDict = new Dictionary<int, List<GiamKhaoPhanCongInfo>>();
                foreach (var sangKienId in sangKienIds)
                {
                    var giamKhaoList = new List<GiamKhaoPhanCongInfo>();
                    if (phanCongDict.ContainsKey(sangKienId))
                    {
                        var chamDiemUserIds = chamDiemDict.ContainsKey(sangKienId) ? chamDiemDict[sangKienId] : new List<int>();
                        foreach (var pc in phanCongDict[sangKienId])
                        {
                            giamKhaoList.Add(new GiamKhaoPhanCongInfo
                            {
                                UserId = pc.UserId,
                                TenNguoiCham = users.ContainsKey(pc.UserId) ? users[pc.UserId] : "",
                                DaCham = chamDiemUserIds.Contains(pc.UserId)
                            });
                        }
                    }
                    giamKhaoDict[sangKienId] = giamKhaoList;
                }

                foreach (var item in result)
                {
                    item.DanhSachGiamKhao = giamKhaoDict.ContainsKey(item.Id) ? giamKhaoDict[item.Id] : new List<GiamKhaoPhanCongInfo>();
                }
            }
            else
            {
                foreach (var item in result)
                {
                    item.DanhSachFile = new List<FileDinhKemGridVo>();
                    item.DanhSachGiamKhao = new List<GiamKhaoPhanCongInfo>();
                }
            }

            return new DataTableResult<SangKienGridVo>
            {
                rows = result,
                total = total
            };
        }

        public DataTableResult<SangKienGridVo> GetKetQua(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = "DiemDanhGia desc, NgayTao desc";
            queryParams.searchColumn = new List<string> { "TenDotXetDuyet", "MoTaChiTiet", "NamXetDuyet" };

            var query = _context.SangKiens.AsNoTracking()
                .GroupJoin(_context.DotXetDuyets.AsNoTracking(),
                    sk => sk.DotXetDuyetId,
                    dot => dot.Id,
                    (sk, dots) => new
                    {
                        SangKien = sk,
                        DotXetDuyet = dots.FirstOrDefault()
                    })
                .GroupJoin(_context.LinhVucSangKiens.AsNoTracking(),
                    sk => sk.SangKien.LinhVucSangKienId,
                    linh => linh.Id,
                    (sk, linhVucs) => new
                    {
                        sk.SangKien,
                        sk.DotXetDuyet,
                        LinhVuc = linhVucs.FirstOrDefault()
                    })
                .AsQueryable();

            query = query.Where(o => o.SangKien.IsDeleted != true);

            var queryData = query.Where(queryParams.searchString).ToList();

            var allResults = queryData.Select(o => new SangKienGridVo
            {
                Id = o.SangKien.Id,
                DotXetDuyetId = o.SangKien.DotXetDuyetId,
                TenDotXetDuyet = o.DotXetDuyet != null ? o.DotXetDuyet.TenDotXetDuyet : string.Empty,
                NamXetDuyet = !string.IsNullOrEmpty(o.SangKien.NamXetDuyet) ? o.SangKien.NamXetDuyet : (o.DotXetDuyet != null ? o.DotXetDuyet.NamXetDuyet : string.Empty),
                MoTaChiTiet = o.SangKien.MoTaChiTiet,
                LinhVucSangKienId = o.SangKien.LinhVucSangKienId,
                TenLinhVuc = o.LinhVuc != null ? o.LinhVuc.TenLinhVuc : string.Empty,
                GhiChu = o.SangKien.GhiChu,
                TrangThai = o.SangKien.TrangThai,
                TongDiem = o.SangKien.TongDiemDouble.HasValue ? (decimal?)Convert.ToDecimal(o.SangKien.TongDiemDouble.Value) : null,
                DiemTrungBinhTong = TinhDiemTrungBinhTuTongDiem(o.SangKien.TongDiemDouble),
                IsDeleted = o.SangKien.IsDeleted,
                NgayTao = o.SangKien.NgayTao,
                NgayXoa = o.SangKien.NgayXoa,
                DanhSachGiamKhao = new List<GiamKhaoPhanCongInfo>()
            }).ToList();

            foreach (var item in allResults)
            {
                item.DiemDanhGia = item.DiemTrungBinhTong;
            }

            var dotXetDuyetIds = allResults.Select(o => o.DotXetDuyetId).Distinct().ToList();
            var allPhanThuong = _context.SangKienPhanThuongs.AsNoTracking()
                .Where(o => dotXetDuyetIds.Contains(o.DotXetDuyetId) && (o.IsDeleted == null || o.IsDeleted != true))
                .ToList();

            var sangKienIds = allResults.Select(o => o.Id).ToList();
            var allThanhVien = _context.SangKienThanhViens.AsNoTracking()
                .Where(o => sangKienIds.Contains(o.SangKienId) && (o.IsDeleted == null || o.IsDeleted != true))
                .ToList();

            var userIds = allThanhVien.Select(o => o.UserId).Distinct().ToList();
            var users = _context.Users.AsNoTracking()
                .Where(o => userIds.Contains(o.Id))
                .ToDictionary(o => o.Id, o => o.Name);

            foreach (var item in allResults)
            {
                var loaiSo = item.LoaiSangKienSo;
                if (loaiSo.HasValue)
                {
                    var phanThuong = allPhanThuong.FirstOrDefault(o => o.DotXetDuyetId == item.DotXetDuyetId && o.GiaiThuongXepLoai == loaiSo.Value);
                    item.TienThuong = phanThuong?.TienThuongXepLoai;
                }

                var thanhVienList = allThanhVien.Where(o => o.SangKienId == item.Id).ToList();
                if (thanhVienList.Any() && item.TienThuong.HasValue && item.TienThuong.Value > 0)
                {
                    item.DanhSachChiTietNhanThuong = thanhVienList.Select(tv => new ChiTietNhanThuongInfo
                    {
                        UserId = tv.UserId,
                        HoTen = users.ContainsKey(tv.UserId) ? users[tv.UserId] : "",
                        TiLeDongGop = tv.TiLeDongGop,
                        TienThuong = (int)Math.Round(item.TienThuong.Value * tv.TiLeDongGop / 100m, 0, MidpointRounding.AwayFromZero)
                    }).OrderByDescending(o => o.TiLeDongGop).ToList();
                }
                else
                {
                    item.DanhSachChiTietNhanThuong = new List<ChiTietNhanThuongInfo>();
                }
            }

            var loaiFilter = queryParams.LoaiSangKienFilter;
            if (!string.IsNullOrEmpty(loaiFilter))
            {
                switch (loaiFilter)
                {
                    case "LoaiI":
                        allResults = allResults.Where(o => o.DiemTrungBinhTong.HasValue && o.DiemTrungBinhTong.Value > 75 && o.DiemTrungBinhTong.Value <= 100).ToList();
                        break;
                    case "LoaiII":
                        allResults = allResults.Where(o => o.DiemTrungBinhTong.HasValue && o.DiemTrungBinhTong.Value > 65 && o.DiemTrungBinhTong.Value <= 75).ToList();
                        break;
                    case "LoaiIII":
                        allResults = allResults.Where(o => o.DiemTrungBinhTong.HasValue && o.DiemTrungBinhTong.Value > 55 && o.DiemTrungBinhTong.Value <= 65).ToList();
                        break;
                    case "DatGiai":
                        allResults = allResults.Where(o => o.DiemTrungBinhTong.HasValue && o.DiemTrungBinhTong.Value >= 55).ToList();
                        break;
                    case "KhongDatGiai":
                        allResults = allResults.Where(o => !o.DiemTrungBinhTong.HasValue || o.DiemTrungBinhTong.Value < 55).ToList();
                        break;
                }
            }

            var total = allResults.Count;

            var result = allResults.AsQueryable().OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();

            return new DataTableResult<SangKienGridVo>
            {
                rows = result,
                total = total
            };
        }

        public SangKienViewModel GetById(int id)
        {
            var item = _context.SangKiens.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item == null)
            {
                return new SangKienViewModel();
            }

            var dotXetDuyet = _context.DotXetDuyets.AsNoTracking().FirstOrDefault(o => o.Id == item.DotXetDuyetId);
            var linhVuc = item.LinhVucSangKienId.HasValue
                ? _context.LinhVucSangKiens.AsNoTracking().FirstOrDefault(o => o.Id == item.LinhVucSangKienId.Value)
                : null;

            return new SangKienViewModel
            {
                Id = item.Id,
                DotXetDuyetId = item.DotXetDuyetId,
                TenDotXetDuyet = dotXetDuyet?.TenDotXetDuyet,
                NamXetDuyet = !string.IsNullOrEmpty(item.NamXetDuyet) ? item.NamXetDuyet : dotXetDuyet?.NamXetDuyet,
                MoTaChiTiet = item.MoTaChiTiet,
                GhiChu = item.GhiChu,
                TrangThai = item.TrangThai,
                LinhVucSangKienId = item.LinhVucSangKienId,
                TenLinhVuc = linhVuc?.TenLinhVuc,
                NguoiNopId = item.NguoiNopId
            };
        }

        private string GetTrangThaiKey(string trangThai)
        {
            if (string.IsNullOrWhiteSpace(trangThai))
            {
                return "choduyet";
            }

            var lower = trangThai.ToLower();
            if (lower.Contains("trả duyệt") || lower.Contains("tra duyet"))
            {
                return "traduyet";
            }

            if (lower.Contains("đã duyệt") || lower.Contains("da duyet"))
            {
                return "daduyet";
            }

            if (lower.Contains("chờ duyệt") || lower.Contains("cho duyet") ||
                lower.Contains("chưa duyệt") || lower.Contains("chua duyet"))
            {
                return "choduyet";
            }

            return "choduyet";
        }

        private decimal? TinhDiemTrungBinhTuTongDiem(double? tongDiemDouble)
        {
            if (!tongDiemDouble.HasValue) return null;
            var tongDiemDecimal = Convert.ToDecimal(tongDiemDouble.Value);
            return Math.Round(tongDiemDecimal, 2, MidpointRounding.AwayFromZero);
        }

        private string LayNamXetDuyetTheoDot(int dotXetDuyetId, string currentValue)
        {
            var dotXetDuyet = _context.DotXetDuyets.AsNoTracking().FirstOrDefault(o => o.Id == dotXetDuyetId);
            if (!string.IsNullOrWhiteSpace(dotXetDuyet?.NamXetDuyet))
            {
                return dotXetDuyet.NamXetDuyet.Trim();
            }

            return string.IsNullOrWhiteSpace(currentValue) ? null : currentValue.Trim();
        }

        public dynamic ThemOrCapNhat(SangKienViewModel sangKienViewModel)
        {
            var errors = _sangKienValidate.Validate(sangKienViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            int sangKienId = sangKienViewModel.Id;

            if (sangKienViewModel.Id == 0)
            {
                var item = _iMapper.Map<SangKienViewModel, SangKien>(sangKienViewModel);
                item.NgayTao = DateTime.Now;
                item.IsDeleted = false;
                item.NamXetDuyet = LayNamXetDuyetTheoDot(item.DotXetDuyetId, item.NamXetDuyet);
                item.NguoiNopId = sangKienViewModel.NguoiNopId;
                if (string.IsNullOrWhiteSpace(item.TrangThai))
                {
                    item.TrangThai = "Chờ duyệt";
                }
                AddFileToEntity(item, sangKienViewModel);
                _context.SangKiens.Add(item);
                _context.SaveChanges();
                sangKienId = item.Id;
            }
            else
            {
                var item = _context.SangKiens.Include(o => o.SangKienFiles).FirstOrDefault(o => o.Id == sangKienViewModel.Id);
                if (item == null) throw new Exception("Sáng kiến đã bị xóa");
                _iMapper.Map(sangKienViewModel, item);
                item.NamXetDuyet = LayNamXetDuyetTheoDot(item.DotXetDuyetId, item.NamXetDuyet);
                if (!sangKienViewModel.IsAdmin)
                {
                    _context.Entry(item).Property(x => x.TrangThai).IsModified = false;
                    _context.Entry(item).Property(x => x.TongDiemDouble).IsModified = false;
                }
                _context.Entry(item).Property(x => x.NguoiNopId).IsModified = false;
                AddFileToEntity(item, sangKienViewModel);
                _context.SaveChanges();
            }

            return new { SangKienId = sangKienId };
        }

        public bool Xoa(int id)
        {
            var item = _context.SangKiens.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                item.IsDeleted = true;
                item.NgayXoa = DateTime.Now;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool KhoiPhuc(int id)
        {
            var item = _context.SangKiens.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                item.IsDeleted = false;
                item.NgayXoa = null;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public List<LookupViewModel> SangKienLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var sangKiens = _context.SangKiens.AsNoTracking().AsQueryable();

            var data = new List<LookupViewModel>
            {
                new LookupViewModel
                {
                    id = 0,
                    text = "--- Chọn ---"
                }
            };

            if (!lookupQueryViewModel.isGetAll)
            {
                sangKiens = sangKiens.Where(o => o.IsDeleted != true);
            }

            var result = sangKiens.Select(o => new LookupViewModel
            {
                id = o.Id,
                text = o.MoTaChiTiet
            }).ToList();

            if (lookupQueryViewModel.isHideOptionChonValueZero)
            {
                return result;
            }

            return data.Concat(result).ToList();
        }

        public List<FileDinhKemGridVo> GetFileDinhKemById(int id)
        {
            return _context.SangKienFiles.AsNoTracking()
                .Where(o => o.SangKienId == id && o.IsDeleted != true)
                .Select(o => new FileDinhKemGridVo
                {
                    Id = o.Id,
                    TenFile = o.TenFile,
                    DuongDan = o.DuongDan
                }).ToList();
        }

        public bool CapNhatTrangThai(int id, string trangThai, string lyDo, decimal? tongDiem)
        {
            var item = _context.SangKiens.FirstOrDefault(o => o.Id == id);
            if (item == null) return false;

            item.TrangThai = trangThai;
            if (!string.IsNullOrWhiteSpace(lyDo))
            {
                item.GhiChu = lyDo;
            }
            if (tongDiem.HasValue)
            {
                item.TongDiem = tongDiem;
            }
            _context.SaveChanges();
            return true;
        }

        public bool CapNhatTongDiemVaTrangThai(int sangKienId, decimal? tongDiem)
        {
            var item = _context.SangKiens.FirstOrDefault(o => o.Id == sangKienId);
            if (item == null) return false;
            item.TongDiem = tongDiem;
            _context.SaveChanges();
            return true;
        }

        private void AddFileToEntity(SangKien item, SangKienViewModel sangKienViewModel)
        {
            if (item.SangKienFiles == null)
            {
                item.SangKienFiles = new List<SangKienFile>();
            }

            var uploadPath = HttpContext.Current.Server.MapPath(BienDungChung.UploadsPath);

            var keepIds = sangKienViewModel.TaiLieuDinhKemIdInts ?? new List<int>();

            foreach (var file in item.SangKienFiles.Where(x => x.IsDeleted != true).ToList())
            {
                if (!keepIds.Contains(file.Id))
                {
                    file.IsDeleted = true;
                    file.NgayXoa = DateTime.Now;
                    file.NguoiXoaId = sangKienViewModel.CurrentUserId;
                }
            }

            if (sangKienViewModel.FileDinhKems == null || sangKienViewModel.FileDinhKems.Count == 0)
            {
                return;
            }

            foreach (var taiLieu in sangKienViewModel.FileDinhKems.Where(f => f != null))
            {
                try
                {
                    item.SangKienFiles.Add(new SangKienFile
                    {
                        TenFile = taiLieu.FileName,
                        DuongDan = FileHelper.SaveHttpFilesToUploads(uploadPath, taiLieu),
                        NgayTao = DateTime.Now
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

    }
}

