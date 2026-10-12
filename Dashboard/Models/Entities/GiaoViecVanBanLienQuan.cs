using Dashboard.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("GiaoViecVanBanLienQuan")]
    public class GiaoViecVanBanLienQuan
    {
        public int Id { get; set; }
        public int? GiaoViecId { get; set; }
        public int? VanBanPhapLuatId { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey("GiaoViecId")]
        public virtual GiaoViec GiaoViec { get; set; }
    }

    public class GiaoViecVanBanLienQuanGridVo
    {
        public int Id { get; set; }
        public int? VanBanPhapLuatId { get; set; }
        public string SoKiHieu { get; set; }
        public string TrichYeu { get; set; }
        public DateTime? NgayBanHanh { get; set; }
        public string NgayBanHanhStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(NgayBanHanh);
            }
        }
        public int? CoQuanBanHanhId { get; set; }
        public string CoQuanBanHanhStr { get; set; }
    }
}