using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DoanVien_DanToc")]
    public class DoanVien_DanToc
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class DoanVien_DanTocViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
    }

    public class DoanVien_DanTocGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public bool? IsDeleted { get; set; }
    }
}

