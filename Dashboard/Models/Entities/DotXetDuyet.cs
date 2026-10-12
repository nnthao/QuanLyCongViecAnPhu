using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DotXetDuyet")]
    public partial class DotXetDuyet
    {
        public DotXetDuyet()
        {
        }

        public int Id { get; set; }
        public string TenDotXetDuyet { get; set; }
        public string NamXetDuyet { get; set; }
        public string GhiChu { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool IsDeleted { get; set; }
        public string TrangThai { get; set; }
    }

    public class DotXetDuyetViewModel
    {
        public int Id { get; set; }
        public string TenDotXetDuyet { get; set; }
        public string NamXetDuyet { get; set; }
        public string GhiChu { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public string TrangThai { get; set; }
    }

    public class DotXetDuyetGridVo
    {
        public int Id { get; set; }
        public string TenDotXetDuyet { get; set; }
        public string NamXetDuyet { get; set; }
        public string GhiChu { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }
        public string TrangThai { get; set; }
    }
}

