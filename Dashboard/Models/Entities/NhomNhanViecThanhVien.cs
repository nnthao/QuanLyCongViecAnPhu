using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("NhomNhanViecThanhVien")]
    public class NhomNhanViecThanhVien
    {
        public int Id { get; set; }
        public int? NhomNhanViecId { get; set; }
        public int? UserId { get; set; }

        public bool IsDeleted { get; set; }

        public virtual NhomNhanViec NhomNhanViec { get; set; }
    }

    public class NhomNhanViecThanhVienGridVo
    {
        public int Id { get; set; }
        public int? NhomNhanViecId { get; set; }
        public int? UserId { get; set; }
        public string TenUser { get; set; }
        public string TenPhongBan { get; set; }
        public string TenChucDanh { get; set; }
    }
}

