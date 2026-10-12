using log4net;
using System;
using System.Data.SqlClient;

namespace Dashboard.AdoNetHelper
{
    public class GiaoViecFileAdoNetHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MvcApplication));

        public static void Them(int giaoViecId, string tenFile, string duongDan, string connStr)
        {
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string sql = @"INSERT INTO GiaoViecFile (GiaoViecId, TenFile, DuongDan, IsDeleted) 
                                   VALUES (@GiaoViecId, @TenFile, @DuongDan, 0)";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@GiaoViecId", giaoViecId);
                        cmd.Parameters.AddWithValue("@TenFile", (object)tenFile ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DuongDan", (object)duongDan ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi thêm GiaoViecFile qua AdoNet: {ex.Message}", ex);
            }
        }
    }
}
