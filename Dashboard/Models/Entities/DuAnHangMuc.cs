using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DuAnHangMuc")]
    public class DuAnHangMuc
    {
        public int Id { get; set; }
        public string TenHangMuc { get; set; }
        public string MoTa { get; set; }
        public int? ThuTu { get; set; }
        public bool? IsDeleted { get; set; }
    }
}