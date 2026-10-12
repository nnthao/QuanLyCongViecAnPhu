using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("QuanLyThongTinTraCuuFile")]
    public partial class QuanLyThongTinTraCuuFile
    {
        public QuanLyThongTinTraCuuFile()
        {
        }

        public int Id { get; set; }
        public int? QuanLyThongTinTraCuuId { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public int? NguoiXoaId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual QuanLyThongTinTraCuu QuanLyThongTinTraCuu { get; set; }
    }

    public class QuanLyThongTinTraCuuFileViewModel
    {
        public int Id { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public int QuanLyThongTinTraCuuId { get; set; }
    }

    public class QuanLyThongTinTraCuuFileGridVo
    {
        public int Id { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool? IsDeleted { get; set; }
        public int QuanLyThongTinTraCuuId { get; set; }
    }
}
