using Dashboard.Common;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Dashboard.AdoNetHelper
{
    public class DanhMucTraCuuThongTinAdoNetHelper
    {
        public static List<DanhMucTraCuuThongTinGridVo> GetAllDanhMucTraCuuThongTinUseAdoDotNet(string connectDBStr)
        {
            var result = new List<DanhMucTraCuuThongTinGridVo>();
            using (var conn = new SqlConnection(connectDBStr))
            {

                string sql = "SELECT Id, Ten, IsDeleted FROM DanhMucTraCuuThongTin";

                try
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var vaiTro = new DanhMucTraCuuThongTinGridVo();
                                vaiTro.Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id");
                                vaiTro.Ten = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Ten");
                                vaiTro.IsDeleted = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsDeleted");

                                result.Add(vaiTro);
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