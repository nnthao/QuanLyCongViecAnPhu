using Dashboard.Common;
using Dashboard.Models.GridVo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Dashboard.AdoNetHelper
{
    public class DieuHanhXeAdoNetHelper
    {
        public static List<DieuHanhXeGridVo> GetDieuHanhXeTrongDanhSachCuocHopIds(List<int> ids, string connStr)
        {
            var result = new List<DieuHanhXeGridVo>();
            if (ids == null || ids.Count == 0)
                return result;

            var idParams = string.Join(",", ids.Select((id, idx) => "@id" + idx));
            var query = $@"
                SELECT Id, CuocHopId
                FROM DieuHanhXe
                WHERE IsDeleted != 1 AND CuocHopId IN ({idParams})";

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                try
                {
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        for (int i = 0; i < ids.Count; i++)
                        {
                            cmd.Parameters.AddWithValue("@id" + i, ids[i]);
                        }

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new DieuHanhXeGridVo
                                {
                                    Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id"),
                                    CuocHopId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "CuocHopId"),
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

            return result;
        }
    }
}