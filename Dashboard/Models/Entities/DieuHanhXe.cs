using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DieuHanhXe")]
    public partial class DieuHanhXe
    {
        public DieuHanhXe()
        {
            LogDieuHanhXes = new List<LogDieuHanhXe>();
            TaiLieuDatXes = new List<TaiLieuDatXe>();
            IsNgoaiTinh = false;
            IsDeleted = false;
        }

        public int Id { get; set; }
        public int? XeId { get; set; }
        public string DuongDi { get; set; }
        public string TieuDe { get; set; }
        public double SoKMDaDi { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string NoiDi { get; set; }
        public string NoiDen { get; set; }
        public string TrangThaiXuLy { get; set; }
        public int NguoiTaoId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime NgayTao { get; set; }
        public int? SoNguoiDi { get; set; }
        public int? TaiXeId { get; set; }
        public string LyDoTraPhieu { get; set; }
        public string GhiChu { get; set; }
        public int? NguoiSuaId { get; set; }
        public DateTime? NgaySua { get; set; }
        public string LyDoHuyBoXe { get; set; }
        public int? SoKmKhiDi { get; set; }
        public int? SoKmKhiVe { get; set; }
        public string DiaDiemDon { get; set; }
        public string SearchStr { get; set; }
        public decimal? NhotMay { get; set; }
        public int? LocNhot { get; set; }
        public DateTime? NgayCapNhatTtXang { get; set; }
        public DateTime? NgayDi { get; set; }
        public int? NguoiDuyetId { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public int? CoQuanId { get; set; }
        public bool? IsNgoaiTinh { get; set; }
        public int? NguoiDuocGanDuyetId { get; set; }
        public int? CuocHopId { get; set; }
        public bool? IsKhuVuc { get; set; }


        public virtual ICollection<LogDieuHanhXe> LogDieuHanhXes { get; set; }
        public virtual ICollection<TaiLieuDatXe> TaiLieuDatXes { get; set; }
        [NotMapped]
        public bool? IsUpdated { get; set; }

    }

}
