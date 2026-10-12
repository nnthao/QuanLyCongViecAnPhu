using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("LogDieuHanhXe")]
    public partial class LogDieuHanhXe
    {
        public LogDieuHanhXe()
        {
        }

        public int Id { get; set; }
        public int? DieuHanhXeId { get; set; }
        public string TrangThaiXuLy { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? NguoiTaoId { get; set; }
        public string LyDoTraPhieu { get; set; }

        public virtual DieuHanhXe DieuHanhXe { get; set; }
    }
}
