using System;
using System.Collections.Generic;

namespace Dashboard.Models.GridVo
{
    public class ThongKeCongViecTheoPhongBanGridVo
    {
        public int PhongBanId { get; set; }
        public string TenPhongBan { get; set; }

        public int SoCongViecHoanThanh
        {
            get
            {
                return SoCongViecHoanThanhDungHan + SoCongViecHoanThanhSomHonHan + SoCongViecHoanThanhTreHan;
            }
        }
        public int SoCongViecHoanThanhDungHan { get; set; }
        public int SoCongViecHoanThanhSomHonHan { get; set; }
        public int SoCongViecHoanThanhTreHan { get; set; }
        public int SoCongViecChuaHoanThanh
        {
            get
            {
                return SoCongViecChuaHoanThanhConHan + SoCongViecChuaHoanThanhTreHan;
            }
        }
        public int SoCongViecChuaHoanThanhConHan { get; set; }
        public int SoCongViecChuaHoanThanhTreHan { get; set; }
        [Obsolete("Sử dụng SoCongViecChuaHoanThanhTreHan thay thế")]
        public int SoCongViecTreHan
        {
            get
            {
                return SoCongViecChuaHoanThanhTreHan;
            }
            set
            {
                SoCongViecChuaHoanThanhTreHan = value;
            }
        }

        public int TongCongViec => SoCongViecHoanThanh + SoCongViecChuaHoanThanh;
        public decimal TiLeHoanThanh
        {
            get
            {
                if (TongCongViec != 0)
                {
                    return Math.Round((decimal)SoCongViecHoanThanh * 100 / TongCongViec, 2);
                }

                return 100;
            }
        }
    }

    public class ThongKeCongViecTheoPhongBanResponse
    {
        public List<ThongKeCongViecTheoPhongBanGridVo> Data { get; set; }
        public List<string> Labels { get; set; }
        public List<int> HoanThanhData { get; set; }
        public List<int> ChuaHoanThanhData { get; set; }

        // Thêm dữ liệu thống kê theo trạng thái
        public ThongKeCongViecTheoTrangThaiResponse TrangThaiData { get; set; }
    }

    public class ThongKeCongViecTheoTrangThaiResponse
    {
        public int HoanThanhSomHonHan { get; set; }
        public int HoanThanhDungHan { get; set; }
        public int HoanThanhTreHan { get; set; }
        public int ChuaHoanThanhConHan { get; set; }
        public int ChuaHoanThanhTreHan { get; set; }

        public int TongCongViec => HoanThanhSomHonHan + HoanThanhDungHan + HoanThanhTreHan + ChuaHoanThanhConHan + ChuaHoanThanhTreHan;
    }
}