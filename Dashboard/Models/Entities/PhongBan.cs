using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("PhongBan")]
    public partial class PhongBan
    {
        public PhongBan()
        {
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public string DiaChi { get; set; }
        public int? PhongBanChaId { get; set; }
        public int? ThuTu { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsPhongNhanViec { get; set; }
        public int? NguoiTheoDoiChiDaoId { get; set; }
    }

    public class PhongBanViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string DiaChi { get; set; }
        public int? PhongBanChaId { get; set; }
        public int? ThuTu { get; set; }
        public bool? IsPhongNhanViec { get; set; }
        public int? NguoiTheoDoiChiDaoId { get; set; }
    }

    public class PhongBanGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string DiaChi { get; set; }
        public int? PhongBanChaId { get; set; }
        public string TenPhongBanCha { get; set; }
        public int? ThuTu { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsPhongNhanViec { get; set; }
        public int? NguoiTheoDoiChiDaoId { get; set; }
        public string TenNguoiTheoDoiChiDao { get; set; }
        public int Level { get; set; }
    }

    public class CapNhatThuTuPhongBanRequest
    {
        public int Id { get; set; }
        public int? ThuTu { get; set; }
    }

    public class CapNhatIsPhongNhanViecRequest
    {
        public int Id { get; set; }
        public bool IsPhongNhanViec { get; set; }
    }
}
