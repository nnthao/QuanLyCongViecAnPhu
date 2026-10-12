namespace Dashboard.Models.GridVo
{
    public class CuocHopImportGridVo
    {
        public CuocHopImportGridVo()
        {
            IsThanhCong = false;
        }

        public int Id { get; set; }
        public string NgayStr { get; set; }
        public string Buoi { get; set; }
        public string ThoiGian { get; set; }
        public string BatDauStr { get; set; }
        public string KetThucStr { get; set; }
        public string NoiDung { get; set; }
        public string ChuTri { get; set; }
        public string ThanhPhan { get; set; }
        public string DiaDiem { get; set; }
        public string GhiChu { get; set; }
        public int? LyDoId { get; set; }
        public string Loi { get; set; }
        public bool IsThanhCong { get; set; }
    }
}
