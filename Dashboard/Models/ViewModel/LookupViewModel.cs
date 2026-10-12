using System.Collections.Generic;

namespace Dashboard.Models.ViewModel
{
    public class LookupViewModel
    {
        public int id { get; set; }
        public string text { get; set; }
        public int? phongBanId { get; set; }
        public int? loaiCongTrinh { get; set; }
        public string diadiem { get; set; }
        public string chuDauTu { get; set; }
    }
    public class SendIdViewModel
    {
        public int id { get; set; }
        public string Guid { get; set; }
    }

    public class LookupViewModelV2
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    public class DataLookupViewModel
    {
        public DataLookupViewModel()
        {
            Data = new List<LookupViewModel>();
        }
        public List<LookupViewModel> Data { get; set; }
    }


}