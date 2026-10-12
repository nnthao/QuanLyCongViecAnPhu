using Dashboard.Models.HtmlHelpers.Select2;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Dashboard.HtmlHelpers
{
    public static class SelectExtensions
    {
        public static MvcHtmlString CustomSelect2Empty(this HtmlHelper htmlHelper, string id, string label, bool isReadonly = false,
            int col = 12, bool required = false, string moreClass = "", string url = "", bool multiSelect2 = false, string placeHolder = "",
            bool allowClear = false)
        {
            var viewModel = new Select2ViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Col = col,
                MoreClass = moreClass,
                Url = url,
                MultiSelect2 = multiSelect2,
                PlaceHolder = placeHolder,
                IsReadonly = isReadonly,
                AllowClear = allowClear
            };
            return htmlHelper.Partial("~/Views/Shared/SelectOption/Select2Empty.cshtml", viewModel);
        }

        public static MvcHtmlString CustomSelect2Ajax(this HtmlHelper htmlHelper, string id, string label,
            int col = 12, bool required = false, string moreClass = "", string url = "", string placeHolder = "",
            bool isReadOnly = false)
        {
            var viewModel = new Select2ViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Col = col,
                MoreClass = moreClass,
                Url = url,
                PlaceHolder = placeHolder,
                IsReadonly = isReadOnly
            };
            return htmlHelper.Partial("~/Views/Shared/SelectOption/Select2Ajax.cshtml", viewModel);
        }
    }
}