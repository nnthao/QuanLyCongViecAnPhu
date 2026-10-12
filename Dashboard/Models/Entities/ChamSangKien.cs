using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("ChamSangKien")]
    public class ChamSangKien
    {
        public int Id { get; set; }
        public int SangKienId { get; set; }
        public int UserId { get; set; }
        public string KetQuaChiTiet { get; set; }
    }

    public class ChamSangKienViewModel
    {
        public int Id { get; set; }
        public int SangKienId { get; set; }
        public int UserId { get; set; }
        public string KetQuaChiTiet { get; set; }
    }
}

