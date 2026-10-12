namespace Dashboard.Models.ViewModel
{
    public enum LoaiGoiYNoiDungEnum
    {
        NoiDungCongViec = 1,
        NoiDungNhiemVu = 2
    }

    public class GoiYNoiDungQueryViewModel
    {
        public string Keyword { get; set; }
        public int Limit { get; set; } = 10;
        public int? Loai { get; set; }
        public int? CurrentUserId { get; set; }
    }

    public class GoiYNoiDungItemVo
    {
        public int Id { get; set; }
        public string NoiDung { get; set; }
        public int SoLanSuDung { get; set; }
    }

    public class LuuGoiYNoiDungViewModel
    {
        public string NoiDung { get; set; }
        public int? Loai { get; set; }
        public int? CurrentUserId { get; set; }
    }
}
