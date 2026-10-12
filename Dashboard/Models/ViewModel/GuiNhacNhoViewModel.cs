using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dashboard.Models.ViewModel
{
    public class GuiNhacNhoViewModel
    {
        public int GiaoViecId { get; set; }
        public string TenCongViec { get; set; }
        public string ThanhViens { get; set; }

        public List<ThanhVienNhacNho> ThanhVienList
        {
            get
            {
                if (!string.IsNullOrEmpty(ThanhViens))
                {
                    return JsonConvert.DeserializeObject<List<ThanhVienNhacNho>>(ThanhViens);
                }
                return new List<ThanhVienNhacNho>();
            }
        }
    }

    public class ThanhVienNhacNho
    {
        public int GiaoViecThanhVienId { get; set; }
        public int? ThanhVienId { get; set; }
        public string HanXuLy { get; set; }
    }
}










