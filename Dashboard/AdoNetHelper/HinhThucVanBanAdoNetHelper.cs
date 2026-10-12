using Dashboard.Common;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Dashboard.AdoNetHelper
{
    public class HinhThucVanBanAdoNetHelper
    {
        public static List<HinhThucVanBanGridVo> GetAllHinhThucVanBanUseAdoDotNet(string connectDBStr)
        {
            var result = new List<HinhThucVanBanGridVo>();
            using (var conn = new SqlConnection(connectDBStr))
            {

                string sql = "SELECT Id, Ten, IsDeleted FROM HinhThucVanBan";

                try
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new HinhThucVanBanGridVo();
                                item.Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id");
                                item.Ten = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Ten");
                                item.IsDeleted = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsDeleted");

                                result.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return result;
        }
    }
}


