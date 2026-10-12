using Dashboard.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DoanVien")]
    public class DoanVien
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public string TenTrim { get; set; }
        public string ChucVu { get; set; }
        public string GhiChu { get; set; }
        public int? DonViDoanId { get; set; }
        public DateTime? ThoiGianDiemDanh { get; set; }
        public int? GioiTinh { get; set; }
        public string QueQuan { get; set; }
        public int? DanTocId { get; set; }
        public string ChuyenMonNghiepVu { get; set; }
        public string LyLuanChinhTri { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string NgayVaoDoan { get; set; }
        public string NgayVaoDang { get; set; }
        public bool? IsDeleted { get; set; }

    }

    public class DoanVienViewModel
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public string TenTrim { get; set; }
        public string ChucVu { get; set; }
        public string GhiChu { get; set; }
        public int? DonViDoanId { get; set; }
        public string ThoiGianDiemDanhStr { get; set; }
        public DateTime? ThoiGianDiemDanh
        {
            get
            {
                return DateTimeHelper.ConvertStrToDateTime(ThoiGianDiemDanhStr);
            }
        }
        public int? GioiTinh { get; set; }
        public string QueQuan { get; set; }
        public int? DanTocId { get; set; }
        public string ChuyenMonNghiepVu { get; set; }
        public string LyLuanChinhTri { get; set; }
        public string NgaySinhStr { get; set; }
        public DateTime? NgaySinh
        {
            get
            {
                return DateTimeHelper.ConvertStrToDateTime(NgaySinhStr);
            }
        }
        public string NgayVaoDoan { get; set; }
        public string NgayVaoDang { get; set; }
    }

    public class DoanVienGridVo
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public string TenTrim { get; set; }
        public string ChucVu { get; set; }
        public string GhiChu { get; set; }
        public int? DonViDoanId { get; set; }
        public string TenDonViDoan { get; set; }
        public DateTime? ThoiGianDiemDanh { get; set; }
        public string ThoiGianDiemDanhStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(ThoiGianDiemDanh);
            }
        }
        public int? GioiTinh { get; set; }
        public string QueQuan { get; set; }
        public int? DanTocId { get; set; }
        public string TenDanToc { get; set; }
        public string ChuyenMonNghiepVu { get; set; }
        public string LyLuanChinhTri { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string NgaySinhStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(NgaySinh);
            }
        }
        public string NgayVaoDoan { get; set; }
        public string NgayVaoDang { get; set; }
        public bool? IsDeleted { get; set; }
    }
}

