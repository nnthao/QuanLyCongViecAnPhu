using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DoKhan")]
    public partial class DoKhan
    {
        public DoKhan()
        {
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class DoKhanViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
    }

    public class DoKhanGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public bool? IsDeleted { get; set; }
    }
}


