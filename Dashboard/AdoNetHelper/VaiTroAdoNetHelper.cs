using Dashboard.Common;
using Dashboard.Models.GridVo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.AdoNetHelper
{
    public class VaiTroAdoNetHelper
    {
        public static List<VaiTroGridVo> GetAllVaiTroUseAdoDotNet(string connectDBStr)
        {
            var list = new List<VaiTroGridVo>();
            using (var conn = new SqlConnection(connectDBStr))
            {
                string sql = "SELECT Id, Ten, PhanQuyenKeys, IsDeleted FROM VaiTro";
                try
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var vaiTro = new VaiTroGridVo();
                                vaiTro.Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id");
                                vaiTro.Ten = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Ten");
                                vaiTro.PhanQuyenKeys = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "PhanQuyenKeys");
                                vaiTro.IsDeleted = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsDeleted");
                                list.Add(vaiTro);
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

        public static List<int> GetVaiTriIdCoQuyen(string quyen)
        {
            var vaiTros = CacheManagerHelper.GetVaiTroCache();
            var list = vaiTros.Where(o => o.IsDeleted != true
                && !string.IsNullOrEmpty(o.PhanQuyenKeys)
                && o.PhanQuyenKeys.Split(',').Any(x => x.Trim() == quyen))
                .Select(o => o.Id).ToList();

            return list;
        }
    }
}
