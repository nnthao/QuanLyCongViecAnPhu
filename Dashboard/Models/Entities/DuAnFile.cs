using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DuAnFile")]
    public class DuAnFile
    {
        public int Id { get; set; }
        public int? DuAnId { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey("DuAnId")]
        public virtual DuAn DuAn { get; set; }
    }

    public class DuAnFileGridVo
    {
        public int Id { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
    }
}
