using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Dashboard.Models.ViewModel
{
    public class MenuGenerateViewModel
    {
        public MenuGenerateViewModel()
        {
            Datas = new List<MenuGenerateViewModel>();
            Permissions = new List<string>();
            IsAnMenu = false;
            MoTa = "";
        }
        public string Ten { get; set; }
        public string Key { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        [ScriptIgnore]
        public List<string> PhanQuyens
        {
            get
            {
                var phanQuyens = new List<string>();
                foreach (var item in Datas)
                {
                    phanQuyens.Add(item.Key);
                }
                return phanQuyens;
            }
        }
        public string MoTa { get; set; }
        public string SpanCount { get; set; }
        [ScriptIgnore]
        public bool IsAnMenu { get; set; }

        public string Type { get; set; }
        public string Title { get; set; }
        public List<string> Permissions { get; set; }

        public List<MenuGenerateViewModel> Datas { get; set; }
    }
}