using Dashboard.Common;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    public enum EnumTrangThaiGiaoViec : int
    {
        [Description("Chưa hoàn thành")]
        ChuaHoanThanh = 0,

        [Description("Chờ đánh giá")]
        ChoDanhGia = 1,

        [Description("Đã hoàn thành")]
        HoanThanh = 2,

        [Description("Chưa đạt (Làm lại)")]
        ChuaDat = 3
    }

    public enum EnumKetQuaDanhGia : int
    {
        [Description("Đạt")]
        Dat = 1,

        [Description("Chưa đạt")]
        ChuaDat = 2
    }

    [Table("GiaoViecDanhGia")]
    public class GiaoViecDanhGia
    {
        public int Id { get; set; }
        public int? GiaoViecId { get; set; }
        public int GiaoViecThanhVienId { get; set; }
        public int NguoiDanhGiaId { get; set; }
        public DateTime NgayDanhGia { get; set; }
        public int KetQuaDanhGia { get; set; }
        public string YKienDanhGia { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? NgayTao { get; set; }

        public virtual GiaoViec GiaoViec { get; set; }
        public virtual GiaoViecThanhVien GiaoViecThanhVien { get; set; }
    }

    public class GiaoViecDanhGiaGridVo
    {
        public int Id { get; set; }
        public int? GiaoViecId { get; set; }
        public int GiaoViecThanhVienId { get; set; }
        public int NguoiDanhGiaId { get; set; }
        public string NguoiDanhGia { get; set; }
        public DateTime NgayDanhGia { get; set; }
        public string NgayDanhGiaStr => DateTimeHelper.ConvertDateTimeToDateTime(NgayDanhGia);
        public int KetQuaDanhGia { get; set; }
        public string TenKetQuaDanhGia => KetQuaDanhGia == (int)EnumKetQuaDanhGia.Dat ? "Đạt" : "Chưa đạt";
        public string YKienDanhGia { get; set; }
    }

    public class LuuDanhGiaViewModel
    {
        public int GiaoViecThanhVienId { get; set; }
        public int KetQuaDanhGia { get; set; }
        public string YKienDanhGia { get; set; }
        public int CurrentUserId { get; set; }
    }

    public class GiaoViecDanhGiaQueryViewModel : QueryParamViewModel
    {
        public int? TrangThaiSearch { get; set; }
        public int CurrentUserId { get; set; }
    }

    public class ChiTietDanhGiaItemVo
    {
        public int GiaoViecThanhVienId { get; set; }
        public int GiaoViecId { get; set; }
        public string TenCongViec { get; set; }
        public string NoiDung { get; set; }
        public string NguoiGiao { get; set; }
        public int? NguoiGiaoId { get; set; }
        public string ThanhVien { get; set; }
        public int? ThanhVienId { get; set; }
        public string TenVaiTroXuLy { get; set; }
        public string HanXuLyStr { get; set; }
        public int TrangThai { get; set; }
        public string TenTrangThai { get; set; }
        public string NgayXacNhanHoanThanhStr { get; set; }
        public string NoiDungBaoCaoCuoi { get; set; }
        public List<GiaoViecThanhVienTienTrinhFileGridVo> FilesBaoCaoCuoi { get; set; } = new List<GiaoViecThanhVienTienTrinhFileGridVo>();
        public List<GiaoViecDanhGiaGridVo> LichSuDanhGias { get; set; } = new List<GiaoViecDanhGiaGridVo>();
        public int? KetQuaDanhGiaGanNhat { get; set; }
        public string YKienDanhGiaGanNhat { get; set; }
        public string NguoiDanhGiaGanNhat { get; set; }
        public string NgayDanhGiaGanNhatStr { get; set; }
    }
}
