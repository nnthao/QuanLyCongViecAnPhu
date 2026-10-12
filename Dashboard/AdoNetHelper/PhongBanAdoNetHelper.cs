using Dashboard.Common;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Dashboard.AdoNetHelper
{
    public class PhongBanAdoNetHelper
    {
        public static List<PhongBanGridVo> GetAllPhongBanUseAdoDotNet(string connectDBStr)
        {
            var result = new List<PhongBanGridVo>();
            using (var conn = new SqlConnection(connectDBStr))
            {
                string sql = "SELECT Id, Ten, IsDeleted, PhongBanChaId, ThuTu, IsPhongNhanViec, NguoiTheoDoiChiDaoId FROM PhongBan ORDER BY ISNULL(ThuTu, 999999) ASC, Ten ASC";

                try
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var vaiTro = new PhongBanGridVo();
                                vaiTro.Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id");
                                vaiTro.PhongBanChaId = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "PhongBanChaId");
                                vaiTro.ThuTu = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "ThuTu");
                                vaiTro.Ten = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "Ten");
                                vaiTro.IsDeleted = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsDeleted");
                                vaiTro.IsPhongNhanViec = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsPhongNhanViec");
                                vaiTro.NguoiTheoDoiChiDaoId = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "NguoiTheoDoiChiDaoId");

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