using System;
using System.Web.Script.Serialization;

namespace Dashboard.Models.ViewModel
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginReturnViewModel
    {
        public bool? IsError { get; set; }
        public string Msg { get; set; }
        public string Token { get; set; }
        [ScriptIgnore]
        public DateTime ExpTimeFormat { get; set; }
        public string ExpTime
        {
            get
            {
                return ExpTimeFormat.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }

    public class LoginAuthenViewModel
    {
        public int Id { get; set; }
        public int? VaiTroId { get; set; }
        public int? PhongBanId { get; set; }
        public string Quyen { get; set; }
        public bool? IsSystemUser { get; set; }
        public bool? IsTaiXe { get; set; }
    }
}