using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Dashboard.Models.GridVo;

namespace Dashboard.Models.Entities
{
    [Table("SangKien")]
    public partial class SangKien
    {
        public SangKien()
        {
            SangKienFiles = new List<SangKienFile>();
        }

        public int Id { get; set; }
        public int DotXetDuyetId { get; set; }
        public int? NguoiNopId { get; set; }
        public int? LinhVucSangKienId { get; set; }
        public string NamXetDuyet { get; set; }
        public string MoTaChiTiet { get; set; }
        public string GhiChu { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool IsDeleted { get; set; }
        public string TrangThai { get; set; }

        [Column("TongDiem")]
        public double? TongDiemDouble { get; set; }

        [NotMapped]
        public decimal? TongDiem
        {
            get
            {
                if (!TongDiemDouble.HasValue) return null;
                return (decimal?)TongDiemDouble.Value;
            }
            set
            {
                if (!value.HasValue)
                {
                    TongDiemDouble = null;
                }
                else
                {
                    TongDiemDouble = (double)value.Value;
                }
            }
        }

        public virtual ICollection<SangKienFile> SangKienFiles { get; set; }
    }

    public class SangKienViewModel
    {
        public SangKienViewModel()
        {
            FileDinhKems = new List<HttpPostedFileBase>();
        }

        public int Id { get; set; }
        public int DotXetDuyetId { get; set; }
        public string TenDotXetDuyet { get; set; }
        public string NamXetDuyet { get; set; }
        public int? LinhVucSangKienId { get; set; }
        public string TenLinhVuc { get; set; }
        public string MoTaChiTiet { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
        public int? NguoiNopId { get; set; }
        public decimal? TongDiem { get; set; }
        public string TaiLieuDinhKemIds { get; set; }
        public List<int> TaiLieuDinhKemIdInts
        {
            get
            {
                if (!string.IsNullOrEmpty(TaiLieuDinhKemIds))
                {
                    return TaiLieuDinhKemIds.Split(',')
                        .Where(o => !string.IsNullOrWhiteSpace(o))
                        .Select(int.Parse)
                        .ToList();
                }

                return new List<int>();
            }
        }
        public List<HttpPostedFileBase> FileDinhKems { get; set; }
        public int? CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class GiamKhaoPhanCongInfo
    {
        public int UserId { get; set; }
        public string TenNguoiCham { get; set; }
        public bool DaCham { get; set; }
    }

    public class ChiTietNhanThuongInfo
    {
        public int UserId { get; set; }
        public string HoTen { get; set; }
        public decimal TiLeDongGop { get; set; }
        public int TienThuong { get; set; }
    }

    public class SangKienGridVo
    {
        public int Id { get; set; }
        public int DotXetDuyetId { get; set; }
        public string TenDotXetDuyet { get; set; }
        public string NamXetDuyet { get; set; }
        public int? LinhVucSangKienId { get; set; }
        public string TenLinhVuc { get; set; }
        public int? NguoiNopId { get; set; }
        public string TenNguoiNop { get; set; }
        public string MoTaChiTiet { get; set; }
        public string GhiChu { get; set; }
        public bool? IsDeleted { get; set; }
        public string TrangThai { get; set; }
        public decimal? DiemDanhGia { get; set; }
        public decimal? TongDiem { get; set; }
        public decimal? DiemTrungBinhTong { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public List<FileDinhKemGridVo> DanhSachFile { get; set; }
        public List<GiamKhaoPhanCongInfo> DanhSachGiamKhao { get; set; }
        public string TrangThaiChamDiem
        {
            get
            {
                if (DanhSachGiamKhao == null || DanhSachGiamKhao.Count == 0)
                {
                    return "ChuaCham";
                }
                var soNguoiDaCham = DanhSachGiamKhao.Count(g => g.DaCham);
                if (soNguoiDaCham == 0)
                {
                    return "ChuaCham";
                }
                if (soNguoiDaCham < DanhSachGiamKhao.Count)
                {
                    return "DangCham";
                }
                return "DaCham";
            }
        }
        public string LoaiSangKien
        {
            get
            {
                if (DiemDanhGia.HasValue)
                {
                    var diem = DiemDanhGia.Value;
                    if (diem > 75 && diem <= 100)
                        return "Loại I";
                    if (diem > 65 && diem <= 75)
                        return "Loại II";
                    if (diem > 55 && diem <= 65)
                        return "Loại III";
                    if (diem >= 55)
                        return "Đạt giải";
                }
                return "Không đạt giải";
            }
        }

        public int? LoaiSangKienSo
        {
            get
            {
                if (DiemDanhGia.HasValue)
                {
                    var diem = DiemDanhGia.Value;
                    if (diem > 75 && diem <= 100)
                        return 1;
                    if (diem > 65 && diem <= 75)
                        return 2;
                    if (diem > 55 && diem <= 65)
                        return 3;
                }
                return null;
            }
        }

        public int? TienThuong { get; set; }
        public List<ChiTietNhanThuongInfo> DanhSachChiTietNhanThuong { get; set; }
    }
}

