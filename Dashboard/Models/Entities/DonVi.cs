using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DonVi")]
    public class DonVi
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string TenTrim { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class DonViViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string TenTrim { get; set; }
    }

    public class DonViGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string TenTrim { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
