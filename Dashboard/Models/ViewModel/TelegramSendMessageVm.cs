using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dashboard.Models.ViewModel
{
    public class TelegramSendMessageVm
    {
        public TelegramSendMessageVm()
        {
            RowButton = 2;
        }

        public string Token { get; set; }
        public string ChatId { get; set; }
        public string Message { get; set; }

        public int MessageId { get; set; }
        public string ButtonStr { get; set; }
        public List<TelegramButton> Buttons
        {
            get
            {
                if (!string.IsNullOrEmpty(ButtonStr))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<List<TelegramButton>>(ButtonStr);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                return new List<TelegramButton>();
            }
        }

        public int RowButton { get; set; }
    }

    public class TelegramButton
    {
        public string Ten { get; set; }
        public string CallBack { get; set; }
        public string Link { get; set; }
    }
}
