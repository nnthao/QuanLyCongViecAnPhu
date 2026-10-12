using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DanhSachCongTrinh")]
    public partial class DanhSachCongTrinh
    {
        public DanhSachCongTrinh()
        {
        }

        public int Id { get; set; }
        public string TenCongTrinh { get; set; }
        public string Diadiem { get; set; }
        public int? LoaiCongTrinh { get; set; }
        public string NamXayDung { get; set; }
        public string ChuDauTu { get; set; }
        public string QuyMo { get; set; }
        public string DienTich { get; set; }
        public string ThongSoKiThuat { get; set; }
        public string GhiChu { get; set; }
        public decimal? MucNuocChet_MNC { get; set; }
        public decimal? MucNuocDangBinhThuong_MNDBT { get; set; }
        public decimal? MucNuocLuuThietKe_MNLTK { get; set; }
        public decimal? MucNuocLuuKiemTra { get; set; }
        public int? NamHoanThanhCongTrinh { get; set; }
        public int? LanNangCapSuaChua { get; set; }
        public int? NamNangCapSuaChuaGanNhat { get; set; }
        public decimal? DungTichChet_TrieuM3 { get; set; }
        public decimal? DungTichUngVoiMNDBT_TrieuM3 { get; set; }
        public decimal? DungTichUngVoiMNLTK_TrieuM3 { get; set; }
        public decimal? DungTichHuuIch_TrieuM3 { get; set; }
        public decimal? DungTichPhongLuu_TrieuM3 { get; set; }
        public decimal? DienTichLuuVuc_Km2 { get; set; }
        public decimal? DienTichTuoiThietKe_Ha { get; set; }
        public decimal? DienTichMatThoangUngVoiMNDBT_Ha { get; set; }
        public decimal? DienTichMatThoangUngVoiMNC_Ha { get; set; }
        public string XaPhuong { get; set; }
        public string DonViQuanLy { get; set; }
        public string NguoiDung { get; set; }
        // Đập Dâng fields
        public decimal? DienTichTuoiThucTe_Ha { get; set; }
        public decimal? CaoTrinhNguongPhanKhongTran_M { get; set; }
        public decimal? ChieuCaoDapLonNhatTrenCongTrinhPhanKhongTran_M { get; set; }
        public decimal? ChieuDaiPhanKhongTran_M { get; set; }
        public decimal? CaoTrinhNguongPhanTran_M { get; set; }
        public decimal? ChieuCaoDapLonNhatTrenCongTrinhPhanTran_M { get; set; }
        public decimal? ChieuDaiPhanTran_M { get; set; }
        public decimal? CotNuocThietKeTrenCongTrinh_M { get; set; }
        public decimal? LuuLuongThietKe { get; set; }
        public decimal? CaoTrinhNguongCongXaCat_M { get; set; }
        public decimal? KichThuocCongXaCat_Cm { get; set; }
        public string VatLieuKetCauCongXaCat { get; set; }
        public decimal? CaoTrinhNguongCongCapNuoc_M { get; set; }
        public decimal? LuuLuongThietKeCongCapNuoc { get; set; }
        public decimal? KichThuocCongCapNuoc_Cm { get; set; }
        public string VatLieuCongCapNuoc { get; set; }
        public string DonViQuanLyCongTrinh { get; set; }
        public string TinhTrangCongTrinh { get; set; }
        public decimal? KinhDo_Longitude { get; set; }
        public decimal? ViDo_Latitude { get; set; }
        public string TrangThai { get; set; }
        public int? ThuTuUuTien { get; set; }
        // Đập Ngăn Mặn fields
        public int? SoLanNangCapSuaChua { get; set; }
        public int? SoKhoang { get; set; }
        public decimal? KichThuocKhoang_M { get; set; }
        public decimal? KichThuocPhanTran_M { get; set; }
        public string TenSong { get; set; }
        public decimal? ChieuCaoDinh_M { get; set; }
        public decimal? DienTichNganMan_Ha { get; set; }
        public decimal? CaoTrinhDinhDapPhanKhongTran_M { get; set; }
        public decimal? MucNuocLonNhat_M { get; set; }
        public decimal? ChieuDaiDapPhanKhongTran_M { get; set; }
        public decimal? ChieuDaiDapPhanTran_M { get; set; }
        // Trạm Khí Tượng fields
        public string LoaiTram { get; set; }
        public string Song { get; set; }
        public string Thiessen { get; set; }
        // Công trình hư hỏng fields
        public string MucDoHuHong { get; set; }
        public DateTime? NgayPhatHienHuHong { get; set; }
        public string NguyenNhanHuHong { get; set; }
        public string TrangThaiXuLy { get; set; }
        // Các trường mới cho báo cáo hồ chứa
        public string DapChinh { get; set; }
        public int? SoDapPhu { get; set; }
        public string CongLayNuoc { get; set; }
        public string TranXaLuu { get; set; }
        public decimal? DungTichLongHoHienTai { get; set; }
        public decimal? KhaNangDapUngNhuCauNuocThangToi { get; set; }
        // Các trường chi tiết cho báo cáo hồ chứa
        public decimal? DapChinhHmax { get; set; }
        public decimal? DapChinhB { get; set; }
        public decimal? DapChinhL { get; set; }
        public string CongLayNuocKichThuoc { get; set; }
        public string CongLayNuocVatLieu { get; set; }
        public decimal? TranXaLuuCaoTrinh { get; set; }
        public decimal? TranXaLuuB { get; set; }
        public string TranXaLuuVatLieu { get; set; }
        public string TranXaLuuHinhThuc { get; set; }
        public int? NguoiXoaId { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class DanhSachCongTrinhViewModel
    {
        public int Id { get; set; }
        public string TenCongTrinh { get; set; }
        public string Diadiem { get; set; }
        public int? LoaiCongTrinh { get; set; }
        public string NamXayDung { get; set; }
        public string ChuDauTu { get; set; }
        public string QuyMo { get; set; }
        public string DienTich { get; set; }
        public string ThongSoKiThuat { get; set; }
        public string GhiChu { get; set; }
        public decimal? MucNuocChet_MNC { get; set; }
        public decimal? MucNuocDangBinhThuong_MNDBT { get; set; }
        public decimal? MucNuocLuuThietKe_MNLTK { get; set; }
        public decimal? MucNuocLuuKiemTra { get; set; }
        public int? NamHoanThanhCongTrinh { get; set; }
        public int? LanNangCapSuaChua { get; set; }
        public int? NamNangCapSuaChuaGanNhat { get; set; }
        public decimal? DungTichChet_TrieuM3 { get; set; }
        public decimal? DungTichUngVoiMNDBT_TrieuM3 { get; set; }
        public decimal? DungTichUngVoiMNLTK_TrieuM3 { get; set; }
        public decimal? DungTichHuuIch_TrieuM3 { get; set; }
        public decimal? DungTichPhongLuu_TrieuM3 { get; set; }
        public decimal? DienTichLuuVuc_Km2 { get; set; }
        public decimal? DienTichTuoiThietKe_Ha { get; set; }
        public decimal? DienTichMatThoangUngVoiMNDBT_Ha { get; set; }
        public decimal? DienTichMatThoangUngVoiMNC_Ha { get; set; }
        public string XaPhuong { get; set; }
        public string DonViQuanLy { get; set; }
        public string NguoiDung { get; set; }
        // Đập Dâng fields
        public decimal? DienTichTuoiThucTe_Ha { get; set; }
        public decimal? CaoTrinhNguongPhanKhongTran_M { get; set; }
        public decimal? ChieuCaoDapLonNhatTrenCongTrinhPhanKhongTran_M { get; set; }
        public decimal? ChieuDaiPhanKhongTran_M { get; set; }
        public decimal? CaoTrinhNguongPhanTran_M { get; set; }
        public decimal? ChieuCaoDapLonNhatTrenCongTrinhPhanTran_M { get; set; }
        public decimal? ChieuDaiPhanTran_M { get; set; }
        public decimal? CotNuocThietKeTrenCongTrinh_M { get; set; }
        public decimal? LuuLuongThietKe { get; set; }
        public decimal? CaoTrinhNguongCongXaCat_M { get; set; }
        public decimal? KichThuocCongXaCat_Cm { get; set; }
        public string VatLieuKetCauCongXaCat { get; set; }
        public decimal? CaoTrinhNguongCongCapNuoc_M { get; set; }
        public decimal? LuuLuongThietKeCongCapNuoc { get; set; }
        public decimal? KichThuocCongCapNuoc_Cm { get; set; }
        public string VatLieuCongCapNuoc { get; set; }
        public string DonViQuanLyCongTrinh { get; set; }
        public string TinhTrangCongTrinh { get; set; }
        public decimal? KinhDo_Longitude { get; set; }
        public decimal? ViDo_Latitude { get; set; }
        public string TrangThai { get; set; }
        public int? ThuTuUuTien { get; set; }
        // Đập Ngăn Mặn fields
        public int? SoLanNangCapSuaChua { get; set; }
        public int? SoKhoang { get; set; }
        public decimal? KichThuocKhoang_M { get; set; }
        public decimal? KichThuocPhanTran_M { get; set; }
        public string TenSong { get; set; }
        public decimal? ChieuCaoDinh_M { get; set; }
        public decimal? DienTichNganMan_Ha { get; set; }
        public decimal? CaoTrinhDinhDapPhanKhongTran_M { get; set; }
        public decimal? MucNuocLonNhat_M { get; set; }
        public decimal? ChieuDaiDapPhanKhongTran_M { get; set; }
        public decimal? ChieuDaiDapPhanTran_M { get; set; }
        // Trạm Khí Tượng fields
        public string LoaiTram { get; set; }
        public string Song { get; set; }
        public string Thiessen { get; set; }
        // Công trình hư hỏng fields
        public string MucDoHuHong { get; set; }
        public DateTime? NgayPhatHienHuHong { get; set; }
        public string NguyenNhanHuHong { get; set; }
        public string TrangThaiXuLy { get; set; }
        // Các trường mới cho báo cáo hồ chứa
        public string DapChinh { get; set; }
        public int? SoDapPhu { get; set; }
        public string CongLayNuoc { get; set; }
        public string TranXaLuu { get; set; }
        public decimal? DungTichLongHoHienTai { get; set; }
        public decimal? KhaNangDapUngNhuCauNuocThangToi { get; set; }
        // Các trường chi tiết cho báo cáo hồ chứa
        public decimal? DapChinhHmax { get; set; }
        public decimal? DapChinhB { get; set; }
        public decimal? DapChinhL { get; set; }
        public string CongLayNuocKichThuoc { get; set; }
        public string CongLayNuocVatLieu { get; set; }
        public decimal? TranXaLuuCaoTrinh { get; set; }
        public decimal? TranXaLuuB { get; set; }
        public string TranXaLuuVatLieu { get; set; }
        public string TranXaLuuHinhThuc { get; set; }
    }

    public class DanhSachCongTrinhGridVo
    {
        public int Id { get; set; }
        public string TenCongTrinh { get; set; }
        public string Diadiem { get; set; }
        public int? LoaiCongTrinh { get; set; }
        public string NamXayDung { get; set; }
        public string ChuDauTu { get; set; }
        public string QuyMo { get; set; }
        public string DienTich { get; set; }
        public string ThongSoKiThuat { get; set; }
        public string GhiChu { get; set; }
        public decimal? MucNuocChet_MNC { get; set; }
        public decimal? MucNuocDangBinhThuong_MNDBT { get; set; }
        public decimal? MucNuocLuuThietKe_MNLTK { get; set; }
        public decimal? MucNuocLuuKiemTra { get; set; }
        public int? NamHoanThanhCongTrinh { get; set; }
        public int? LanNangCapSuaChua { get; set; }
        public int? NamNangCapSuaChuaGanNhat { get; set; }
        public decimal? DungTichChet_TrieuM3 { get; set; }
        public decimal? DungTichUngVoiMNDBT_TrieuM3 { get; set; }
        public decimal? DungTichUngVoiMNLTK_TrieuM3 { get; set; }
        public decimal? DungTichHuuIch_TrieuM3 { get; set; }
        public decimal? DungTichPhongLuu_TrieuM3 { get; set; }
        public decimal? DienTichLuuVuc_Km2 { get; set; }
        public decimal? DienTichTuoiThietKe_Ha { get; set; }
        public decimal? DienTichMatThoangUngVoiMNDBT_Ha { get; set; }
        public decimal? DienTichMatThoangUngVoiMNC_Ha { get; set; }
        public string XaPhuong { get; set; }
        public string DonViQuanLy { get; set; }
        public string NguoiDung { get; set; }
        // Đập Dâng fields
        public decimal? DienTichTuoiThucTe_Ha { get; set; }
        public decimal? CaoTrinhNguongPhanKhongTran_M { get; set; }
        public decimal? ChieuCaoDapLonNhatTrenCongTrinhPhanKhongTran_M { get; set; }
        public decimal? ChieuDaiPhanKhongTran_M { get; set; }
        public decimal? CaoTrinhNguongPhanTran_M { get; set; }
        public decimal? ChieuCaoDapLonNhatTrenCongTrinhPhanTran_M { get; set; }
        public decimal? ChieuDaiPhanTran_M { get; set; }
        public decimal? CotNuocThietKeTrenCongTrinh_M { get; set; }
        public decimal? LuuLuongThietKe { get; set; }
        public decimal? CaoTrinhNguongCongXaCat_M { get; set; }
        public decimal? KichThuocCongXaCat_Cm { get; set; }
        public string VatLieuKetCauCongXaCat { get; set; }
        public decimal? CaoTrinhNguongCongCapNuoc_M { get; set; }
        public decimal? LuuLuongThietKeCongCapNuoc { get; set; }
        public decimal? KichThuocCongCapNuoc_Cm { get; set; }
        public string VatLieuCongCapNuoc { get; set; }
        public string DonViQuanLyCongTrinh { get; set; }
        public string TinhTrangCongTrinh { get; set; }
        public decimal? KinhDo_Longitude { get; set; }
        public decimal? ViDo_Latitude { get; set; }
        public string TrangThai { get; set; }
        public int? ThuTuUuTien { get; set; }
        // Đập Ngăn Mặn fields
        public int? SoLanNangCapSuaChua { get; set; }
        public int? SoKhoang { get; set; }
        public decimal? KichThuocKhoang_M { get; set; }
        public decimal? KichThuocPhanTran_M { get; set; }
        public string TenSong { get; set; }
        public decimal? ChieuCaoDinh_M { get; set; }
        public decimal? DienTichNganMan_Ha { get; set; }
        public decimal? CaoTrinhDinhDapPhanKhongTran_M { get; set; }
        public decimal? MucNuocLonNhat_M { get; set; }
        public decimal? ChieuDaiDapPhanKhongTran_M { get; set; }
        public decimal? ChieuDaiDapPhanTran_M { get; set; }
        // Trạm Khí Tượng fields
        public string LoaiTram { get; set; }
        public string Song { get; set; }
        public string Thiessen { get; set; }
        // Công trình hư hỏng fields
        public string MucDoHuHong { get; set; }
        public DateTime? NgayPhatHienHuHong { get; set; }
        public string NguyenNhanHuHong { get; set; }
        public string TrangThaiXuLy { get; set; }
        // Các trường mới cho báo cáo hồ chứa
        public string DapChinh { get; set; }
        public int? SoDapPhu { get; set; }
        public string CongLayNuoc { get; set; }
        public string TranXaLuu { get; set; }
        public decimal? DungTichLongHoHienTai { get; set; }
        public decimal? KhaNangDapUngNhuCauNuocThangToi { get; set; }
        // Các trường chi tiết cho báo cáo hồ chứa
        public decimal? DapChinhHmax { get; set; }
        public decimal? DapChinhB { get; set; }
        public decimal? DapChinhL { get; set; }
        public string CongLayNuocKichThuoc { get; set; }
        public string CongLayNuocVatLieu { get; set; }
        public decimal? TranXaLuuCaoTrinh { get; set; }
        public decimal? TranXaLuuB { get; set; }
        public string TranXaLuuVatLieu { get; set; }
        public string TranXaLuuHinhThuc { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
