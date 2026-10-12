using System;
using System.ComponentModel.DataAnnotations;

namespace Dashboard.Models.ViewModel
{
    public class HisThongTinBenhNhanRequestViewModel
    {
        [Required(ErrorMessage = "Ma benh nhan la bat buoc.")]
        public string MaBenhNhan { get; set; }

        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }

        public string Uuid { get; set; }
        public string Cookie { get; set; }

        public int Page { get; set; } = 1;
        public int Rows { get; set; } = 20;

        public string TenBenhNhan { get; set; }
        public string MaChanDoanRaVien { get; set; }
        public string ChanDoanRaVien { get; set; }
        public string MaBenhAn { get; set; }
        public string MaBHYT { get; set; }
        public string MaCap { get; set; }
        public string SoLuuTru { get; set; }
        public string MaLuuTruKhoa { get; set; }
        public string NgaySinh { get; set; }
        public string Khoa { get; set; }
        public string SoVaoVien { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string MaLK { get; set; }
        public string CCCD { get; set; }
        public string TuNgayRaVien { get; set; } = "-1";
        public string DenNgayRaVien { get; set; } = "-1";
    }
}
