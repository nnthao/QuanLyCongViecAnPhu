using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Models.CommonModel
{
    public class FileModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}