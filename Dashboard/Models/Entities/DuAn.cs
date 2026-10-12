using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dashboard.Models.Entities
{
    [Table("DuAn")]
    public class DuAn
    {
        public DuAn()
        {
            DuAnThanhViens = new List<DuAnThanhVien>();
            DuAnFiles = new List<DuAnFile>();
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsThuNghiem { get; set; }

        public virtual ICollection<DuAnThanhVien> DuAnThanhViens { get; set; }
        public virtual ICollection<DuAnFile> DuAnFiles { get; set; }
    }

    public class DuAnGridVo
    {
        public DuAnGridVo()
        {
            ThanhViens = new List<int?>();
            TruongNhomIds = new List<int?>();
            DanhSachThanhVien = new List<DuAnThanhVienInfo>();
            DuAnFiles = new List<DuAnFileGridVo>();
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsThuNghiem { get; set; }
        public int SoThanhVien { get; set; }
        public int SoTruongNhom { get; set; }
        public List<int?> ThanhViens { get; set; }
        public List<int?> TruongNhomIds { get; set; }
        public List<DuAnThanhVienInfo> DanhSachThanhVien { get; set; }
        public List<DuAnFileGridVo> DuAnFiles { get; set; }
        public string TenThanhVien { get; set; }
        public string TenTruongNhom { get; set; }
        public bool? IsTruongNhom { get; set; }
        public int? MucDoHoTro { get; set; }
    }

    public class DuAnThanhVienInfo
    {
        public int UserId { get; set; }
        public string TenUser { get; set; }
        public int? MucDoHoTro { get; set; }
        public bool IsTruongNhom { get; set; }
    }

    public class DuAnViewModel
    {
        public DuAnViewModel()
        {
            FileDinhKems = new List<HttpPostedFileBase>();
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsThuNghiem { get; set; }
        public string UserIds { get; set; }
        public string TruongNhomIds { get; set; }
        public string ThanhVienMucDoHoTroJson { get; set; }
        public int? CurrentUserId { get; set; }
        public string TaiLieuDinhKemIds { get; set; }
        public List<int> TaiLieuDinhKemIdInts
        {
            get
            {
                return !string.IsNullOrEmpty(TaiLieuDinhKemIds) ?
                    TaiLieuDinhKemIds.Split(',').ToList().Where(o => !string.IsNullOrEmpty(o)).Select(o => System.Convert.ToInt32(o)).ToList() : new List<int>();
            }
        }
        public List<HttpPostedFileBase> FileDinhKems { get; set; }
    }

    public class ThanhVienMucDoHoTroInfo
    {
        public int UserId { get; set; }
        public int? MucDoHoTro { get; set; }
    }
}
