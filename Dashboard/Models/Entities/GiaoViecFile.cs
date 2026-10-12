using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("GiaoViecFile")]
    public class GiaoViecFile
    {
        public int Id { get; set; }
        public int? GiaoViecId { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("GiaoViecId")]
        public virtual GiaoViec GiaoViec { get; set; }
    }

    public class GiaoViecFileGridVo
    {
        public int Id { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
    }
}