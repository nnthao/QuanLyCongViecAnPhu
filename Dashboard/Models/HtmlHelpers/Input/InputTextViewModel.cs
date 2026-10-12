using System.Collections.Generic;

namespace Dashboard.Models.HtmlHelpers.Input
{
    public class InputTextViewModel : ControlSharedViewModelBase
    {
        public InputTextViewModel()
        {
            IsReadonly = false;
        }
        public string Class { get; set; }
        public string Style { get; set; }
        public bool Required { get; set; }
        public string PlaceHolderText { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsReadonly { get; set; }
        public string MoreClass { get; set; }
        public string TextboxType { get; set; }
        public string value { get; set; }
        public string ErrorId => Id + "Error";
        public bool IsDecimal { get; set; }
        public string Min { get; set; }
        public string multiple { get; set; }
        public string acceptfile { get; set; }
        public string Value1Radio { get; set; }
        public string Value2Radio { get; set; }
        public bool Ckeditor { get; set; }
        public int MaxLength { get; set; }
        public bool IsChecked { get; set; }
        public bool PreviewImage { get; set; }
        public string Key { get; set; }
        public string MoTa { get; set; }
        public bool IsMultiple { get; set; }
        public bool ShowDelete { get; set; }
        public string IdContainer { get; set; }
    }

    public class VaiTroGridViewModel : ControlSharedViewModelBase
    {
        public VaiTroGridViewModel()
        {
            Datas = new List<VaiTroGridData>();
        }
        public List<VaiTroGridData> Datas { get; set; }
    }

    public class VaiTroGridData
    {
        public string Key { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
    }

    public class MultiSelectViewModel : ControlSharedViewModelBase
    {
        public MultiSelectViewModel()
        {
            Items = new List<MultiSelectItem>();
        }
        public string PlaceHolderText { get; set; }
        public string MoreClass { get; set; }
        public List<MultiSelectItem> Items { get; set; }
        public string SelectAllText { get; set; }
        public string DataAttributeName { get; set; }
        public string OnChangeCallback { get; set; }
    }

    public class MultiSelectItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string DataAttributeValue { get; set; }
    }
}