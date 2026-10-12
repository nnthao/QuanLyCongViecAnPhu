using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("UserPersonalDriveFolder")]
    public class UserPersonalDriveFolder
    {
        public UserPersonalDriveFolder()
        {
            UserPersonalDriveFiles = new List<UserPersonalDriveFile>();
        }

        public int Id { get; set; }
        public int NguoiTaoId { get; set; }
        public int? ParentFolderId { get; set; }
        public string Name { get; set; }
        public string FolderName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<UserPersonalDriveFile> UserPersonalDriveFiles { get; set; }
    }
}
