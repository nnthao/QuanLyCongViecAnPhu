using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("DatabaseVersion")]
    public class DatabaseVersion
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}