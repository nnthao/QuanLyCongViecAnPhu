using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("UserPersonalDriveFolderShare")]
    public class UserPersonalDriveFolderShare
    {
        public int Id { get; set; }
        public int UserPersonalDriveFolderId { get; set; }
        public int UserId { get; set; }
        public int SharedByUserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
