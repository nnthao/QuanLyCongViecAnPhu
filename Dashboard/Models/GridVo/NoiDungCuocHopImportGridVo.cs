namespace Dashboard.Models.GridVo
{
    public class NoiDungCuocHopImportGridVo
    {
        public NoiDungCuocHopImportGridVo()
        {
            IsDaXong = false;
        }
        public int? Id { get; set; }
        public string ThoiGian { get; set; }
        public string NoiDung { get; set; }
        public string NguoiTrinhBay { get; set; }
        public bool? IsDaXong { get; set; }
        public int? CuocHopId { get; set; }
        public string TrangThai { get; set; }
    }
}