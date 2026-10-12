using System;
using System.ComponentModel;
using System.Configuration;

namespace Dashboard.Common
{
    public class BienDungChung
    {
        public static readonly string pattern = @"((09|03|07|08|05)+([0-9]{8})\b)";

        public static string FolderSavePath => "/FileTemp";
        public static string FolderTempPath => "/Temp";
        public static string UploadsPath => "/Uploads";

        public static string JwtKeySecret => "AppDieuHanhVNPTQNI@20250519103000";

        public static string JwtClaimInfoKey => "Info";

        public static readonly string DatabaseChecked = "DatabaseChecked";

        public const int LOAI_LICH_TUAN = 1;
        public const int LOAI_LICH_THANG = 2;

        public const int LOAI_FILE_EXCEL = 1;
        public const int LOAI_FILE_PDF = 2;

        public const string LichHopTuanExportTemplate = "/Temp/LichHopTuan.xlsx";
        public const string LichHopXeExportTemplate = "/Temp/LichHopXe.xlsx";
        public const string MenuJsonPath = "/Temp/Menu.json";
        public const string VersionPath = "/Temp/version.json";

        public static string ConnectDBStr = ConfigurationManager.ConnectionStrings["ConnectDB"].ConnectionString;
        public static int IsBatThongBao = ConfigurationManager.AppSettings["IsBatThongBao"] != null
            ? Convert.ToInt32(ConfigurationManager.AppSettings["IsBatThongBao"]) : 0;
        public static int IsBatThongBaoTelegram = ConfigurationManager.AppSettings["IsBatThongBaoTelegram"] != null
          ? Convert.ToInt32(ConfigurationManager.AppSettings["IsBatThongBaoTelegram"]) : 0;

        public static string TelegramToken = ConfigurationManager.AppSettings["TelegramToken"] != null
     ? ConfigurationManager.AppSettings["TelegramToken"].ToString() : "";

        public static string TelegramChatId = ConfigurationManager.AppSettings["TelegramChatId"] != null
     ? ConfigurationManager.AppSettings["TelegramChatId"].ToString() : "";

        public static string BotUrl = ConfigurationManager.AppSettings["BotUrl"] != null
     ? ConfigurationManager.AppSettings["BotUrl"].ToString() : "";

        public static string Domain = ConfigurationManager.AppSettings["Domain"] != null
     ? ConfigurationManager.AppSettings["Domain"].ToString() : "";

        public static string ScheduledUrl = ConfigurationManager.AppSettings["ScheduledUrl"] != null
     ? ConfigurationManager.AppSettings["ScheduledUrl"].ToString() : "";

        public static int ScheduledIntervalMinutes = ConfigurationManager.AppSettings["ScheduledIntervalMinutes"] != null
     ? Convert.ToInt32(ConfigurationManager.AppSettings["ScheduledIntervalMinutes"]) : 30;

        public const string FileAllowUploads = ".pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.txt,.jpg,.jpeg,.png,.gif,.zip,.rar";

        public static string AllowedFileExtensions => ConfigurationManager.AppSettings["AllowedFileExtensions"] != null
            ? ConfigurationManager.AppSettings["AllowedFileExtensions"].ToString() : ".doc,.docx,.xls,.xlsx,.ppt,.pdf,.pptx,.jpg,.jpeg,.png,.gif,.bmp,.webp,.svg,.txt,.zip,.rar";

        //public static string zaloflag => ConfigurationManager.AppSettings["zaloflag"]; // = rỗng để kết nối với webhook zalo
        public static string AccessToken => ConfigurationManager.AppSettings["AccessToken"];
        public static string RefreshToken=> ConfigurationManager.AppSettings["RefreshToken"];
        public static long AppId => long.Parse(ConfigurationManager.AppSettings["AppId"]);
        public static string AppSecret => ConfigurationManager.AppSettings["AppSecret"];
        public static int SoNgaySapDenHan => ConfigurationManager.AppSettings["SoNgaySapDenHan"] != null
            ? Convert.ToInt32(ConfigurationManager.AppSettings["SoNgaySapDenHan"]) : 1;

        public static string IofficeHost => ConfigurationManager.AppSettings["IofficeHost"] != null
            ? ConfigurationManager.AppSettings["IofficeHost"].ToString() : "https://office.quangngai.gov.vn/qlvb_qni";

        public static string IofficeUsername => ConfigurationManager.AppSettings["IofficeUsername"] != null
            ? ConfigurationManager.AppSettings["IofficeUsername"].ToString() : "admin";

        public static string IofficePassword => ConfigurationManager.AppSettings["IofficePassword"] != null
            ? ConfigurationManager.AppSettings["IofficePassword"].ToString() : "3#3O89nMo2O8i7hT@A5g!";

        //Trạng thái thông tin tra cứu
        public const int HOATDONG = 1;
        public const int KHONGHOATDONG = 1;

        // trạng thái cuộc họp
        public const string STATUS_CUOCHOP_CHODUYET = "ChoDuyet";
        public const string STATUS_CUOCHOP_DADUYET = "DaDuyet";
        public const string STATUS_CUOCHOP_TRADUYET = "TraDuyet";
    }

    public enum QuyenMoRong
    {
        [Description("Quyền tham gia dự án")]
        QuyenThamGiaDuAn,
        [Description("Quyền Giao việc tất cả phòng ban")]
        QuyenGiaoViecTatCaPhongBan,
        [Description("Quyền CRUD tất cả phòng ban")]
        QuyenCRUDTatCaPhongBan,
        [Description("Nhận tin nhắn thống kê công việc hàng ngày")]
        NhanTinNhanThongKeCongViecHangNgay,
    }
}