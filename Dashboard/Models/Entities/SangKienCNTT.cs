using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Dashboard.Models.GridVo;

namespace Dashboard.Models.Entities
{
    [Table("SangKienCNTT")]
    public class SangKienCNTT
    {
        public SangKienCNTT()
        {
            SangKienCNTTFiles = new List<SangKienCNTTFile>();
        }

        public int Id { get; set; }
        public int? NguoiNopId { get; set; }
        public string TenSangKien { get; set; }
        public string MoTaChiTiet { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
        public decimal? TiLeChuTri { get; set; }
        public string NamSangKien { get; set; }
        public int? TongTienThuong { get; set; }

        [Column("GiaiThuongXepLoai")]
        public string XepLoaiSangKien { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<SangKienCNTTFile> SangKienCNTTFiles { get; set; }
    }

    public class SangKienCNTTViewModel
    {
        public SangKienCNTTViewModel()
        {
            FileDinhKems = new List<HttpPostedFileBase>();
        }

        public int Id { get; set; }
        public int? NguoiNopId { get; set; }
        public string TenNguoiNop { get; set; }
        public string TenSangKien { get; set; }
        public string MoTaChiTiet { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
        public decimal? TiLeChuTri { get; set; }
        public string NamSangKien { get; set; }
        public int? TongTienThuong { get; set; }
        public string XepLoaiSangKien { get; set; }
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

    public class SangKienCNTTGridVo
    {
        public int Id { get; set; }
        public int? NguoiNopId { get; set; }
        public string TenNguoiNop { get; set; }
        public string TenSangKien { get; set; }
        public string NamSangKien { get; set; }
        public string MoTaChiTiet { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
        public string XepLoaiSangKien { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public List<FileDinhKemGridVo> DanhSachFile { get; set; }
    }
}

