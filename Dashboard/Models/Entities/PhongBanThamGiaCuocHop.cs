using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("PhongBanThamGiaCuocHop")]
    public class PhongBanThamGiaCuocHop
    {
        public int Id { get; set; }
        public int? CuocHopId { get; set; }
        public int? PhongBanId { get; set; }

        public virtual CuocHop CuocHop { get; set; }
        public virtual PhongBan PhongBan { get; set; }
    }

    public class PhongBanThamGiaCuocHopGridVo
    {
        public int Id { get; set; }
        public int? CuocHopId { get; set; }
        public int? PhongBanId { get; set; }
    }
}
