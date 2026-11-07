using Dashboard.Common;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Dashboard.Common
{
    public static class GiaoViecHelper
    {
        public static async Task GuiThongBaoCapNhatTienTrinh(int giaoViecThanhVienId, string noiDungXuLy)
        {
            await Task.CompletedTask;
        }

        public static async Task CapNhatTrangThaiHoanThanhCongViecToanBoAdoNet(int giaoViecId)
        {
            if (giaoViecId <= 0) return;

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                await conn.OpenAsync();

                string sqlCheck = @"
                    SELECT COUNT(1) AS TongSo,
                           SUM(CASE WHEN IsHoanThanh = 1 AND TrangThai = 2 THEN 1 ELSE 0 END) AS SoHoanThanh
                    FROM GiaoViecThanhVien
                    WHERE GiaoViecId = @GiaoViecId AND IsDeleted = 0";

                int tongSo = 0;
                int soHoanThanh = 0;

                using (var cmdCheck = new SqlCommand(sqlCheck, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@GiaoViecId", giaoViecId);
                    using (var reader = await cmdCheck.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            tongSo = reader["TongSo"] != DBNull.Value ? Convert.ToInt32(reader["TongSo"]) : 0;
                            soHoanThanh = reader["SoHoanThanh"] != DBNull.Value ? Convert.ToInt32(reader["SoHoanThanh"]) : 0;
                        }
                    }
                }

                bool isHoanThanhToanBo = tongSo > 0 && tongSo == soHoanThanh;

                using (var cmdUpdate = conn.CreateCommand())
                {
                    cmdUpdate.CommandText = @"UPDATE GiaoViec SET IsHoanThanh = @IsHoanThanh WHERE Id = @Id";
                    cmdUpdate.Parameters.AddWithValue("@IsHoanThanh", isHoanThanhToanBo ? 1 : 0);
                    cmdUpdate.Parameters.AddWithValue("@Id", giaoViecId);
                    await cmdUpdate.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
