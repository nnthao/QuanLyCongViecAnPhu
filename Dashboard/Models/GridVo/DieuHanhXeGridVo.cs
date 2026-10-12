using Dashboard.Common;
using System;
using System.Web.Script.Serialization;

namespace Dashboard.Models.GridVo
{
    public class DieuHanhXeGridVo
    {
        public int Id { get; set; }
        public string TieuDe { get; set; }
        public string NoiDi { get; set; }
        public string NoiDen { get; set; }
        [ScriptIgnore]
        public DateTime? ThoiGianBatDau { get; set; }
        public string ThoiGianBatDauStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(ThoiGianBatDau);
            }
        }
        [ScriptIgnore]
        public DateTime? ThoiGianKetThuc { get; set; }
        public string ThoiGianKetThucStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(ThoiGianKetThuc);
            }
        }
        public string TrangThaiXuLy { get; set; }

        [ScriptIgnore]
        public DateTime NgayTao { get; set; }
        public string NgayTaoStr
        {
            get
            {
                return DateTimeHelper.DateTimeFormatVi(NgayTao);
            }
        }

        public int? SoNguoiDi { get; set; }
        public string LyDoTraPhieu { get; set; }
        public int NguoiTaoId { get; set; }
        public string NguoiTao { get; set; }
        public string CoQuan { get; set; }
        public string GhiChu { get; set; }
        public bool IsShowTraPhieuDaDuyet
        {
            get
            {
                return ThoiGianBatDau > DateTime.Now;
            }
        }

        [ScriptIgnore]
        public DateTime? NgaySua { get; set; }
        public string NgaySuaStr
        {
            get
            {
                return DateTimeHelper.DateTimeFormatVi(NgaySua);
            }
        }

        public string NguoiSua { get; set; }
        public int? LaiXeId { get; set; }
        public string LaiXe { get; set; }
        public int? XeId { get; set; }
        public string Xe { get; set; }
        public string LyDoHuyBoXe { get; set; }
        public int? SoKmKhiDi { get; set; }
        public int? SoKmKhiVe { get; set; }
        public int? SoKmDaDi
        {
            get
            {
                return SoKmKhiVe.GetValueOrDefault() - SoKmKhiDi.GetValueOrDefault();
            }
        }

        public decimal? TongSoXang
        {
            get
            {
                return (SoKmDaDi.GetValueOrDefault() * DinhMucXang) / 100;
            }
        }

        public string DiaDiemDon { get; set; }
        [ScriptIgnore]
        public string SearchStr { get; set; }
        public decimal? DinhMucXang { get; set; }
        public string DinhMucXangStr
        {
            get
            {
                return DinhMucXang.GetValueOrDefault() + " lít/ 100 km";
            }
        }
        public decimal? NhotMay { get; set; }
        public int? LocNhot { get; set; }
        [ScriptIgnore]
        public DateTime? NgayCapNhatTtXang { get; set; }
        public string NgayCapNhatTtXangStr
        {
            get
            {
                return NgayCapNhatTtXang == null ? DateTimeHelper.DateTimeFormatViOnlyDate(DateTime.Now)
                    : DateTimeHelper.DateTimeFormatViOnlyDate(NgayCapNhatTtXang);
            }
        }

        public DateTime? NgayDuyet { get; set; }
        public string NgayDuyetStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(NgayDuyet);
            }
        }
        public int? NguoiDuyetId { get; set; }
        public string NguoiDuyet { get; set; }
        public bool? IsNgoaiTinh { get; set; }
        public int? NguoiDuocGanDuyetId { get; set; }
        public int? CuocHopId { get; set; }
        public string NgayDiStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(ThoiGianBatDau);
            }
        }
        public bool? IsKhuVuc { get; set; }
    }
}