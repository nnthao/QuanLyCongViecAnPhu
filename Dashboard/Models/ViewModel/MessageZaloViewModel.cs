using System;
namespace Dashboard.Models.ViewModel
{
    public class MessageZaloViewModel
    {
        public UserId recipient;
        public Messages message;
    }
    public  class UserId
    {
        public string user_id;
    }
    public class Messages
    {
        public string text;
    }
    public class MessageId
    {
        public string message_id;
        public string user_id;
    }
    public class MessageZaloViewModelHCC
    {
        public MessageId recipient;
        public Messages message;
    }

    public class ZaloTestMessageRequest
    {
        public string SoDienThoai { get; set; }
        public string NoiDung { get; set; }
    }

    public class ZaloApiMessageRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string UserZaloId { get; set; }
        public string MessageId { get; set; }
        public string NoiDung { get; set; }
        public bool ChoPhepLamMoiToken { get; set; }
    }

    public class ZaloReplyMessageRequest
    {
        public string NoiDung { get; set; }
        public string MessageId { get; set; }
        public string UserZaloId { get; set; }
    }

    public class ZaloTokenUpdateRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class ZaloTokenInfo
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class ZaloRefreshTokenResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class ZaloSendMessageResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ZaloLienKetRequest
    {
        public string SoDienThoai { get; set; }
        public string MessageId { get; set; }
        public string UserZaloId { get; set; }
    }

    public enum ZaloLienKetStatus
    {
        ThanhCong = 1,
        ChuaCoTrenHeThong = 2,
        DaLienKet = 3
    }

    public class ZaloLienKetResult
    {
        public ZaloLienKetStatus Status { get; set; }
        public string Message { get; set; }
        public Entities.User User { get; set; }
    }
}
