using Dashboard.Common;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Dashboard.AdoNetHelper
{
    public class DoKhanAdoNetHelper
    {
        public static List<DoKhanGridVo> GetAllDoKhanUseAdoDotNet(string connectDBStr)
        {
            var result = new List<DoKhanGridVo>();
            using (var conn = new SqlConnection(connectDBStr))
            {

                string sql = "SELECT Id, Ten, IsDeleted FROM DoKhan";

                try
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new DoKhanGridVo();
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


