using Dashboard.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("GiaoViecLichSu")]
    public class GiaoViecLichSu
    {
        public int Id { get; set; }
        public int GiaoViecId { get; set; }
        public int? GiaoViecThanhVienId { get; set; }
        public int? NguoiThucHienId { get; set; }
        public string HanhDong { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayTao { get; set; }
    }

    public class GiaoViecLichSuGridVo
    {
        public DateTime NgayTao { get; set; }
        public string NgayTaoStr => NgayTao.ToString("dd/MM/yyyy HH:mm:ss");
        public string HanhDong { get; set; }
        public string NguoiThucHien { get; set; }
        public string NoiDung { get; set; }
    }

    public class GiaoViecLichSuQueryViewModel
    {
        public int GiaoViecId { get; set; }
        public int CurrentUserId { get; set; }
        public bool CanViewAll { get; set; }
    }
}
