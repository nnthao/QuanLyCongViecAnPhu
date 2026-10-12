using Dashboard.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dashboard.Models.Entities
{
    [Table("GiaoViec")]
    public class GiaoViec
    {
        public GiaoViec()
        {
            GiaoViecThanhViens = new List<GiaoViecThanhVien>();
            GiaoViecFiles = new List<GiaoViecFile>();
            GiaoViecVanBanLienQuans = new List<GiaoViecVanBanLienQuan>();
        }

        public int Id { get; set; }
        public string TenCongViec { get; set; }
        public string NoiDung { get; set; }
        public string GhiChu { get; set; }
        public int? NguoiGiaoId { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsHoanThanh { get; set; }
        public DateTime? HanXuLy { get; set; }
        public bool? IsTuGiao { get; set; }
        public string IofficeNumber { get; set; }
        public string VanBanId { get; set; }

        public ICollection<GiaoViecThanhVien> GiaoViecThanhViens { get; set; }
        public ICollection<GiaoViecFile> GiaoViecFiles { get; set; }
        public ICollection<GiaoViecVanBanLienQuan> GiaoViecVanBanLienQuans { get; set; }
    }

    public class GiaoViecViewModel
    {
        public GiaoViecViewModel()
        {
            FileDinhKems = new List<HttpPostedFileBase>();
        }
        public int Id { get; set; }
        public string TenCongViec { get; set; }
        public string NoiDung { get; set; }
        public string GhiChu { get; set; }
        public string IofficeNumber { get; set; }
        public string VanBanId { get; set; }
        public int? CurrentUserId { get; set; }

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

        public string IofficeFileStr { get; set; }
        public List<IofficeFileAttachmentViewModel> IofficeFiles
        {
            get
            {
                if (!string.IsNullOrEmpty(IofficeFileStr))
                {
                    return JsonConvert.DeserializeObject<List<IofficeFileAttachmentViewModel>>(IofficeFileStr) ?? new List<IofficeFileAttachmentViewModel>();
                }

                return new List<IofficeFileAttachmentViewModel>();
            }
        }

        public string ThanhVienStr { get; set; }
        public List<GiaoViecThanhVienViewModel> ThanhViens
        {
            get
            {
                if (!string.IsNullOrEmpty(ThanhVienStr))
                {
                    return JsonConvert.DeserializeObject<List<GiaoViecThanhVienViewModel>>(ThanhVienStr);
                }

                return new List<GiaoViecThanhVienViewModel>();
            }
        }
    }

    public class IofficeFileAttachmentViewModel
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
    }

    public class GiaoViecGridVo
    {
        public GiaoViecGridVo()
        {
            GiaoViecThanhViens = new List<GiaoViecThanhVienGridVo>();
            GiaoViecFiles = new List<GiaoViecFileGridVo>();
            GiaoViecVanBanLienQuans = new List<GiaoViecVanBanLienQuanGridVo>();
        }

        public int Id { get; set; }
        public string TenCongViec { get; set; }
        public string NoiDung { get; set; }
        public string GhiChu { get; set; }
        public int? NguoiGiaoId { get; set; }
        public string NguoiGiao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NgayTaoStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(NgayTao);
            }
        }
        public bool? IsDeleted { get; set; }

        public List<GiaoViecThanhVienGridVo> GiaoViecThanhViens { get; set; }
        public List<GiaoViecFileGridVo> GiaoViecFiles { get; set; }
        public List<GiaoViecVanBanLienQuanGridVo> GiaoViecVanBanLienQuans { get; set; }

        public string ThanhVienStrs
        {
            get
            {
                return string.Join("; ", GiaoViecThanhViens.Select(o => o.ThanhVien));
            }
        }

        public string NoiDungNhiemVuStrs
        {
            get
            {
                if (GiaoViecThanhViens == null || !GiaoViecThanhViens.Any()) return string.Empty;
                var listNoiDung = GiaoViecThanhViens
                    .Where(o => !string.IsNullOrWhiteSpace(o.NoiDungNhiemVu))
                    .Select(o => o.NoiDungNhiemVu)
                    .Distinct()
                    .ToList();
                return string.Join("; ", listNoiDung);
            }
        }

        //Noi dung và hạn sử lý
        public string NoiDungThanhVienCanXuLy { get; set; }
        public DateTime? HanXuLyThanhVienCanXuLy { get; set; }
        public string HanXuLyThanhVienCanXuLyStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(HanXuLyThanhVienCanXuLy);
            }
        }
        public int? SoGioUocTinhThanhVienCanXuLy { get; set; }
        public string TenVaiTroXuLyThanhVienCanXuLy { get; set; }
        public bool? IsCanDinhKemFileKhiXacNhanHoanThanhThanhVienCanXuLy { get; set; }
        public int? NguoiTheoDoiChiDaoThanhVienCanXuLyId { get; set; }
        public string TenNguoiTheoDoiChiDaoThanhVienCanXuLy { get; set; }
        public string NguoiTheoDoiChiDaoThanhVienCanXuLy { get; set; }

        public bool? IsHoanThanh { get; set; }
        public bool? IsHoanThanhThanhVienCanXuLy { get; set; }
        public bool? IsTuGiao { get; set; }
        public string IofficeNumber { get; set; }
        public string VanBanId { get; set; }
    }

    public class GiaoViecFormQueryViewModel
    {
        public int Id { get; set; }
        public string SoKyHieu { get; set; }
        public string TrichYeu { get; set; }
        public string IofficeNumber { get; set; }
        public string VanBanId { get; set; }
        public string Loai { get; set; }
        public string NoiDung { get; set; }
        public string GhiChu { get; set; }
    }
}

