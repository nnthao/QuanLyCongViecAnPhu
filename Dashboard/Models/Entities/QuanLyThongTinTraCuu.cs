using Dashboard.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dashboard.Models.Entities
{
    [Table("QuanLyThongTinTraCuu")]
    public partial class QuanLyThongTinTraCuu
    {
        public QuanLyThongTinTraCuu()
        {
            QuanLyThongTinTraCuuChiTiets = new List<QuanLyThongTinTraCuuChiTiet>();
            QuanLyThongTinTraCuuFiles = new List<QuanLyThongTinTraCuuFile>();
        }

        public int Id { get; set; }
        public string MaTraCuu { get; set; }
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public int? TrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? NguoiTaoId { get; set; }
        public DateTime? NgayXoa { get; set; }
        public int? NguoiXoaId { get; set; }
        public bool? IsDeleted { get; set; }
        public int? DanhMucTraCuuThongTinId { get; set; }

        public ICollection<QuanLyThongTinTraCuuChiTiet> QuanLyThongTinTraCuuChiTiets { get; set; }
        public ICollection<QuanLyThongTinTraCuuFile> QuanLyThongTinTraCuuFiles { get; set; }
    }



    public class QuanLyThongTinTraCuuViewModel
    {
        public QuanLyThongTinTraCuuViewModel()
        {
            FileDinhKems = new List<HttpPostedFileBase>();
        }


        public int Id { get; set; }
        public string MaTraCuu { get; set; }
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public DateTime? NgayHieuLuc
        {
            get
            {
                return DateTimeHelper.ConvertStrToDate(NgayHieuLucStr);
            }
        }
        public string NgayHieuLucStr { get; set; }
        public int? TrangThai { get; set; }
        public DateTime? NgayTao
        {
            get
            {
                return DateTime.Now;
            }
        }
        public string NgayTaoStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(NgayTao);
            }
        }
        public int? NguoiTaoId { get; set; }
        public DateTime? NgayXoa
        {
            get
            {
                return DateTime.Now;
            }
        }
        public int? NguoiXoaId { get; set; }
        public int? CurrentUserId { get; set; }

        public int? DanhMucTraCuuThongTinId { get; set; }
        public string VanBanLienQuanIds { get; set; }
        public List<int> VanBanLienQuanIdInts
        {
            get
            {
                return !string.IsNullOrEmpty(VanBanLienQuanIds) ?
                    VanBanLienQuanIds.Split(',').ToList().Where(o => !string.IsNullOrEmpty(o)).Select(o => Convert.ToInt32(o)).ToList() : new List<int>();
            }
        }
        public string TaiLieuDinhKemIds { get; set; }
        public List<int> TaiLieuDinhKemIdInts
        {
            get
            {
                return !string.IsNullOrEmpty(TaiLieuDinhKemIds) ?
                    TaiLieuDinhKemIds.Split(',').ToList().Where(o => !string.IsNullOrEmpty(o)).Select(o => Convert.ToInt32(o)).ToList() : new List<int>();
            }
        }
        public List<HttpPostedFileBase> FileDinhKems { get; set; }
    }

    public class QuanLyThongTinTraCuuGridVo
    {
        public int Id { get; set; }
        public string MaTraCuu { get; set; }
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public string NgayHieuLucStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(NgayHieuLuc);
            }
        }
        public int? TrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NgayTaoStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(NgayTao);
            }
        }
        public int? NguoiTaoId { get; set; }
        public DateTime? NgayXoa { get; set; }
        public string NgayXoaStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(NgayXoa);
            }
        }
        public int? NguoiXoaId { get; set; }
        public bool? IsDeleted { get; set; }

        public int? DanhMucTraCuuThongTinId { get; set; }
        public string DanhMucTraCuuThongTin { get; set; }
    }
}
