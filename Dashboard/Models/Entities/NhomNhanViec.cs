using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("NhomNhanViec")]
    public class NhomNhanViec
    {
        public NhomNhanViec()
        {
            NhomNhanViecThanhViens = new HashSet<NhomNhanViecThanhVien>();
        }
        public int Id { get; set; }
        public int? NguoiTaoId { get; set; }

        public string Ten { get; set; }

        public string MoTa { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<NhomNhanViecThanhVien> NhomNhanViecThanhViens { get; set; }
    }

    public class NhomNhanViecGridVo
    {
        public NhomNhanViecGridVo()
        {
            ThanhViens = new List<int?>();
            ThanhVienNhomNhanViecs = new List<ThanhVienNhomNhanViecGridVo>();
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public bool IsDeleted { get; set; }
        public List<int?> ThanhViens { get; set; }
        public List<ThanhVienNhomNhanViecGridVo> ThanhVienNhomNhanViecs { get; set; }
    }

    public class ThanhVienNhomNhanViecGridVo
    {
        public int? UserId { get; set; }
        public string Name { get; set; }
    }


    public class NhomNhanViecViewModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }

        public string MoTa { get; set; }

        public bool IsDeleted { get; set; }

        public int? NguoiTaoId { get; set; }
        public string UserIds { get; set; }
        public List<int> UserIntIds
        {
            get
            {
                return string.IsNullOrEmpty(UserIds) ? new List<int>() :
                    new List<int>(Array.ConvertAll(UserIds.Split(','), int.Parse));
            }
        }
    }
}

