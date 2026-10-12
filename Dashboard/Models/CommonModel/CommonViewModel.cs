using System.ComponentModel;

namespace Dashboard.Models.CommonModel
{
    public class CommonViewModel
    {
    }

    public enum TrangThaiHoatDong : int
    {
        [Description("Hoạt động")]
        HoatDong = 1,
        [Description("Không hoạt động")]
        KhongHoatDong = 2
    }
}