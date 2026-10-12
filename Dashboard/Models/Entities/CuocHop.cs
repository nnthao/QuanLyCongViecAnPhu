using Dashboard.Models.GridVo;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("CuocHop")]
    public partial class CuocHop
    {
        public CuocHop()
        {
            TaiLieuCuocHops = new List<TaiLieuCuocHop>();
            NguoiThamGias = new List<NguoiThamGia>();
            PhongBanThamGiaCuocHops = new List<PhongBanThamGiaCuocHop>();
        }

        public int Id { get; set; }
        public string TenCuocHop { get; set; }
        public int? PhongHopId { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public DateTime? ThoiGianKetThucTemp { get; set; }
        public string NoiDung { get; set; }
        public bool? IsDeleted { get; set; }
        public int? NguoiTaoId { get; set; }
        public int? SoNguoiThamGia { get; set; }
        public string GhiChu { get; set; }
        //public string TrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgaySua { get; set; }
        public int? NguoiSuaId { get; set; }
        public string LyDoTraPhieu { get; set; }
        //public string NguoiChuTri { get; set; }
        public string NguoiThamGia { get; set; }
        public string NguoiThamGiaKhac { get; set; }
        public string PhongHopTxt { get; set; }
        public int? CoQuanId { get; set; }
        public int? NguoiDuyetId { get; set; }
        public DateTime? NgayDuyet { get; set; }
        //public string CoQuanChuTriId { get; set; }
        //public string CoQuanThamGiaId { get; set; }
        //public string CoQuanNguoiChuTriThamGiaId { get; set; }
        //public int? NguoiDuocGanDuyetId { get; set; }
        //public EnumLoaiLichHop? LoaiLichHop { get; set; }
        public string CuocHopTrungId { get; set; }
        public int? LyDoId { get; set; }
        public bool? IsThongBao { get; set; }

        public virtual User NguoiTao { get; set; }
        public virtual User NguoiSua { get; set; }
        public virtual User NguoiDuyet { get; set; }

        public virtual ICollection<TaiLieuCuocHop> TaiLieuCuocHops { get; set; }
        public virtual ICollection<NguoiThamGia> NguoiThamGias { get; set; }
        public virtual ICollection<PhongBanThamGiaCuocHop> PhongBanThamGiaCuocHops { get; set; }

        [NotMapped]
        public bool? IsUpdated { get; set; }
    }

    public enum EnumLoaiLichHop : int
    {
        [Description("Đăng ký lịch cá nhân")]
        LichHopCaNhan = 1,
        [Description("Đăng ký (cần duyệt)")]
        LichHopDangKyCanDuyet = 2
    }

    public class FileExportVm
    {
        public FileExportVm()
        {
            Lichs = new List<NgaySscheduler>();
        }

        public string FolderSave { get; set; }
        public string FileTemplate { get; set; }
        public List<NgaySscheduler> Lichs { get; set; }
        public bool IsTuan { get; internal set; }
    }

    public class ExcelSheetFileExportVm
    {
        public ExcelSheetFileExportVm()
        {
            CuocHops = new List<DieuHanhXeGridVo>();
        }

        public ExcelWorksheet WorkSheetCurrent { get; set; }
        public int RowNumber { get; set; }
        public string TenCa { get; set; }
        public List<DieuHanhXeGridVo> CuocHops { get; set; }
        public bool IsSang { get; set; } = true;
        public bool IsLaiXe { get; set; } = true;
    }
}
