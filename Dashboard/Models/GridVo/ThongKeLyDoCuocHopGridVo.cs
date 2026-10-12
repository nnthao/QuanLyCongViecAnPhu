using System;
using System.Collections.Generic;

namespace Dashboard.Models.GridVo
{
    public class ThongKeLyDoCuocHopGridVo
    {
        public int LyDoId { get; set; }
        public string TenLyDo { get; set; }
        public string MaMau { get; set; }
        public int SoLuong { get; set; }
    }

    public class ThongKeLyDoCuocHopResponse
    {
        public List<ThongKeLyDoCuocHopGridVo> Data { get; set; }
        public List<string> Labels { get; set; }
        public List<int> SoLuongData { get; set; }
        public List<string> MaMauData { get; set; }
    }

    public class ThongKeNguoiCoNhieuLyDoGridVo
    {
        public int NguoiTaoId { get; set; }
        public string TenNguoi { get; set; }
        public int SoLuong { get; set; }
    }

    public class ChiTietLyDoCuocHopGridVo
    {
        public int CuocHopId { get; set; }
        public string TenPhongBan { get; set; }
        public string TenNguoi { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public string ThoiGianBatDauStr { get; set; }
        public string TenLyDo { get; set; }
    }

    public class ThongKeTop10ThanhVienCoNhieuCongViecGridVo
    {
        public int ThanhVienId { get; set; }
        public string TenThanhVien { get; set; }
        public int? PhongBanId { get; set; }
        public string TenPhongBan { get; set; }
        public int SoLuong { get; set; }
    }
}

