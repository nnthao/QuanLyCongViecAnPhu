using Dashboard.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Dashboard.Models.Entities
{
    [Table("GiaoViecThanhVienTienTrinh")]
    public class GiaoViecThanhVienTienTrinh
    {
        public GiaoViecThanhVienTienTrinh()
        {
            GiaoViecThanhVienTienTrinhFiles = new List<GiaoViecThanhVienTienTrinhFile>();
        }

        public int Id { get; set; }
        public int? GiaoViecThanhVienId { get; set; }
        public string NoiDungXuLy { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool? IsDeleted { get; set; }
        public int? PhanTramHoanThanh { get; set; }

        public virtual GiaoViecThanhVien GiaoViecThanhVien { get; set; }
        public ICollection<GiaoViecThanhVienTienTrinhFile> GiaoViecThanhVienTienTrinhFiles { get; set; }
    }

    public class GiaoViecThanhVienTienTrinhViewModel
    {
        public GiaoViecThanhVienTienTrinhViewModel()
        {
            FileDinhKems = new List<HttpPostedFileBase>();
        }

        public int Id { get; set; }
        public int? GiaoViecThanhVienId { get; set; }
        public string NoiDungXuLy { get; set; }

        public List<HttpPostedFileBase> FileDinhKems { get; set; }
        public int? PhanTramHoanThanh { get; set; }
    }

    public class HuyXacNhanHoanThanhViewModel
    {
        public int GiaoViecThanhVienId { get; set; }
        public int CurrentUserId { get; set; }
    }

    public class GiaoViecThanhVienTienTrinhGridVo
    {
        public GiaoViecThanhVienTienTrinhGridVo()
        {
            Files = new List<GiaoViecThanhVienTienTrinhFileGridVo>();
        }

        public int Id { get; set; }
        public int? GiaoViecThanhVienId { get; set; }
        public string NoiDungXuLy { get; set; }
        public DateTime? NgayTao { get; set; }

        public string NgayTaoStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(NgayTao);
            }
        }

        public int? PhanTramHoanThanh { get; set; }
        public int? CurrentUserId { get; set; }

        public List<GiaoViecThanhVienTienTrinhFileGridVo> Files { get; set; }
        public int? ThanhVienId { get; internal set; }
        public string ThanhVien { get; internal set; }
    }

    [Table("GiaoViecThanhVienTienTrinhFile")]
    public class GiaoViecThanhVienTienTrinhFile
    {
        public int Id { get; set; }
        public int? GiaoViecThanhVienTienTrinhId { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual GiaoViecThanhVienTienTrinh GiaoViecThanhVienTienTrinh { get; set; }
    }

    public class GiaoViecThanhVienTienTrinhFileGridVo
    {
        public int Id { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
    }

    public class GiaoViecThanhVienTienTrinhTheoGiaoViecThanhVienInfo
    {
        public int GiaoViecThanhVienId { get; set; }
        public List<GiaoViecThanhVienTienTrinhGridVo> TienTrinhs { get; set; }

        public GiaoViecThanhVienTienTrinhTheoGiaoViecThanhVienInfo()
        {
            TienTrinhs = new List<GiaoViecThanhVienTienTrinhGridVo>();
        }
    }
}

