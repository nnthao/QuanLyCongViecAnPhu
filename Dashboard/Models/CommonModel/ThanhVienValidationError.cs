using System.Collections.Generic;

namespace Dashboard.Models.CommonModel
{
    public class ThanhVienValidationError
    {
        public string TenNguoiDung { get; set; }

        public List<string> DanhSachLoi { get; set; }

        public ThanhVienValidationError()
        {
            DanhSachLoi = new List<string>();
        }
    }
}

