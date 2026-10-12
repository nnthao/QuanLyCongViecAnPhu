using Dashboard.Common;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Dashboard.AdoNetHelper
{
    public class LyDoAdoNetHelper
    {
        public static List<LyDoGridVo> GetAllLyDoUseAdoDotNet(string connectDBStr)
        {
            var list = new List<LyDoGridVo>();
            using (var conn = new SqlConnection(connectDBStr))
            {
                string sql = "SELECT Id, Ten, MaMau, IsDeleted FROM LyDo";
                try
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var chucDanh = new LyDoGridVo();
                                chucDanh.Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id");
                                chucDanh.Ten = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Ten");
                                chucDanh.MaMau = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "MaMau");
                                chucDanh.IsDeleted = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsDeleted");
                                list.Add(chucDanh);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return list;

        }
    }
}