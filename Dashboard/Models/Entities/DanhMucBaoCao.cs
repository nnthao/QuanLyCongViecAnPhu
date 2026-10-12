using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DanhMucBaoCao")]
    public partial class DanhMucBaoCao
    {
        public DanhMucBaoCao()
        {
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class DanhMucBaoCaoViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
    }

    public class DanhMucBaoCaoGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public bool? IsDeleted { get; set; }
    }
}


