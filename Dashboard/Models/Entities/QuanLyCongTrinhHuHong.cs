using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("CongTrinhHuHong")]
    public partial class CongTrinhHuHong
    {
        public CongTrinhHuHong()
        {
        }

        public int Id { get; set; }
        public string TenCongTrinh { get; set; }
        public string Diadiem { get; set; }
        public int? LoaiCongTrinh { get; set; }
        public string NamXayDung { get; set; }
        public string ChuDauTu { get; set; }
        public string QuyMo { get; set; }
        public string DienTich { get; set; }
        public string ThongSoKiThuat { get; set; }
        public string GhiChu { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }
    }

    [Table("CongTrinhHuHongChiTiet")]
    public partial class CongTrinhHuHongChiTiet
    {
        public CongTrinhHuHongChiTiet()
        {
        }

        public int Id { get; set; }
        public int? CongTrinhHuHongId { get; set; }
        public string TenCongTrinh { get; set; }
        public string Diadiem { get; set; }
        public int? LoaiCongTrinh { get; set; }
        public string NamXayDung { get; set; }
        public string ChuDauTu { get; set; }
        public string QuyMo { get; set; }
        public string DienTich { get; set; }
        public string ThongSoKiThuat { get; set; }
        public string GhiChu { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual CongTrinhHuHong CongTrinhHuHong { get; set; }
    }

    [Table("CongTrinhHuHongFile")]
    public partial class CongTrinhHuHongFile
    {
        public CongTrinhHuHongFile()
        {
        }

        public int Id { get; set; }
        public int? CongTrinhHuHongId { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual CongTrinhHuHong CongTrinhHuHong { get; set; }
    }

    public class QuanLyCongTrinhHuHongViewModel
    {
        public int Id { get; set; }
        public string TenCongTrinh { get; set; }
        public string Diadiem { get; set; }
        public int? LoaiCongTrinh { get; set; }
        public string NamXayDung { get; set; }
        public string ChuDauTu { get; set; }
        public string QuyMo { get; set; }
        public string DienTich { get; set; }
        public string ThongSoKiThuat { get; set; }
        public string GhiChu { get; set; }
    }

    public class QuanLyCongTrinhHuHongGridVo
    {
        public int Id { get; set; }
        public string TenCongTrinh { get; set; }
        public string Diadiem { get; set; }
        public int? LoaiCongTrinh { get; set; }
        public string NamXayDung { get; set; }
        public string ChuDauTu { get; set; }
        public string QuyMo { get; set; }
        public string DienTich { get; set; }
        public string ThongSoKiThuat { get; set; }
        public string GhiChu { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class QuanLyCongTrinhHuHongChiTietViewModel
    {
        public int Id { get; set; }
        public int? CongTrinhHuHongId { get; set; }
        public string TenCongTrinh { get; set; }
        public string Diadiem { get; set; }
        public int? LoaiCongTrinh { get; set; }
        public string NamXayDung { get; set; }
        public string ChuDauTu { get; set; }
        public string QuyMo { get; set; }
        public string DienTich { get; set; }
        public string ThongSoKiThuat { get; set; }
        public string GhiChu { get; set; }
    }

    public class QuanLyCongTrinhHuHongChiTietGridVo
    {
        public int Id { get; set; }
        public int? CongTrinhHuHongId { get; set; }
        public string TenCongTrinh { get; set; }
        public string Diadiem { get; set; }
        public int? LoaiCongTrinh { get; set; }
        public string NamXayDung { get; set; }
        public string ChuDauTu { get; set; }
        public string QuyMo { get; set; }
        public string DienTich { get; set; }
        public string ThongSoKiThuat { get; set; }
        public string GhiChu { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class QuanLyCongTrinhHuHongFileViewModel
    {
        public int Id { get; set; }
        public int? CongTrinhHuHongId { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
    }

    public class QuanLyCongTrinhHuHongFileGridVo
    {
        public int Id { get; set; }
        public int? CongTrinhHuHongId { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
