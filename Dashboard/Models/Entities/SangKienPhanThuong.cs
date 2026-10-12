using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("SangKienPhanThuong")]
    public class SangKienPhanThuong
    {
        public int Id { get; set; }
        public int DotXetDuyetId { get; set; }
        public int? GiaiThuongXepLoai { get; set; }
        public int? TienThuongXepLoai { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class SangKienPhanThuongViewModel
    {
        public int Id { get; set; }
        public int DotXetDuyetId { get; set; }
        public int? GiaiThuongXepLoai { get; set; }
        public int? TienThuongXepLoai { get; set; }
        public string TenXepLoai { get; set; }
    }

    public class SangKienPhanThuongGridVo
    {
        public int Id { get; set; }
        public int DotXetDuyetId { get; set; }
        public int? GiaiThuongXepLoai { get; set; }
        public int? TienThuongXepLoai { get; set; }
        public string TenXepLoai
        {
            get
            {
                switch (GiaiThuongXepLoai)
                {
                    case 1: return "Loại I";
                    case 2: return "Loại II";
                    case 3: return "Loại III";
                    default: return "Khác";
                }
            }
        }
    }
}


