namespace Dashboard.Models.GridVo
{
    public class FileDinhKemGridVo
    {
        public int Id { get; set; }
        public string Loai { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }

        public bool IsUpload { get { return true; } }
    }
}