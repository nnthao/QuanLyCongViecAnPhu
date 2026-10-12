using System.Collections.Generic;

namespace Dashboard.Models.ViewModel
{
    public class HisThongTinBenhNhanResponseViewModel
    {
        public int total { get; set; }
        public int page { get; set; }
        public int records { get; set; }
        public List<HisThongTinBenhNhanRowViewModel> rows { get; set; }
        public string uuid { get; set; }
    }
}
