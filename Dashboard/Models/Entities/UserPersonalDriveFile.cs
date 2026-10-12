using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("UserPersonalDriveFile")]
    public class UserPersonalDriveFile
    {
        public int Id { get; set; }
        public int NguoiTaoId { get; set; }
        public int? UserPersonalDriveFolderId { get; set; }
        public string FileName { get; set; }
        public string DuongDan { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual UserPersonalDriveFolder UserPersonalDriveFolder { get; set; }
    }
}
