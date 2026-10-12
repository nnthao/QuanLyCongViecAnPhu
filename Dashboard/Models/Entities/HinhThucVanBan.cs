using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("HinhThucVanBan")]
    public partial class HinhThucVanBan
    {
        public HinhThucVanBan()
        {
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class HinhThucVanBanViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
    }

    public class HinhThucVanBanGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public bool? IsDeleted { get; set; }
    }
}


