using System;
using System.Text;

namespace Dashboard.Models.ViewModel
{
    public class ErrorViewModel
    {
        public string Message { get; set; }
        public string MessageDecode
        {
            get
            {
                if (string.IsNullOrEmpty(Message))
                {
                    return "";
                }

                return Encoding.UTF8.GetString(Convert.FromBase64String(Message));
            }
        }
    }
}