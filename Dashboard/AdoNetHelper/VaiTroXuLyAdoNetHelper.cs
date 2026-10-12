using Dashboard.Common;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;//

namespace Dashboard.AdoNetHelper
{
    public class VaiTroXuLyAdoNetHelper
    {
        public static List<VaiTroXuLyGridVo> GetAllVaiTroXuLyUseAdoDotNet(string connectDBStr)
        {
            var result = new List<VaiTroXuLyGridVo>();
            using (var conn = new SqlConnection(connectDBStr))
            {
                string sql = "SELECT Id, Ten, ThuTu, IsDeleted, IsThongKeGiaoViec, IsCanDinhKemFileKhiXacNhanHoanThanh FROM VaiTroXuLy ORDER BY ISNULL(ThuTu, 999999) ASC, Ten ASC";

                try
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new VaiTroXuLyGridVo();
                                item.Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id");
                                item.Ten = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Ten");
                                item.ThuTu = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "ThuTu");
                                item.IsDeleted = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsDeleted");
                                item.IsThongKeGiaoViec = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsThongKeGiaoViec");
                                item.IsCanDinhKemFileKhiXacNhanHoanThanh = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsCanDinhKemFileKhiXacNhanHoanThanh");

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
