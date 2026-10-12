using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("TaiLieuCuocHop")]
    public class TaiLieuCuocHop
    {
        public int Id { get; set; }
        public int? CuocHopId { get; set; }
        public string Ten { get; set; }
        public string Loai { get; set; }
        public string DuongDan { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual CuocHop CuocHop { get; set; }
    }
}