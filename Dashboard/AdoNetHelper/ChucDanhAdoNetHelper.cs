using Dashboard.Common;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Dashboard.AdoNetHelper
{
    public class ChucDanhAdoNetHelper
    {
        public static List<ChucDanhGridVo> GetAllChucDanhUseAdoDotNet(string connectDBStr)
        {
            var list = new List<ChucDanhGridVo>();
            using (var conn = new SqlConnection(connectDBStr))
            {
                string sql = "SELECT Id, Ten, ThuTu, IsDeleted FROM ChucDanh";
                try
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var chucDanh = new ChucDanhGridVo();
                                chucDanh.Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id");
                                chucDanh.Ten = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Ten");
                                chucDanh.ThuTu = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "ThuTu");
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