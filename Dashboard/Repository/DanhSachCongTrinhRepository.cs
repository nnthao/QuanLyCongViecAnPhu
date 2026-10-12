using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Models.Validates;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface IDanhSachCongTrinhRepository
    {
        List<LookupViewModel> DanhSachCongTrinhLookup(LookupQueryViewModel lookupQueryViewModel, int? loaiCongTrinh = null);
        DataTableResult<DanhSachCongTrinhGridVo> GetAll(QueryParamViewModel queryParams);
        DanhSachCongTrinhGridVo GetById(int id);
        DanhSachCongTrinhGridVo GetByIdWithHuHong(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(DanhSachCongTrinhViewModel danhSachCongTrinhViewModel);
        dynamic CapNhatThongTinHuHong(DanhSachCongTrinhViewModel danhSachCongTrinhViewModel);
        bool Xoa(int Id);
    }

    public class DanhSachCongTrinhRepository : IDanhSachCongTrinhRepository
    {
        private readonly IContext _context;
        private readonly IDanhSachCongTrinhValidate _danhSachCongTrinhValidate;

        public DanhSachCongTrinhRepository(IContext context, IDanhSachCongTrinhValidate danhSachCongTrinhValidate)
        {
            _context = context;
            _danhSachCongTrinhValidate = danhSachCongTrinhValidate;
        }
        //required
        private readonly string sortStringDefault = "TenCongTrinh asc";
        private readonly List<string> searchCol = new List<string> { "TenCongTrinh", "Diadiem", "ChuDauTu", "QuyMo", "DienTich", "ThongSoKiThuat" };
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DanhSachCongTrinhViewModel, DanhSachCongTrinh>()
                .AfterMap((s, d) =>
                {
                    
                });
        }, NullLoggerFactory.Instance).CreateMapper();

        public DataTableResult<DanhSachCongTrinhGridVo> GetAll(QueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.DanhSachCongTrinhs.AsNoTracking();

            if (queryParams.DaXoaSearch == false)
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            // Filter theo loại công trình
            if (!string.IsNullOrEmpty(queryParams.loaiCongTrinh))
            {
                if (int.TryParse(queryParams.loaiCongTrinh, out int loaiCongTrinh))
                {
                    query = query.Where(o => o.LoaiCongTrinh == loaiCongTrinh);
                }
            }

            // Filter theo trạng thái xử lý
            if (!string.IsNullOrEmpty(queryParams.trangThai))
            {
                query = query.Where(o => o.TrangThaiXuLy == queryParams.trangThai);
            }

            // Filter theo ngày phát hiện
            if (queryParams.TuNgay.HasValue)
            {
                query = query.Where(o => o.NgayPhatHienHuHong >= queryParams.TuNgay.Value);
            }

            if (queryParams.DenNgay.HasValue)
            {
                query = query.Where(o => o.NgayPhatHienHuHong <= queryParams.DenNgay.Value);
            }

            // Filter theo Xã/Phường
            if (!string.IsNullOrEmpty(queryParams.xaPhuong))
            {
                query = query.Where(o => o.XaPhuong == queryParams.xaPhuong);
            }

            // Filter theo Đơn vị quản lý
            if (!string.IsNullOrEmpty(queryParams.donViQuanLy))
            {
                query = query.Where(o => o.DonViQuanLy == queryParams.donViQuanLy);
            }

            // Filter theo Phân loại hồ
            if (!string.IsNullOrEmpty(queryParams.phanLoaiHo))
            {
                // Logic phân loại hồ dựa trên dung tích ứng với MNDBT
                switch (queryParams.phanLoaiHo.ToLower())
                {
                    case "nho":
                        query = query.Where(o => o.DungTichUngVoiMNDBT_TrieuM3 < 10);
                        break;
                    case "vua":
                        query = query.Where(o => o.DungTichUngVoiMNDBT_TrieuM3 >= 10 && o.DungTichUngVoiMNDBT_TrieuM3 < 100);
                        break;
                    case "lon":
                        query = query.Where(o => o.DungTichUngVoiMNDBT_TrieuM3 >= 100);
                        break;
                }
            }

            var dateTimeCurrent = DateTime.Now;
            var data = query.Select(cv => new DanhSachCongTrinhGridVo
            {
                Id = cv.Id,
                TenCongTrinh = cv.TenCongTrinh,
                Diadiem = cv.Diadiem,
                LoaiCongTrinh = cv.LoaiCongTrinh,
                NamXayDung = cv.NamXayDung,
                ChuDauTu = cv.ChuDauTu,
                QuyMo = cv.QuyMo,
                DienTich = cv.DienTich,
                ThongSoKiThuat = cv.ThongSoKiThuat,
                GhiChu = cv.GhiChu,
                // Công trình hư hỏng fields
                MucDoHuHong = cv.MucDoHuHong,
                NgayPhatHienHuHong = cv.NgayPhatHienHuHong,
                NguyenNhanHuHong = cv.NguyenNhanHuHong,
                TrangThaiXuLy = cv.TrangThaiXuLy,
                // Các trường báo cáo hồ chứa: chỉ map những cột chắc chắn đã có
                NamHoanThanhCongTrinh = cv.NamHoanThanhCongTrinh,
                DienTichLuuVuc_Km2 = cv.DienTichLuuVuc_Km2,
                DienTichTuoiThietKe_Ha = cv.DienTichTuoiThietKe_Ha,
                DienTichTuoiThucTe_Ha = cv.DienTichTuoiThucTe_Ha,
                DungTichUngVoiMNDBT_TrieuM3 = cv.DungTichUngVoiMNDBT_TrieuM3,
                // Các trường chi tiết - để null (UI hiển thị trống) nếu DB chưa có cột
                DapChinhHmax = (decimal?)null,
                DapChinhB = (decimal?)null,
                DapChinhL = (decimal?)null,
                CongLayNuocKichThuoc = (string)null,
                CongLayNuocVatLieu = (string)null,
                TranXaLuuCaoTrinh = (decimal?)null,
                TranXaLuuB = (decimal?)null,
                TranXaLuuVatLieu = (string)null,
                TranXaLuuHinhThuc = (string)null,
                DungTichLongHoHienTai = (decimal?)null,
                KhaNangDapUngNhuCauNuocThangToi = (decimal?)null,
                TrangThai = cv.TrangThai,
                XaPhuong = cv.XaPhuong,
                DonViQuanLy = cv.DonViQuanLy,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<DanhSachCongTrinhGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(DanhSachCongTrinhViewModel danhSachCongTrinhViewModel)
        {
            var errors = _danhSachCongTrinhValidate.Validate(danhSachCongTrinhViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (danhSachCongTrinhViewModel.Id == 0)
            {
                var danhSachCongTrinh = _iMapper.Map<DanhSachCongTrinhViewModel, DanhSachCongTrinh>(danhSachCongTrinhViewModel);
                danhSachCongTrinh.NgayTao = DateTime.Now;
                danhSachCongTrinh.IsDeleted = false;
                _context.DanhSachCongTrinhs.Add(danhSachCongTrinh);
                _context.SaveChanges();
                return new { Id = danhSachCongTrinh.Id };
            }
            else
            {
                var danhSachCongTrinh = _context.DanhSachCongTrinhs.FirstOrDefault(o => o.Id == danhSachCongTrinhViewModel.Id);
                if (danhSachCongTrinh != null)
                {
                    danhSachCongTrinh.TenCongTrinh = danhSachCongTrinhViewModel.TenCongTrinh;
                    danhSachCongTrinh.Diadiem = danhSachCongTrinhViewModel.Diadiem;
                    danhSachCongTrinh.LoaiCongTrinh = danhSachCongTrinhViewModel.LoaiCongTrinh;
                    danhSachCongTrinh.NamXayDung = danhSachCongTrinhViewModel.NamXayDung;
                    danhSachCongTrinh.ChuDauTu = danhSachCongTrinhViewModel.ChuDauTu;
                    danhSachCongTrinh.QuyMo = danhSachCongTrinhViewModel.QuyMo;
                    danhSachCongTrinh.DienTich = danhSachCongTrinhViewModel.DienTich;
                    danhSachCongTrinh.ThongSoKiThuat = danhSachCongTrinhViewModel.ThongSoKiThuat;
                    danhSachCongTrinh.GhiChu = danhSachCongTrinhViewModel.GhiChu;
                    // Hồ fields
                    danhSachCongTrinh.MucNuocChet_MNC = danhSachCongTrinhViewModel.MucNuocChet_MNC;
                    danhSachCongTrinh.MucNuocDangBinhThuong_MNDBT = danhSachCongTrinhViewModel.MucNuocDangBinhThuong_MNDBT;
                    danhSachCongTrinh.MucNuocLuuThietKe_MNLTK = danhSachCongTrinhViewModel.MucNuocLuuThietKe_MNLTK;
                    danhSachCongTrinh.MucNuocLuuKiemTra = danhSachCongTrinhViewModel.MucNuocLuuKiemTra;
                    danhSachCongTrinh.NamHoanThanhCongTrinh = danhSachCongTrinhViewModel.NamHoanThanhCongTrinh;
                    danhSachCongTrinh.LanNangCapSuaChua = danhSachCongTrinhViewModel.LanNangCapSuaChua;
                    danhSachCongTrinh.NamNangCapSuaChuaGanNhat = danhSachCongTrinhViewModel.NamNangCapSuaChuaGanNhat;
                    danhSachCongTrinh.DungTichChet_TrieuM3 = danhSachCongTrinhViewModel.DungTichChet_TrieuM3;
                    danhSachCongTrinh.DungTichUngVoiMNDBT_TrieuM3 = danhSachCongTrinhViewModel.DungTichUngVoiMNDBT_TrieuM3;
                    danhSachCongTrinh.DungTichUngVoiMNLTK_TrieuM3 = danhSachCongTrinhViewModel.DungTichUngVoiMNLTK_TrieuM3;
                    danhSachCongTrinh.DungTichHuuIch_TrieuM3 = danhSachCongTrinhViewModel.DungTichHuuIch_TrieuM3;
                    danhSachCongTrinh.DungTichPhongLuu_TrieuM3 = danhSachCongTrinhViewModel.DungTichPhongLuu_TrieuM3;
                    danhSachCongTrinh.DienTichLuuVuc_Km2 = danhSachCongTrinhViewModel.DienTichLuuVuc_Km2;
                    danhSachCongTrinh.DienTichTuoiThietKe_Ha = danhSachCongTrinhViewModel.DienTichTuoiThietKe_Ha;
                    danhSachCongTrinh.DienTichMatThoangUngVoiMNDBT_Ha = danhSachCongTrinhViewModel.DienTichMatThoangUngVoiMNDBT_Ha;
                    danhSachCongTrinh.DienTichMatThoangUngVoiMNC_Ha = danhSachCongTrinhViewModel.DienTichMatThoangUngVoiMNC_Ha;
                    danhSachCongTrinh.XaPhuong = danhSachCongTrinhViewModel.XaPhuong;
                    danhSachCongTrinh.DonViQuanLy = danhSachCongTrinhViewModel.DonViQuanLy;
                    danhSachCongTrinh.NguoiDung = danhSachCongTrinhViewModel.NguoiDung;
                    // Đập Dâng fields
                    danhSachCongTrinh.DienTichTuoiThucTe_Ha = danhSachCongTrinhViewModel.DienTichTuoiThucTe_Ha;
                    danhSachCongTrinh.CaoTrinhNguongPhanKhongTran_M = danhSachCongTrinhViewModel.CaoTrinhNguongPhanKhongTran_M;
                    danhSachCongTrinh.ChieuCaoDapLonNhatTrenCongTrinhPhanKhongTran_M = danhSachCongTrinhViewModel.ChieuCaoDapLonNhatTrenCongTrinhPhanKhongTran_M;
                    danhSachCongTrinh.ChieuDaiPhanKhongTran_M = danhSachCongTrinhViewModel.ChieuDaiPhanKhongTran_M;
                    danhSachCongTrinh.CaoTrinhNguongPhanTran_M = danhSachCongTrinhViewModel.CaoTrinhNguongPhanTran_M;
                    danhSachCongTrinh.ChieuCaoDapLonNhatTrenCongTrinhPhanTran_M = danhSachCongTrinhViewModel.ChieuCaoDapLonNhatTrenCongTrinhPhanTran_M;
                    danhSachCongTrinh.ChieuDaiPhanTran_M = danhSachCongTrinhViewModel.ChieuDaiPhanTran_M;
                    danhSachCongTrinh.CotNuocThietKeTrenCongTrinh_M = danhSachCongTrinhViewModel.CotNuocThietKeTrenCongTrinh_M;
                    danhSachCongTrinh.LuuLuongThietKe = danhSachCongTrinhViewModel.LuuLuongThietKe;
                    danhSachCongTrinh.CaoTrinhNguongCongXaCat_M = danhSachCongTrinhViewModel.CaoTrinhNguongCongXaCat_M;
                    danhSachCongTrinh.KichThuocCongXaCat_Cm = danhSachCongTrinhViewModel.KichThuocCongXaCat_Cm;
                    danhSachCongTrinh.VatLieuKetCauCongXaCat = danhSachCongTrinhViewModel.VatLieuKetCauCongXaCat;
                    danhSachCongTrinh.CaoTrinhNguongCongCapNuoc_M = danhSachCongTrinhViewModel.CaoTrinhNguongCongCapNuoc_M;
                    danhSachCongTrinh.LuuLuongThietKeCongCapNuoc = danhSachCongTrinhViewModel.LuuLuongThietKeCongCapNuoc;
                    danhSachCongTrinh.KichThuocCongCapNuoc_Cm = danhSachCongTrinhViewModel.KichThuocCongCapNuoc_Cm;
                    danhSachCongTrinh.VatLieuCongCapNuoc = danhSachCongTrinhViewModel.VatLieuCongCapNuoc;
                    danhSachCongTrinh.DonViQuanLyCongTrinh = danhSachCongTrinhViewModel.DonViQuanLyCongTrinh;
                    danhSachCongTrinh.TinhTrangCongTrinh = danhSachCongTrinhViewModel.TinhTrangCongTrinh;
                    danhSachCongTrinh.KinhDo_Longitude = danhSachCongTrinhViewModel.KinhDo_Longitude;
                    danhSachCongTrinh.ViDo_Latitude = danhSachCongTrinhViewModel.ViDo_Latitude;
                    danhSachCongTrinh.TrangThai = danhSachCongTrinhViewModel.TrangThai;
                    danhSachCongTrinh.ThuTuUuTien = danhSachCongTrinhViewModel.ThuTuUuTien;
                    // Đập Ngăn Mặn fields
                    danhSachCongTrinh.SoLanNangCapSuaChua = danhSachCongTrinhViewModel.SoLanNangCapSuaChua;
                    danhSachCongTrinh.SoKhoang = danhSachCongTrinhViewModel.SoKhoang;
                    danhSachCongTrinh.KichThuocKhoang_M = danhSachCongTrinhViewModel.KichThuocKhoang_M;
                    danhSachCongTrinh.KichThuocPhanTran_M = danhSachCongTrinhViewModel.KichThuocPhanTran_M;
                    danhSachCongTrinh.TenSong = danhSachCongTrinhViewModel.TenSong;
                    danhSachCongTrinh.ChieuCaoDinh_M = danhSachCongTrinhViewModel.ChieuCaoDinh_M;
                    danhSachCongTrinh.DienTichNganMan_Ha = danhSachCongTrinhViewModel.DienTichNganMan_Ha;
                    danhSachCongTrinh.CaoTrinhDinhDapPhanKhongTran_M = danhSachCongTrinhViewModel.CaoTrinhDinhDapPhanKhongTran_M;
                    danhSachCongTrinh.MucNuocLonNhat_M = danhSachCongTrinhViewModel.MucNuocLonNhat_M;
                    danhSachCongTrinh.ChieuDaiDapPhanKhongTran_M = danhSachCongTrinhViewModel.ChieuDaiDapPhanKhongTran_M;
                    danhSachCongTrinh.ChieuDaiDapPhanTran_M = danhSachCongTrinhViewModel.ChieuDaiDapPhanTran_M;
                    // Trạm Khí Tượng fields
                    danhSachCongTrinh.LoaiTram = danhSachCongTrinhViewModel.LoaiTram;
                    danhSachCongTrinh.Song = danhSachCongTrinhViewModel.Song;
                    danhSachCongTrinh.Thiessen = danhSachCongTrinhViewModel.Thiessen;
                    _context.SaveChanges();
                    return new { Id = danhSachCongTrinh.Id };
                }
            }

            return new { Id = 0 };
        }

        public dynamic CapNhatThongTinHuHong(DanhSachCongTrinhViewModel danhSachCongTrinhViewModel)
        {
            var danhSachCongTrinh = _context.DanhSachCongTrinhs.FirstOrDefault(o => o.Id == danhSachCongTrinhViewModel.Id);
            if (danhSachCongTrinh != null)
            {
                // Chỉ cập nhật các trường hư hỏng
                danhSachCongTrinh.MucDoHuHong = danhSachCongTrinhViewModel.MucDoHuHong;
                
                // Xử lý ngày tháng từ DateTime? sang DateTime?
                danhSachCongTrinh.NgayPhatHienHuHong = danhSachCongTrinhViewModel.NgayPhatHienHuHong;
                
                danhSachCongTrinh.NguyenNhanHuHong = danhSachCongTrinhViewModel.NguyenNhanHuHong;
                danhSachCongTrinh.TrangThaiXuLy = danhSachCongTrinhViewModel.TrangThaiXuLy;
                
                var result = _context.SaveChanges();
                return new { Id = danhSachCongTrinh.Id, Success = true };
            }
            return new { Id = 0, Success = false };
        }

        public bool Xoa(int Id)
        {
            var danhSachCongTrinh = _context.DanhSachCongTrinhs.FirstOrDefault(o => o.Id == Id);
            if (danhSachCongTrinh != null)
            {
                danhSachCongTrinh.IsDeleted = true;
                danhSachCongTrinh.NgayXoa = DateTime.Now;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool KhoiPhuc(int Id)
        {
            var danhSachCongTrinh = _context.DanhSachCongTrinhs.FirstOrDefault(o => o.Id == Id);
            if (danhSachCongTrinh != null)
            {
                danhSachCongTrinh.IsDeleted = false;
                danhSachCongTrinh.NgayXoa = null;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public DanhSachCongTrinhGridVo GetById(int id)
        {
            var item = _context.DanhSachCongTrinhs.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new DanhSachCongTrinhGridVo
                {
                    Id = item.Id,
                    TenCongTrinh = item.TenCongTrinh,
                    Diadiem = item.Diadiem,
                    LoaiCongTrinh = item.LoaiCongTrinh,
                    NamXayDung = item.NamXayDung,
                    ChuDauTu = item.ChuDauTu,
                    QuyMo = item.QuyMo,
                    DienTich = item.DienTich,
                    ThongSoKiThuat = item.ThongSoKiThuat,
                    GhiChu = item.GhiChu,
                    // Hồ fields
                    MucNuocChet_MNC = item.MucNuocChet_MNC,
                    MucNuocDangBinhThuong_MNDBT = item.MucNuocDangBinhThuong_MNDBT,
                    MucNuocLuuThietKe_MNLTK = item.MucNuocLuuThietKe_MNLTK,
                    MucNuocLuuKiemTra = item.MucNuocLuuKiemTra,
                    NamHoanThanhCongTrinh = item.NamHoanThanhCongTrinh,
                    LanNangCapSuaChua = item.LanNangCapSuaChua,
                    NamNangCapSuaChuaGanNhat = item.NamNangCapSuaChuaGanNhat,
                    DungTichChet_TrieuM3 = item.DungTichChet_TrieuM3,
                    DungTichUngVoiMNDBT_TrieuM3 = item.DungTichUngVoiMNDBT_TrieuM3,
                    DungTichUngVoiMNLTK_TrieuM3 = item.DungTichUngVoiMNLTK_TrieuM3,
                    DungTichHuuIch_TrieuM3 = item.DungTichHuuIch_TrieuM3,
                    DungTichPhongLuu_TrieuM3 = item.DungTichPhongLuu_TrieuM3,
                    DienTichLuuVuc_Km2 = item.DienTichLuuVuc_Km2,
                    DienTichTuoiThietKe_Ha = item.DienTichTuoiThietKe_Ha,
                    DienTichMatThoangUngVoiMNDBT_Ha = item.DienTichMatThoangUngVoiMNDBT_Ha,
                    DienTichMatThoangUngVoiMNC_Ha = item.DienTichMatThoangUngVoiMNC_Ha,
                    XaPhuong = item.XaPhuong,
                    DonViQuanLy = item.DonViQuanLy,
                    NguoiDung = item.NguoiDung,
                    // Đập Dâng fields
                    DienTichTuoiThucTe_Ha = item.DienTichTuoiThucTe_Ha,
                    CaoTrinhNguongPhanKhongTran_M = item.CaoTrinhNguongPhanKhongTran_M,
                    ChieuCaoDapLonNhatTrenCongTrinhPhanKhongTran_M = item.ChieuCaoDapLonNhatTrenCongTrinhPhanKhongTran_M,
                    ChieuDaiPhanKhongTran_M = item.ChieuDaiPhanKhongTran_M,
                    CaoTrinhNguongPhanTran_M = item.CaoTrinhNguongPhanTran_M,
                    ChieuCaoDapLonNhatTrenCongTrinhPhanTran_M = item.ChieuCaoDapLonNhatTrenCongTrinhPhanTran_M,
                    ChieuDaiPhanTran_M = item.ChieuDaiPhanTran_M,
                    CotNuocThietKeTrenCongTrinh_M = item.CotNuocThietKeTrenCongTrinh_M,
                    LuuLuongThietKe = item.LuuLuongThietKe,
                    CaoTrinhNguongCongXaCat_M = item.CaoTrinhNguongCongXaCat_M,
                    KichThuocCongXaCat_Cm = item.KichThuocCongXaCat_Cm,
                    VatLieuKetCauCongXaCat = item.VatLieuKetCauCongXaCat,
                    CaoTrinhNguongCongCapNuoc_M = item.CaoTrinhNguongCongCapNuoc_M,
                    LuuLuongThietKeCongCapNuoc = item.LuuLuongThietKeCongCapNuoc,
                    KichThuocCongCapNuoc_Cm = item.KichThuocCongCapNuoc_Cm,
                    VatLieuCongCapNuoc = item.VatLieuCongCapNuoc,
                    DonViQuanLyCongTrinh = item.DonViQuanLyCongTrinh,
                    TinhTrangCongTrinh = item.TinhTrangCongTrinh,
                    KinhDo_Longitude = item.KinhDo_Longitude,
                    ViDo_Latitude = item.ViDo_Latitude,
                    TrangThai = item.TrangThai,
                    ThuTuUuTien = item.ThuTuUuTien,
                    // Đập Ngăn Mặn fields
                    SoLanNangCapSuaChua = item.SoLanNangCapSuaChua,
                    SoKhoang = item.SoKhoang,
                    KichThuocKhoang_M = item.KichThuocKhoang_M,
                    KichThuocPhanTran_M = item.KichThuocPhanTran_M,
                    TenSong = item.TenSong,
                    ChieuCaoDinh_M = item.ChieuCaoDinh_M,
                    DienTichNganMan_Ha = item.DienTichNganMan_Ha,
                    CaoTrinhDinhDapPhanKhongTran_M = item.CaoTrinhDinhDapPhanKhongTran_M,
                    MucNuocLonNhat_M = item.MucNuocLonNhat_M,
                    ChieuDaiDapPhanKhongTran_M = item.ChieuDaiDapPhanKhongTran_M,
                    ChieuDaiDapPhanTran_M = item.ChieuDaiDapPhanTran_M,
                    // Trạm Khí Tượng fields
                    LoaiTram = item.LoaiTram,
                    Song = item.Song,
                    Thiessen = item.Thiessen,
                    IsDeleted = item.IsDeleted
                };
            }

            return new DanhSachCongTrinhGridVo();
        }

        public DanhSachCongTrinhGridVo GetByIdWithHuHong(int id)
        {
            var item = _context.DanhSachCongTrinhs.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new DanhSachCongTrinhGridVo
                {
                    Id = item.Id,
                    TenCongTrinh = item.TenCongTrinh,
                    Diadiem = item.Diadiem,
                    LoaiCongTrinh = item.LoaiCongTrinh,
                    NamXayDung = item.NamXayDung,
                    ChuDauTu = item.ChuDauTu,
                    QuyMo = item.QuyMo,
                    DienTich = item.DienTich,
                    ThongSoKiThuat = item.ThongSoKiThuat,
                    GhiChu = item.GhiChu,
                    // Hồ fields
                    MucNuocChet_MNC = item.MucNuocChet_MNC,
                    MucNuocDangBinhThuong_MNDBT = item.MucNuocDangBinhThuong_MNDBT,
                    MucNuocLuuThietKe_MNLTK = item.MucNuocLuuThietKe_MNLTK,
                    MucNuocLuuKiemTra = item.MucNuocLuuKiemTra,
                    NamHoanThanhCongTrinh = item.NamHoanThanhCongTrinh,
                    LanNangCapSuaChua = item.LanNangCapSuaChua,
                    NamNangCapSuaChuaGanNhat = item.NamNangCapSuaChuaGanNhat,
                    DungTichChet_TrieuM3 = item.DungTichChet_TrieuM3,
                    DungTichUngVoiMNDBT_TrieuM3 = item.DungTichUngVoiMNDBT_TrieuM3,
                    DungTichUngVoiMNLTK_TrieuM3 = item.DungTichUngVoiMNLTK_TrieuM3,
                    DungTichHuuIch_TrieuM3 = item.DungTichHuuIch_TrieuM3,
                    DungTichPhongLuu_TrieuM3 = item.DungTichPhongLuu_TrieuM3,
                    DienTichLuuVuc_Km2 = item.DienTichLuuVuc_Km2,
                    DienTichTuoiThietKe_Ha = item.DienTichTuoiThietKe_Ha,
                    DienTichMatThoangUngVoiMNDBT_Ha = item.DienTichMatThoangUngVoiMNDBT_Ha,
                    DienTichMatThoangUngVoiMNC_Ha = item.DienTichMatThoangUngVoiMNC_Ha,
                    XaPhuong = item.XaPhuong,
                    DonViQuanLy = item.DonViQuanLy,
                    NguoiDung = item.NguoiDung,
                    // Đập Dâng fields
                    DienTichTuoiThucTe_Ha = item.DienTichTuoiThucTe_Ha,
                    CaoTrinhNguongPhanKhongTran_M = item.CaoTrinhNguongPhanKhongTran_M,
                    ChieuCaoDapLonNhatTrenCongTrinhPhanKhongTran_M = item.ChieuCaoDapLonNhatTrenCongTrinhPhanKhongTran_M,
                    ChieuDaiPhanKhongTran_M = item.ChieuDaiPhanKhongTran_M,
                    CaoTrinhNguongPhanTran_M = item.CaoTrinhNguongPhanTran_M,
                    ChieuCaoDapLonNhatTrenCongTrinhPhanTran_M = item.ChieuCaoDapLonNhatTrenCongTrinhPhanTran_M,
                    ChieuDaiPhanTran_M = item.ChieuDaiPhanTran_M,
                    CotNuocThietKeTrenCongTrinh_M = item.CotNuocThietKeTrenCongTrinh_M,
                    LuuLuongThietKe = item.LuuLuongThietKe,
                    CaoTrinhNguongCongXaCat_M = item.CaoTrinhNguongCongXaCat_M,
                    KichThuocCongXaCat_Cm = item.KichThuocCongXaCat_Cm,
                    VatLieuKetCauCongXaCat = item.VatLieuKetCauCongXaCat,
                    CaoTrinhNguongCongCapNuoc_M = item.CaoTrinhNguongCongCapNuoc_M,
                    LuuLuongThietKeCongCapNuoc = item.LuuLuongThietKeCongCapNuoc,
                    KichThuocCongCapNuoc_Cm = item.KichThuocCongCapNuoc_Cm,
                    VatLieuCongCapNuoc = item.VatLieuCongCapNuoc,
                    DonViQuanLyCongTrinh = item.DonViQuanLyCongTrinh,
                    TinhTrangCongTrinh = item.TinhTrangCongTrinh,
                    KinhDo_Longitude = item.KinhDo_Longitude,
                    ViDo_Latitude = item.ViDo_Latitude,
                    TrangThai = item.TrangThai,
                    ThuTuUuTien = item.ThuTuUuTien,
                    // Đập Ngăn Mặn fields
                    SoLanNangCapSuaChua = item.SoLanNangCapSuaChua,
                    SoKhoang = item.SoKhoang,
                    KichThuocKhoang_M = item.KichThuocKhoang_M,
                    KichThuocPhanTran_M = item.KichThuocPhanTran_M,
                    TenSong = item.TenSong,
                    ChieuCaoDinh_M = item.ChieuCaoDinh_M,
                    DienTichNganMan_Ha = item.DienTichNganMan_Ha,
                    CaoTrinhDinhDapPhanKhongTran_M = item.CaoTrinhDinhDapPhanKhongTran_M,
                    MucNuocLonNhat_M = item.MucNuocLonNhat_M,
                    ChieuDaiDapPhanKhongTran_M = item.ChieuDaiDapPhanKhongTran_M,
                    ChieuDaiDapPhanTran_M = item.ChieuDaiDapPhanTran_M,
                    // Trạm Khí Tượng fields
                    LoaiTram = item.LoaiTram,
                    Song = item.Song,
                    Thiessen = item.Thiessen,
                    // Công trình hư hỏng fields
                    MucDoHuHong = item.MucDoHuHong,
                    NgayPhatHienHuHong = item.NgayPhatHienHuHong,
                    NguyenNhanHuHong = item.NguyenNhanHuHong,
                    TrangThaiXuLy = item.TrangThaiXuLy,
                    IsDeleted = item.IsDeleted
                };
            }
            return new DanhSachCongTrinhGridVo();
        }

        public List<LookupViewModel> DanhSachCongTrinhLookup(LookupQueryViewModel lookupQueryViewModel, int? loaiCongTrinh = null)
        {
            var data = new List<LookupViewModel>
                {
                    new LookupViewModel { id = 0, text = "--- Chọn ---" }
                };

            var query = _context.DanhSachCongTrinhs.AsNoTracking().Where(o => o.IsDeleted != true);

            // Filter theo loại công trình nếu có
            if (loaiCongTrinh.HasValue && loaiCongTrinh.Value > 0)
            {
                query = query.Where(o => o.LoaiCongTrinh == loaiCongTrinh.Value);
            }

            var results = query.Select(o => new LookupViewModel
            {
                id = o.Id,
                text = o.TenCongTrinh,
                loaiCongTrinh = o.LoaiCongTrinh,
                diadiem = o.Diadiem,
                chuDauTu = o.ChuDauTu
            }).Take(50).ToList();

            data.AddRange(results);

            return data;
        }
    }
}
