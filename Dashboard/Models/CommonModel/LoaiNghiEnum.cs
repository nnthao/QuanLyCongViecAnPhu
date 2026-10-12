using System.ComponentModel;

namespace Dashboard.Models.CommonModel
{
    public enum LoaiNghiEnum
    {
        [Description("Nghỉ sáng")]
        Sang = 1,
        [Description("Nghỉ chiều")]
        Chieu = 2,
        [Description("Nghỉ cả ngày")]
        CaNgay = 3,
        [Description("Đi trễ")]
        DiTre = 4
    }
}
