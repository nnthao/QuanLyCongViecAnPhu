using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Repository
{
    public interface IDashboardRepository
    {
        DashboardSummaryVo GetSummaryMetrics(DashboardQueryViewModel query);
        List<DashboardDepartmentStatVo> GetDepartmentStats(DashboardQueryViewModel query);
        List<DashboardRiskDepartmentVo> GetRiskDepartments(DashboardQueryViewModel query);
        List<DashboardRiskMemberVo> GetRiskMembers(DashboardQueryViewModel query);
        List<DashboardTodayScheduleVo> GetTodaySchedule();
        List<DashboardNotificationVo> GetRecentNotifications();
        DataTableResult<DashboardCongViecChiTietVo> GetListChiTietCongViecDashboard(DashboardDetailQueryViewModel query);
    }

    public class DashboardRepository : IDashboardRepository
    {
        private readonly IContext _context;

        public DashboardRepository(IContext context)
        {
            _context = context;
        }

        public DashboardSummaryVo GetSummaryMetrics(DashboardQueryViewModel query)
        {
            var todayStart = DateTime.Now.Date;
            var nearDueEnd = DateTime.Now.Date.AddDays(BienDungChung.SoNgaySapDenHan).AddHours(23).AddMinutes(59).AddSeconds(59);
            var tuNgay = query.TuNgay;
            var denNgay = query.DenNgay;

            var queryable = _context.GiaoViecThanhViens.AsNoTracking()
                .Where(gvtv => !gvtv.IsDeleted
                            && gvtv.VaiTroXuLy != null
                            && !gvtv.VaiTroXuLy.IsDeleted
                            && gvtv.VaiTroXuLy.IsThongKeGiaoViec == true
                            && gvtv.GiaoViec != null
                            && (gvtv.GiaoViec.IsDeleted == null || gvtv.GiaoViec.IsDeleted == false)
                            && (gvtv.GiaoViec.IsTuGiao == null || gvtv.GiaoViec.IsTuGiao == false)
                            && gvtv.GiaoViec.NgayTao >= tuNgay
                            && gvtv.GiaoViec.NgayTao <= denNgay);

            if (query.PhongBanId.GetValueOrDefault() > 0)
            {
                var phongBanId = query.PhongBanId.Value;
                queryable = from gvtv in queryable
                            join u in _context.Users.AsNoTracking() on gvtv.ThanhVienId equals u.Id
                            where u.PhongBanId == phongBanId && (u.IsDeleted == null || u.IsDeleted == false)
                            select gvtv;
            }
            else
            {
                queryable = from gvtv in queryable
                            join u in _context.Users.AsNoTracking() on gvtv.ThanhVienId equals u.Id
                            where (u.IsDeleted == null || u.IsDeleted == false)
                            join pb in _context.PhongBans.AsNoTracking() on u.PhongBanId equals pb.Id
                            where !pb.IsDeleted && (pb.IsPhongNhanViec == null || pb.IsPhongNhanViec == true)
                            select gvtv;
            }

            if (query.IsCaNhan && query.CurrentUserId > 0)
            {
                var currentUserId = query.CurrentUserId;
                queryable = queryable.Where(gvtv => gvtv.ThanhVienId == currentUserId);
            }

            var listData = queryable.Select(gvtv => new
            {
                gvtv.IsHoanThanh,
                gvtv.HanXuLy,
                gvtv.NgayHoanThanh
            }).ToList();

            var result = new DashboardSummaryVo
            {
                TongCongViec = listData.Count,
                DangThucHien = listData.Count(x => (x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value >= todayStart),
                DaHoanThanh = listData.Count(x => x.IsHoanThanh == true),
                SapDenHan = listData.Count(x => (x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value >= todayStart && x.HanXuLy.Value <= nearDueEnd),
                TreHan = listData.Count(x => (x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value < todayStart)
            };

            return result;
        }

        public List<DashboardDepartmentStatVo> GetDepartmentStats(DashboardQueryViewModel query)
        {
            var todayStart = DateTime.Now.Date;
            var nearDueEnd = DateTime.Now.Date.AddDays(BienDungChung.SoNgaySapDenHan).AddHours(23).AddMinutes(59).AddSeconds(59);
            var tuNgay = query.TuNgay;
            var denNgay = query.DenNgay;

            var queryable = from pb in _context.PhongBans.AsNoTracking()
                            where !pb.IsDeleted && (pb.IsPhongNhanViec == null || pb.IsPhongNhanViec == true)
                            join u in _context.Users.AsNoTracking() on pb.Id equals u.PhongBanId into uGroup
                            from u in uGroup.DefaultIfEmpty()
                            where u == null || (u.IsDeleted == null || u.IsDeleted == false)
                            join gvtv in _context.GiaoViecThanhViens.AsNoTracking() on u.Id equals gvtv.ThanhVienId into gvtvGroup
                            from gvtv in gvtvGroup.DefaultIfEmpty()
                            where gvtv == null || (!gvtv.IsDeleted
                                  && gvtv.VaiTroXuLy != null
                                  && !gvtv.VaiTroXuLy.IsDeleted
                                  && gvtv.VaiTroXuLy.IsThongKeGiaoViec == true
                                  && gvtv.GiaoViec != null
                                  && (gvtv.GiaoViec.IsDeleted == null || gvtv.GiaoViec.IsDeleted == false)
                                  && (gvtv.GiaoViec.IsTuGiao == null || gvtv.GiaoViec.IsTuGiao == false)
                                  && gvtv.GiaoViec.NgayTao >= tuNgay
                                  && gvtv.GiaoViec.NgayTao <= denNgay)
                            select new
                            {
                                PhongBanId = pb.Id,
                                TenPhongBan = pb.Ten,
                                ThuTu = pb.ThuTu,
                                GiaoViecThanhVienId = (int?)gvtv.Id,
                                IsHoanThanh = gvtv != null ? gvtv.IsHoanThanh : (bool?)null,
                                HanXuLy = gvtv != null ? gvtv.HanXuLy : (DateTime?)null,
                                ThanhVienId = gvtv != null ? gvtv.ThanhVienId : (int?)null
                            };

            if (query.PhongBanId.GetValueOrDefault() > 0)
            {
                var phongBanId = query.PhongBanId.Value;
                queryable = queryable.Where(x => x.PhongBanId == phongBanId);
            }

            if (query.IsCaNhan && query.CurrentUserId > 0)
            {
                var currentUserId = query.CurrentUserId;
                queryable = queryable.Where(x => x.ThanhVienId == currentUserId);
            }

            var rawList = queryable.ToList();

            var groupResult = rawList.GroupBy(x => new { x.PhongBanId, x.TenPhongBan, x.ThuTu })
                .OrderBy(g => g.Key.ThuTu ?? int.MaxValue)
                .ThenBy(g => g.Key.TenPhongBan)
                .Select(g => new DashboardDepartmentStatVo
                {
                    PhongBanId = g.Key.PhongBanId,
                    TenPhongBan = g.Key.TenPhongBan,
                    DangThucHien = g.Count(x => x.GiaoViecThanhVienId.HasValue && (x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value >= todayStart),
                    DaHoanThanh = g.Count(x => x.GiaoViecThanhVienId.HasValue && x.IsHoanThanh == true),
                    SapDenHan = g.Count(x => x.GiaoViecThanhVienId.HasValue && (x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value >= todayStart && x.HanXuLy.Value <= nearDueEnd),
                    TreHan = g.Count(x => x.GiaoViecThanhVienId.HasValue && (x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value < todayStart),
                    Tong = g.Count(x => x.GiaoViecThanhVienId.HasValue)
                })
                .ToList();

            return groupResult;
        }

        public List<DashboardRiskDepartmentVo> GetRiskDepartments(DashboardQueryViewModel query)
        {
            var todayStart = DateTime.Now.Date;
            var nearDueEnd = DateTime.Now.Date.AddDays(BienDungChung.SoNgaySapDenHan).AddHours(23).AddMinutes(59).AddSeconds(59);
            var tuNgay = query.TuNgay;
            var denNgay = query.DenNgay;

            var queryable = from gvtv in _context.GiaoViecThanhViens.AsNoTracking()
                            where !gvtv.IsDeleted
                               && gvtv.VaiTroXuLy != null
                               && !gvtv.VaiTroXuLy.IsDeleted
                               && gvtv.VaiTroXuLy.IsThongKeGiaoViec == true
                               && gvtv.GiaoViec != null
                               && (gvtv.GiaoViec.IsDeleted == null || gvtv.GiaoViec.IsDeleted == false)
                               && (gvtv.GiaoViec.IsTuGiao == null || gvtv.GiaoViec.IsTuGiao == false)
                               && gvtv.GiaoViec.NgayTao >= tuNgay
                               && gvtv.GiaoViec.NgayTao <= denNgay
                            join u in _context.Users.AsNoTracking() on gvtv.ThanhVienId equals u.Id
                            where u.IsDeleted == null || u.IsDeleted == false
                            join pb in _context.PhongBans.AsNoTracking() on u.PhongBanId equals pb.Id
                            where !pb.IsDeleted && (pb.IsPhongNhanViec == null || pb.IsPhongNhanViec == true)
                            select new
                            {
                                PhongBanId = pb.Id,
                                TenPhongBan = pb.Ten,
                                gvtv.IsHoanThanh,
                                gvtv.HanXuLy,
                                gvtv.ThanhVienId
                            };

            if (query.PhongBanId.GetValueOrDefault() > 0)
            {
                var phongBanId = query.PhongBanId.Value;
                queryable = queryable.Where(x => x.PhongBanId == phongBanId);
            }

            if (query.IsCaNhan && query.CurrentUserId > 0)
            {
                var currentUserId = query.CurrentUserId;
                queryable = queryable.Where(x => x.ThanhVienId == currentUserId);
            }

            var rawList = queryable.ToList();

            var result = rawList.GroupBy(x => new { x.PhongBanId, x.TenPhongBan })
                .Select(g => new DashboardRiskDepartmentVo
                {
                    PhongBanId = g.Key.PhongBanId,
                    TenPhongBan = g.Key.TenPhongBan,
                    SapHanCount = g.Count(x => (x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value >= todayStart && x.HanXuLy.Value <= nearDueEnd),
                    TreHanCount = g.Count(x => (x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value < todayStart),
                    TongCount = g.Count()
                })
                .Where(x => x.SapHanCount > 0 || x.TreHanCount > 0)
                .OrderByDescending(x => x.TreHanCount * 2 + x.SapHanCount)
                .ThenBy(x => x.TenPhongBan)
                .Take(5)
                .ToList();

            return result;
        }

        public List<DashboardRiskMemberVo> GetRiskMembers(DashboardQueryViewModel query)
        {
            var todayStart = DateTime.Now.Date;
            var nearDueEnd = DateTime.Now.Date.AddDays(BienDungChung.SoNgaySapDenHan).AddHours(23).AddMinutes(59).AddSeconds(59);
            var tuNgay = query.TuNgay;
            var denNgay = query.DenNgay;

            var queryable = from gvtv in _context.GiaoViecThanhViens.AsNoTracking()
                            where !gvtv.IsDeleted
                               && gvtv.VaiTroXuLy != null
                               && !gvtv.VaiTroXuLy.IsDeleted
                               && gvtv.VaiTroXuLy.IsThongKeGiaoViec == true
                               && gvtv.GiaoViec != null
                               && (gvtv.GiaoViec.IsDeleted == null || gvtv.GiaoViec.IsDeleted == false)
                               && (gvtv.GiaoViec.IsTuGiao == null || gvtv.GiaoViec.IsTuGiao == false)
                               && gvtv.GiaoViec.NgayTao >= tuNgay
                               && gvtv.GiaoViec.NgayTao <= denNgay
                            join u in _context.Users.AsNoTracking() on gvtv.ThanhVienId equals u.Id
                            where u.IsDeleted == null || u.IsDeleted == false
                            join pb in _context.PhongBans.AsNoTracking() on u.PhongBanId equals pb.Id into pbGroup
                            from pb in pbGroup.DefaultIfEmpty()
                            where pb == null || (!pb.IsDeleted && (pb.IsPhongNhanViec == null || pb.IsPhongNhanViec == true))
                            select new
                            {
                                ThanhVienId = u.Id,
                                TenThanhVien = u.Name,
                                TenPhongBan = pb != null ? pb.Ten : "",
                                PhongBanId = u.PhongBanId,
                                gvtv.IsHoanThanh,
                                gvtv.HanXuLy
                            };

            if (query.PhongBanId.GetValueOrDefault() > 0)
            {
                var phongBanId = query.PhongBanId.Value;
                queryable = queryable.Where(x => x.PhongBanId == phongBanId);
            }

            if (query.IsCaNhan && query.CurrentUserId > 0)
            {
                var currentUserId = query.CurrentUserId;
                queryable = queryable.Where(x => x.ThanhVienId == currentUserId);
            }

            var rawList = queryable.ToList();

            var result = rawList.GroupBy(x => new { x.ThanhVienId, x.TenThanhVien, x.TenPhongBan })
                .Select(g => new DashboardRiskMemberVo
                {
                    ThanhVienId = g.Key.ThanhVienId,
                    TenThanhVien = g.Key.TenThanhVien,
                    TenPhongBan = g.Key.TenPhongBan,
                    DangLamCount = g.Count(x => (x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value >= todayStart),
                    SapHanCount = g.Count(x => (x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value >= todayStart && x.HanXuLy.Value <= nearDueEnd),
                    TreHanCount = g.Count(x => (x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value < todayStart),
                    TongCount = g.Count()
                })
                .OrderByDescending(x => x.TreHanCount * 2 + x.SapHanCount)
                .ThenBy(x => x.TenPhongBan)
                .Take(5)
                .ToList();

            return result;
        }

        public List<DashboardTodayScheduleVo> GetTodaySchedule()
        {
            var now = DateTime.Now;
            var todayStart = now.Date;

            var list = _context.CuocHops.AsNoTracking()
                .Include(ch => ch.NguoiThamGias)
                .Include(ch => ch.PhongBanThamGiaCuocHops)
                .Where(ch => (ch.IsDeleted == null || ch.IsDeleted == false)
                          && (ch.ThoiGianKetThuc.HasValue ? ch.ThoiGianKetThuc.Value >= todayStart : (ch.ThoiGianBatDau.HasValue && ch.ThoiGianBatDau.Value >= todayStart)))
                .OrderBy(ch => ch.ThoiGianBatDau)
                .Take(10)
                .ToList();

            var usersCache = CacheManagerHelper.GetUserCache();
            var phongBansCache = CacheManagerHelper.GetPhongBanCache();

            string[] themes = new[] { "", "teal", "orange" };
            int index = 0;
            return list.Select(ch =>
            {
                var rawName = ch.TenCuocHop ?? "";
                var cleanName = System.Text.RegularExpressions.Regex.Replace(rawName, @"^Ghi\s*nhớ[:\s-]*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();

                string ngayStr = "";
                if (ch.ThoiGianBatDau.HasValue)
                {
                    if (ch.ThoiGianBatDau.Value.Date == todayStart)
                    {
                        ngayStr = "Hôm nay";
                    }
                    else if (ch.ThoiGianBatDau.Value.Date == todayStart.AddDays(1))
                    {
                        ngayStr = "Ngày mai";
                    }
                    else
                    {
                        ngayStr = ch.ThoiGianBatDau.Value.ToString("dd/MM");
                    }
                }

                var thanhPhanList = new List<string>();
                if (!string.IsNullOrWhiteSpace(ch.NguoiThamGiaKhac))
                {
                    thanhPhanList.Add(ch.NguoiThamGiaKhac.Trim());
                }
                if (ch.NguoiThamGias != null && ch.NguoiThamGias.Count > 0 && usersCache != null)
                {
                    var userNames = ch.NguoiThamGias
                        .Where(ntg => ntg.UserId.HasValue)
                        .Select(ntg => usersCache.FirstOrDefault(u => u.Id == ntg.UserId.Value)?.Name)
                        .Where(name => !string.IsNullOrWhiteSpace(name))
                        .ToList();
                    if (userNames.Count > 0)
                    {
                        thanhPhanList.Add(string.Join(", ", userNames));
                    }
                }
                if (ch.PhongBanThamGiaCuocHops != null && ch.PhongBanThamGiaCuocHops.Count > 0 && phongBansCache != null)
                {
                    var pbNames = ch.PhongBanThamGiaCuocHops
                        .Where(pbtg => pbtg.PhongBanId.HasValue)
                        .Select(pbtg => phongBansCache.FirstOrDefault(pb => pb.Id == pbtg.PhongBanId.Value)?.Ten)
                        .Where(name => !string.IsNullOrWhiteSpace(name))
                        .ToList();
                    if (pbNames.Count > 0)
                    {
                        thanhPhanList.Add(string.Join(", ", pbNames));
                    }
                }

                var item = new DashboardTodayScheduleVo
                {
                    Id = ch.Id,
                    TenCuocHop = cleanName,
                    GioBatDauStr = ch.ThoiGianBatDau.HasValue ? ch.ThoiGianBatDau.Value.ToString("HH:mm") : "--:--",
                    GioKetThucStr = ch.ThoiGianKetThuc.HasValue ? ch.ThoiGianKetThuc.Value.ToString("HH:mm") : "--:--",
                    DiaDiem = ch.PhongHopTxt,
                    ThanhPhanThamDu = string.Join("; ", thanhPhanList),
                    ThemeClass = themes[index % themes.Length],
                    NgayStr = ngayStr
                };
                index++;
                return item;
            }).ToList();
        }

        public List<DashboardNotificationVo> GetRecentNotifications()
        {
            var list = _context.ThongBaos.AsNoTracking()
                .Where(tb => tb.IsDeleted == null || tb.IsDeleted == false)
                .OrderByDescending(tb => tb.Id)
                .Take(5)
                .ToList();

            string[] icons = new[] { "fa-exclamation", "fa-user-circle", "fa-calendar-plus-o" };
            int idx = 0;
            return list.Select(tb =>
            {
                var ngayTao = tb.NgayTao;
                string timeAgoStr = "Vừa xong";
                if (ngayTao.HasValue)
                {
                    var diff = DateTime.Now - ngayTao.Value;
                    if (diff.TotalMinutes < 60)
                    {
                        timeAgoStr = $"{(int)Math.Max(1, diff.TotalMinutes)} phút trước";
                    }
                    else if (diff.TotalHours < 24)
                    {
                        timeAgoStr = $"{(int)diff.TotalHours} giờ trước";
                    }
                    else
                    {
                        timeAgoStr = ngayTao.Value.ToString("dd/MM/yyyy HH:mm");
                    }
                }
                var item = new DashboardNotificationVo
                {
                    Id = tb.Id,
                    NoiDung = tb.NoiDung,
                    ThoiGianAgo = timeAgoStr,
                    IconClass = icons[idx % icons.Length]
                };
                idx++;
                return item;
            }).ToList();
        }

        public DataTableResult<DashboardCongViecChiTietVo> GetListChiTietCongViecDashboard(DashboardDetailQueryViewModel query)
        {
            var todayStart = DateTime.Now.Date;
            var nearDueEnd = DateTime.Now.Date.AddDays(BienDungChung.SoNgaySapDenHan).AddHours(23).AddMinutes(59).AddSeconds(59);
            var tuNgay = query.TuNgay;
            var denNgay = query.DenNgay;

            var queryable = from gvtv in _context.GiaoViecThanhViens.AsNoTracking()
                            where !gvtv.IsDeleted
                               && gvtv.VaiTroXuLy != null
                               && !gvtv.VaiTroXuLy.IsDeleted
                               && gvtv.VaiTroXuLy.IsThongKeGiaoViec == true
                               && gvtv.GiaoViec != null
                               && (gvtv.GiaoViec.IsDeleted == null || gvtv.GiaoViec.IsDeleted == false)
                               && (gvtv.GiaoViec.IsTuGiao == null || gvtv.GiaoViec.IsTuGiao == false)
                               && gvtv.GiaoViec.NgayTao >= tuNgay
                               && gvtv.GiaoViec.NgayTao <= denNgay
                            join u in _context.Users.AsNoTracking() on gvtv.ThanhVienId equals u.Id into uGroup
                            from u in uGroup.DefaultIfEmpty()
                            join pb in _context.PhongBans.AsNoTracking() on u.PhongBanId equals pb.Id into pbGroup
                            from pb in pbGroup.DefaultIfEmpty()
                            where pb == null || (!pb.IsDeleted && (pb.IsPhongNhanViec == null || pb.IsPhongNhanViec == true))
                            select new
                            {
                                GiaoViecId = gvtv.GiaoViec.Id,
                                GiaoViecThanhVienId = gvtv.Id,
                                TenCongViec = gvtv.GiaoViec.TenCongViec,
                                NoiDung = gvtv.GiaoViec.NoiDung,
                                PhongBanId = u != null ? u.PhongBanId : (int?)null,
                                TenPhongBan = pb != null ? pb.Ten : "",
                                ThanhVienId = gvtv.ThanhVienId,
                                TenThanhVien = u != null ? u.Name : "",
                                NgayGiaoViec = gvtv.GiaoViec.NgayTao,
                                HanXuLy = gvtv.HanXuLy,
                                NgayHoanThanh = gvtv.NgayHoanThanh,
                                IsHoanThanh = gvtv.IsHoanThanh
                            };

            if (!string.IsNullOrWhiteSpace(query.StatusFilter))
            {
                switch (query.StatusFilter.Trim())
                {
                    case "DangThucHien":
                        queryable = queryable.Where(x => (x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value >= todayStart);
                        break;
                    case "DaHoanThanh":
                        queryable = queryable.Where(x => x.IsHoanThanh == true);
                        break;
                    case "SapDenHan":
                        queryable = queryable.Where(x => (x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value >= todayStart && x.HanXuLy.Value <= nearDueEnd);
                        break;
                    case "TreHan":
                        queryable = queryable.Where(x => ((x.IsHoanThanh == null || x.IsHoanThanh == false) && x.HanXuLy.HasValue && x.HanXuLy.Value < todayStart)
                                                       || (x.IsHoanThanh == true && x.NgayHoanThanh.HasValue && x.HanXuLy.HasValue && DbFunctions.TruncateTime(x.NgayHoanThanh) > DbFunctions.TruncateTime(x.HanXuLy)));
                        break;
                }
            }

            if (query.PhongBanId.GetValueOrDefault() > 0)
            {
                var phongBanId = query.PhongBanId.Value;
                queryable = queryable.Where(x => x.PhongBanId == phongBanId);
            }

            if (query.ThanhVienId.GetValueOrDefault() > 0)
            {
                var thanhVienId = query.ThanhVienId.Value;
                queryable = queryable.Where(x => x.ThanhVienId == thanhVienId);
            }
            else if (query.IsCaNhan && query.CurrentUserId > 0)
            {
                var currentUserId = query.CurrentUserId;
                queryable = queryable.Where(x => x.ThanhVienId == currentUserId);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchText))
            {
                var text = query.SearchText.Trim();
                queryable = queryable.Where(x => x.TenCongViec.Contains(text) || x.NoiDung.Contains(text) || x.TenThanhVien.Contains(text) || x.TenPhongBan.Contains(text));
            }

            int total = queryable.Count();

            var pagedData = queryable.OrderByDescending(x => x.NgayGiaoViec)
                .Skip(query.Offset)
                .Take(query.Limit)
                .ToList();

            var rows = pagedData.Select(item =>
            {
                string trangThaiText = "Đang thực hiện";
                string badgeClass = "badge-primary";

                if (item.IsHoanThanh == true)
                {
                    if (item.NgayHoanThanh.HasValue && item.HanXuLy.HasValue && item.NgayHoanThanh.Value.Date > item.HanXuLy.Value.Date)
                    {
                        trangThaiText = "Hoàn thành trễ hạn";
                        badgeClass = "badge-danger";
                    }
                    else
                    {
                        trangThaiText = "Đã hoàn thành";
                        badgeClass = "badge-success";
                    }
                }
                else
                {
                    if (item.HanXuLy.HasValue && item.HanXuLy.Value.Date < DateTime.Now.Date)
                    {
                        trangThaiText = "Trễ hạn";
                        badgeClass = "badge-danger";
                    }
                    else if (item.HanXuLy.HasValue && item.HanXuLy.Value.Date <= DateTime.Now.Date.AddDays(BienDungChung.SoNgaySapDenHan))
                    {
                        trangThaiText = "Sắp đến hạn";
                        badgeClass = "badge-warning";
                    }
                }

                return new DashboardCongViecChiTietVo
                {
                    GiaoViecId = item.GiaoViecId,
                    GiaoViecThanhVienId = item.GiaoViecThanhVienId,
                    ThanhVienId = item.ThanhVienId,
                    TenCongViec = item.TenCongViec,
                    NoiDung = item.NoiDung,
                    TenPhongBan = item.TenPhongBan,
                    TenThanhVien = item.TenThanhVien,
                    NgayGiaoViecStr = item.NgayGiaoViec.HasValue ? item.NgayGiaoViec.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    HanXuLyStr = item.HanXuLy.HasValue ? item.HanXuLy.Value.ToString("dd/MM/yyyy") : "",
                    NgayHoanThanhStr = item.NgayHoanThanh.HasValue ? item.NgayHoanThanh.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    TrangThaiText = trangThaiText,
                    TrangThaiBadgeClass = badgeClass
                };
            }).ToList();

            return new DataTableResult<DashboardCongViecChiTietVo>
            {
                rows = rows,
                total = total
            };
        }
    }
}
