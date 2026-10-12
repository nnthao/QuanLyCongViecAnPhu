using Dashboard.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("CongViecChung")]
    public class CongViecChung
    {
        public CongViecChung()
        {
            CongViecChungThanhViens = new List<CongViecChungThanhVien>();
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual ICollection<CongViecChungThanhVien> CongViecChungThanhViens { get; set; }
    }
 
    public class CongViecChungViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public DateTime? NgayTao 
        { 
            get 
            {
                return DateTimeHelper.ConvertStrToDate(NgayTaoStr);
            } 
        }
        public string NgayTaoStr { get; set; }
        public string ThanhVienNames { get; set; }
        public List<int> SelectedUserIds { get; set; }
    }
 
    public class CongViecChungGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NgayTaoStr 
        { 
            get 
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(NgayTao);
            } 
        }
        public string ThanhVienNames { get; set; }
        public int SoThanhVien { get; set; }
        public List<int> SelectedUserIds { get; set; }
    }
 
    public class CongViecChungThongKeGridVo
    {
        public string TenUser { get; set; }
        public int SoLuongCongViec { get; set; }
        public int SoLuongThanhVien { get; set; } 
    }

    public class CongViecChungThongKeQueryViewModel
    {
        public string TuNgayStr { get; set; }
        public string DenNgayStr { get; set; }
        public int PhongBanId { get; set; }

        public DateTime? TuNgay 
        { 
            get 
            {
                return DateTimeHelper.ConvertStrToDate(TuNgayStr);
            } 
        }

        public DateTime? DenNgay 
        { 
            get 
            {
                return DateTimeHelper.ConvertStrToDate(DenNgayStr);
            } 
        }
    }
}
