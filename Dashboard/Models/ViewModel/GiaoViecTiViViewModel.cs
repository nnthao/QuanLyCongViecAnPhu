using System;
using System.Collections.Generic;

namespace Dashboard.Models.ViewModel
{
    public class GiaoViecTiViQueryViewModel
    {
        public int? Nam { get; set; }
        public int? PhongBanId { get; set; }
        public int? Limit { get; set; }
    }

    public class GiaoViecTiViItemVo
    {
        public int STT { get; set; }
        public string SttStr { get; set; }
        public int GiaoViecId { get; set; }
        public string TenCongViec { get; set; }
        public string NoiDung { get; set; }
        public string TenPhongBan { get; set; }
        public string TenPhuTrach { get; set; }
        public string HanXuLyStr { get; set; }
        public DateTime? HanXuLy { get; set; }
        public int SoNgayDiff { get; set; }
        public string BadgeText { get; set; }
    }

    public class GiaoViecTiViPhongBanVo
    {
        public int PhongBanId { get; set; }
        public string TenPhongBan { get; set; }
        public int SoLuong { get; set; }
    }

    public class LichCongTacTiViItemVo
    {
        public int STT { get; set; }
        public string SttStr { get; set; }
        public int Id { get; set; }
        public string TenCuocHop { get; set; }
        public string NoiDung { get; set; }
        public string ThoiGianStr { get; set; }
        public string DiaDiem { get; set; }
        public string ChuTriHoacThamGia { get; set; }
        public string ThanhPhanThamDu { get; set; }
        public string NguoiThamGiaKhac { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
    }

    public class GiaoViecTiViDataVo
    {
        public int TongNhiemVu { get; set; }
        public int DangThucHien { get; set; }
        public int QuaHan { get; set; }
        public int SapDenHan { get; set; }
        public int HoanThanh { get; set; }
        public double TyLeHoanThanh { get; set; }
        public List<GiaoViecTiViItemVo> DsNhiemVuQuaHan { get; set; }
        public List<GiaoViecTiViItemVo> DsNhiemVuSapDenHan { get; set; }
        public List<LichCongTacTiViItemVo> DsLichCongTac { get; set; }
        public List<GiaoViecTiViPhongBanVo> ThongKePhongBan { get; set; }

        public GiaoViecTiViDataVo()
        {
            DsNhiemVuQuaHan = new List<GiaoViecTiViItemVo>();
            DsNhiemVuSapDenHan = new List<GiaoViecTiViItemVo>();
            DsLichCongTac = new List<LichCongTacTiViItemVo>();
            ThongKePhongBan = new List<GiaoViecTiViPhongBanVo>();
        }
    }
}
