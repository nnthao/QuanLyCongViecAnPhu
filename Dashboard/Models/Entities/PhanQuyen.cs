using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace Dashboard.Models.Entities
{
    [Table("PhanQuyen")]
    public class PhanQuyen
    {
        public PhanQuyen()
        {

        }

        public int Id { get; set; }
        public int? MenuId { get; set; }
        public int? VaiTroId { get; set; }
    }
}