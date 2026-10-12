using Dashboard.AdoNetHelper;
using Dashboard.Models.Entities;
using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Common
{
    [DisallowConcurrentExecution]
    public class ThongKeCongViecHangNgayJob : IJob
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(typeof(ThongKeCongViecHangNgayJob));

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var ngayHienTai = DateTime.Now.Date;
                var ngaySapDenHan = ngayHienTai.AddDays(BienDungChung.SoNgaySapDenHan);

                using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
                {
                    await conn.OpenAsync();

                    // 1. Thống kê và gửi thông báo công việc hàng ngày cá nhân cho từng người thực hiện
                    await GuiThongKeChoNguoiThucHienAsync(conn, ngayHienTai, ngaySapDenHan);

                    // 2. Lấy thống kê công việc các phòng ban
                    var danhSachPhongBanThongKe = await LayThongKeCacPhongBanAsync(conn, ngayHienTai, ngaySapDenHan);

                    // 3. Thống kê số lượng công việc trễ hạn, sắp đến hạn của phòng cho lãnh đạo phòng (User có IsLanhDaoPhong = true)
                    await GuiThongKeChoLanhDaoPhongAsync(conn, ngayHienTai, danhSachPhongBanThongKe);

                    // 4. Thống kê công việc trễ hạn, sắp đến hạn của tất cả phòng ban cho lãnh đạo cơ quan (User có quyền NhanTinNhanThongKeCongViecHangNgay)
                    await GuiThongKeChoLanhDaoCoQuanAsync(conn, ngayHienTai, danhSachPhongBanThongKe);
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi thực thi ThongKeCongViecHangNgayJob: {ex.Message}", ex);
            }
        }

        private async Task GuiThongKeChoNguoiThucHienAsync(SqlConnection conn, DateTime ngayHienTai, DateTime ngaySapDenHan)
        {
            try
            {
                var danhSachThanhVienThongKe = new List<ThanhVienThongKeHangNgayItem>();
                string sqlThongKeCaNhan = @"SELECT 
                        u.Id AS UserId,
                        u.Name AS UserName,
                        COALESCE(SUM(CASE 
                            WHEN (gvtv.IsHoanThanh IS NULL OR gvtv.IsHoanThanh = 0) THEN 1 
                            ELSE 0 
                        END), 0) AS SoViecCanLam,
                        COALESCE(SUM(CASE 
                            WHEN (gvtv.IsHoanThanh IS NULL OR gvtv.IsHoanThanh = 0)
                                AND gvtv.HanXuLy IS NOT NULL
                                AND CAST(gvtv.HanXuLy AS DATE) >= @NgayHienTai 
                                AND CAST(gvtv.HanXuLy AS DATE) <= @NgaySapDenHan THEN 1
                            ELSE 0
                        END), 0) AS SoViecSapDenHan,
                        COALESCE(SUM(CASE 
                            WHEN (gvtv.IsHoanThanh IS NULL OR gvtv.IsHoanThanh = 0)
                                AND gvtv.HanXuLy IS NOT NULL
                                AND CAST(gvtv.HanXuLy AS DATE) < @NgayHienTai THEN 1
                            ELSE 0
                        END), 0) AS SoViecQuaHan
                    FROM Users u
                    INNER JOIN GiaoViecThanhVien gvtv ON u.Id = gvtv.ThanhVienId AND gvtv.IsDeleted = 0
                    INNER JOIN VaiTroXuLy vt ON gvtv.VaiTroXuLyId = vt.Id AND (vt.IsDeleted = 0 OR vt.IsDeleted IS NULL) AND vt.IsThongKeGiaoViec = 1
                    INNER JOIN GiaoViec gv ON gvtv.GiaoViecId = gv.Id 
                        AND (gv.IsDeleted = 0 OR gv.IsDeleted IS NULL)
                        AND (gv.IsTuGiao IS NULL OR gv.IsTuGiao = 0)
                    WHERE (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                    GROUP BY u.Id, u.Name
                    HAVING COALESCE(SUM(CASE WHEN (gvtv.IsHoanThanh IS NULL OR gvtv.IsHoanThanh = 0) THEN 1 ELSE 0 END), 0) > 0";

                using (var cmd = new SqlCommand(sqlThongKeCaNhan, conn))
                {
                    cmd.Parameters.AddWithValue("@NgayHienTai", ngayHienTai);
                    cmd.Parameters.AddWithValue("@NgaySapDenHan", ngaySapDenHan);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            danhSachThanhVienThongKe.Add(new ThanhVienThongKeHangNgayItem
                            {
                                UserId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "UserId"),
                                UserName = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "UserName"),
                                SoViecCanLam = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoViecCanLam"),
                                SoViecSapDenHan = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoViecSapDenHan"),
                                SoViecQuaHan = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoViecQuaHan")
                            });
                        }
                    }
                }

                foreach (var item in danhSachThanhVienThongKe)
                {
                    var noiDungCaNhan = $"Thống kê công việc hôm nay ({ngayHienTai:dd/MM/yyyy}): {item.SoViecCanLam} việc cần làm, {item.SoViecSapDenHan} việc gần đến hạn, {item.SoViecQuaHan} việc quá hạn.";

                    await ThongBaoAdoNetHelper.Them(new ThongBao
                    {
                        NoiDung = noiDungCaNhan,
                        NguoiNhanId = item.UserId,
                        EnumLoaiThongBao = EnumLoaiThongBao.ThongKeCongViecHangNgay
                    }, BienDungChung.ConnectDBStr);
                }

                log.Info($"Đã gửi thông báo thống kê công việc hàng ngày cho {danhSachThanhVienThongKe.Count} người thực hiện.");
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi trong GuiThongKeChoNguoiThucHienAsync: {ex.Message}", ex);
            }
        }

        private async Task<List<PhongBanThongKeHangNgayItem>> LayThongKeCacPhongBanAsync(SqlConnection conn, DateTime ngayHienTai, DateTime ngaySapDenHan)
        {
            var danhSachPhongBanThongKe = new List<PhongBanThongKeHangNgayItem>();
            string sqlThongKe = @"SELECT 
                    pb.Id AS PhongBanId,
                    pb.Ten AS TenPhongBan,
                    pb.ThuTu,
                    COALESCE(SUM(CASE 
                        WHEN (gvtv.IsHoanThanh IS NULL OR gvtv.IsHoanThanh = 0)
                            AND gvtv.HanXuLy IS NOT NULL
                            AND CAST(gvtv.HanXuLy AS DATE) < @NgayHienTai THEN 1
                        ELSE 0
                    END), 0) AS SoViecTreHan,
                    COALESCE(SUM(CASE 
                        WHEN (gvtv.IsHoanThanh IS NULL OR gvtv.IsHoanThanh = 0)
                            AND gvtv.HanXuLy IS NOT NULL
                            AND CAST(gvtv.HanXuLy AS DATE) >= @NgayHienTai 
                            AND CAST(gvtv.HanXuLy AS DATE) <= @NgaySapDenHan THEN 1
                        ELSE 0
                    END), 0) AS SoViecSapDenHan
                FROM PhongBan pb
                LEFT JOIN Users u ON pb.Id = u.PhongBanId AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                LEFT JOIN GiaoViecThanhVien gvtv ON u.Id = gvtv.ThanhVienId 
                    AND gvtv.IsDeleted = 0
                    AND gvtv.VaiTroXuLyId IN (SELECT Id FROM VaiTroXuLy WHERE (IsDeleted = 0 OR IsDeleted IS NULL) AND IsThongKeGiaoViec = 1)
                LEFT JOIN GiaoViec gv ON gvtv.GiaoViecId = gv.Id 
                    AND (gv.IsDeleted = 0 OR gv.IsDeleted IS NULL)
                    AND (gv.IsTuGiao IS NULL OR gv.IsTuGiao = 0)
                WHERE pb.IsDeleted = 0 AND (pb.IsPhongNhanViec IS NULL OR pb.IsPhongNhanViec = 1)
                GROUP BY pb.Id, pb.Ten, pb.ThuTu
                ORDER BY ISNULL(pb.ThuTu, 999999) ASC, pb.Ten ASC";

            using (var cmd = new SqlCommand(sqlThongKe, conn))
            {
                cmd.Parameters.AddWithValue("@NgayHienTai", ngayHienTai);
                cmd.Parameters.AddWithValue("@NgaySapDenHan", ngaySapDenHan);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        danhSachPhongBanThongKe.Add(new PhongBanThongKeHangNgayItem
                        {
                            PhongBanId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "PhongBanId"),
                            TenPhongBan = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenPhongBan"),
                            SoViecTreHan = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoViecTreHan"),
                            SoViecSapDenHan = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoViecSapDenHan")
                        });
                    }
                }
            }

            return danhSachPhongBanThongKe;
        }

        private async Task GuiThongKeChoLanhDaoPhongAsync(SqlConnection conn, DateTime ngayHienTai, List<PhongBanThongKeHangNgayItem> danhSachPhongBanThongKe)
        {
            try
            {
                var lanhDaoPhongs = new List<UserThongTinThongKeHangNgay>();
                string sqlLanhDao = @"SELECT u.Id, u.Name, u.PhongBanId, pb.Ten AS TenPhongBan
                    FROM Users u
                    LEFT JOIN PhongBan pb ON u.PhongBanId = pb.Id
                    WHERE (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                        AND u.IsLanhDaoPhong = 1
                        AND u.PhongBanId IS NOT NULL AND u.PhongBanId > 0";

                using (var cmdLanhDao = new SqlCommand(sqlLanhDao, conn))
                using (var readerLanhDao = await cmdLanhDao.ExecuteReaderAsync())
                {
                    while (await readerLanhDao.ReadAsync())
                    {
                        lanhDaoPhongs.Add(new UserThongTinThongKeHangNgay
                        {
                            Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(readerLanhDao, "Id"),
                            Name = SqlServeHelper.LayGiaTriCuaCotLaString(readerLanhDao, "Name"),
                            PhongBanId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(readerLanhDao, "PhongBanId"),
                            TenPhongBan = SqlServeHelper.LayGiaTriCuaCotLaString(readerLanhDao, "TenPhongBan")
                        });
                    }
                }

                foreach (var ld in lanhDaoPhongs)
                {
                    var pbInfo = danhSachPhongBanThongKe.FirstOrDefault(x => x.PhongBanId == ld.PhongBanId);
                    var tenPhong = pbInfo != null ? pbInfo.TenPhongBan : (!string.IsNullOrEmpty(ld.TenPhongBan) ? ld.TenPhongBan : "phòng");
                    var soTreHan = pbInfo != null ? pbInfo.SoViecTreHan : 0;
                    var soSapDenHan = pbInfo != null ? pbInfo.SoViecSapDenHan : 0;

                    string noiDungPhongBan;
                    if (soTreHan > 0 || soSapDenHan > 0)
                    {
                        noiDungPhongBan = $"Thống kê công việc phòng {tenPhong} hôm nay ({ngayHienTai:dd/MM/yyyy}): {soTreHan} việc trễ hạn, {soSapDenHan} việc sắp đến hạn.";
                    }
                    else
                    {
                        noiDungPhongBan = $"Thống kê công việc phòng {tenPhong} hôm nay ({ngayHienTai:dd/MM/yyyy}): Phòng hiện tại không có công việc trễ hạn hoặc sắp đến hạn.";
                    }

                    await ThongBaoAdoNetHelper.Them(new ThongBao
                    {
                        NoiDung = noiDungPhongBan,
                        NguoiNhanId = ld.Id,
                        EnumLoaiThongBao = EnumLoaiThongBao.ThongKeCongViecPhongBan
                    }, BienDungChung.ConnectDBStr);
                }

                log.Info($"Đã gửi thông báo thống kê công việc phòng ban cho {lanhDaoPhongs.Count} lãnh đạo phòng.");
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi trong GuiThongKeChoLanhDaoPhongAsync: {ex.Message}", ex);
            }
        }

        private async Task GuiThongKeChoLanhDaoCoQuanAsync(SqlConnection conn, DateTime ngayHienTai, List<PhongBanThongKeHangNgayItem> danhSachPhongBanThongKe)
        {
            try
            {
                var chiTietPhongBan = new List<string>();
                foreach (var pb in danhSachPhongBanThongKe)
                {
                    if (pb.SoViecTreHan > 0 || pb.SoViecSapDenHan > 0)
                    {
                        chiTietPhongBan.Add($"{pb.TenPhongBan}: {pb.SoViecTreHan} việc trễ hạn, {pb.SoViecSapDenHan} việc sắp đến hạn");
                    }
                }

                string noiDungThongBao;
                if (chiTietPhongBan.Count > 0)
                {
                    noiDungThongBao = $"Thống kê công việc của các phòng hôm nay ({ngayHienTai:dd/MM/yyyy}): {string.Join("; ", chiTietPhongBan)}";
                }
                else
                {
                    noiDungThongBao = $"Thống kê công việc của các phòng hôm nay ({ngayHienTai:dd/MM/yyyy}): Hiện tại các phòng không có công việc trễ hạn hoặc sắp đến hạn";
                }

                var quyenCanKiemTra = QuyenMoRong.NhanTinNhanThongKeCongViecHangNgay.ToString();
                var vaiTroCoQuyenIds = new List<int>();
                string sqlVaiTro = "SELECT Id, PhanQuyenKeys FROM VaiTro WHERE (IsDeleted = 0 OR IsDeleted IS NULL)";
                using (var cmdVaiTro = new SqlCommand(sqlVaiTro, conn))
                using (var readerVaiTro = await cmdVaiTro.ExecuteReaderAsync())
                {
                    while (await readerVaiTro.ReadAsync())
                    {
                        var vtId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(readerVaiTro, "Id");
                        var phanQuyenKeys = SqlServeHelper.LayGiaTriCuaCotLaString(readerVaiTro, "PhanQuyenKeys");
                        if (!string.IsNullOrEmpty(phanQuyenKeys) && phanQuyenKeys.Split(',').Any(x => x.Trim() == quyenCanKiemTra))
                        {
                            vaiTroCoQuyenIds.Add(vtId);
                        }
                    }
                }

                if (vaiTroCoQuyenIds.Count == 0)
                {
                    log.Info("Không có vai trò nào được cấp quyền NhanTinNhanThongKeCongViecHangNgay.");
                    return;
                }

                var users = new List<UserThongTinThongKeHangNgay>();
                string sqlUsers = "SELECT Id, Name, VaiTroId, NhomVaiTroId FROM Users WHERE (IsDeleted = 0 OR IsDeleted IS NULL)";
                using (var cmdUsers = new SqlCommand(sqlUsers, conn))
                using (var readerUsers = await cmdUsers.ExecuteReaderAsync())
                {
                    while (await readerUsers.ReadAsync())
                    {
                        users.Add(new UserThongTinThongKeHangNgay
                        {
                            Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(readerUsers, "Id"),
                            Name = SqlServeHelper.LayGiaTriCuaCotLaString(readerUsers, "Name"),
                            VaiTroId = readerUsers["VaiTroId"] != DBNull.Value ? Convert.ToInt32(readerUsers["VaiTroId"]) : (int?)null,
                            NhomVaiTroId = SqlServeHelper.LayGiaTriCuaCotLaString(readerUsers, "NhomVaiTroId")
                        });
                    }
                }

                var usersNhanThongBao = users.Where(u =>
                    (u.VaiTroId.HasValue && vaiTroCoQuyenIds.Contains(u.VaiTroId.Value)) ||
                    (!string.IsNullOrEmpty(u.NhomVaiTroId) && u.NhomVaiTroId.Split(',')
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Select(x => { int.TryParse(x, out int id); return id; })
                        .Any(id => vaiTroCoQuyenIds.Contains(id)))
                ).ToList();

                if (usersNhanThongBao.Count == 0)
                {
                    log.Info("Không tìm thấy người dùng nào có quyền NhanTinNhanThongKeCongViecHangNgay.");
                    return;
                }

                foreach (var user in usersNhanThongBao)
                {
                    await ThongBaoAdoNetHelper.Them(new ThongBao
                    {
                        NoiDung = noiDungThongBao,
                        NguoiNhanId = user.Id,
                        EnumLoaiThongBao = EnumLoaiThongBao.ThongKeCongViecTatCaPhongBan
                    }, BienDungChung.ConnectDBStr);
                }

                log.Info($"Đã gửi thông báo thống kê công việc của các phòng cho {usersNhanThongBao.Count} lãnh đạo cơ quan.");
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi trong GuiThongKeChoLanhDaoCoQuanAsync: {ex.Message}", ex);
            }
        }
    }

    public class ThanhVienThongKeHangNgayItem
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int SoViecCanLam { get; set; }
        public int SoViecSapDenHan { get; set; }
        public int SoViecQuaHan { get; set; }
    }

    public class PhongBanThongKeHangNgayItem
    {
        public int PhongBanId { get; set; }
        public string TenPhongBan { get; set; }
        public int SoViecTreHan { get; set; }
        public int SoViecSapDenHan { get; set; }
    }

    public class UserThongTinThongKeHangNgay
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? VaiTroId { get; set; }
        public string NhomVaiTroId { get; set; }
        public int? PhongBanId { get; set; }
        public string TenPhongBan { get; set; }
    }
}
