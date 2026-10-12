using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("VaiTro")]
    public partial class VaiTro
    {
        public VaiTro()
        {
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public string PhanQuyenKeys { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsDoanhNghiep { get; set; }

        public string VietTat { get; set; }
    }

    public class VaiTroViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string PhanQuyenStr { get; set; }
    }

    public class GanVaiTroKhacViewModel
    {
        public int VaiTroId { get; set; }
        public string PhongBanIdsStr { get; set; }
    }
}
