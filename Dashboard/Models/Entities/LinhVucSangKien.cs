using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("LinhVucSangKien")]
    public partial class LinhVucSangKien
    {
        public LinhVucSangKien()
        {
        }

        public int Id { get; set; }
        public string TenLinhVuc { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class LinhVucSangKienViewModel
    {
        public int Id { get; set; }
        public string TenLinhVuc { get; set; }
    }

    public class LinhVucSangKienGridVo
    {
        public int Id { get; set; }
        public string TenLinhVuc { get; set; }
        public bool? IsDeleted { get; set; }
    }
}

