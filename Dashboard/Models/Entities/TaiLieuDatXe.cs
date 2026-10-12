using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("TaiLieuDatXe")]
    public class TaiLieuDatXe
    {
        public TaiLieuDatXe()
        {
        }

        public int Id { get; set; }
        public int? DieuHanhXeId { get; set; }
        public string Ten { get; set; }
        public string Loai { get; set; }
        public string DuongDan { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual DieuHanhXe DieuHanhXe { get; set; }
    }

    public class TaiLieuDatXeGridVo
    {
        public int Id { get; set; }
        public int? DieuHanhXeId { get; set; }
        public string Name { get; set; }
        public string Loai { get; set; }
        public string DuongDan { get; set; }
        public bool IsUpload => true;
    }
}
