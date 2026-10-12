using Dashboard.Common;
using Dashboard.Models.Entities;
using log4net;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Dashboard.AdoNetHelper
{
    public class ThongBaoAdoNetHelper
    {
        private static ILog log = log4net.LogManager.GetLogger(typeof(MvcApplication));
        public static async Task Them(ThongBao thongBao, string connStr)
        {
            try
            {
                if (BienDungChung.IsBatThongBao == 1)
                {
                    using (var conn = new SqlConnection(connStr))
                    {
                        await conn.OpenAsync();

                        string sql = @"INSERT INTO ThongBao (NoiDung,NgayTao,NguoiNhanId, EnumLoaiThongBao, ThongBaoInfoJson, IsDaXem) 
                       VALUES (@NoiDung, @NgayTao, @NguoiNhanId, @EnumLoaiThongBao, @ThongBaoInfoJson, @IsDaXem)";

                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@NoiDung", thongBao.NoiDung);
                            cmd.Parameters.AddWithValue("@NgayTao", DateTime.Now);
                            cmd.Parameters.AddWithValue("@NguoiNhanId", thongBao.NguoiNhanId);
                            cmd.Parameters.AddWithValue("@IsDaXem", 0);
                            cmd.Parameters.AddWithValue("@EnumLoaiThongBao", thongBao.EnumLoaiThongBao);
                            cmd.Parameters.AddWithValue("@ThongBaoInfoJson", (object)thongBao.ThongBaoInfoJson ?? DBNull.Value);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                log.Error(exception.Message + Environment.NewLine + exception.StackTrace);
            }

            if (thongBao.NguoiNhanId.GetValueOrDefault() != 0)
            {
                var user = CacheManagerHelper.GetUserByIdFromCache(thongBao.NguoiNhanId.Value);
                if (user != null)
                {
                    var thongBaoVo = new ThongBaoGridVo
                    {
                        Id = thongBao.Id,
                        NoiDung = thongBao.NoiDung,
                        EnumLoaiThongBao = thongBao.EnumLoaiThongBao
                    };
                    var domain = CacheManagerHelper.GetDomainCache();

                    if (!string.IsNullOrEmpty(user.UserZaloId))
                    {
                        try
                        {
                            var textZalo = $"{thongBaoVo.TieuDe.ToUpper()}:\n{thongBao.NoiDung}";
                            if (!string.IsNullOrEmpty(domain))
                            {
                                textZalo += $"\nTruy cập: {domain}";
                            }
                            ZaloHelper.GuiTinNhanQuaZaloOA(textZalo, user.UserZaloId);
                        }
                        catch (Exception exZalo)
                        {
                            log.Error($"Lỗi khi gửi tin nhắn Zalo tới user {user.Name}: {exZalo.Message}", exZalo);
                        }
                    }

                    if (BienDungChung.IsBatThongBaoTelegram == 1 && !string.IsNullOrEmpty(user.TelegramId))
                    {
                        try
                        {
                            var message = thongBaoVo.TieuDe.ToUpper() + ":\n" + thongBaoVo.NoiDung;
                            if (!string.IsNullOrEmpty(domain))
                            {
                                message += $"\n\n<a href='{domain}'>🔗 Truy cập trang</a>";
                            }
                            var item = TelegramHelper.SendMessageAsync(new Models.ViewModel.TelegramSendMessageVm
                            {
                                ChatId = user.TelegramId,
                                Message = message
                            });
                        }
                        catch (Exception exTelegram)
                        {
                            log.Error($"Lỗi khi gửi tin nhắn Telegram tới user {user.Name}: {exTelegram.Message}", exTelegram);
                        }
                    }
                }
            }
        }

        public static void Xoa(int id, string connStr)
        {
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    string sql = "UPDATE ThongBao SET IsDeleted = 1 WHERE Id = @Id";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static void KhoiPhuc(int id, string connStr)
        {
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    string sql = "UPDATE ThongBao SET IsDeleted = 0 WHERE Id = @Id";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static void DanhDauDaXem(int id, string connStr)
        {
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    string sql = "UPDATE ThongBao SET IsDaXem = 1 WHERE Id = @Id";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void DanhDauDaXemNhieu(string ids, string connStr)
        {
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    string sql = $"UPDATE ThongBao SET IsDaXem = 1 WHERE Id IN ({ids})";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void DanhDauDaXemTatCa(int userId, string connStr)
        {
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    string sql = $"UPDATE ThongBao SET IsDaXem = 1 WHERE NguoiNhanId = {userId} ";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static int DemThongBaoChuaXem(int userId, string connStr)
        {
            int count = 0;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"SELECT COUNT(*) FROM ThongBao 
                                   WHERE NguoiNhanId = @NguoiNhanId 
                                     AND (IsDaXem IS NULL OR IsDaXem = 0) 
                                     AND (IsDeleted IS NULL OR IsDeleted = 0)";
                try
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NguoiNhanId", userId);
                        count = (int)cmd.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            return count;
        }
    }
}