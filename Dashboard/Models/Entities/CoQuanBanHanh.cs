using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("CoQuanBanHanh")]
    public partial class CoQuanBanHanh
    {
        public CoQuanBanHanh()
        {
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CoQuanBanHanhViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
    }

    public class CoQuanBanHanhGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public bool? IsDeleted { get; set; }
    }
}


