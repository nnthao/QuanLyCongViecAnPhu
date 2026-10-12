using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("SangKienFile")]
    public class SangKienFile
    {
        public int Id { get; set; }

        [Column("SangkienId")]
        public int SangKienId { get; set; }

        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual SangKien SangKien { get; set; }
    }
}

