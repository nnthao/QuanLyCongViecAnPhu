using System;
using System.Collections.Generic;
using Dashboard.Models.ViewModel;

namespace Dashboard.Models.ViewModel
{
    public class DashboardQueryViewModel
    {
        public int? Nam { get; set; }
        public int? PhongBanId { get; set; }
        public bool IsCaNhan { get; set; }
        public int CurrentUserId { get; set; }

        public DateTime TuNgay
        {
            get
            {
                int year = Nam.GetValueOrDefault() > 0 ? Nam.Value : DateTime.Now.Year;
                return new DateTime(year, 1, 1, 0, 0, 0);
            }
        }

        public DateTime DenNgay
        {
            get
            {
                int year = Nam.GetValueOrDefault() > 0 ? Nam.Value : DateTime.Now.Year;
                return new DateTime(year, 12, 31, 23, 59, 59);
            }
        }
    }

    public class DashboardSummaryVo
    {
        public int DangThucHien { get; set; }
        public int DaHoanThanh { get; set; }
        public int SapDenHan { get; set; }
        public int TreHan { get; set; }
        public int TongCongViec { get; set; }
    }

    public class DashboardDepartmentStatVo
    {
        public int PhongBanId { get; set; }
        public string TenPhongBan { get; set; }
        public int DangThucHien { get; set; }
        public int DaHoanThanh { get; set; }
        public int SapDenHan { get; set; }
        public int TreHan { get; set; }
        public int Tong { get; set; }

        public double PhantramDangThucHien => Tong > 0 ? Math.Round((double)DangThucHien * 100 / Tong, 1) : 0;
        public double PhantramDaHoanThanh => Tong > 0 ? Math.Round((double)DaHoanThanh * 100 / Tong, 1) : 0;
        public double PhantramSapDenHan => Tong > 0 ? Math.Round((double)SapDenHan * 100 / Tong, 1) : 0;
        public double PhantramTreHan => Tong > 0 ? Math.Round((double)TreHan * 100 / Tong, 1) : 0;
    }

    public class DashboardRiskDepartmentVo
    {
        public int PhongBanId { get; set; }
        public string TenPhongBan { get; set; }
        public int SapHanCount { get; set; }
        public int TreHanCount { get; set; }
        public int TongCount { get; set; }
    }

    public class DashboardRiskMemberVo
    {
        public int ThanhVienId { get; set; }
        public string TenThanhVien { get; set; }
        public string TenPhongBan { get; set; }
        public string AvatarInitials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TenThanhVien)) return "NV";
                var parts = TenThanhVien.Trim().Split(' ');
                if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
                return (parts[parts.Length - 2][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
            }
        }
        public int DangLamCount { get; set; }
        public int SapHanCount { get; set; }
        public int TreHanCount { get; set; }
        public int TongCount { get; set; }
    }

    public class DashboardTodayScheduleVo
    {
        public int Id { get; set; }
        public string GioBatDauStr { get; set; }
        public string GioKetThucStr { get; set; }
        public string TenCuocHop { get; set; }
        public string DiaDiem { get; set; }
        public string ThanhPhanThamDu { get; set; }
        public string ThemeClass { get; set; }
        public string NgayStr { get; set; }
    }

    public class DashboardNotificationVo
    {
        public int Id { get; set; }
        public string NoiDung { get; set; }
        public string ThoiGianAgo { get; set; }
        public string IconClass { get; set; }
    }

    public class DashboardDataResponseVo
    {
        public DashboardSummaryVo Summary { get; set; }
        public List<DashboardDepartmentStatVo> DepartmentStats { get; set; }
        public List<DashboardRiskDepartmentVo> RiskDepartments { get; set; }
        public List<DashboardRiskMemberVo> RiskMembers { get; set; }
        public List<DashboardTodayScheduleVo> TodaySchedule { get; set; }
        public List<DashboardNotificationVo> Notifications { get; set; }

        public DashboardDataResponseVo()
        {
            Summary = new DashboardSummaryVo();
            DepartmentStats = new List<DashboardDepartmentStatVo>();
            RiskDepartments = new List<DashboardRiskDepartmentVo>();
            RiskMembers = new List<DashboardRiskMemberVo>();
            TodaySchedule = new List<DashboardTodayScheduleVo>();
            Notifications = new List<DashboardNotificationVo>();
        }
    }

    public class DashboardDetailQueryViewModel
    {
        public int? Nam { get; set; }
        public int? PhongBanId { get; set; }
        public int? ThanhVienId { get; set; }
        public bool IsCaNhan { get; set; }
        public string StatusFilter { get; set; }
        public string SearchText { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; } = 10;
        public int CurrentUserId { get; set; }

        public DateTime TuNgay
        {
            get
            {
                int year = Nam.GetValueOrDefault() > 0 ? Nam.Value : DateTime.Now.Year;
                return new DateTime(year, 1, 1, 0, 0, 0);
            }
        }

        public DateTime DenNgay
        {
            get
            {
                int year = Nam.GetValueOrDefault() > 0 ? Nam.Value : DateTime.Now.Year;
                return new DateTime(year, 12, 31, 23, 59, 59);
            }
        }
    }

    public class DashboardCongViecChiTietVo
    {
        public int GiaoViecId { get; set; }
        public int GiaoViecThanhVienId { get; set; }
        public int? ThanhVienId { get; set; }
        public string TenCongViec { get; set; }
        public string NoiDung { get; set; }
        public string TenPhongBan { get; set; }
        public string TenThanhVien { get; set; }
        public string NgayGiaoViecStr { get; set; }
        public string HanXuLyStr { get; set; }
        public string NgayHoanThanhStr { get; set; }
        public string TrangThaiText { get; set; }
        public string TrangThaiBadgeClass { get; set; }
    }
}
