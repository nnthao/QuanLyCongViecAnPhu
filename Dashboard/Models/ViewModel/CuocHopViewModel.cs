using Dashboard.Common;
using Dashboard.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Models.ViewModel
{
    public class CuocHopViewModel
    {
        public CuocHopViewModel()
        {
            FileDinhKems = new List<HttpPostedFileBase>();
        }

        public int Id { get; set; }
        public string TenCuocHop { get; set; }
        public string NguoiThamGia { get; set; }
        public string NguoiThamGiaKhac { get; set; }
        public string PhongBanThamGiaIds { get; set; }
        public string NguoiChuTri { get; set; }
        public int? PhongHopId { get; set; }
        public int? LyDoId { get; set; }
        public string NgayDienRaStr { get; set; }
        public string BatDauStr { get; set; }
        public string KetThucStr { get; set; }
        public DateTime? ThoiGianBatDau
        {
            get
            {
                if (!string.IsNullOrEmpty(NgayDienRaStr) && !string.IsNullOrEmpty(BatDauStr))
                {
                    return DateTimeHelper.ConvertStrToDateTime(NgayDienRaStr + " " + BatDauStr);
                }
                return null;
            }
        }

        public DateTime? ThoiGianKetThuc
        {
            get
            {
                if (!string.IsNullOrEmpty(NgayDienRaStr) && !string.IsNullOrEmpty(KetThucStr))
                {
                    return DateTimeHelper.ConvertStrToDateTime(NgayDienRaStr + " " + KetThucStr);
                }
                return null;
            }
        }
        public string NoiDung { get; set; }
        public int? NguoiPhuTrachId { get; set; }
        public int? SoNguoiThamGia { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
        public int? NguoiTaoId { get; set; }
        public string LyDoTraPhieu { get; set; }

        public string PhongHopTxt { get; set; }
        public int? NguoiDuyetId { get; set; }
        public EnumLoaiLichHop? LoaiLichHop { get; set; }

        public List<HttpPostedFileBase> FileDinhKems { get; set; }
        public string NguoiLienQuanCuocHopStrs { get; set; }

        public List<int> NguoiLienQuanCuocHopList
        {
            get
            {
                var datas = new List<int>();
                if (!string.IsNullOrEmpty(NguoiLienQuanCuocHopStrs))
                {
                    try
                    {
                        var jsonData = JsonConvert.DeserializeObject<List<UserGridVo>>(NguoiLienQuanCuocHopStrs);
                        if (jsonData.Count > 0)
                        {
                            datas = jsonData.Select(o => (int)o.Id).ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }

                return datas;
            }
        }
        public string ChecklistStr { get; set; }
        public int? CoQuanId { get; set; }
        public int? NguoiDuocGanDuyetId { get; set; }
        public string CoQuanChuTriId { get; set; }
        public string CoQuanThamGiaId { get; set; }

        public bool QuyenChinhSuaLichHopDaDuyet { get; set; }
        public bool IsAddOrUpdate { get; set; }
        public int? CurrentUserId { get; set; }
        public bool IsQuyenDangKyLichKhongCanDuyetChoNhanSu { get; set; }
        public bool? IsThongBao { get; set; }
    }

    public class TaoLichHopTuDiemDanhRequest
    {
        public int DiemDanhId { get; set; }
        public int CurrentUserId { get; set; }
        public string LyDoChuyenNghi { get; set; }
        public string GhiChu { get; set; }
    }
}
