namespace Dashboard.Models.GridVo
{
    public class TaiLieuCuocHopGridVo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CuocHopId { get; set; }

        public bool IsUpload => true;
        public string DuongDan { get; set; }
    }
}