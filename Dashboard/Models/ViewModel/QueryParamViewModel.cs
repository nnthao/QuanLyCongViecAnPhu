using Dashboard.Common;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard.Models.ViewModel
{
    public class QueryParamViewModel
    {
        public QueryParamViewModel()
        {

        }

        public int Id { get; set; }
        public string search { get; set; }

        public string searchDecode
        {
            get
            {
                if (!string.IsNullOrEmpty(search))
                {
                    try
                    {
                        return Encoding.UTF8.GetString(Convert.FromBase64String(search));
                    }
                    catch
                    {
                        return "";
                    }
                }

                return "";
            }
        }

        public string sort { get; set; }
        public string order { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public string sortStringDefault { get; set; }

        public string sortString
        {
            get
            {
                if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(order))
                {
                    return sort + " " + order;
                }

                return sortStringDefault;
            }
        }

        public List<string> searchColumn { get; set; }

        public string searchString
        {
            get
            {
                var searchConditionList = new List<string>();
                int number = 0;
                if (searchColumn != null && searchColumn.Count > 0 && !string.IsNullOrEmpty(searchDecode))
                {
                    var searchValue = searchDecode.Replace("\"", "\"\"").Replace("\\", "\\\\");
                    foreach (var col in searchColumn)
                    {
                        if (col == "TongSo" || col == "TuoiDang")
                        {
                            if (int.TryParse(searchDecode, out number))
                                searchConditionList.Add($"{col}>= {number}");
                        }
                        else
                        {
                            searchConditionList.Add($"{col}.ToLower().Contains(\"{searchValue.ToLower()}\")");
                        }
                    }
                    return String.Join(" OR ", searchConditionList);
                }

                return "1=1";
            }
        }

        public int? UserId { get; set; }
        public bool DaXoaSearch { get; set; }

        public int VaiTroIdSearch { get; set; }
        public int? PhongBanId { get; set; }
        public int? PhongBanChaId { get; set; }
        public string PhongBanIdsStr { get; set; }
        public string LyDoIdsStr { get; set; }

        public string TuNgayStr { get; set; }
        public DateTime? TuNgay
        {
            get
            {
                var dt = DateTimeHelper.ConvertStrToDate(TuNgayStr);
                if (dt.HasValue)
                {
                    return DateTimeHelper.BeginningOfDay(dt.Value);
                }
                return null;
            }
        }
        public string DenNgayStr { get; set; }
        public DateTime? DenNgay
        {
            get
            {
                var dt = DateTimeHelper.ConvertStrToDate(DenNgayStr);
                if (dt.HasValue)
                {
                    return DateTimeHelper.EndOfDay(dt.Value);
                }
                return null;
            }
        }

        public int? NguoiDuyetId { get; set; }
        //
        public string TuanHienTai { get; set; }
        public string TuanTimKiem { get; set; }

        public DateTime LichHopTuNgay
        {
            get
            {
                if (string.IsNullOrEmpty(TuanTimKiem) && string.IsNullOrEmpty(TuanHienTai))
                {
                    return DateTime.Now;
                }

                var ngayLay = !string.IsNullOrEmpty(TuanTimKiem) ? TuanTimKiem : TuanHienTai;
                var arrTuan = ngayLay.Split('-').Select(o => o.Trim()).ToList();

                return DateTimeHelper.BeginningOfDay(DateTimeHelper.ConvertStrToDate(arrTuan.First()).GetValueOrDefault());
            }
        }

        public DateTime LichHopDenNgay
        {
            get
            {
                if (string.IsNullOrEmpty(TuanTimKiem) && string.IsNullOrEmpty(TuanHienTai))
                {
                    return DateTime.Now;
                }

                var ngayLay = !string.IsNullOrEmpty(TuanTimKiem) ? TuanTimKiem : TuanHienTai;
                var arrTuan = ngayLay.Split('-').Select(o => o.Trim()).ToList();

                return DateTimeHelper.EndOfDay(DateTimeHelper.ConvertStrToDate(arrTuan.Last()).GetValueOrDefault());
            }
        }

        public string ThangHienTai { get; set; }
        public int? LoaiLich { get; set; }
        public int? LoaiFile { get; set; }
        public bool? IsLichCaNhan { get; set; }
        public int CoQuanCurrentId { get; set; }
        public List<int?> PhongBanIds { get; set; }
        public int? CurrentUserId { get; set; }
        public bool IsSystemUser { get; set; }

        // Filter parameters for DanhSachCongTrinh
        public string loaiCongTrinh { get; set; }
        public string trangThai { get; set; }
        // Filter parameters for BaoCaoHoChua
        public string xaPhuong { get; set; }
        public string donViQuanLy { get; set; }
        public string phanLoaiHo { get; set; }
        // Filter parameters for KetQua
        public string LoaiSangKienFilter { get; set; }
        // Filter parameters for Sáng kiến
        public int? DotXetDuyetIdFilter { get; set; }
        public string NamXetDuyetFilter { get; set; }
        public string TrangThaiDuyetFilter { get; set; }
        public int? LinhVucSangKienIdFilter { get; set; }
        // Filter parameters for Sáng kiến CNTT
        public string NamSangKienFilter { get; set; }
        public string XepLoaiSangKienFilter { get; set; }
    }

    public class UserQueryViewModel : QueryParamViewModel
    {
        public bool CoTelegramSearch { get; set; }
        public bool ChuaKetNoiTelegramSearch { get; set; }
        public bool TaiKhoanBiKhoaSearch { get; set; }
    }

    public class QuanLyThongTinTraCuuQueryViewModel : QueryParamViewModel
    {
        public int? DanhMucTraCuuThongTinId { get; set; }
        public int? TrangThaiId { get; set; }
    }

    public class NhomNhanViecQueryViewModel : QueryParamViewModel
    {

    }

    public class DuAnQueryViewModel : QueryParamViewModel
    {
        public bool IsThuNghiemSearch { get; set; }
    }

    public class GiaoViecQueryViewModel : QueryParamViewModel
    {
        public GiaoViecQueryViewModel()
        {
            PhongBanIds = new List<int>();
            IsHoanThanhTypeList = new List<int>();
            ThanhVienIdList = new List<int>();
        }
        public int? NguoiDuocGiaoId { get; set; }
        public int? NguoiNhanChuyenGiaoId { get; set; }
        public int? NguoiGiaoId { get; set; }
        public int IsHoanThanhType { get; set; }
        public List<int> PhongBanIds { get; set; }
        public string PhongBanIdsStr { get; set; }
        public List<int> IsHoanThanhTypeList { get; set; }
        public string IsHoanThanhTypeListStr { get; set; }
        public string ThanhVienIdListStr { get; set; }
        public List<int> ThanhVienIdList { get; set; }
        public int TrangThaiSearch { get; set; }
    }

    public class LookupQueryViewModel
    {
        public LookupQueryViewModel()
        {
            isHideOptionChonValueZero = false;
            phongBanId = 0;
            isGetAll = false;
            tenKemTen = false;
            phongBanLoaiTruId = 0;
            phongHopLoaiTruId = 0;
            coQuanLoaiTruId = 0;
            phongBanIds = new List<int>();
        }

        public bool isHideOptionChonValueZero { get; set; }
        public int phongBanId { get; set; }
        public bool isGetAll { get; set; }
        public bool tenKemTen { get; set; }
        public int phongBanLoaiTruId { get; set; }
        public int phongHopLoaiTruId { get; set; }
        public int coQuanLoaiTruId { get; set; }
        public List<int> phongBanIds { get; set; }
        public DateTime? TuNgayFilter { get; set; }
        public DateTime? DenNgayFilter { get; set; }
    }

    public class BaoCaoChuyenNganhQueryViewModel : QueryParamViewModel
    {
        public int? DanhMucBaoCaoId { get; set; }
        public int? QuanLyBaoCaoChuyenNganhEnumId { get; set; }
        public string PhongBanIdsStr { get; set; }
    }
    public class CuocHopQueryParamViewModel : QueryParamViewModel
    {
        public CuocHopQueryParamViewModel()
        {
            TrangThaiXuLy = new List<string>();
        }
        public List<string> TrangThaiXuLy { get; set; }
        public int? NguoiDuocGanDuyetIdSearch { get; set; }
        public EnumLoaiLichHop? EnumLoaiLichHopSearch { get; set; }
        public string TrangThaiXuLySearch { get; set; }

        public bool? IsDieuPhieuCuocHopQuery { get; set; }
    }

    public class SoKiHieuVanBanQueryViewModel : QueryParamViewModel
    {
        public int? UserIdFilter { get; set; }
    }

    public class DiemDanhQueryViewModel : QueryParamViewModel
    {
    }
}