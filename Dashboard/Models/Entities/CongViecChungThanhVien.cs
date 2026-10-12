using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("CongViecChungThanhVien")]
    public class CongViecChungThanhVien
    {
        public int Id { get; set; }
        public int? CongViecChungId { get; set; }
        public int? UserId { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual CongViecChung CongViecChung { get; set; }
        public virtual User User { get; set; }
    }
}
