using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("ChucDanh")]
    public class ChucDanh
    {
        public ChucDanh()
        {
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public int? ThuTu { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class ChucDanhGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public int? ThuTu { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class ChucDanhViewModel : ChucDanhGridVo
    {
    }

    public class CapNhatThuTuChucDanhRequest
    {
        public int Id { get; set; }
        public int? ThuTu { get; set; }
    }
}