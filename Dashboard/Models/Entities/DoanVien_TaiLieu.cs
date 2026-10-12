using Dashboard.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DoanVien_TaiLieu")]
    public class DoanVien_TaiLieu
    {
        public int Id { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class DoanVien_TaiLieuViewModel
    {
        public int Id { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public DateTime? NgayTao { get; set; }
    }

    public class DoanVien_TaiLieuGridVo
    {
        public int Id { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NgayTaoStr { get { return DateTimeHelper.ConvertDateTimeToDateTime(NgayTao); } }
        public bool? IsDeleted { get; set; }
    }
}

