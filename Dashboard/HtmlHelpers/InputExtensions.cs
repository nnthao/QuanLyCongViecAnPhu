using Dashboard.Models.HtmlHelpers.Input;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Dashboard.HtmlHelpers
{
    public static class InputExtensions
    {
        public static MvcHtmlString CustomTextBox(this HtmlHelper htmlHelper, string id, string label,
            int col = 12, bool required = false, int length = Int32.MaxValue,
            bool isReadonly = false, string cssClass = "", string moreClass = "", string value = "",
            bool isDateType = false, bool isNumber = false,
            string placeHolderText = "", bool isPasswordType = false, bool isDisabled = false)
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Class = cssClass,
                Length = length,
                PlaceHolderText = placeHolderText,
                Col = col,
                IsDisabled = isDisabled,
                MoreClass = moreClass,
                IsReadonly = isReadonly,
                value = value
            };
            if (isPasswordType)
            {
                viewModel.TextboxType = "type=password";
            }
            else if (isDateType)
            {
                viewModel.TextboxType = "type=date";
            }
            else if (isNumber)
            {
                viewModel.TextboxType = "type=number";
            }
            else
            {
                viewModel.TextboxType = "type=text";
            }
            return htmlHelper.Partial("~/Views/Shared/Input/InputText.cshtml", viewModel);
        }

        public static MvcHtmlString CustomRadioTrueFalse(this HtmlHelper htmlHelper, string id, string label,
            int col = 12, bool required = false,
            bool isReadonly = false, string cssClass = "", string moreClass = "", string value1 = "Có", string value2 = "Không")
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Class = cssClass,
                Col = col,
                MoreClass = moreClass,
                IsReadonly = isReadonly,
                Value1Radio = value1,
                Value2Radio = value2
            };
            return htmlHelper.Partial("~/Views/Shared/Input/RadioTrueFalse.cshtml", viewModel);
        }

        public static MvcHtmlString FileDinhKem(this HtmlHelper htmlHelper, string id)
        {
            var viewModel = new InputTextViewModel
            {
                Id = id
            };
            return htmlHelper.Partial("~/Views/Shared/Input/FormUploadFileDinhKem.cshtml", viewModel);
        }

        public static MvcHtmlString FileDinhKemForUpload(this HtmlHelper htmlHelper, string id, bool showDelete = true)
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                ShowDelete = showDelete
            };
            return htmlHelper.Partial("~/Views/Shared/Input/FormDataUploadFileDinhKem.cshtml", viewModel);
        }

        public static MvcHtmlString CustomDatePicker(this HtmlHelper htmlHelper, string id, string label,
            int col = 12, bool required = false, string value = "",
            bool isReadonly = false, string cssClass = "", string moreClass = "")
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Class = cssClass,
                Col = col,
                MoreClass = moreClass,
                IsReadonly = isReadonly,
                value = value
            };
            return htmlHelper.Partial("~/Views/Shared/Input/Datepicker.cshtml", viewModel);
        }
        public static MvcHtmlString CustomDateTimePicker(this HtmlHelper htmlHelper, string id, string label,
            int col = 12, bool required = false,
            bool isReadonly = false, string cssClass = "", string moreClass = "")
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Class = cssClass,
                Col = col,
                MoreClass = moreClass,
                IsReadonly = isReadonly
            };
            return htmlHelper.Partial("~/Views/Shared/Input/Datetimepicker.cshtml", viewModel);
        }
        public static MvcHtmlString TextBoxReadOnly(this HtmlHelper htmlHelper, string id, string label,
            int col = 12, string cssClass = "", string moreClass = "", string placeHolderText = "")
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Class = cssClass,
                PlaceHolderText = placeHolderText,
                Col = col,
                MoreClass = moreClass,
            };

            return htmlHelper.Partial("~/Views/Shared/Input/InputReadOnly.cshtml", viewModel);
        }

        public static MvcHtmlString CustomNumber(this HtmlHelper htmlHelper, string id, string label,
            int col = 12, bool required = false, int length = Int32.MaxValue, string value = "",
            bool isReadonly = false, string cssClass = "", string moreClass = "",
            bool isDecimal = false, string placeHolderText = "", bool isDisabled = false, string min = "")
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Class = cssClass,
                Length = length,
                PlaceHolderText = placeHolderText,
                Col = col,
                IsDisabled = isDisabled,
                MoreClass = moreClass,
                IsReadonly = isReadonly,
                IsDecimal = isDecimal,
                Min = min,
                value = value

            };
            return htmlHelper.Partial("~/Views/Shared/Input/InputNumber.cshtml", viewModel);
        }

        public static MvcHtmlString InputFile(this HtmlHelper htmlHelper, string id, string label, string acceptfile = "", string multiple = null,
            int col = 12, bool required = false, string cssClass = "", string moreClass = "", bool preViewImage = false)
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Class = cssClass,
                Col = col,
                MoreClass = moreClass,
                acceptfile = acceptfile,
                multiple = multiple,
                PreviewImage = preViewImage
            };
            return htmlHelper.Partial("~/Views/Shared/Input/InputFile.cshtml", viewModel);
        }
        public static MvcHtmlString CustomDateTimeRangePicker(this HtmlHelper htmlHelper, string id, string label,
                int col = 12, bool required = false,
                bool isReadonly = false, string cssClass = "", string moreClass = "")
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Class = cssClass,
                Col = col,
                MoreClass = moreClass,
                IsReadonly = isReadonly
            };
            return htmlHelper.Partial("~/Views/Shared/Input/DateTimeRangePicker.cshtml", viewModel);
        }
        public static MvcHtmlString CustomTextArea(this HtmlHelper htmlHelper, string id, string label, bool isReadonly = false,
                  int col = 12, int row = 3, bool required = false, string cssClass = "", string moreClass = "", bool Ckeditor = false,
                  int maxlength = 5000, string value = "")
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Class = cssClass,
                Col = col,
                Row = row,
                MoreClass = moreClass,
                IsReadonly = isReadonly,
                Ckeditor = Ckeditor,
                MaxLength = maxlength,
                value = value
            };
            return htmlHelper.Partial("~/Views/Shared/Input/InputTextArea.cshtml", viewModel);
        }

        public static MvcHtmlString CustomCheckBox(this HtmlHelper htmlHelper, string id, string label,
           int col = 12, bool required = false, bool isChecked = false,
           bool isDisabled = false, string cssClass = "", string moreClass = "")
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Class = cssClass,
                Col = col,
                IsDisabled = isDisabled,
                MoreClass = moreClass,
                IsChecked = isChecked
            };
            return htmlHelper.Partial("~/Views/Shared/Input/CheckBox.cshtml", viewModel);
        }

        public static MvcHtmlString CustomSkeleton(this HtmlHelper htmlHelper, string id)
        {
            var viewModel = new InputTextViewModel
            {
                Id = id
            };

            return htmlHelper.Partial("~/Views/Shared/Input/Skeleton.cshtml", viewModel);
        }

        public static MvcHtmlString CustomSingleFileInput(this HtmlHelper htmlHelper, string id, string label,
           int col = 12, bool required = false,
           bool isReadonly = false, string cssClass = "", string moreClass = "", bool isDisabled = false)
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Class = cssClass,
                Col = col,
                IsDisabled = isDisabled,
                MoreClass = moreClass,
                IsReadonly = isReadonly
            };
            return htmlHelper.Partial("~/Views/Shared/Input/SingleFileInput.cshtml", viewModel);
        }

        public static MvcHtmlString CustomVaiTroGridCollapse(this HtmlHelper htmlHelper, string id, string label,
           List<VaiTroGridData> datas)
        {
            var viewModel = new VaiTroGridViewModel
            {
                Id = id,
                Label = label,
                Datas = datas
            };
            return htmlHelper.Partial("~/Views/Shared/Input/VaiTroGridCollapse.cshtml", viewModel);
        }
        public static MvcHtmlString CustomVaiTroGridSingle(this HtmlHelper htmlHelper, string id, string label = "",
           string key = "", string moTa = "")
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Key = key,
                MoTa = moTa
            };
            return htmlHelper.Partial("~/Views/Shared/Input/VaiTroGridSingle.cshtml", viewModel);
        }
        public static MvcHtmlString CustomMultiFileInput(this HtmlHelper htmlHelper, string id, string label,
           int col = 12, bool required = false, bool isMultiple = false,
           bool isReadonly = false, string cssClass = "", string moreClass = "", bool isDisabled = false)
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Class = cssClass,
                Col = col,
                IsDisabled = isDisabled,
                MoreClass = moreClass,
                IsReadonly = isReadonly,
                IsMultiple = isMultiple
            };
            return htmlHelper.Partial("~/Views/Shared/Input/CustomMultiFileInput.cshtml", viewModel);
        }

        public static MvcHtmlString CustomDateTimePickerContainer(this HtmlHelper htmlHelper, string id, string label,
            int col = 12, bool required = false, bool isReadonly = false, string cssClass = "", string moreClass = "",
            string idContainer = "")
        {
            var viewModel = new InputTextViewModel
            {
                Id = id,
                Label = label,
                Required = required,
                Class = cssClass,
                Col = col,
                MoreClass = moreClass,
                IsReadonly = isReadonly,
                IdContainer = idContainer
            };
            return htmlHelper.Partial("~/Views/Shared/Input/DatetimePickerCustom.cshtml", viewModel);
        }

        public static MvcHtmlString CustomDropDownList(this HtmlHelper htmlHelper, string id, string label,
            int col = 12, bool required = false, IList<SelectListItem> options = null,
            string cssClass = "", string moreClass = "", bool isDisabled = false)
        {
            // Build a simple bootstrap form-group with label and select element
            var colClass = $"col-sm-{col} col-xs-12";

            var labelRequired = required ? "<span class=\"text-danger\">*</span>" : string.Empty;
            var disabledAttr = isDisabled ? " disabled\" disabled=\"disabled" : string.Empty;
            var selectClass = string.IsNullOrWhiteSpace(cssClass) ? "form-control" : $"form-control {cssClass}";

            var html = new System.Text.StringBuilder();
            html.Append($"<div class=\"{colClass} {moreClass}\">");
            html.Append("<div class=\"form-group\">");
            html.Append($"<label for=\"{id}\">{label} {labelRequired}</label>");
            html.Append($"<select id=\"{id}\" name=\"{id}\" class=\"{selectClass}\"{disabledAttr}>");

            if (options != null)
            {
                foreach (var opt in options)
                {
                    var selected = opt.Selected ? " selected=\"selected\"" : string.Empty;
                    var value = System.Web.HttpUtility.HtmlEncode(opt.Value ?? string.Empty);
                    var text = System.Web.HttpUtility.HtmlEncode(opt.Text ?? string.Empty);
                    html.Append($"<option value=\"{value}\"{selected}>{text}</option>");
                }
            }

            html.Append("</select>");
            html.Append("</div>");
            html.Append("</div>");

            return new MvcHtmlString(html.ToString());
        }

        public static MvcHtmlString CustomMultiSelect(this HtmlHelper htmlHelper, string id, string label,
            List<MultiSelectItem> items, int col = 12, string placeHolderText = "--- Chọn ---",
            string moreClass = "", string selectAllText = "Lựa chọn tất cả", string dataAttributeName = "", string onChangeCallback = "")
        {
            var viewModel = new MultiSelectViewModel
            {
                Id = id,
                Label = label,
                Col = col,
                MoreClass = moreClass,
                PlaceHolderText = placeHolderText,
                Items = items ?? new List<MultiSelectItem>(),
                SelectAllText = selectAllText,
                DataAttributeName = dataAttributeName,
                OnChangeCallback = onChangeCallback
            };
            return htmlHelper.Partial("~/Views/Shared/Input/MultiSelect.cshtml", viewModel);
        }

        public static MvcHtmlString CustomMultiSelect<T>(this HtmlHelper htmlHelper, string id, string label,
            List<T> items, Func<T, int> getId, Func<T, string> getText, int col = 12,
            string placeHolderText = "--- Chọn ---", string moreClass = "", string selectAllText = "Lựa chọn tất cả",
            string dataAttributeName = "", Func<T, string> getDataAttributeValue = null, string onChangeCallback = "")
        {
            var multiSelectItems = new List<MultiSelectItem>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    var multiSelectItem = new MultiSelectItem
                    {
                        Id = getId(item),
                        Text = getText(item)
                    };
                    if (getDataAttributeValue != null)
                    {
                        multiSelectItem.DataAttributeValue = getDataAttributeValue(item);
                    }
                    multiSelectItems.Add(multiSelectItem);
                }
            }
            return CustomMultiSelect(htmlHelper, id, label, multiSelectItems, col, placeHolderText, moreClass, selectAllText, dataAttributeName, onChangeCallback);
        }
    }
}