using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("PhanCongCham")]
    public class PhanCongCham
    {
        public int Id { get; set; }

        [Column("SangkienId")]
        public int SangKienId { get; set; }
        public int UserId { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }
        public string TrangThai { get; set; }
    }

    public class PhanCongChamViewModel
    {
        public int Id { get; set; }
        public int SangKienId { get; set; }
        public int UserId { get; set; }
        public DateTime? NgayPhanCong { get; set; }
        public string GhiChu { get; set; }

        public string TenNguoiCham { get; set; }
    }
}

