using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DonViDoan")]
    public class DonViDoan
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string TenTrim { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class DonViDoanViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string TenTrim { get; set; }
    }

    public class DonViDoanGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string TenTrim { get; set; }
        public bool? IsDeleted { get; set; }
    }
}

