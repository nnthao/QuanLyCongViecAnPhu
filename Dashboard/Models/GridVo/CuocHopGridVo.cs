using Dashboard.Common;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Dashboard.Models.GridVo
{
    public class CuocHopGridVo
    {
        public CuocHopGridVo()
        {
            LoaiLichHop = EnumLoaiLichHop.LichHopCaNhan;
            CuocHopTrung = new List<CuocHopGridVo>();
            NguoiCanThongBaoIds = new List<int>();
            TaiLieuCuocHops = new List<TaiLieuCuocHopGridVo>();
            NguoiThamGias = new List<NguoiThamGiaGridVo>();
            PhongBanThamGiaCuocHops = new List<PhongBanThamGiaCuocHopGridVo>();
        }
        public int Id { get; set; }
        public string TenCuocHop { get; set; }
        public int? PhongHopId { get; set; }
        public int? LyDoId { get; set; }
        public string LyDoTxt { get; set; }
        public string LyDoMaMau { get; set; }
        public string TenPhongHop { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public string ThoiGianBatDauStr
        {
            get
            {
                return DateTimeHelper.DateTimeFormatVi(ThoiGianBatDau);
            }
        }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string ThoiGianKetThucStr
        {
            get
            {
                return DateTimeHelper.DateTimeFormatVi(ThoiGianKetThuc);
            }
        }
        //public string NoiDung { get; set; }
        public bool? IsDeleted { get; set; }
        public string NguoiThamGia { get; set; }
        public string NguoiThamGiaStr { get; set; }
        public string NguoiThamGiaKhac { get; set; }
        public string LoaiThamGia { get; set; }
        public string TrangThai { get; set; }
        public int? SoNguoiThamGia { get; set; }
        public string GhiChu { get; set; }
        public string LyDoTraPhieu { get; set; }
        public string CoQuan { get; set; }
        public bool IsShowTraPhieuDaDuyet
        {
            get
            {
                return ThoiGianBatDau.GetValueOrDefault() > DateTime.Now;
            }
        }
        public bool IsShowNutSua
        {
            get
            {
                return true;
            }
        }

        public bool IsShowNutXoa
        {
            get
            {
                return ThoiGianBatDau.GetValueOrDefault() > DateTime.Now;
            }
        }

        [ScriptIgnore]
        public DateTime? NgayTao { get; set; }
        public string NgayTaoStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(NgayTao);
            }
        }
        public string Checklists { get; set; }
        [ScriptIgnore]
        public string SearchStr { get; set; }

        public string NguoiChuTri { get; set; }
        public string NguoiChuTriStr { get; set; }
        public string PhongHopTxt { get; set; }

        public string NgayDienRaStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(ThoiGianBatDau);
            }
        }

        public string BatDauStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToTimeOnly(ThoiGianBatDau);
            }
        }

        public string KetThucStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToTimeOnly(ThoiGianKetThuc);
            }
        }

        public int? NguoiDuyetId { get; set; }
        public string NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; internal set; }
        public string NgayDuyetStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(NgayDuyet);
            }
        }

        public int? NguoiTaoId { get; set; }
        public string NguoiTao { get; set; }

        public string CoQuanChuTriId { get; set; }
        public string CoQuanThamGiaId { get; set; }
        public string CoQuanChuTri { get; internal set; }
        public string CoQuanThamGia { get; internal set; }
        public EnumLoaiLichHop? LoaiLichHop { get; set; }
        public int? NguoiDuocGanDuyetId { get; set; }
        public string CuocHopTrungId { get; set; }
        public List<CuocHopGridVo> CuocHopTrung { get; set; }
        public string PhongHopTongTxt { get; set; }
        public int? DieuHanhXeId { get; set; }

        public List<int> NguoiCanThongBaoIds { get; set; }

        public List<TaiLieuCuocHopGridVo> TaiLieuCuocHops { get; set; }
        public List<NguoiThamGiaGridVo> NguoiThamGias { get; set; }
        public List<PhongBanThamGiaCuocHopGridVo> PhongBanThamGiaCuocHops { get; set; }
        public string PhongBanThamGiaIds { get; set; }
        public string PhongBanThamGia { get; set; }
        public bool? IsThongBao { get; set; }
    }

    public class NgaySscheduler
    {
        public NgaySscheduler()
        {
            Sangs = new List<CuocHopGridVo>();
            Chieus = new List<CuocHopGridVo>();

            XeSangs = new List<DieuHanhXeGridVo>();
            XeChieus = new List<DieuHanhXeGridVo>();
        }
        public DateTime Ngay { get; set; }
        public string Thu
        {
            get
            {
                return DateTimeHelper.GetThuTiengViet(Ngay);
            }
        }

        public string NgayStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(Ngay);
            }
        }

        public int NgayOnlyStr
        {
            get
            {
                return Ngay.Day;
            }
        }

        public List<CuocHopGridVo> Sangs { get; set; }
        public List<CuocHopGridVo> Chieus { get; set; }

        public List<DieuHanhXeGridVo> XeSangs { get; set; }
        public List<DieuHanhXeGridVo> XeChieus { get; set; }
    }
}
