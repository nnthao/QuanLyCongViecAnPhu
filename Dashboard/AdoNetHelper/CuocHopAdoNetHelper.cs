using Dashboard.Common;
using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Dashboard.AdoNetHelper
{
    public class CuocHopAdoNetHelper
    {
        public static CuocHop GetCuocHopByIdUseAdoNet(string connStr, int id)
        {
            CuocHop cuocHop = null;

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @" SELECT Id,TenCuocHop, PhongHopId, ThoiGianBatDau, ThoiGianKetThuc, ThoiGianKetThucTemp, NguoiChuTri, NguoiThamGia, NguoiThamGiaKhac, CoQuanChuTriId, CoQuanThamGiaId, PhongHopTxt
                           FROM CuocHop
                           WHERE Id = @Id";
                try
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cuocHop = new CuocHop
                                {
                                    Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id"),
                                    TenCuocHop = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenCuocHop"),
                                    PhongHopId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "PhongHopId"),
                                    ThoiGianBatDau = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "ThoiGianBatDau"),
                                    ThoiGianKetThuc = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "ThoiGianKetThuc"),
                                    ThoiGianKetThucTemp = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "ThoiGianKetThucTemp"),
                                    NguoiThamGia = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "NguoiThamGia"),
                                    NguoiThamGiaKhac = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "NguoiThamGiaKhac"),
                                    PhongHopTxt = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "PhongHopTxt"),
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return cuocHop;
        }

        public static void UpdateCuocHopBiTrungVaoCotKiemTra(int id, List<int> cuocHopKhacIds, string connStr)
        {
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"UPDATE CuocHop
                       SET CuocHopTrungId = @CuocHopTrungId
                       WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CuocHopTrungId", string.Join(",", cuocHopKhacIds));
                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<CuocHop> GetCuocHopTrungPhong(CuocHop cuocHopHienTai, string connStr)
        {
            var list = new List<CuocHop>();

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = $@"SELECT Id, PhongHopId, ThoiGianBatDau, ThoiGianKetThuc, ThoiGianKetThucTemp
                        FROM CuocHop
                        WHERE PhongHopId = {cuocHopHienTai.PhongHopId}
                          AND TrangThai = '{BienDungChung.STATUS_CUOCHOP_DADUYET}'
                          AND LoaiLichHop != @LoaiLichHop
                          AND Id <> {cuocHopHienTai.Id}
                          AND @BatDau < ThoiGianKetThucTemp
                          AND @KetThuc > ThoiGianBatDau";

                try
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@BatDau", cuocHopHienTai.ThoiGianBatDau);
                        cmd.Parameters.AddWithValue("@KetThuc", cuocHopHienTai.ThoiGianKetThucTemp);
                        cmd.Parameters.AddWithValue("@LoaiLichHop", EnumLoaiLichHop.LichHopCaNhan);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new CuocHop
                                {
                                    Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id"),
                                    PhongHopId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "PhongHopId"),
                                    ThoiGianBatDau = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "ThoiGianBatDau"),
                                    ThoiGianKetThuc = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "ThoiGianKetThuc"),
                                    ThoiGianKetThucTemp = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "ThoiGianKetThucTemp"),
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

            return list;
        }

        public static List<CuocHop> GetAllCuocHopCuaPhong(int phongId, string connStr)
        {
            var list = new List<CuocHop>();

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = $@"SELECT Id, PhongHopId, ThoiGianBatDau, ThoiGianKetThuc, ThoiGianKetThucTemp
                        FROM CuocHop
                        WHERE PhongHopId = {phongId}
                        AND TrangThai = '{BienDungChung.STATUS_CUOCHOP_DADUYET}'
                        AND LoaiLichHop != @LoaiLichHop
                        AND ThoiGianBatDau > @ThoiGianBatDau";
                try
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ThoiGianBatDau", DateTime.Now.AddDays(-1));
                        cmd.Parameters.AddWithValue("@LoaiLichHop", EnumLoaiLichHop.LichHopCaNhan);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new CuocHop
                                {
                                    Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id"),
                                    PhongHopId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "PhongHopId"),
                                    ThoiGianBatDau = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "ThoiGianBatDau"),
                                    ThoiGianKetThuc = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "ThoiGianKetThuc"),
                                    ThoiGianKetThucTemp = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "ThoiGianKetThucTemp"),
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

            return list;
        }
    }
}