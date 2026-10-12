using Dashboard.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("QuanLyThongTinTraCuuChiTiet")]
    public partial class QuanLyThongTinTraCuuChiTiet
    {
        public QuanLyThongTinTraCuuChiTiet()
        {

        }

        public int Id { get; set; }
        public int? QuanLyThongTinTraCuuId { get; set; }
        public int? VanBanPhapLuatId { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? NguoiTaoId { get; set; }
        public DateTime? NgayXoa { get; set; }
        public int? NguoiXoaId { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual QuanLyThongTinTraCuu QuanLyThongTinTraCuu { get; set; }
    }

    public partial class QuanLyThongTinTraCuuChiTietGridVo
    {
        public QuanLyThongTinTraCuuChiTietGridVo()
        {
        }

        public int Id { get; set; }
        public int? QuanLyThongTinTraCuuId { get; set; }
        public int? VanBanPhapLuatId { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NgayTaoStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(NgayTao);
            }
        }

        public int? NguoiTaoId { get; set; }
        public DateTime? NgayXoa { get; set; }
        public string NgayXoaStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(NgayXoa);
            }
        }
        public int? NguoiXoaId { get; set; }
        public bool? IsDeleted { get; set; }
    }
}