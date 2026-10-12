using Dashboard.Common;
using Dashboard.Models.ViewModel;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Dashboard.AdoNetHelper
{
    public class GiaoViecTiViAdoNetHelper
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(typeof(GiaoViecTiViAdoNetHelper));

        public static GiaoViecTiViDataVo GetThongTinDataTiVi(GiaoViecTiViQueryViewModel queryViewModel)
        {
            var result = new GiaoViecTiViDataVo();
            var today = DateTime.Today;
            var currentYear = (queryViewModel != null && queryViewModel.Nam.GetValueOrDefault() > 0) ? queryViewModel.Nam.Value : today.Year;
            var tuNgay = new DateTime(currentYear, 1, 1, 0, 0, 0);
            var denNgay = new DateTime(currentYear, 12, 31, 23, 59, 59);

            try
            {
                using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
                {
                    conn.Open();

                    string sql = @"
                        SELECT 
                            gv.Id AS GiaoViecId,
                            gv.TenCongViec,
                            gv.NoiDung,
                            gv.HanXuLy AS GvHanXuLy,
                            gv.NgayTao,
                            gv.IsDeleted AS GvIsDeleted,
                            gv.IsHoanThanh AS GvIsHoanThanh,
                            gv.IsTuGiao,
                            gvtv.Id AS GiaoViecThanhVienId,
                            gvtv.ThanhVienId,
                            u.Name AS TenThanhVien,
                            u.PhongBanId,
                            pb.Ten AS TenPhongBan,
                            gvtv.HanXuLy AS TvHanXuLy,
                            gvtv.IsHoanThanh AS TvIsHoanThanh,
                            gvtv.NgayHoanThanh AS TvNgayHoanThanh,
                            gvtv.IsDeleted AS TvIsDeleted
                        FROM GiaoViecThanhVien gvtv
                        INNER JOIN GiaoViec gv ON gvtv.GiaoViecId = gv.Id 
                                              AND (gv.IsDeleted = 0 OR gv.IsDeleted IS NULL)
                                              AND (gv.IsTuGiao = 0 OR gv.IsTuGiao IS NULL)
                        INNER JOIN VaiTroXuLy vt ON gvtv.VaiTroXuLyId = vt.Id
                                              AND (vt.IsDeleted = 0 OR vt.IsDeleted IS NULL)
                                              AND vt.IsThongKeGiaoViec = 1
                        INNER JOIN Users u ON gvtv.ThanhVienId = u.Id 
                                          AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                        INNER JOIN PhongBan pb ON u.PhongBanId = pb.Id 
                                              AND (pb.IsDeleted = 0 OR pb.IsDeleted IS NULL)
                                              AND (pb.IsPhongNhanViec = 1 OR pb.IsPhongNhanViec IS NULL)
                        WHERE (gvtv.IsDeleted = 0 OR gvtv.IsDeleted IS NULL)
                          AND gv.NgayTao >= @TuNgay 
                          AND gv.NgayTao <= @DenNgay";

                    if (queryViewModel != null && queryViewModel.PhongBanId.GetValueOrDefault() > 0)
                    {
                        sql += " AND u.PhongBanId = @PhongBanId";
                    }

                    var rawItems = new List<RawTaskRow>();

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                        if (queryViewModel != null && queryViewModel.PhongBanId.GetValueOrDefault() > 0)
                        {
                            cmd.Parameters.AddWithValue("@PhongBanId", queryViewModel.PhongBanId.Value);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var gvId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "GiaoViecId");
                                var ten = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenCongViec");
                                var noiDung = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "NoiDung");
                                if (string.IsNullOrWhiteSpace(ten))
                                {
                                    ten = noiDung;
                                }

                                var gvHanXuLy = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "GvHanXuLy");
                                var tvHanXuLy = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "TvHanXuLy");
                                var hanXuLy = tvHanXuLy ?? gvHanXuLy;
                                var ngayTao = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "NgayTao");

                                var thanhVienId = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "ThanhVienId");
                                var phongBanId = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "PhongBanId");
                                var tenThanhVien = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenThanhVien");
                                var tenPhongBan = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenPhongBan");

                                var tvIsHoanThanh = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "TvIsHoanThanh");
                                var gvIsHoanThanh = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "GvIsHoanThanh");
                                var tvNgayHoanThanh = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "TvNgayHoanThanh");
                                var isHoanThanh = tvIsHoanThanh.GetValueOrDefault() || gvIsHoanThanh.GetValueOrDefault();

                                rawItems.Add(new RawTaskRow
                                {
                                    GiaoViecId = gvId,
                                    TenCongViec = ten,
                                    NoiDung = noiDung,
                                    HanXuLy = hanXuLy,
                                    NgayTao = ngayTao,
                                    NgayHoanThanh = tvNgayHoanThanh,
                                    ThanhVienId = thanhVienId,
                                    PhongBanId = phongBanId,
                                    TenThanhVien = tenThanhVien,
                                    TenPhongBan = tenPhongBan,
                                    IsHoanThanh = isHoanThanh
                                });
                            }
                        }
                    }

                    result.TongNhiemVu = rawItems.Count;
                    result.DangThucHien = rawItems.Count(o => !o.IsHoanThanh && (!o.HanXuLy.HasValue || o.HanXuLy.Value.Date >= today));
                    result.HoanThanh = rawItems.Count(o => o.IsHoanThanh);

                    var overdueRaw = rawItems
                        .Where(o => !o.IsHoanThanh && o.HanXuLy.HasValue && o.HanXuLy.Value.Date < today)
                        .OrderBy(o => o.HanXuLy)
                        .ToList();

                    var sttQuaHan = 1;
                    foreach (var item in overdueRaw)
                    {
                        var diffDays = (today - item.HanXuLy.Value.Date).Days;
                        var phuTrachName = !string.IsNullOrWhiteSpace(item.TenThanhVien)
                            ? item.TenThanhVien
                            : (!string.IsNullOrWhiteSpace(item.TenPhongBan) ? item.TenPhongBan : "Chưa phân công");
                        var phongBanName = !string.IsNullOrWhiteSpace(item.TenPhongBan) ? item.TenPhongBan : "Cơ quan";

                        result.DsNhiemVuQuaHan.Add(new GiaoViecTiViItemVo
                        {
                            STT = sttQuaHan,
                            SttStr = sttQuaHan.ToString("D2"),
                            GiaoViecId = item.GiaoViecId,
                            TenCongViec = item.TenCongViec,
                            NoiDung = (!string.IsNullOrWhiteSpace(item.NoiDung) && item.NoiDung != item.TenCongViec) ? item.NoiDung : "",
                            TenPhongBan = phongBanName,
                            TenPhuTrach = phuTrachName,
                            HanXuLy = item.HanXuLy,
                            HanXuLyStr = item.HanXuLy.Value.ToString("dd/MM/yyyy"),
                            SoNgayDiff = diffDays,
                            BadgeText = $"Quá hạn {diffDays} ngày"
                        });
                        sttQuaHan++;
                    }
                    result.QuaHan = result.DsNhiemVuQuaHan.Count;

                    var nearDueEndDate = today.AddDays(BienDungChung.SoNgaySapDenHan);
                    var upcomingRaw = rawItems
                        .Where(o => !o.IsHoanThanh && o.HanXuLy.HasValue && o.HanXuLy.Value.Date >= today && o.HanXuLy.Value.Date <= nearDueEndDate)
                        .OrderBy(o => o.HanXuLy)
                        .ToList();

                    var sttSapDenHan = 1;
                    foreach (var item in upcomingRaw)
                    {
                        var diffDays = (item.HanXuLy.Value.Date - today).Days;
                        var phuTrachName = !string.IsNullOrWhiteSpace(item.TenThanhVien)
                            ? item.TenThanhVien
                            : (!string.IsNullOrWhiteSpace(item.TenPhongBan) ? item.TenPhongBan : "Chưa phân công");
                        var phongBanName = !string.IsNullOrWhiteSpace(item.TenPhongBan) ? item.TenPhongBan : "Cơ quan";

                        string badgeText = diffDays == 0 ? "Hôm nay" : (diffDays == 1 ? "Ngày mai" : $"Còn {diffDays} ngày");

                        result.DsNhiemVuSapDenHan.Add(new GiaoViecTiViItemVo
                        {
                            STT = sttSapDenHan,
                            SttStr = sttSapDenHan.ToString("D2"),
                            GiaoViecId = item.GiaoViecId,
                            TenCongViec = item.TenCongViec,
                            NoiDung = (!string.IsNullOrWhiteSpace(item.NoiDung) && item.NoiDung != item.TenCongViec) ? item.NoiDung : "",
                            TenPhongBan = phongBanName,
                            TenPhuTrach = phuTrachName,
                            HanXuLy = item.HanXuLy,
                            HanXuLyStr = item.HanXuLy.Value.ToString("dd/MM/yyyy"),
                            SoNgayDiff = diffDays,
                            BadgeText = badgeText
                        });
                        sttSapDenHan++;
                    }
                    result.SapDenHan = result.DsNhiemVuSapDenHan.Count;

                    var deptGroups = rawItems
                        .Where(o => o.PhongBanId.HasValue && o.PhongBanId.Value > 0)
                        .GroupBy(o => new { PhongBanId = o.PhongBanId.Value, TenPhongBan = o.TenPhongBan })
                        .Select(g => new
                        {
                            PhongBanId = g.Key.PhongBanId,
                            TenPhongBan = g.Key.TenPhongBan,
                            Count = g.Count()
                        })
                        .OrderByDescending(x => x.Count)
                        .Take(6)
                        .ToList();

                    foreach (var g in deptGroups)
                    {
                        var pbName = !string.IsNullOrWhiteSpace(g.TenPhongBan) ? g.TenPhongBan : $"Phòng {g.PhongBanId}";
                        result.ThongKePhongBan.Add(new GiaoViecTiViPhongBanVo
                        {
                            PhongBanId = g.PhongBanId,
                            TenPhongBan = pbName,
                            SoLuong = g.Count
                        });
                    }

                    if (result.TongNhiemVu > 0)
                    {
                        result.TyLeHoanThanh = Math.Round(((double)result.HoanThanh / result.TongNhiemVu) * 100, 1);
                    }

                    // Lấy Lịch công tác trong ngày từ bảng CuocHop
                    var todayStart = today;
                    var tomorrowStart = today.AddDays(1);
                    string sqlCuocHop = @"
                        SELECT 
                            ch.Id,
                            ch.TenCuocHop,
                            ch.NoiDung,
                            ch.ThoiGianBatDau,
                            ch.ThoiGianKetThuc,
                            ch.PhongHopId,
                            ch.PhongHopTxt,
                            ch.NguoiTaoId,
                            ch.NguoiThamGia,
                            ch.NguoiThamGiaKhac
                        FROM CuocHop ch
                        WHERE (ch.IsDeleted = 0 OR ch.IsDeleted IS NULL)
                          AND (
                              (ch.ThoiGianBatDau >= @TodayStart AND ch.ThoiGianBatDau < @TomorrowStart)
                              OR (ch.ThoiGianKetThuc >= @TodayStart AND ch.ThoiGianKetThuc < @TomorrowStart)
                          )
                        ORDER BY ch.ThoiGianBatDau ASC";

                    var rawCuocHopList = new List<RawCuocHopRow>();

                    using (var cmdCh = new SqlCommand(sqlCuocHop, conn))
                    {
                        cmdCh.Parameters.AddWithValue("@TodayStart", todayStart);
                        cmdCh.Parameters.AddWithValue("@TomorrowStart", tomorrowStart);

                        using (var readerCh = cmdCh.ExecuteReader())
                        {
                            while (readerCh.Read())
                            {
                                var chId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(readerCh, "Id");
                                var rawTen = SqlServeHelper.LayGiaTriCuaCotLaString(readerCh, "TenCuocHop");
                                var cleanTen = System.Text.RegularExpressions.Regex.Replace(rawTen ?? "", @"^Ghi\s*nhớ[:\s-]*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                                var noiDung = SqlServeHelper.LayGiaTriCuaCotLaString(readerCh, "NoiDung");
                                var tgBatDau = SqlServeHelper.LayGiaTriCuaCotLaDateTime(readerCh, "ThoiGianBatDau");
                                var tgKetThuc = SqlServeHelper.LayGiaTriCuaCotLaDateTime(readerCh, "ThoiGianKetThuc");
                                var phongHopId = SqlServeHelper.LayGiaTriCuaCotLaInteger(readerCh, "PhongHopId");
                                var phongHopTxt = SqlServeHelper.LayGiaTriCuaCotLaString(readerCh, "PhongHopTxt");
                                var nguoiTaoId = SqlServeHelper.LayGiaTriCuaCotLaInteger(readerCh, "NguoiTaoId");
                                var rawNguoiThamGia = SqlServeHelper.LayGiaTriCuaCotLaString(readerCh, "NguoiThamGia");
                                var rawNguoiThamGiaKhac = SqlServeHelper.LayGiaTriCuaCotLaString(readerCh, "NguoiThamGiaKhac");

                                rawCuocHopList.Add(new RawCuocHopRow
                                {
                                    Id = chId,
                                    TenCuocHop = !string.IsNullOrWhiteSpace(cleanTen) ? cleanTen : "Lịch công tác",
                                    NoiDung = noiDung,
                                    ThoiGianBatDau = tgBatDau,
                                    ThoiGianKetThuc = tgKetThuc,
                                    PhongHopId = phongHopId,
                                    PhongHopTxt = phongHopTxt,
                                    NguoiTaoId = nguoiTaoId,
                                    NguoiThamGiaRaw = rawNguoiThamGia,
                                    NguoiThamGiaKhac = rawNguoiThamGiaKhac
                                });
                            }
                        }
                    }

                    if (rawCuocHopList.Count > 0)
                    {
                        var allChIds = rawCuocHopList.Select(o => o.Id).ToList();
                        var participantNamesByCuocHop = new Dictionary<int, List<string>>();

                        string sqlNguoiThamGia = $@"
                            SELECT 
                                nt.CuocHopId,
                                u.Name AS TenNguoiDung
                            FROM NguoiThamGia nt
                            INNER JOIN Users u ON nt.UserId = u.Id AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                            WHERE nt.CuocHopId IN ({string.Join(",", allChIds)})
                            UNION ALL
                            SELECT
                                pbtg.CuocHopId,
                                pb.Ten AS TenNguoiDung
                            FROM PhongBanThamGiaCuocHop pbtg
                            INNER JOIN PhongBan pb ON pbtg.PhongBanId = pb.Id AND (pb.IsDeleted = 0 OR pb.IsDeleted IS NULL)
                            WHERE pbtg.CuocHopId IN ({string.Join(",", allChIds)})";

                        try
                        {
                            using (var cmdNt = new SqlCommand(sqlNguoiThamGia, conn))
                            using (var readerNt = cmdNt.ExecuteReader())
                            {
                                while (readerNt.Read())
                                {
                                    var cId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(readerNt, "CuocHopId");
                                    var uName = SqlServeHelper.LayGiaTriCuaCotLaString(readerNt, "TenNguoiDung");
                                    if (cId > 0 && !string.IsNullOrWhiteSpace(uName))
                                    {
                                        if (!participantNamesByCuocHop.ContainsKey(cId))
                                        {
                                            participantNamesByCuocHop[cId] = new List<string>();
                                        }
                                        if (!participantNamesByCuocHop[cId].Contains(uName))
                                        {
                                            participantNamesByCuocHop[cId].Add(uName);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception exNt)
                        {
                            log.Warn($"Không lấy được NguoiThamGia table: {exNt.Message}");
                        }

                        var usersCache = CacheManagerHelper.GetUserCache();
                        var sttLich = 1;

                        foreach (var row in rawCuocHopList)
                        {
                            string thoiGianStr = row.ThoiGianBatDau.HasValue ? row.ThoiGianBatDau.Value.ToString("HH:mm") : "--:--";
                            if (row.ThoiGianKetThuc.HasValue)
                            {
                                thoiGianStr += " - " + row.ThoiGianKetThuc.Value.ToString("HH:mm");
                            }

                            // Xử lý Địa điểm (phòng họp)
                            var listDiaDiem = new List<string>();
                            if (row.PhongHopId.HasValue && row.PhongHopId.Value > 0)
                            {
                                var ph = CacheManagerHelper.GetPhongHopByIdFromCache(row.PhongHopId.Value);
                                if (ph != null && !string.IsNullOrWhiteSpace(ph.TenPhong))
                                {
                                    listDiaDiem.Add(ph.TenPhong);
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(row.PhongHopTxt) && !listDiaDiem.Contains(row.PhongHopTxt))
                            {
                                listDiaDiem.Add(row.PhongHopTxt);
                            }
                            var diaDiemStr = listDiaDiem.Count > 0 ? string.Join("; ", listDiaDiem) : "Tại đơn vị";

                            // Xử lý Thành phần tham dự / Người tham gia
                            var listNguoiThamGia = new List<string>();
                            if (!string.IsNullOrWhiteSpace(row.NguoiThamGiaKhac))
                            {
                                listNguoiThamGia.Add(row.NguoiThamGiaKhac.Trim());
                            }

                            if (listNguoiThamGia.Count == 0 && participantNamesByCuocHop.ContainsKey(row.Id) && participantNamesByCuocHop[row.Id].Count > 0)
                            {
                                listNguoiThamGia.AddRange(participantNamesByCuocHop[row.Id]);
                            }

                            if (listNguoiThamGia.Count == 0 && !string.IsNullOrWhiteSpace(row.NguoiThamGiaRaw))
                            {
                                if (usersCache != null && usersCache.Count > 0)
                                {
                                    var namesFromIds = UltiHelper.TaoTenNguoiDungTuIds(row.NguoiThamGiaRaw, usersCache);
                                    if (!string.IsNullOrWhiteSpace(namesFromIds))
                                    {
                                        listNguoiThamGia.Add(namesFromIds);
                                    }
                                }

                                if (listNguoiThamGia.Count == 0)
                                {
                                    listNguoiThamGia.Add(row.NguoiThamGiaRaw);
                                }
                            }

                            if (listNguoiThamGia.Count == 0 && row.NguoiTaoId.HasValue && row.NguoiTaoId.Value > 0 && usersCache != null)
                            {
                                var creator = usersCache.FirstOrDefault(u => u.Id == row.NguoiTaoId.Value);
                                if (creator != null && !string.IsNullOrWhiteSpace(creator.Name))
                                {
                                    listNguoiThamGia.Add(creator.Name);
                                }
                            }

                            var chuTriStr = listNguoiThamGia.Count > 0 ? string.Join(", ", listNguoiThamGia) : "Toàn thể đơn vị";

                            result.DsLichCongTac.Add(new LichCongTacTiViItemVo
                            {
                                STT = sttLich,
                                SttStr = sttLich.ToString("D2"),
                                Id = row.Id,
                                TenCuocHop = row.TenCuocHop,
                                NoiDung = !string.IsNullOrWhiteSpace(row.NoiDung) ? row.NoiDung : "",
                                ThoiGianStr = thoiGianStr,
                                DiaDiem = diaDiemStr,
                                ChuTriHoacThamGia = chuTriStr,
                                ThanhPhanThamDu = chuTriStr,
                                NguoiThamGiaKhac = row.NguoiThamGiaKhac,
                                ThoiGianBatDau = row.ThoiGianBatDau,
                                ThoiGianKetThuc = row.ThoiGianKetThuc
                            });
                            sttLich++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi lấy dữ liệu TV GiaoViec: {ex.Message}", ex);
            }

            return result;
        }

        private class RawCuocHopRow
        {
            public int Id { get; set; }
            public string TenCuocHop { get; set; }
            public string NoiDung { get; set; }
            public DateTime? ThoiGianBatDau { get; set; }
            public DateTime? ThoiGianKetThuc { get; set; }
            public int? PhongHopId { get; set; }
            public string PhongHopTxt { get; set; }
            public int? NguoiTaoId { get; set; }
            public string NguoiThamGiaRaw { get; set; }
            public string NguoiThamGiaKhac { get; set; }
        }

        private class RawTaskRow
        {
            public int GiaoViecId { get; set; }
            public string TenCongViec { get; set; }
            public string NoiDung { get; set; }
            public DateTime? HanXuLy { get; set; }
            public DateTime? NgayTao { get; set; }
            public DateTime? NgayHoanThanh { get; set; }
            public int? ThanhVienId { get; set; }
            public int? PhongBanId { get; set; }
            public string TenThanhVien { get; set; }
            public string TenPhongBan { get; set; }
            public bool IsHoanThanh { get; set; }
        }
    }
}
