using Dashboard.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DiemDanh")]
    public class DiemDanh
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime? NgayNghi { get; set; }
        public int LoaiNghi { get; set; }
        public string LyDo { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class DiemDanhViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string TuNgayStr { get; set; }
        public string DenNgayStr { get; set; }

        public DateTime? TuNgay
        {
            get
            {
                return DateTimeHelper.ConvertStrToDate(TuNgayStr);
            }
        }

        public DateTime? DenNgay
        {
            get
            {
                return DateTimeHelper.ConvertStrToDate(DenNgayStr);
            }
        }

        public DateTime? NgayNghi
        {
            get
            {
                return DateTimeHelper.ConvertStrToDate(NgayNghiStr) ?? TuNgay;
            }
        }
        public string NgayNghiStr { get; set; }
        public int LoaiNghi { get; set; }

        public string LyDo { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class DiemDanhSearchViewModel
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? UserId { get; set; }
        public string FromDateStr { get; set; }
        public DateTime? FromDate { get { return DateTimeHelper.ConvertStrToDate(FromDateStr); } }
        public string ToDateStr { get; set; }
        public DateTime? ToDate { get { return DateTimeHelper.ConvertStrToDate(ToDateStr); } }
        public int? LoaiThongKe { get; set; } // 1 = Nghỉ phép, 2 = Đi trễ, null = Tất cả
    }

    public class DiemDanhReportViewModel
    {
        public int Stt { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int SoLanNghiLe { get; set; }
        public int SoLanNghiCaNgay { get; set; }
        public int SoLanDiTre { get; set; }
        public double TongNgayNghi { get; set; }
    }

    public class DiemDanhSummaryViewModel
    {
        public int TotalHalfDays { get; set; }
        public int TotalFullDays { get; set; }
        public double TotalDaysOff { get; set; }
        public int TotalLateDays { get; set; }
    }

    public class DiemDanhDetailByDateViewModel
    {
        public string NgayNghiStr { get; set; }
        public string ChiTietNghi { get; set; }
        public string LyDo { get; set; }
        public double SoNgayNghi { get; set; }
    }

    public class DiemDanhGridVo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime? NgayNghi { get; set; }
        public string NgayNghiStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(NgayNghi);
            }
        }
        public int LoaiNghi { get; set; }
        public string LyDo { get; set; }
        public bool? IsDeleted { get; set; }

        public string LoaiNghiStr
        {
            get
            {
                switch ((Dashboard.Models.CommonModel.LoaiNghiEnum)LoaiNghi)
                {
                    case Dashboard.Models.CommonModel.LoaiNghiEnum.Sang: return "Sáng";
                    case Dashboard.Models.CommonModel.LoaiNghiEnum.Chieu: return "Chiều";
                    case Dashboard.Models.CommonModel.LoaiNghiEnum.CaNgay: return "Cả ngày";
                    case Dashboard.Models.CommonModel.LoaiNghiEnum.DiTre: return "Đi trễ";
                    default: return "";
                }
            }
        }

        public string UserName { get; set; }
    }
}
