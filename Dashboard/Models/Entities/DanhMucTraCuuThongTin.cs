using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DanhMucTraCuuThongTin")]
    public partial class DanhMucTraCuuThongTin
    {
        public DanhMucTraCuuThongTin()
        {
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class DanhMucTraCuuThongTinViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
    }

    public class DanhMucTraCuuThongTinGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
