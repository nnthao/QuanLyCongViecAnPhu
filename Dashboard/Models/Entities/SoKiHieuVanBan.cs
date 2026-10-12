using Dashboard.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("SoKiHieuVanBan")]
    public class SoKiHieuVanBan
    {
        public int Id { get; set; }
        public int? SoKiHieu { get; set; }
        public int? UserId { get; set; }
        public DateTime? NgayLay { get; set; }
        public int? Nam { get; set; }
        public int? HinhThucVanBanId { get; set; }
    }

    public class SoKiHieuVanBanGridVo
    {
        public int Id { get; set; }
        public int? SoKiHieu { get; set; }
        public int? UserId { get; set; }
        public string TenNguoiLay { get; set; }
        public DateTime? NgayLay { get; set; }
        public string NgayLayStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(NgayLay);
            }
        }
        public int? Nam { get; set; }
        public int? HinhThucVanBanId { get; set; }
        public string TenHinhThucVanBan { get; set; }
        public bool IsAllowDeleted { get; set; }
    }

    public class SoKiHieuVanBanViewModel
    {
        public int Id { get; set; }
        public int? SoKiHieu { get; set; }
        public int? UserId { get; set; }
        public DateTime? NgayLay { get; set; }
        public int? Nam { get; set; }
        public int? HinhThucVanBanId { get; set; }
    }
}

