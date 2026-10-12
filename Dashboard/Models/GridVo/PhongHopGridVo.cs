using System.Web.Script.Serialization;

namespace Dashboard.Models.GridVo
{
    public class PhongHopGridVo
    {
        public int Id { get; set; }
        public string TenPhong { get; set; }
        public string DiaDiem { get; set; }
        public int SoNguoi { get; set; }
        public bool? IsDeleted { get; set; }
        [ScriptIgnore]
        public string SearchStr { get; set; }
    }
}