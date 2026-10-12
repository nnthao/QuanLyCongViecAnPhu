using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("SangKienThanhVien")]
    public class SangKienThanhVien
    {
        public int Id { get; set; }
        public int SangKienId { get; set; }
        public int UserId { get; set; }
        public decimal TiLeDongGop { get; set; }
        public string VaiTro { get; set; }
        public string GhiChu { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class SangKienThanhVienViewModel
    {
        public int Id { get; set; }
        public int SangKienId { get; set; }
        public int UserId { get; set; }
        public decimal TiLeDongGop { get; set; }
        public string VaiTro { get; set; }
        public string GhiChu { get; set; }
        public string TenThanhVien { get; set; }
    }

    public class SangKienThanhVienGridVo
    {
        public int Id { get; set; }
        public int SangKienId { get; set; }
        public int UserId { get; set; }
        public decimal TiLeDongGop { get; set; }
        public string VaiTro { get; set; }
        public string GhiChu { get; set; }
        public string TenThanhVien { get; set; }
    }
}


