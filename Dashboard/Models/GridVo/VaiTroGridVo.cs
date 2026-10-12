using System.Collections.Generic;
using Dashboard.Models.ViewModel;

namespace Dashboard.Models.GridVo
{
    public class VaiTroGridVo
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public int UserTotal { get; set; }
        public bool IsUsed => UserTotal > 0;
        public string PhanQuyenKeys { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class PhanQuyenMenu
    {
        public int Id { get; set; }
        public string MoTa { get; set; }
    }
}