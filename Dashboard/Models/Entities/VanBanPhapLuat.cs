using Dashboard.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;

namespace Dashboard.Models.Entities
{
    [Table("VanBanPhapLuat")]
    public partial class VanBanPhapLuat
    {
        public VanBanPhapLuat()
        {
            VanBanPhapLuatFiles = new List<VanBanPhapLuatFile>();
        }

        public int Id { get; set; }
        public string SoKiHieu { get; set; }
        public string TrichYeu { get; set; }
        public int? HinhThucVanBanId { get; set; }
        public int? CoQuanBanHanhId { get; set; }
        public int? DoKhanId { get; set; }
        public DateTime? NgayBanHanh { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public int? NguoiXoaId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<VanBanPhapLuatFile> VanBanPhapLuatFiles { get; set; }
    }

    public class VanBanPhapLuatViewModel
    {
        public VanBanPhapLuatViewModel()
        {
            FileDinhKems = new List<HttpPostedFileBase>();
        }
        public int Id { get; set; }
        public string SoKiHieu { get; set; }
        public string TrichYeu { get; set; }
        public int? HinhThucVanBanId { get; set; }
        public int? CoQuanBanHanhId { get; set; }
        public int? DoKhanId { get; set; }
        public string NgayBanHanhStr { get; set; }
        public DateTime? NgayBanHanh
        {
            get
            {
                return DateTimeHelper.ConvertStrToDate(NgayBanHanhStr);
            }
        }

        public string TaiLieuDinhKemIds { get; set; }
        public List<int> TaiLieuDinhKemIdInts
        {
            get
            {
                if (!string.IsNullOrEmpty(TaiLieuDinhKemIds))
                {
                    return TaiLieuDinhKemIds.Split(',').Where(c => !string.IsNullOrEmpty(c))
                        .Select(o => Convert.ToInt32(o)).ToList();
                }

                return new List<int>();
            }
        }

        public List<HttpPostedFileBase> FileDinhKems { get; set; }
        public int CurrentUserId { get; set; }
        public DateTime? NgayTao
        {
            get
            {
                return DateTime.Now;
            }
        }
    }

    public class VanBanPhapLuatGridVo
    {
        public int Id { get; set; }
        public string SoKiHieu { get; set; }
        public string TrichYeu { get; set; }
        public int? HinhThucVanBanId { get; set; }
        public string HinhThucVanBan { get; set; }
        public int? CoQuanBanHanhId { get; set; }
        public string CoQuanBanHanh { get; set; }
        public int? DoKhanId { get; set; }
        public string DoKhan { get; set; }
        public DateTime? NgayBanHanh { get; set; }
        public string NgayBanHanhStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(NgayBanHanh);
            }
        }
        public bool? IsDeleted { get; set; }
    }
}


