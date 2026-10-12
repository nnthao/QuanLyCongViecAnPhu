using Dashboard.Models.ViewModel;
using log4net;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Dashboard.Common
{
    public class TelegramHelper
    {
        private static ILog log = log4net.LogManager.GetLogger(typeof(MvcApplication));

        public static void InitBotAsync()
        {
            if (!string.IsNullOrEmpty(BienDungChung.TelegramToken))
            {
                try
                {
                    var botClient = new TelegramBotClient(BienDungChung.TelegramToken);

                    var cts = new CancellationTokenSource();
                    var receiverOptions = new ReceiverOptions
                    {
                        AllowedUpdates = Array.Empty<UpdateType>()
                    };

                    botClient.StartReceiving(
                        HandleUpdateAsync,
                        HandleErrorAsync,
                        receiverOptions,
                        cancellationToken: cts.Token
                    );

                    log.Info("Telegram bot đã khởi tạo thành công");

                    SendMessageAsync(new TelegramSendMessageVm
                    {
                        ChatId = BienDungChung.TelegramChatId,
                        Message = $"Service đã khởi động thành công lúc {DateTime.Now}",
                    });
                }
                catch (Exception ex)
                {
                    log.Error("Lỗi khởi tạo bot telegram: " + ex.Message, ex);
                }
            }
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                string text = update.Message.Text;
                long chatId = update.Message.Chat.Id;

                switch (text)
                {
                    case "/id":
                        _ = SendMessageAsync(new TelegramSendMessageVm
                        {
                            ChatId = $"{chatId}",
                            Message = $"{chatId}"
                        });
                        break;

                    case "/now":
                        _ = SendMessageAsync(new TelegramSendMessageVm
                        {
                            ChatId = $"{chatId}",
                            Message = $"Thời gian HT: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}",
                        });
                        break;

                    case "/start":
                        _ = SendMessageAsync(new TelegramSendMessageVm
                        {
                            ChatId = $"{chatId}",
                            Message = $"Nhập số điện thoại trên hệ thống để kết nối Telegram",
                        });
                        break;

                    default:
                        if (IsValidVietnamPhoneNumber(text))
                        {
                            var userInfo = GetUserInfoByPhone(text);
                            if (userInfo == null)
                            {
                                _ = SendMessageAsync(new TelegramSendMessageVm
                                {
                                    ChatId = $"{chatId}",
                                    Message = "Không tìm thấy số điện thoại trong hệ thống.",
                                });
                            }
                            else if (!string.IsNullOrEmpty(userInfo.TelegramId))
                            {
                                _ = SendMessageAsync(new TelegramSendMessageVm
                                {
                                    ChatId = $"{chatId}",
                                    Message = $"Số điện thoại đã được liên kết với email: {userInfo.Email}",
                                });
                            }
                            else
                            {
                                bool updateSuccess = UpdateTelegramIdByPhone(chatId, text);
                                if (updateSuccess)
                                {
                                    var domain = CacheManagerHelper.GetDomainCache();
                                    _ = SendMessageAsync(new TelegramSendMessageVm
                                    {
                                        ChatId = $"{chatId}",
                                        Message = $"Kết nối telegram thành công với email: {userInfo.Email}!<a href='{domain}'> Click vào đường dẫn này </a> để đăng nhập lại hệ thống điều hành.",
                                    });
                                }
                                else
                                {
                                    _ = SendMessageAsync(new TelegramSendMessageVm
                                    {
                                        ChatId = $"{chatId}",
                                        Message = "Không tìm thấy số điện thoại trong hệ thống.",
                                    });
                                }
                            }
                        }
                        else
                        {
                            _ = SendMessageAsync(new TelegramSendMessageVm
                            {
                                ChatId = $"{chatId}",
                                Message = "Lệnh không hợp lệ.",
                            });
                        }
                        break;
                }
            }
        }

        private static Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
        {
            log.Error("Lỗi xử lý telegram bot: " + exception.Message, exception);
            return Task.CompletedTask;
        }

        public static async Task SendMessageAsync(TelegramSendMessageVm telegramSendMessageVm)
        {
            log.Info("Chat Id: " + telegramSendMessageVm.ChatId);
            log.Info("Message: " + telegramSendMessageVm.Message);
            log.Info("BienDungChung.TelegramToken: " + BienDungChung.TelegramToken);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if (!string.IsNullOrEmpty(BienDungChung.TelegramToken))
            {
                var botClient = new TelegramBotClient(BienDungChung.TelegramToken);
                if (botClient == null) return;

                try
                {
                    var message = await botClient.SendMessage(
                          chatId: telegramSendMessageVm.ChatId,
                          text: telegramSendMessageVm.Message,
                          parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
                      );

                    var messageId = message.MessageId;
                    log.Info("messageId " + messageId);
                }
                catch (Exception ex)
                {
                    log.Error("Lỗi gửi tin nhắn telegram: " + ex.Message, ex);
                }
            }
        }

        public static bool IsValidVietnamPhoneNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            string normalized = input.Trim();
            normalized = normalized.Replace(" ", string.Empty)
                                   .Replace("-", string.Empty)
                                   .Replace(".", string.Empty);

            if (normalized.StartsWith("+84"))
            {
                normalized = "0" + normalized.Substring(3);
            }

            return Regex.IsMatch(normalized, "^0(3|5|7|8|9)\\d{8}$");
        }

        private static Models.Entities.User GetUserInfoByPhone(string phoneNumber)
        {
            try
            {
                using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
                {
                    conn.Open();
                    string sql = "SELECT Id, Email, Phone, TelegramId FROM Users WHERE Phone = @Phone";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var user = new Models.Entities.User
                                {
                                    Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id"),
                                    Email = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Email"),
                                    Phone = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Phone"),
                                    TelegramId = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TelegramId")
                                };
                                return user;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi lấy thông tin user theo số điện thoại: {ex.Message}", ex);
                return null;
            }
        }

        private static bool UpdateTelegramIdByPhone(long telegramId, string phoneNumber)
        {
            try
            {
                using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
                {
                    conn.Open();
                    string sql = "UPDATE Users SET TelegramId = @TelegramId, TelegramDateUpdate = @TelegramDateUpdate WHERE Phone = @Phone";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TelegramId", telegramId.ToString());
                        cmd.Parameters.AddWithValue("@TelegramDateUpdate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            log.Info($"Đã cập nhật TelegramId {telegramId} cho user có phone {phoneNumber}");
                            CacheManagerHelper.InitUserCache();
                            return true;
                        }

                        log.Warn($"Không tìm thấy user có phone {phoneNumber}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi cập nhật TelegramId: {ex.Message}", ex);
                return false;
            }
        }
    }
}