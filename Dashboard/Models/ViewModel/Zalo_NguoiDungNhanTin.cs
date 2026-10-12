using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Models.ViewModel
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Sender
    {
        public string id { get; set; }
    }

    public class Recipient
    {
        public string id { get; set; }
    }

    public class Message
    {
        public string text { get; set; }
        public string msg_id { get; set; }
    }

    public class Zalo_NguoiDungNhanTin
    {
        public string app_id { get; set; }
        public Sender sender { get; set; }
        public string user_id_by_app { get; set; }
        public Recipient recipient { get; set; }
        public string event_name { get; set; }
        public Message message { get; set; }
        public string timestamp { get; set; }
    }


}