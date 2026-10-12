using Dashboard.Common;
using Dashboard.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Dashboard.Models.Entities
{
    [Table("DuAnTrienKhai")]
    public class DuAnTrienKhai
    {
        public DuAnTrienKhai()
        {
            DuAnTrienKhaiHangMucs = new List<DuAnTrienKhaiHangMuc>();
        }

        public int Id { get; set; }
        public int? DuAnId { get; set; }
        public int? DonViId { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual DuAn DuAn { get; set; }
        public virtual DonVi DonVi { get; set; }
        public virtual ICollection<DuAnTrienKhaiHangMuc> DuAnTrienKhaiHangMucs { get; set; }
    }

    [Table("DuAnTrienKhaiHangMuc")]
    public class DuAnTrienKhaiHangMuc
    {
        public int Id { get; set; }
        public int? DuAnTrienKhaiId { get; set; }
        public int? DuAnHangMucId { get; set; }
        public string TenHangMuc { get; set; }
        public string MoTa { get; set; }
        public int? ThuTu { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int? TienDo { get; set; }
        public string GhiChu { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual DuAnTrienKhai DuAnTrienKhai { get; set; }
    }

    public class DuAnTrienKhaiGridVo
    {
        public DuAnTrienKhaiGridVo()
        {
            HangMucs = new List<DuAnTrienKhaiHangMucGridVo>();
        }

        public int Id { get; set; }
        public int? DuAnId { get; set; }
        public int? DonViId { get; set; }
        public string Ten { get; set; }
        public string TenDuAn { get; set; }
        public string TenDonVi { get; set; }
        public string MoTa { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public bool? IsDeleted { get; set; }
        public int SoHangMuc { get; set; }
        public int SoHangMucHoanThanh { get; set; }
        public int TienDoTong { get; set; }
        public string TrangThai { get; set; }
        public List<DuAnTrienKhaiHangMucGridVo> HangMucs { get; set; }
        public string NgayBatDauStr => DateTimeHelper.ConvertDateTimeToDateOnly(NgayBatDau);
        public string NgayKetThucStr => DateTimeHelper.ConvertDateTimeToDateOnly(NgayKetThuc);
    }

    public class DuAnTrienKhaiHangMucGridVo
    {
        public int Id { get; set; }
        public int? DuAnHangMucId { get; set; }
        public string TenHangMuc { get; set; }
        public string MoTa { get; set; }
        public int? ThuTu { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int? TienDo { get; set; }
        public string GhiChu { get; set; }
        public string NgayBatDauStr => DateTimeHelper.ConvertDateTimeToDateOnly(NgayBatDau);
        public string NgayKetThucStr => DateTimeHelper.ConvertDateTimeToDateOnly(NgayKetThuc);
    }

    public class DuAnTrienKhaiViewModel
    {
        public int Id { get; set; }
        public int? DuAnId { get; set; }
        public int? DonViId { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public string NgayBatDauStr { get; set; }
        public string NgayKetThucStr { get; set; }
        public string HangMucsJson { get; set; }

        [NotMapped]
        public DateTime? NgayBatDau => DateTimeHelper.ConvertStrToDate(NgayBatDauStr);

        [NotMapped]
        public DateTime? NgayKetThuc => DateTimeHelper.ConvertStrToDate(NgayKetThucStr);

        [NotMapped]
        public List<DuAnTrienKhaiHangMucInfo> HangMucs
        {
            get
            {
                if (string.IsNullOrWhiteSpace(HangMucsJson))
                {
                    return new List<DuAnTrienKhaiHangMucInfo>();
                }

                try
                {
                    return JsonConvert.DeserializeObject<List<DuAnTrienKhaiHangMucInfo>>(HangMucsJson) ?? new List<DuAnTrienKhaiHangMucInfo>();
                }
                catch
                {
                    return new List<DuAnTrienKhaiHangMucInfo>();
                }
            }
        }
    }

    public class DuAnTrienKhaiHangMucInfo
    {
        public int Id { get; set; }
        public int? DuAnHangMucId { get; set; }
        public string TenHangMuc { get; set; }
        public string MoTa { get; set; }
        public int? ThuTu { get; set; }
        public string NgayBatDauStr { get; set; }
        public string NgayKetThucStr { get; set; }
        public int? TienDo { get; set; }
        public string GhiChu { get; set; }
        public bool? IsDeleted { get; set; }

        [NotMapped]
        public DateTime? NgayBatDau => DateTimeHelper.ConvertStrToDate(NgayBatDauStr);

        [NotMapped]
        public DateTime? NgayKetThuc => DateTimeHelper.ConvertStrToDate(NgayKetThucStr);
    }

    public class DuAnTrienKhaiQueryViewModel : QueryParamViewModel
    {
        public int? DuAnIdSearch { get; set; }
        public int? DonViIdSearch { get; set; }
    }
}
