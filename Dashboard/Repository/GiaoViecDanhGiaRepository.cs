using Dashboard.AdoNetHelper;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.CommonModel;
using Dashboard.Models.Validates;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace Dashboard.Repository
{
    public interface IGiaoViecDanhGiaRepository
    {
        DataTableResult<GiaoViecThanhVienGridVo> GetListDanhGia(GiaoViecDanhGiaQueryViewModel queryParams);
        ChiTietDanhGiaItemVo GetChiTietDanhGia(int giaoViecThanhVienId);
        dynamic LuuDanhGia(LuuDanhGiaViewModel viewModel);
        int DemSoLuongCongViecChoDanhGia(int userId);
    }

    public class GiaoViecDanhGiaRepository : IGiaoViecDanhGiaRepository
    {
        private readonly IContext _context;
        private readonly string sortStringDefault = "NgayTao desc";
        private readonly List<string> searchCol = new List<string> { "TenCongViec", "NoiDung", "NoiDungNhiemVu" };

        public GiaoViecDanhGiaRepository(IContext context)
        {
            _context = context;
        }

        public DataTableResult<GiaoViecThanhVienGridVo> GetListDanhGia(GiaoViecDanhGiaQueryViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.GiaoViecThanhViens
                .Include(o => o.GiaoViec)
                .Where(o => o.IsDeleted != true
                    && (o.GiaoViec.IsDeleted == null || o.GiaoViec.IsDeleted == false)
                    && (o.GiaoViec.NguoiGiaoId == queryParams.CurrentUserId || o.NguoiTheoDoiChiDaoId == queryParams.CurrentUserId))
                .AsNoTracking()
                .AsQueryable();

            // Lọc theo trạng thái
            if (queryParams.TrangThaiSearch.HasValue)
            {
                int tt = queryParams.TrangThaiSearch.Value;
                if (tt == (int)EnumTrangThaiGiaoViec.ChoDanhGia)
                {
                    query = query.Where(o => o.TrangThai == (int)EnumTrangThaiGiaoViec.ChoDanhGia);
                }
                else if (tt == (int)EnumTrangThaiGiaoViec.HoanThanh)
                {
                    query = query.Where(o => o.TrangThai == (int)EnumTrangThaiGiaoViec.HoanThanh || (o.IsHoanThanh == true && o.TrangThai == null));
                }
                else if (tt == (int)EnumTrangThaiGiaoViec.ChuaDat)
                {
                    query = query.Where(o => o.TrangThai == (int)EnumTrangThaiGiaoViec.ChuaDat);
                }
                else if (tt == (int)EnumTrangThaiGiaoViec.ChuaHoanThanh)
                {
                    query = query.Where(o => o.TrangThai == (int)EnumTrangThaiGiaoViec.ChuaHoanThanh || (o.TrangThai == null && o.IsHoanThanh != true));
                }
            }
            else
            {
                // Mặc định: lấy các nhiệm vụ đang Chờ đánh giá
                query = query.Where(o => o.TrangThai == (int)EnumTrangThaiGiaoViec.ChoDanhGia);
            }

            // Lọc theo khoảng ngày
            if (queryParams.TuNgay.HasValue)
            {
                query = query.Where(o => (o.NgayXacNhanHoanThanh ?? o.GiaoViec.NgayTao) >= queryParams.TuNgay.Value);
            }
            if (queryParams.DenNgay.HasValue)
            {
                query = query.Where(o => (o.NgayXacNhanHoanThanh ?? o.GiaoViec.NgayTao) <= queryParams.DenNgay.Value);
            }

            var projected = query.Select(x => new GiaoViecThanhVienGridVo
            {
                Id = x.Id,
                GiaoViecId = x.GiaoViec.Id,
                TenCongViec = x.GiaoViec.TenCongViec,
                NoiDung = x.GiaoViec.NoiDung,
                GhiChu = x.GiaoViec.GhiChu,
                NgayTao = x.GiaoViec.NgayTao,
                NguoiGiaoId = x.GiaoViec.NguoiGiaoId,
                ThanhVienId = x.ThanhVienId,
                HanXuLy = x.HanXuLy,
                NoiDungNhiemVu = x.NoiDungNhiemVu,
                VaiTroXuLyId = x.VaiTroXuLyId,
                NguoiTheoDoiChiDaoId = x.NguoiTheoDoiChiDaoId,
                TrangThai = x.TrangThai,
                NgayXacNhanHoanThanh = x.NgayXacNhanHoanThanh,
                IsHoanThanh = x.IsHoanThanh,
                NgayHoanThanh = x.NgayHoanThanh,
                KetQuaDanhGia = x.KetQuaDanhGia,
                YKienDanhGia = x.YKienDanhGia,
                NguoiDanhGiaId = x.NguoiDanhGiaId
            }).Where(queryParams.searchString);

            var total = projected.Count();
            var rows = projected.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();

            // Populate cached display names
            foreach (var row in rows)
            {
                if (row.ThanhVienId.GetValueOrDefault() > 0)
                {
                    var u = CacheManagerHelper.GetUserByIdFromCache(row.ThanhVienId.Value);
                    if (u != null) row.ThanhVien = u.Name;
                }
                if (row.NguoiGiaoId.GetValueOrDefault() > 0)
                {
                    var u = CacheManagerHelper.GetUserByIdFromCache(row.NguoiGiaoId.Value);
                    if (u != null) row.NguoiGiao = u.Name;
                }
                if (row.VaiTroXuLyId.GetValueOrDefault() > 0)
                {
                    var vt = CacheManagerHelper.GetVaiTroXuLyByIdFromCache(row.VaiTroXuLyId.Value);
                    if (vt != null) row.TenVaiTroXuLy = vt.Ten;
                }
                if (row.NguoiDanhGiaId.GetValueOrDefault() > 0)
                {
                    var u = CacheManagerHelper.GetUserByIdFromCache(row.NguoiDanhGiaId.Value);
                    if (u != null) row.NguoiDanhGia = u.Name;
                }
                if (row.NguoiTheoDoiChiDaoId.GetValueOrDefault() > 0)
                {
                    var u = CacheManagerHelper.GetUserByIdFromCache(row.NguoiTheoDoiChiDaoId.Value);
                    if (u != null) row.NguoiTheoDoiChiDao = u.Name;
                }
            }

            return new DataTableResult<GiaoViecThanhVienGridVo>
            {
                rows = rows,
                total = total
            };
        }

        public ChiTietDanhGiaItemVo GetChiTietDanhGia(int giaoViecThanhVienId)
        {
            var thanhVien = _context.GiaoViecThanhViens
                .Include(o => o.GiaoViec)
                .AsNoTracking()
                .FirstOrDefault(o => o.Id == giaoViecThanhVienId && o.IsDeleted != true);

            if (thanhVien == null) return null;

            var result = new ChiTietDanhGiaItemVo
            {
                GiaoViecThanhVienId = thanhVien.Id,
                GiaoViecId = thanhVien.GiaoViecId.GetValueOrDefault(),
                TenCongViec = thanhVien.GiaoViec != null ? thanhVien.GiaoViec.TenCongViec : "",
                NoiDung = thanhVien.GiaoViec != null ? thanhVien.GiaoViec.NoiDung : "",
                NguoiGiaoId = thanhVien.GiaoViec != null ? thanhVien.GiaoViec.NguoiGiaoId : null,
                ThanhVienId = thanhVien.ThanhVienId,
                HanXuLyStr = DateTimeHelper.ConvertDateTimeToDateOnly(thanhVien.HanXuLy),
                TrangThai = thanhVien.TrangThai.GetValueOrDefault(),
                NgayXacNhanHoanThanhStr = DateTimeHelper.ConvertDateTimeToDateTime(thanhVien.NgayXacNhanHoanThanh),
                KetQuaDanhGiaGanNhat = thanhVien.KetQuaDanhGia,
                YKienDanhGiaGanNhat = thanhVien.YKienDanhGia,
                NgayDanhGiaGanNhatStr = DateTimeHelper.ConvertDateTimeToDateTime(thanhVien.NgayDanhGia)
            };

            if (result.TrangThai == (int)EnumTrangThaiGiaoViec.HoanThanh)
            {
                result.TenTrangThai = "Đã hoàn thành";
            }
            else if (result.TrangThai == (int)EnumTrangThaiGiaoViec.ChoDanhGia)
            {
                result.TenTrangThai = "Chờ đánh giá";
            }
            else if (result.TrangThai == (int)EnumTrangThaiGiaoViec.ChuaDat)
            {
                result.TenTrangThai = "Chưa đạt";
            }
            else
            {
                result.TenTrangThai = "Chưa hoàn thành";
            }

            if (thanhVien.ThanhVienId.GetValueOrDefault() > 0)
            {
                var u = CacheManagerHelper.GetUserByIdFromCache(thanhVien.ThanhVienId.Value);
                if (u != null) result.ThanhVien = u.Name;
            }
            if (thanhVien.GiaoViec != null && thanhVien.GiaoViec.NguoiGiaoId.GetValueOrDefault() > 0)
            {
                var u = CacheManagerHelper.GetUserByIdFromCache(thanhVien.GiaoViec.NguoiGiaoId.Value);
                if (u != null) result.NguoiGiao = u.Name;
            }
            if (thanhVien.VaiTroXuLyId.GetValueOrDefault() > 0)
            {
                var vt = CacheManagerHelper.GetVaiTroXuLyByIdFromCache(thanhVien.VaiTroXuLyId.Value);
                if (vt != null) result.TenVaiTroXuLy = vt.Ten;
            }
            if (thanhVien.NguoiDanhGiaId.GetValueOrDefault() > 0)
            {
                var u = CacheManagerHelper.GetUserByIdFromCache(thanhVien.NguoiDanhGiaId.Value);
                if (u != null) result.NguoiDanhGiaGanNhat = u.Name;
            }

            // Lấy tiến trình gần nhất (báo cáo hoàn thành)
            var tienTrinhCuoi = _context.GiaoViecThanhVienTienTrinhs
                .Include(o => o.GiaoViecThanhVienTienTrinhFiles)
                .AsNoTracking()
                .Where(o => o.GiaoViecThanhVienId == giaoViecThanhVienId && o.IsDeleted != true)
                .OrderByDescending(o => o.NgayTao)
                .FirstOrDefault();

            if (tienTrinhCuoi != null)
            {
                result.NoiDungBaoCaoCuoi = tienTrinhCuoi.NoiDungXuLy;
                if (tienTrinhCuoi.GiaoViecThanhVienTienTrinhFiles != null)
                {
                    result.FilesBaoCaoCuoi = tienTrinhCuoi.GiaoViecThanhVienTienTrinhFiles
                        .Where(f => f.IsDeleted != true)
                        .Select(f => new GiaoViecThanhVienTienTrinhFileGridVo
                        {
                            Id = f.Id,
                            TenFile = f.TenFile,
                            DuongDan = f.DuongDan
                        }).ToList();
                }
            }

            // Lấy lịch sử đánh giá
            var lichSus = _context.GiaoViecDanhGias
                .AsNoTracking()
                .Where(o => o.GiaoViecThanhVienId == giaoViecThanhVienId && o.IsDeleted != true)
                .OrderByDescending(o => o.NgayDanhGia)
                .ToList();

            foreach (var ls in lichSus)
            {
                var lsVo = new GiaoViecDanhGiaGridVo
                {
                    Id = ls.Id,
                    GiaoViecId = ls.GiaoViecId,
                    GiaoViecThanhVienId = ls.GiaoViecThanhVienId,
                    NguoiDanhGiaId = ls.NguoiDanhGiaId,
                    NgayDanhGia = ls.NgayDanhGia,
                    KetQuaDanhGia = ls.KetQuaDanhGia,
                    YKienDanhGia = ls.YKienDanhGia
                };
                if (ls.NguoiDanhGiaId > 0)
                {
                    var u = CacheManagerHelper.GetUserByIdFromCache(ls.NguoiDanhGiaId);
                    if (u != null) lsVo.NguoiDanhGia = u.Name;
                }
                result.LichSuDanhGias.Add(lsVo);
            }

            return result;
        }

        public dynamic LuuDanhGia(LuuDanhGiaViewModel viewModel)
        {
            var thanhVien = _context.GiaoViecThanhViens
                .Include(o => o.GiaoViec)
                .FirstOrDefault(o => o.Id == viewModel.GiaoViecThanhVienId && o.IsDeleted != true);

            if (thanhVien == null)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "GiaoViecThanhVienId",
                            Description = "Không tìm thấy nhiệm vụ cần đánh giá."
                        }
                    }
                };
            }

            // Quyền đánh giá: Người giao việc hoặc Người theo dõi chỉ đạo
            bool isNguoiGiao = thanhVien.GiaoViec != null && thanhVien.GiaoViec.NguoiGiaoId == viewModel.CurrentUserId;
            bool isNguoiTheoDoi = thanhVien.NguoiTheoDoiChiDaoId.GetValueOrDefault() == viewModel.CurrentUserId;
            var currentUser = CacheManagerHelper.GetUserByIdFromCache(viewModel.CurrentUserId);
            bool isSystem = currentUser != null && currentUser.IsSystemUser == true;

            if (!isNguoiGiao && !isNguoiTheoDoi && !isSystem)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "Permission",
                            Description = "Bạn không có quyền đánh giá nhiệm vụ này."
                        }
                    }
                };
            }

            if (viewModel.KetQuaDanhGia != (int)EnumKetQuaDanhGia.Dat && viewModel.KetQuaDanhGia != (int)EnumKetQuaDanhGia.ChuaDat)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "KetQuaDanhGia",
                            Description = "Vui lòng chọn kết quả đánh giá (Đạt hoặc Chưa đạt)."
                        }
                    }
                };
            }

            if (viewModel.KetQuaDanhGia == (int)EnumKetQuaDanhGia.ChuaDat && string.IsNullOrWhiteSpace(viewModel.YKienDanhGia))
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "YKienDanhGia",
                            Description = "Vui lòng nhập lý do/ý kiến khi đánh giá Chưa đạt để thành viên thực hiện lại."
                        }
                    }
                };
            }

            var now = DateTime.Now;

            // Lưu log lịch sử đánh giá
            var logDanhGia = new GiaoViecDanhGia
            {
                GiaoViecId = thanhVien.GiaoViecId,
                GiaoViecThanhVienId = thanhVien.Id,
                NguoiDanhGiaId = viewModel.CurrentUserId,
                NgayDanhGia = now,
                KetQuaDanhGia = viewModel.KetQuaDanhGia,
                YKienDanhGia = viewModel.YKienDanhGia,
                IsDeleted = false,
                NgayTao = now
            };
            _context.GiaoViecDanhGias.Add(logDanhGia);

            thanhVien.NguoiDanhGiaId = viewModel.CurrentUserId;
            thanhVien.NgayDanhGia = now;
            thanhVien.KetQuaDanhGia = viewModel.KetQuaDanhGia;
            thanhVien.YKienDanhGia = viewModel.YKienDanhGia;

            if (viewModel.KetQuaDanhGia == (int)EnumKetQuaDanhGia.Dat)
            {
                thanhVien.TrangThai = (int)EnumTrangThaiGiaoViec.HoanThanh;
                thanhVien.IsHoanThanh = true;
                if (!thanhVien.NgayHoanThanh.HasValue)
                {
                    thanhVien.NgayHoanThanh = now;
                }
            }
            else
            {
                // Chưa đạt: quay lại trạng thái chưa hoàn thành để thành viên làm lại
                thanhVien.TrangThai = (int)EnumTrangThaiGiaoViec.ChuaDat;
                thanhVien.IsHoanThanh = false;
                thanhVien.NgayHoanThanh = null;
            }

            _context.SaveChanges();

            int giaoViecThanhVienId = thanhVien.Id;
            int giaoViecId = thanhVien.GiaoViecId.GetValueOrDefault();
            int thanhVienId = thanhVien.ThanhVienId.GetValueOrDefault();
            string tenCongViec = thanhVien.GiaoViec != null ? thanhVien.GiaoViec.TenCongViec : "";

            Task.Run(async () =>
            {
                await CapNhatTrangThaiHoanThanhCongViecToanBoAdoNet(giaoViecId);
            });

            // Gửi thông báo cho thành viên thực hiện
            if (thanhVienId > 0)
            {
                string noiDungThongBao;
                if (viewModel.KetQuaDanhGia == (int)EnumKetQuaDanhGia.Dat)
                {
                    noiDungThongBao = $"Nhiệm vụ \"{tenCongViec}\" của bạn đã được đánh giá: ĐẠT.";
                    if (!string.IsNullOrWhiteSpace(viewModel.YKienDanhGia))
                    {
                        noiDungThongBao += $"\nNhận xét: {viewModel.YKienDanhGia}";
                    }
                }
                else
                {
                    noiDungThongBao = $"Nhiệm vụ \"{tenCongViec}\" của bạn được đánh giá: CHƯA ĐẠT.\nLý do: {viewModel.YKienDanhGia}\nVui lòng thực hiện lại.";
                }

                ThongBaoAdoNetHelper.Them(new ThongBao
                {
                    NoiDung = noiDungThongBao,
                    NguoiNhanId = thanhVienId,
                    EnumLoaiThongBao = EnumLoaiThongBao.CongViecMoiCanXuLy,
                    NgayTao = DateTime.Now,
                    IsDaXem = false
                }, BienDungChung.ConnectDBStr);
            }

            return true;
        }

        public int DemSoLuongCongViecChoDanhGia(int userId)
        {
            return _context.GiaoViecThanhViens
                .Where(o => o.IsDeleted != true
                    && (o.GiaoViec.IsDeleted == null || o.GiaoViec.IsDeleted == false)
                    && (o.GiaoViec.NguoiGiaoId == userId || o.NguoiTheoDoiChiDaoId == userId)
                    && o.TrangThai == (int)EnumTrangThaiGiaoViec.ChoDanhGia)
                .Count();
        }

        private static async Task CapNhatTrangThaiHoanThanhCongViecToanBoAdoNet(int giaoViecId)
        {
            if (giaoViecId <= 0) return;

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                await conn.OpenAsync();

                // Lấy tổng số thành viên và số lượng thành viên đã hoàn thành (TrangThai = 2 hoặc IsHoanThanh = 1)
                string sqlCheck = @"
                    SELECT COUNT(1) AS TongSo,
                           SUM(CASE WHEN IsHoanThanh = 1 AND TrangThai = 2 THEN 1 ELSE 0 END) AS SoHoanThanh
                    FROM GiaoViecThanhVien
                    WHERE GiaoViecId = @GiaoViecId AND IsDeleted = 0";

                int tongSo = 0;
                int soHoanThanh = 0;

                using (var cmdCheck = new SqlCommand(sqlCheck, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@GiaoViecId", giaoViecId);
                    using (var reader = await cmdCheck.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            tongSo = reader["TongSo"] != DBNull.Value ? Convert.ToInt32(reader["TongSo"]) : 0;
                            soHoanThanh = reader["SoHoanThanh"] != DBNull.Value ? Convert.ToInt32(reader["SoHoanThanh"]) : 0;
                        }
                    }
                }

                bool isHoanThanhToanBo = tongSo > 0 && tongSo == soHoanThanh;

                using (var cmdUpdate = conn.CreateCommand())
                {
                    cmdUpdate.CommandText = @"UPDATE GiaoViec SET IsHoanThanh = @IsHoanThanh WHERE Id = @Id";
                    cmdUpdate.Parameters.AddWithValue("@IsHoanThanh", isHoanThanhToanBo ? 1 : 0);
                    cmdUpdate.Parameters.AddWithValue("@Id", giaoViecId);
                    await cmdUpdate.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
