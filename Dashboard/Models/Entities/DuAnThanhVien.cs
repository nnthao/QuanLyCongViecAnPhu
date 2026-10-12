using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DuAnThanhVien")]
    public class DuAnThanhVien
    {
        public int Id { get; set; }
        public int? DuAnId { get; set; }
        public int? UserId { get; set; }
        public int? MucDoHoTro { get; set; }
        public bool? IsTruongNhom { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual DuAn DuAn { get; set; }
        public virtual User User { get; set; }
    }

    public class DuAnThanhVienGridVo
    {
        public int Id { get; set; }
        public int DuAnId { get; set; }
        public string TenDuAn { get; set; }
        public int? MucDoHoTro { get; set; }
        public int UserId { get; set; }
        public string TenUser { get; set; }
        public bool? IsTruongNhom { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class DuAnThanhVienViewModel
    {
        public int Id { get; set; }
        public int DuAnId { get; set; }
        public int UserId { get; set; }
        public bool? IsTruongNhom { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class ThongKeThanhVienGridVo
    {
        public int UserId { get; set; }
        public string TenUser { get; set; }
        public int SoNhomThamGia { get; set; }
        public int TongMucDoHoTro { get; set; }
        public List<string> DanhSachTenDuAn { get; set; }

        public ThongKeThanhVienGridVo()
        {
            DanhSachTenDuAn = new List<string>();
        }
    }

    public class ThongKeDuAnGridVo
    {
        public int DuAnId { get; set; }
        public string TenDuAn { get; set; }
        public int? MucDoHoTro { get; set; }
        public int SoThanhVien { get; set; }
        public List<string> DanhSachTenThanhVien { get; set; }

        public ThongKeDuAnGridVo()
        {
            DanhSachTenThanhVien = new List<string>();
        }
    }

    public class MatranThanhVienDuAnInfo
    {
        public int UserId { get; set; }
        public int DuAnId { get; set; }
    }

    public class DuAnThanhVienThongKeGridVo
    {
        public int UserId { get; set; }
        public string TenUser { get; set; }
        public int SoNhomThamGia { get; set; }
        public int SoDuAnThuNghiem { get; set; }
        public int SoDuAnTruongNhom { get; set; }
        public List<int> DanhSachDuAnThamGia { get; set; }
        public List<int> DanhSachDuAnThuNghiemThamGia { get; set; }

        public DuAnThanhVienThongKeGridVo()
        {
            DanhSachDuAnThamGia = new List<int>();
            DanhSachDuAnThuNghiemThamGia = new List<int>();
        }
    }

    public class TruongNhomThongKeGridVo
    {
        public int UserId { get; set; }
        public string TenUser { get; set; }
        public int SoNhomLamTruongNhom { get; set; }
        public List<DuAnInfo> DanhSachNhom { get; set; }

        public TruongNhomThongKeGridVo()
        {
            DanhSachNhom = new List<DuAnInfo>();
        }
    }

    public class DuAnInfo
    {
        public int DuAnId { get; set; }
        public string TenDuAn { get; set; }
        public int SoThanhVien { get; set; }
        public List<ThanhVienInfo> DanhSachThanhVien { get; set; }

        public DuAnInfo()
        {
            DanhSachThanhVien = new List<ThanhVienInfo>();
        }
    }

    public class ThanhVienInfo
    {
        public int UserId { get; set; }
        public string TenUser { get; set; }
    }
}
