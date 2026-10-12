using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("NguoiThamGia")]
    public class NguoiThamGia
    {
        public int Id { get; set; }
        public int? CuocHopId { get; set; }
        public int? UserId { get; set; }

        public virtual CuocHop CuocHop { get; set; }
        public virtual User User { get; set; }
    }
    public class NguoiThamGiaGridVo
    {
        public int Id { get; set; }
        public int? CuocHopId { get; set; }
        public int? UserId { get; set; }
    }
}

