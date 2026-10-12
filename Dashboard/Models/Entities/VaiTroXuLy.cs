using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("VaiTroXuLy")]
    public partial class VaiTroXuLy
    {
        public VaiTroXuLy()
        {
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public int? ThuTu { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsThongKeGiaoViec { get; set; }
        public bool? IsCanDinhKemFileKhiXacNhanHoanThanh { get; set; }
    }

    public class VaiTroXuLyViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public int? ThuTu { get; set; }
        public bool? IsThongKeGiaoViec { get; set; }
        public bool? IsCanDinhKemFileKhiXacNhanHoanThanh { get; set; }
    }

    public class VaiTroXuLyGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public int? ThuTu { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsThongKeGiaoViec { get; set; }
        public bool? IsCanDinhKemFileKhiXacNhanHoanThanh { get; set; }
    }

    public class CapNhatThuTuVaiTroXuLyRequest
    {
        public int Id { get; set; }
        public int? ThuTu { get; set; }
    }

    public class CapNhatIsThongKeGiaoViecRequest
    {
        public int Id { get; set; }
        public bool IsThongKeGiaoViec { get; set; }
    }

    public class CapNhatIsCanDinhKemFileKhiXacNhanHoanThanhRequest
    {
        public int Id { get; set; }
        public bool IsCanDinhKemFileKhiXacNhanHoanThanh { get; set; }
    }
}
