using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("GiaoViecGoiYNoiDung")]
    public class GiaoViecGoiYNoiDung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string NoiDung { get; set; }

        public int SoLanSuDung { get; set; }

        public int? NguoiTaoId { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public int? Loai { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
