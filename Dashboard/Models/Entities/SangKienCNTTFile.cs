using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("SangKienCNTTFile")]
    public class SangKienCNTTFile
    {
        public int Id { get; set; }
        public int SangKienCNTTId { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual SangKienCNTT SangKienCNTT { get; set; }
    }
}


