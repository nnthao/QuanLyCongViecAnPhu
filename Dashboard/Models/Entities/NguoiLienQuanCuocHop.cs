using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dashboard.Models.Entities
{
    [Table("NguoiLienQuanCuocHop")]
    public class NguoiLienQuanCuocHop
    {
        public int Id { get; set; }
        public int? CuocHopId { get; set; }
        public int? UserId { get; set; }

        public virtual CuocHop CuocHop { get; set; }
        public virtual User User { get; set; }
    }
}