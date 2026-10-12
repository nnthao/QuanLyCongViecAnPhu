using Dashboard.Common;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace Dashboard.AdoNetHelper
{
    public class UserAdoNetHelper
    {
        public static List<UserGridVo> GetTruongDonViByPhongBanIds(List<int> phongBanIds, string connStr)
        {
            if (phongBanIds == null || phongBanIds.Count == 0)
                return new List<UserGridVo>();

            var users = new List<UserGridVo>();
            var idsParam = string.Join(",", phongBanIds);


            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                try
                {
                    string sql = $@"SELECT Id, Name, PhongBanId FROM Users WHERE PhongBanId IN ({idsParam})
                                AND IsTruongDonVi = 1";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new UserGridVo
                                {
                                    Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id"),
                                    Name = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Name"),
                                    PhongBanId = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "PhongBanId")
                                });
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return users;
        }

        public static List<UserGridVo> GetAllUserUseAdoDotNet(string connStr)
        {
            var list = new List<UserGridVo>();

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = $@"SELECT Id, Email, Name, Phone, VaiTroId, IsSystemUser, IsDeleted, PhongBanId, ChucDanhId, NhomVaiTroId, TelegramId, UserZaloId, IsLanhDaoPhong
                        FROM Users WHERE IsSystemUser != 1 or IsSystemUser is null ";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new UserGridVo
                            {
                                Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id"),
                                Email = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Email"),
                                Name = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Name"),
                                Phone = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Phone"),
                                VaiTroId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "VaiTroId"),
                                IsSystemUser = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsSystemUser"),
                                IsDeleted = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsDeleted"),
                                PhongBanId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "PhongBanId"),
                                ChucDanhId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "ChucDanhId"),
                                NhomVaiTroId = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "NhomVaiTroId"),
                                TelegramId = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TelegramId"),
                                UserZaloId = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "UserZaloId"),
                                IsLanhDaoPhong = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsLanhDaoPhong")
                            });
                        }
                    }
                }
            }

            return list;
        }

        public static async Task TangLanLoginFailAsync(int userId, string connStr)
        {
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Lấy LoginFail hiện tại
                int? loginFail = null;
                using (var cmd = new SqlCommand("SELECT LoginFail FROM Users WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", userId);
                    var result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        loginFail = Convert.ToInt32(result);
                    }
                }

                // Tăng hoặc gán LoginFail
                if (loginFail == null)
                {
                    loginFail = 1;
                }
                else
                {
                    loginFail++;
                }

                // Update lại vào DB
                using (var cmd = new SqlCommand("UPDATE Users SET LoginFail = @LoginFail WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@LoginFail", loginFail);
                    cmd.Parameters.AddWithValue("@Id", userId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public static async Task ResetLoginFail(int userId, string connStr)
        {
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                // Update lại vào DB
                using (var cmd = new SqlCommand("UPDATE Users SET LoginFail = @LoginFail WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@LoginFail", 0);
                    cmd.Parameters.AddWithValue("@Id", userId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public static List<UserGridVo> GetAllUserDuyetLichDangKy()
        {
            var users = CacheManagerHelper.GetUserCache();


            return users.Where(u => u.IsDeleted != true).ToList();
        }
        public static List<UserGridVo> GetTruongDonViByCoQuanIds(List<int> coQuanIds, string connStr)
        {
            if (coQuanIds == null || coQuanIds.Count == 0)
                return new List<UserGridVo>();

            var users = new List<UserGridVo>();
            var idsParam = string.Join(",", coQuanIds);


            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                try
                {
                    string sql = $@"SELECT Id, Name, CoQuanId FROM Users WHERE CoQuanId IN ({idsParam})
                                AND IsTruongDonVi = 1";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new UserGridVo
                                {
                                    Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id"),
                                    Name = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Name"),
                                    //CoQuanId = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "CoQuanId")
                                });
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return users;
        }
    }
}