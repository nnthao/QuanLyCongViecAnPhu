using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.CommonModel;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;

namespace Dashboard.Repository
{
    public interface ISangKienCNTTRepository
    {
        DataTableResult<SangKienCNTTGridVo> GetAll(QueryParamViewModel queryParams);
        SangKienCNTTViewModel GetById(int id);
        int ThemOrCapNhat(SangKienCNTTViewModel viewModel);
        void Xoa(int id);
        void KhoiPhuc(int id);
        object GetThongKeTongSoSangKien();
        object GetThongKeXepLoai();
        object GetThongKeQuyTienThuong();
        object GetThongKeTongTienThuongThanhVien();
    }

    public class SangKienCNTTRepository : ISangKienCNTTRepository
    {
        private readonly IContext _context;
        private readonly ISangKienCNTTFileRepository _sangKienCNTTFileRepository;

        public SangKienCNTTRepository(IContext context, ISangKienCNTTFileRepository sangKienCNTTFileRepository)
        {
            _context = context;
            _sangKienCNTTFileRepository = sangKienCNTTFileRepository;
        }

        public DataTableResult<SangKienCNTTGridVo> GetAll(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = "NgayTao desc";
            queryParams.searchColumn = new List<string> { "TenSangKien", "MoTaChiTiet", "TenNguoiNop" };

            var query = _context.SangKienCNTTs.AsNoTracking()
                .GroupJoin(_context.Users.AsNoTracking(),
                    sk => sk.NguoiNopId,
                    user => user.Id,
                    (sk, users) => new
                    {
                        SangKienCNTT = sk,
                        NguoiNop = users.FirstOrDefault()
                    })
                .AsQueryable();

            if (queryParams.DaXoaSearch)
            {
                query = query.Where(o => o.SangKienCNTT.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.SangKienCNTT.IsDeleted != true);
            }

            if (!string.IsNullOrWhiteSpace(queryParams.NamSangKienFilter))
            {
                var namFilter = queryParams.NamSangKienFilter.Trim();
                query = query.Where(o => o.SangKienCNTT.NamSangKien != null && o.SangKienCNTT.NamSangKien.Contains(namFilter));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.XepLoaiSangKienFilter))
            {
                var xepLoaiFilter = queryParams.XepLoaiSangKienFilter.Trim();
                query = query.Where(o => o.SangKienCNTT.XepLoaiSangKien != null && o.SangKienCNTT.XepLoaiSangKien == xepLoaiFilter);
            }

            var queryData = query.Where(queryParams.searchString).ToList();

            var allResults = queryData.Select(o => new SangKienCNTTGridVo
            {
                Id = o.SangKienCNTT.Id,
                NguoiNopId = o.SangKienCNTT.NguoiNopId,
                TenNguoiNop = o.NguoiNop != null ? o.NguoiNop.Name : string.Empty,
                TenSangKien = o.SangKienCNTT.TenSangKien,
                NamSangKien = o.SangKienCNTT.NamSangKien,
                MoTaChiTiet = o.SangKienCNTT.MoTaChiTiet,
                GhiChu = o.SangKienCNTT.GhiChu,
                TrangThai = o.SangKienCNTT.TrangThai,
                XepLoaiSangKien = o.SangKienCNTT.XepLoaiSangKien,
                IsDeleted = o.SangKienCNTT.IsDeleted,
                NgayTao = o.SangKienCNTT.NgayTao,
                NgayXoa = o.SangKienCNTT.NgayXoa,
                DanhSachFile = new List<FileDinhKemGridVo>()
            }).ToList();

            var sangKienIds = allResults.Select(o => o.Id).ToList();
            var allFiles = _context.SangKienCNTTFiles.AsNoTracking()
                .Where(o => sangKienIds.Contains(o.SangKienCNTTId) && (o.IsDeleted == null || o.IsDeleted != true))
                .ToList();

