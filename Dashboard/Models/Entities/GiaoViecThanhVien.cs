using Dashboard.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("GiaoViecThanhVien")]
    public class GiaoViecThanhVien
    {
        public GiaoViecThanhVien()
        {
            GiaoViecThanhVienTienTrinhs = new List<GiaoViecThanhVienTienTrinh>();
        }

        public int Id { get; set; }
        public int? GiaoViecId { get; set; }
        public int? ThanhVienId { get; set; }
        public DateTime? HanXuLy { get; set; }
        public string NoiDungNhiemVu { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsHoanThanh { get; set; }
        public DateTime? NgayHoanThanh { get; set; }
        public int? GiaoViecThanhVienChuyenGiaoId { get; set; }
        public DateTime? NgayChuyenGiao { get; set; }
        public int? SoGioUocTinh { get; set; }
        public DateTime? NgayNhacHan { get; set; }
        public int? VaiTroXuLyId { get; set; }
        public int? NguoiTheoDoiChiDaoId { get; set; }
        public int? TrangThai { get; set; }
        public DateTime? NgayXacNhanHoanThanh { get; set; }
        public int? NguoiDanhGiaId { get; set; }
        public DateTime? NgayDanhGia { get; set; }
        public int? KetQuaDanhGia { get; set; }
        public string YKienDanhGia { get; set; }

        public virtual VaiTroXuLy VaiTroXuLy { get; set; }

        public virtual GiaoViec GiaoViec { get; set; }

        public virtual ICollection<GiaoViecThanhVienTienTrinh> GiaoViecThanhVienTienTrinhs { get; set; }

    }

    public class GiaoViecThanhVienGridVo
    {
        public GiaoViecThanhVienGridVo()
        {
            GiaoViecThanhVienTienTrinhs = new List<GiaoViecThanhVienTienTrinhGridVo>();
            GiaoViecThanhViens = new List<GiaoViecThanhVienGridVo>();
        }
        public int Id { get; set; }
        public int GiaoViecId { get; set; }
        public string ThanhVien { get; set; }
        public int? ThanhVienId { get; set; }
        public DateTime? HanXuLy { get; set; }
        public string HanXuLyStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(HanXuLy);
            }
        }
        public string NoiDungNhiemVu { get; set; }
        public int? SoGioUocTinh { get; set; }
        public int? VaiTroXuLyId { get; set; }
        public string TenVaiTroXuLy { get; set; }

        public int? NguoiGiaoId { get; set; }
        public string NguoiGiao { get; set; }
        public string TenCongViec { get; internal set; }
        public string NoiDung { get; set; }
        public string NoiDungBaoCao { get; set; }
        public string GhiChu { get; internal set; }
        public DateTime? NgayTao { get; internal set; }
        public string NgayTaoStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(NgayTao);
            }
        }

        public DateTime? NgayHoanThanh { get; set; }
        public string NgayHoanThanhStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(NgayHoanThanh);
            }
        }

        public DateTime NgayHienTai => DateTime.Now;
        public int SoNgayConLai
        {
            get
            {
                if (HanXuLy.HasValue)
                {
                    var ngayHienTai = DateTime.Now.Date;
                    var ngayHan = HanXuLy.Value.Date;
                    return (ngayHan - ngayHienTai).Days;
                }
                return 0;
            }
        }

        public bool? IsHoanThanh { get; set; }
        public int PhanTramHoanThanh { get; set; }
        public string TrangThaiHoanThanh
        {
            get
            {
                var dauNgayHienTai = DateTimeHelper.BeginningOfDay(DateTime.Now);
                var dauNgayHan = DateTimeHelper.BeginningOfDay(HanXuLy.GetValueOrDefault());

                if (IsHoanThanh == true && NgayHoanThanh.GetValueOrDefault() < dauNgayHan)
                {
                    return "Sớm hạn";
                }
                else if (IsHoanThanh == true && NgayHoanThanh.GetValueOrDefault().Date == HanXuLy.GetValueOrDefault().Date)
                {
                    return "Đúng hạn";
                }
                else if (IsHoanThanh != true && HanXuLy.GetValueOrDefault() >= dauNgayHienTai)
                {
                    return "Còn hạn";
                }
                else
                {
                    return "Trễ hạn";
                }
            }
        }

        public string TrangThaiConHan
        {
            get
            {
                if (SoNgayConLai < 0 && IsHoanThanh != true)
                {
                    return "Trễ hạn";
                }
                else if (SoNgayConLai >= 0 && IsHoanThanh != true)
                {
                    return "Còn hạn";
                }

                return "Đúng hạn";
            }
        }
        public List<GiaoViecThanhVienTienTrinhGridVo> GiaoViecThanhVienTienTrinhs { get; set; }
        public List<GiaoViecThanhVienTienTrinhFileGridVo> GiaoViecThanhVienTienTrinhFiles { get; set; }
        public DateTime? NgayGiaoViec { get; set; }
        public string NgayGiaoViecStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(NgayGiaoViec);
            }
        }

        public string PhongBan { get; internal set; }
        public int PhongBanId { get; internal set; }
        public DateTime? NgayChuyenGiao { get; set; }
        public string NgayChuyenGiaoStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateOnly(NgayChuyenGiao);
            }
        }
        public int? GiaoViecThanhVienChuyenGiaoId { get; set; }
        public string NguoiChuyenGiao { get; set; }
        public List<GiaoViecThanhVienGridVo> GiaoViecThanhViens { get; set; }
        public bool? IsTuGiao { get; set; }
        public int? NguoiTheoDoiChiDaoId { get; set; }
        public int? TrangThai { get; set; }
        public DateTime? NgayXacNhanHoanThanh { get; set; }
        public int? NguoiDanhGiaId { get; set; }
        public DateTime? NgayDanhGia { get; set; }
        public int? KetQuaDanhGia { get; set; }
        public string YKienDanhGia { get; set; }
        public string TenTrangThai
        {
            get
            {
                if (TrangThai == (int)EnumTrangThaiGiaoViec.HoanThanh || (IsHoanThanh == true && !TrangThai.HasValue))
                {
                    return "Đã hoàn thành";
                }
                if (TrangThai == (int)EnumTrangThaiGiaoViec.ChoDanhGia)
                {
                    return "Chờ đánh giá";
                }
                if (TrangThai == (int)EnumTrangThaiGiaoViec.ChuaDat)
                {
                    return "Chưa đạt";
                }
                return "Chưa hoàn thành";
            }
        }
        public string NgayXacNhanHoanThanhStr => DateTimeHelper.ConvertDateTimeToDateTime(NgayXacNhanHoanThanh);
        public string NguoiDanhGia { get; set; }
        public string NgayDanhGiaStr => DateTimeHelper.ConvertDateTimeToDateTime(NgayDanhGia);
        public string TenKetQuaDanhGia => KetQuaDanhGia == (int)EnumKetQuaDanhGia.Dat ? "Đạt" : (KetQuaDanhGia == (int)EnumKetQuaDanhGia.ChuaDat ? "Chưa đạt" : "");
        public bool CanDanhGia { get; set; }
        public bool CanHuyDanhGia { get; set; }
        public string TenNguoiTheoDoiChiDao { get; set; }
        public string NguoiTheoDoiChiDao { get; set; }
    }

    public class GiaoViecNguoiNhanChuyenGiaoVo
    {
        public int GiaoViecThanhVienId { get; set; }
        public List<GiaoViecThanhVienGridVo> NguoiNhans { get; set; }

        public GiaoViecNguoiNhanChuyenGiaoVo()
        {
            NguoiNhans = new List<GiaoViecThanhVienGridVo>();
        }
    }

    public class NguoiChuyenGiaoInfo
    {
        public int GiaoViecThanhVienChuyenGiaoId { get; set; }
        public string TenNguoiChuyenGiao { get; set; }
    }

    public class GiaoViecThanhVienTienTrinhInfo
    {
        public int? GiaoViecThanhVienId { get; set; }
        public int? PhanTramHoanThanh { get; set; }
    }

    public class GiaoViecThanhVienTheoGiaoViecInfo
    {
        public int GiaoViecId { get; set; }
        public List<GiaoViecThanhVienGridVo> GiaoViecThanhViens { get; set; }

        public GiaoViecThanhVienTheoGiaoViecInfo()
        {
            GiaoViecThanhViens = new List<GiaoViecThanhVienGridVo>();
        }
    }

    public class GiaoViecThanhVienViewModel
    {
        public int Id { get; set; }
        public string ThanhVien { get; set; }
        public int? ThanhVienId { get; set; }
        public string HanXuLyStr { get; set; }
        public DateTime? HanXuLy
        {
            get
            {
                return DateTimeHelper.ConvertStrToDate(HanXuLyStr);
            }
        }
        public string NoiDungNhiemVu { get; set; }
        public int? SoGioUocTinh { get; set; }
        public int? VaiTroXuLyId { get; set; }
        public int? NguoiTheoDoiChiDaoId { get; set; }
        public int? TrangThai { get; set; }
        public DateTime? NgayXacNhanHoanThanh { get; set; }
        public int? NguoiDanhGiaId { get; set; }
        public DateTime? NgayDanhGia { get; set; }
        public int? KetQuaDanhGia { get; set; }
        public string YKienDanhGia { get; set; }
    }
}