using Dashboard.Common;
using Dashboard.Models.GridVo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Dashboard.AdoNetHelper
{
    public class PhongHopAdoNetHelper
    {
        public static List<PhongHopGridVo> GetAllPhongHopUseAdoDotNet(string connectDBStr)
        {
            var list = new List<PhongHopGridVo>();
            using (var conn = new SqlConnection(connectDBStr))
            {
                string sql = "SELECT Id, TenPhong, IsDeleted FROM PhongHop";
                try
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var phongHop = new PhongHopGridVo();
                                phongHop.Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id");
                                phongHop.TenPhong = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenPhong");
                                phongHop.IsDeleted = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsDeleted");
                                list.Add(phongHop);
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