            foreach (var item in allResults)
            {
                item.DanhSachFile = allFiles.Where(o => o.SangKienCNTTId == item.Id)
                    .Select(o => new FileDinhKemGridVo
                    {
                        Id = o.Id,
                        TenFile = o.TenFile,
                        DuongDan = o.DuongDan
                    }).ToList();
            }

            var total = allResults.Count;
            var result = allResults.AsQueryable().OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();

            return new DataTableResult<SangKienCNTTGridVo>
            {
                rows = result,
                total = total
            };
        }

        public SangKienCNTTViewModel GetById(int id)
        {
            var item = _context.SangKienCNTTs.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item == null)
            {
                return new SangKienCNTTViewModel();
            }

            var nguoiNop = item.NguoiNopId.HasValue
                ? _context.Users.AsNoTracking().FirstOrDefault(o => o.Id == item.NguoiNopId.Value)
                : null;

            return new SangKienCNTTViewModel
            {
                Id = item.Id,
                NguoiNopId = item.NguoiNopId,
                TenNguoiNop = nguoiNop?.Name,
                TenSangKien = item.TenSangKien,
                MoTaChiTiet = item.MoTaChiTiet,
                GhiChu = item.GhiChu,
                TrangThai = item.TrangThai,
                TiLeChuTri = item.TiLeChuTri,
                NamSangKien = item.NamSangKien,
                TongTienThuong = item.TongTienThuong,
                XepLoaiSangKien = item.XepLoaiSangKien
            };
        }

        public int ThemOrCapNhat(SangKienCNTTViewModel viewModel)
        {
            if (viewModel.Id == 0)
            {
                var entity = new SangKienCNTT
                {
                    NguoiNopId = viewModel.NguoiNopId,
                    TenSangKien = viewModel.TenSangKien,
                    MoTaChiTiet = viewModel.MoTaChiTiet,
                    GhiChu = viewModel.GhiChu,
                    TrangThai = viewModel.TrangThai ?? "Chờ duyệt",
                    TiLeChuTri = viewModel.TiLeChuTri,
                    NamSangKien = viewModel.NamSangKien,
                    TongTienThuong = viewModel.TongTienThuong,
                    XepLoaiSangKien = viewModel.XepLoaiSangKien,
                    NgayTao = DateTime.Now,
                    IsDeleted = false
                };
                _context.SangKienCNTTs.Add(entity);
                _context.SaveChanges();

                LuuFileDinhKem(entity.Id, viewModel);

                return entity.Id;
            }
            else
            {
                var entity = _context.SangKienCNTTs.FirstOrDefault(o => o.Id == viewModel.Id);
                if (entity != null)
                {
                    entity.TenSangKien = viewModel.TenSangKien;
                    entity.MoTaChiTiet = viewModel.MoTaChiTiet;
                    entity.GhiChu = viewModel.GhiChu;
                    entity.TiLeChuTri = viewModel.TiLeChuTri;
                    entity.NamSangKien = viewModel.NamSangKien;
                    entity.TongTienThuong = viewModel.TongTienThuong;
                    entity.XepLoaiSangKien = viewModel.XepLoaiSangKien;
                    if (!string.IsNullOrEmpty(viewModel.TrangThai))
                    {
                        entity.TrangThai = viewModel.TrangThai;
                    }
                    _context.SaveChanges();

                    XoaFileDinhKemKhongConTrongDanhSach(entity.Id, viewModel.TaiLieuDinhKemIdInts);
                    LuuFileDinhKem(entity.Id, viewModel);
                }

                return viewModel.Id;
            }
        }

        private void LuuFileDinhKem(int sangKienCNTTId, SangKienCNTTViewModel viewModel)
        {
            if (viewModel.FileDinhKems == null || !viewModel.FileDinhKems.Any())
            {
                return;
            }

            var uploadPath = HttpContext.Current.Server.MapPath(BienDungChung.UploadsPath);

            foreach (var file in viewModel.FileDinhKems.Where(f => f != null && f.ContentLength > 0))
            {
                try
                {
                    var filePath = FileHelper.SaveHttpFilesToUploads(uploadPath, file);
                    var sangKienFile = new SangKienCNTTFile
                    {
                        SangKienCNTTId = sangKienCNTTId,
                        TenFile = file.FileName,
                        DuongDan = filePath,
                        NgayTao = DateTime.Now,
                        IsDeleted = false
                    };
                    _context.SangKienCNTTFiles.Add(sangKienFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            _context.SaveChanges();
        }

        private void XoaFileDinhKemKhongConTrongDanhSach(int sangKienCNTTId, List<int> fileIdsGiu)
        {
            var existingFiles = _context.SangKienCNTTFiles
                .Where(o => o.SangKienCNTTId == sangKienCNTTId && (o.IsDeleted == null || o.IsDeleted != true))
                .ToList();

            foreach (var file in existingFiles)
            {
                if (!fileIdsGiu.Contains(file.Id))
                {
                    file.IsDeleted = true;
                    file.NgayXoa = DateTime.Now;
                }
            }
            _context.SaveChanges();
        }

        public void Xoa(int id)
        {
            var entity = _context.SangKienCNTTs.FirstOrDefault(o => o.Id == id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.NgayXoa = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public void KhoiPhuc(int id)
        {
            var entity = _context.SangKienCNTTs.FirstOrDefault(o => o.Id == id);
            if (entity != null)
            {
                entity.IsDeleted = false;
                entity.NgayXoa = null;
                _context.SaveChanges();
            }
        }

        public object GetThongKeTongSoSangKien()
        {
            var data = _context.SangKienCNTTs.AsNoTracking()
                .Where(o => o.IsDeleted != true)
                .GroupBy(o => o.NamSangKien)
                .Select(g => new
                {
                    Nam = g.Key ?? "Chưa có năm",
                    SoLuong = g.Count()
                })
                .OrderBy(o => o.Nam)
                .ToList();

            return new
            {
                labels = data.Select(o => o.Nam).ToList(),
                datasets = new[]
                {
                    new
                    {
                        label = "Tổng số sáng kiến",
                        data = data.Select(o => o.SoLuong).ToList(),
                        backgroundColor = new[] { "#36A2EB", "#FF6384", "#FFCE56", "#4BC0C0", "#9966FF", "#FF9F40", "#FF6384" },
                        borderColor = new[] { "#36A2EB", "#FF6384", "#FFCE56", "#4BC0C0", "#9966FF", "#FF9F40", "#FF6384" },
                        borderWidth = 1
                    }
                }
            };
        }

        public object GetThongKeXepLoai()
        {
            var data = _context.SangKienCNTTs.AsNoTracking()
                .Where(o => o.IsDeleted != true && !string.IsNullOrEmpty(o.XepLoaiSangKien))
                .GroupBy(o => o.XepLoaiSangKien)
                .Select(g => new
                {
                    XepLoai = g.Key,
                    SoLuong = g.Count()
                })
                .ToList();

            var total = data.Sum(o => o.SoLuong);
            var labels = new List<string>();
            var values = new List<int>();
            var colors = new List<string> { "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0" };

            foreach (var item in data)
            {
                labels.Add(item.XepLoai);
                values.Add(item.SoLuong);
            }

            return new
            {
                labels = labels,
                datasets = new[]
                {
                    new
                    {
                        data = values,
                        backgroundColor = colors.Take(labels.Count).ToArray(),
                        borderColor = colors.Take(labels.Count).ToArray(),
                        borderWidth = 1
                    }
                }
            };
        }

        public object GetThongKeQuyTienThuong()
        {
            var data = _context.SangKienCNTTs.AsNoTracking()
                .Where(o => o.IsDeleted != true && o.TongTienThuong.HasValue && o.TongTienThuong.Value > 0)
                .GroupBy(o => o.NamSangKien)
                .Select(g => new
                {
                    Nam = g.Key ?? "Chưa có năm",
                    TongTien = g.Sum(o => o.TongTienThuong.Value)
                })
                .OrderBy(o => o.Nam)
                .ToList();

            return new
            {
                labels = data.Select(o => o.Nam).ToList(),
                datasets = new[]
                {
                    new
                    {
                        label = "Quỹ tiền thưởng (VNĐ)",
                        data = data.Select(o => o.TongTien).ToList(),
                        backgroundColor = "#4BC0C0",
                        borderColor = "#4BC0C0",
                        borderWidth = 1
                    }
                }
            };
        }

        public object GetThongKeTongTienThuongThanhVien()
        {
            var sangKiens = _context.SangKienCNTTs.AsNoTracking()
                .Where(o => o.IsDeleted != true && o.TongTienThuong.HasValue && o.TongTienThuong.Value > 0)
                .ToList();

            var thanhViens = _context.SangKienThanhVienCNTTs.AsNoTracking()
                .Where(o => o.IsDeleted != true)
                .ToList();

            var userTienThuong = new Dictionary<int, Dictionary<string, decimal>>();
            var userNames = new Dictionary<int, string>();

            foreach (var sangKien in sangKiens)
            {
                var nam = sangKien.NamSangKien ?? "Chưa có năm";
                var tongTien = sangKien.TongTienThuong.Value;

                var thanhVienList = thanhViens.Where(o => o.SangKienCNTTId == sangKien.Id).ToList();

                foreach (var thanhVien in thanhVienList)
                {
                    var userId = thanhVien.UserId;
                    var tiLeDongGop = thanhVien.TiLeDongGop;
                    var tienThuong = tongTien * tiLeDongGop / 100;

                    if (!userTienThuong.ContainsKey(userId))
                    {
                        userTienThuong[userId] = new Dictionary<string, decimal>();
                    }

                    if (!userTienThuong[userId].ContainsKey(nam))
                    {
                        userTienThuong[userId][nam] = 0;
                    }

                    userTienThuong[userId][nam] += tienThuong;

                    if (!userNames.ContainsKey(userId))
                    {
                        var user = _context.Users.AsNoTracking().FirstOrDefault(o => o.Id == userId);
                        userNames[userId] = user != null ? user.Name : $"User {userId}";
                    }
                }
            }

            var tongTienTheoUser = userTienThuong.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Values.Sum()
            );

            var topUsers = tongTienTheoUser.OrderByDescending(o => o.Value)
                .Take(10)
                .Select(o => o.Key)
                .ToList();

            var allYears = sangKiens
                .Select(o => o.NamSangKien ?? "Chưa có năm")
                .Distinct()
                .OrderBy(o => o == "Chưa có năm" ? "ZZZ" : o)
                .ToList();

            if (allYears.Count == 0)
            {
                allYears.Add("Chưa có năm");
            }

            var colors = new[] { "#36A2EB", "#FF6384", "#FFCE56", "#4BC0C0", "#9966FF", "#FF9F40", "#FF6384", "#C9CBCF", "#4BC0C0", "#FF6384" };

            var datasets = new List<object>();
            for (int i = 0; i < topUsers.Count; i++)
            {
                var userId = topUsers[i];
                var userName = userNames.ContainsKey(userId) ? userNames[userId] : $"User {userId}";
                var data = new List<decimal>();

                foreach (var nam in allYears)
                {
                    if (userTienThuong.ContainsKey(userId) && userTienThuong[userId].ContainsKey(nam))
                    {
                        data.Add(userTienThuong[userId][nam]);
                    }
                    else
                    {
                        data.Add(0);
                    }
                }

                datasets.Add(new
                {
                    label = userName,
                    data = data,
                    backgroundColor = colors[i % colors.Length],
                    borderColor = colors[i % colors.Length],
                    borderWidth = 1
                });
            }

            return new
            {
                labels = allYears,
                datasets = datasets
            };
        }
    }
}

