using Dashboard.Common;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("ThongBao")]
    public class ThongBao
    {
        public ThongBao()
        {

        }

        public int Id { get; set; }
        public string NoiDung { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? NguoiNhanId { get; set; }
        public EnumLoaiThongBao? EnumLoaiThongBao { get; set; }
        public bool? IsDaXem { get; set; }
        public bool? IsDeleted { get; set; }
        public string ThongBaoInfoJson { get; set; }
        public int? CuocHopId { get; set; }
    }

    public class ThongBaoGridVo
    {
        public ThongBaoGridVo()
        {
            CuocHopId = 0;
        }


        public int Id { get; set; }
        public string NoiDung { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NgayTaoStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(NgayTao);
            }
        }
        public int? NguoiNhanId { get; set; }
        public string NguoiNhan { get; set; }
        public int? CuocHopId { get; set; }
        public bool? IsDaXem { get; set; }
        public EnumLoaiThongBao? EnumLoaiThongBao { get; set; }
        public string KhoangThoiGian
        {
            get
            {
                if (NgayTao != null)
                {
                    return DateTimeHelper.GetThoiGianTuongDoi(NgayTao.Value);
                }
                return "";
            }
        }

        public string ThongBaoInfoJson { get; set; }
        public ThongBaoThongTinJson ThongBaoThongTinJson
        {
            get
            {
                if (!string.IsNullOrEmpty(ThongBaoInfoJson))
                {
                    return JsonConvert.DeserializeObject<ThongBaoThongTinJson>(ThongBaoInfoJson);
                }

                return new ThongBaoThongTinJson();
            }
        }

        public string TieuDe
        {
            get
            {
                return !string.IsNullOrEmpty(ThongBaoLinkTieuDeVos.TieuDe) ?
                    ThongBaoLinkTieuDeVos.TieuDe : "Thông báo";
            }
        }

        public string Link
        {
            get
            {
                return !string.IsNullOrEmpty(ThongBaoLinkTieuDeVos.Link) ?
                    ThongBaoLinkTieuDeVos.Link : "/ThongBao";
            }
        }

        public ThongBaoLinkTieuDeVo ThongBaoLinkTieuDeVos
        {
            get
            {
                if (EnumLoaiThongBao == Entities.EnumLoaiThongBao.CongViecMoiCanXuLy)
                {
                    return new ThongBaoLinkTieuDeVo
                    {
                        Link = "/GiaoViecCanXuLy",
                        TieuDe = "Công việc mới cần xử lý"
                    };
                }
                else if (EnumLoaiThongBao == Entities.EnumLoaiThongBao.CongViecSapDenHan)
                {
                    return new ThongBaoLinkTieuDeVo
                    {
                        Link = "/GiaoViecCanXuLy",
                        TieuDe = "Công việc sắp đến hạn"
                    };
                }
                else if (EnumLoaiThongBao == Entities.EnumLoaiThongBao.ThongKeCongViecHangNgay)
                {
                    return new ThongBaoLinkTieuDeVo
                    {
                        Link = "/GiaoViecCanXuLy",
                        TieuDe = "Thống kê công việc hàng ngày"
                    };
                }
                else if (EnumLoaiThongBao == Entities.EnumLoaiThongBao.ThongKeCongViecPhongBan)
                {
                    return new ThongBaoLinkTieuDeVo
                    {
                        Link = "/GiaoViec",
                        TieuDe = "Thống kê công việc phòng ban"
                    };
                }
                else if (EnumLoaiThongBao == Entities.EnumLoaiThongBao.ThongKeCongViecTatCaPhongBan)
                {
                    return new ThongBaoLinkTieuDeVo
                    {
                        Link = "/GiaoViecThongKe",
                        TieuDe = "Thống kê công việc các phòng ban"
                    };
                }

                return new ThongBaoLinkTieuDeVo();
            }
        }
    }

    public class ThongBaoLinkTieuDeVo
    {
        public string Link { get; set; }
        public string TieuDe { get; set; }
    }

    public class ThongBaoThongTinJson
    {
        public int? GiaoViecId { get; set; }
        public int? GiaoViecThanhVienId { get; set; }
        public int? CuocHopId { get; set; }
    }

    public enum EnumLoaiThongBao : int
    {
        [Description("Công việc mới cần xử lý")]
        CongViecMoiCanXuLy = 1,
        [Description("Công việc đã thay đổi thông tin")]
        CongViecDaThayDoiThongTin = 2,
        [Description("Công việc của bạn đã bị xóa")]
        CongViecDaBiXoa = 3,
        [Description("Công việc của thành viên đã hoàn thành")]
        CongViecCuaThanhVienDaHoanThanh = 4,
        [Description("Công việc sắp đến hạn")]
        CongViecSapDenHan = 5,
        [Description("Thành viên cập nhật tiến trình")]
        ThanhVienCapNhatTienTrinh = 6,
        [Description("Nhắc nhở cuộc họp")]
        NhacNhoCuocHop = 7,
        [Description("Nhắc nhở cuộc họp trước 30 phút")]
        NhacNhoCuocHopTruoc30Phut = 8,
        [Description("Thống kê công việc hàng ngày cá nhân")]
        ThongKeCongViecHangNgay = 9,
        [Description("Thống kê công việc phòng ban cho lãnh đạo phòng")]
        ThongKeCongViecPhongBan = 10,
        [Description("Thống kê công việc các phòng ban cho lãnh đạo cơ quan")]
        ThongKeCongViecTatCaPhongBan = 11
    }
}
