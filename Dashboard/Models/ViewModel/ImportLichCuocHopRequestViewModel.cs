using Dashboard.Models.GridVo;
using System.Collections.Generic;

namespace Dashboard.Models.ViewModel
{
    public class ImportLichCuocHopRequestViewModel
    {
        public ImportLichCuocHopRequestViewModel()
        {
            DanhSachCuocHop = new List<CuocHopImportGridVo>();
        }

        public int? LyDoId { get; set; }
        public int UserId { get; set; }
        public int? PhongBanId { get; set; }
        public List<CuocHopImportGridVo> DanhSachCuocHop { get; set; }
    }
}
