using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("LyDo")]
    public class LyDo
    {
        public LyDo()
        {
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public string MaMau { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class LyDoGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string MaMau { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class LyDoViewModel : LyDoGridVo
    {
    }
}