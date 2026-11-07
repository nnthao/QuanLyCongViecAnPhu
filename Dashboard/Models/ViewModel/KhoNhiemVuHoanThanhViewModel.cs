using Dashboard.Models.CommonModel;
using System;

namespace Dashboard.Models.ViewModel
{
    public class KhoNhiemVuHoanThanhQueryViewModel : QueryParamViewModel
    {
        public int? ThanhVienId { get; set; }
        public int? NguoiGiaoId { get; set; }
    }

    public class HuyDanhGiaViewModel
    {
        public int GiaoViecThanhVienId { get; set; }
        public string LyDoHuy { get; set; }
        public int CurrentUserId { get; set; }
    }
}
