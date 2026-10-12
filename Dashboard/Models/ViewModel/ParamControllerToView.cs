namespace Dashboard.Models.ViewModel
{
    public class ParamControllerToView
    {
        public int Id { get; set; }
        public int CuocHopId { get; set; }
        public int CoQuanCurrentId { get; set; }
        public int PhongBanId { get; set; }
        public int PhongBanCurrentId { get; set; }
        public int UserCurrentId { get; set; }
        public DataLookupViewModel PhongBanLookups { get; set; }
        public DataLookupViewModel CoQuanLookups { get; set; }
        //
        public bool XemLichHopCuaTatCaDonVi { get; set; }
        public bool IsQuyenDangKyLichKhongCanDuyetChoNhanSu { get; set; }
        //
        public bool IsQuyenXemTatCaLichXe { get; set; }
        //
        public string Ticket { get; set; }
        public string Value { get; set; }
        //
        public int GiaoViecThanhVienId { get; set; }
        public int GiaoViecId { get; set; }
        public int ThanhVienId { get; set; }
        public bool IsXemThongTinDaXuLy { get; set; }
    }
}