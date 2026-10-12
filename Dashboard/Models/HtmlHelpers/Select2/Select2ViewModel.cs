using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Models.HtmlHelpers.Select2
{
    public class Select2ViewModel : ControlSharedViewModelBase
    {
        public string Class { get; set; }
        public string Style { get; set; }
        public bool Required { get; set; }
        public string MoreClass { get; set; }
        public string ErrorId => Id + "Error";
        public string Url { get; set; }
        public string PlaceHolder { get; set; }
        public bool MultiSelect2 { get; set; }
        public bool IsReadonly { get; set; }
        public bool AllowClear { get; set; }

        public string TriggerElmId { get; set; }
        public string CallRefreshElmId { get; set; }
        public string UrlCallRefreshElmId { get; set; }
    }
}