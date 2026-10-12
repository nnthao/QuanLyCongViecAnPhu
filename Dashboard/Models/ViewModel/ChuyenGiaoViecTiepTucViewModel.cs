using Dashboard.Common;
using Dashboard.Models.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.Models.ViewModel
{
    public class ChuyenGiaoViecTiepTucViewModel
    {
        public int GiaoViecId { get; set; }
        public int GiaoViecThanhVienId { get; set; }
        public int? CurrentUserId { get; set; }
        public string ThanhVienStr { get; set; }

        public List<GiaoViecThanhVienViewModel> ThanhViens
        {
            get
            {
                if (!string.IsNullOrEmpty(ThanhVienStr))
                {
                    return JsonConvert.DeserializeObject<List<GiaoViecThanhVienViewModel>>(ThanhVienStr);
                }

                return new List<GiaoViecThanhVienViewModel>();
            }
        }
    }
}

