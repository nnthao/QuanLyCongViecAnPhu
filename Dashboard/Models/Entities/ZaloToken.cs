using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models.Entities
{
    [Table("ZaloToken")]
    public class ZaloToken
    {
        public int Id { get; set; }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}

