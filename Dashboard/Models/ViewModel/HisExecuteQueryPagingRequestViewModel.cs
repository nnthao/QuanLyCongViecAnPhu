using System.Collections.Generic;

namespace Dashboard.Models.ViewModel
{
    public class HisExecuteQueryPagingRequestViewModel
    {
        public string func { get; set; }
        public string uuid { get; set; }
        public List<string> @params { get; set; }
        public List<HisRequestOptionViewModel> options { get; set; }
    }
}
