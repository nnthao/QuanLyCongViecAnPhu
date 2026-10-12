using System.Web;

namespace Dashboard.Models.ViewModel
{
    public class VnEduImportRequestViewModel
    {
        public HttpPostedFileBase FileImport { get; set; }

        public int? Khoi { get; set; }
    }

    public class VnEduImportResultViewModel
    {
        public bool Success { get; set; }

        public string DownloadUrl { get; set; }

        public string FileName { get; set; }

        public int RecordCount { get; set; }
    }
}
