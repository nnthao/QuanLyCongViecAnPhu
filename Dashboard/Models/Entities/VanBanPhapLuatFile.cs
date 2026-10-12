using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("VanBanPhapLuatFile")]
    public partial class VanBanPhapLuatFile
    {
        public VanBanPhapLuatFile()
        {
        }

        public int Id { get; set; }
        public int? VanBanPhapLuatId { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public int? NguoiXoaId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual VanBanPhapLuat VanBanPhapLuat { get; set; }
    }

    public class VanBanPhapLuatFileViewModel
    {
        public int Id { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public int VanBanPhapLuatId { get; set; }
    }

    public class VanBanPhapLuatFileGridVo
    {
        public int Id { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool? IsDeleted { get; set; }
        public int VanBanPhapLuatId { get; set; }
    }
}


