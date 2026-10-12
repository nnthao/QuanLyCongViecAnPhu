using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Dashboard.Repository
{
    public interface IGiaoViecThongKeRepository
    {
        DataTableResult<GiaoViecThanhVienGridVo> GetListChiTietHoanThanhCongViecPhongBan(GiaoViecQueryViewModel queryViewModel);
        List<ThongKeCongViecTheoPhongBanGridVo> GetThongKeCongViecTheoPhongBan(GiaoViecQueryViewModel queryViewModel);
        List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoNhieuCongViec(GiaoViecQueryViewModel queryViewModel);
        List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoItCongViec(GiaoViecQueryViewModel queryViewModel);
        List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoNhieuCongViecSomHan(GiaoViecQueryViewModel queryViewModel);
        List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoNhieuCongViecTreHan(GiaoViecQueryViewModel queryViewModel);
    }

    public class GiaoViecThongKeRepository : IGiaoViecThongKeRepository
    {
        private readonly IContext _context;

        public GiaoViecThongKeRepository(IContext context)
        {
            _context = context;
        }

        public DataTableResult<GiaoViecThanhVienGridVo> GetListChiTietHoanThanhCongViecPhongBan(GiaoViecQueryViewModel queryViewModel)
        {
            var result = new List<GiaoViecThanhVienGridVo>();
            var total = 0;

            var isHoanThanh = "";
            var isHoanThanhList = new List<string>();
            //hoan thanh sơm hạn
            if (queryViewModel.IsHoanThanhTypeList.Contains(1))
            {
                isHoanThanhList.Add("  (gvtv.IsHoanThanh = 1 AND gvtv.NgayHoanThanh IS NOT NULL AND gvtv.HanXuLy IS NOT NULL AND CAST(gvtv.NgayHoanThanh AS DATE) < CAST(gvtv.HanXuLy AS DATE)) ");
            }
            //hoan thanh đúng hạn
            if (queryViewModel.IsHoanThanhTypeList.Contains(2))
            {
                isHoanThanhList.Add("  (gvtv.IsHoanThanh = 1 AND gvtv.NgayHoanThanh IS NOT NULL AND gvtv.HanXuLy IS NOT NULL AND CAST(gvtv.NgayHoanThanh AS DATE) = CAST(gvtv.HanXuLy AS DATE))  ");
            }
            //chưa hoan thanh con hạn
            if (queryViewModel.IsHoanThanhTypeList.Contains(3))
            {
                isHoanThanhList.Add($" ((gvtv.IsHoanThanh IS NULL OR gvtv.IsHoanThanh = 0) AND gvtv.HanXuLy IS NOT NULL AND gvtv.HanXuLy >= CAST('{DateTimeHelper.BeginningOfDayForSqlQuery(DateTime.Now)}' AS DATETIME))  ");
            }
            //chưa hoàn thành trễ hạn: chưa hoàn thành và quá hạn
            if (queryViewModel.IsHoanThanhTypeList.Contains(4))
            {
                isHoanThanhList.Add($" ((gvtv.IsHoanThanh IS NULL OR gvtv.IsHoanThanh = 0) AND gvtv.HanXuLy IS NOT NULL AND gvtv.HanXuLy < CAST('{DateTimeHelper.BeginningOfDayForSqlQuery(DateTime.Now)}' AS DATETIME)) ");
            }
            //hoàn thành trễ hạn: đã hoàn thành nhưng quá hạn
            if (queryViewModel.IsHoanThanhTypeList.Contains(5))
            {
                isHoanThanhList.Add("  (gvtv.IsHoanThanh = 1 AND gvtv.NgayHoanThanh IS NOT NULL AND gvtv.HanXuLy IS NOT NULL AND CAST(gvtv.NgayHoanThanh AS DATE) > CAST(gvtv.HanXuLy AS DATE)) ");
            }

            if (isHoanThanhList.Count != 0)
            {
                isHoanThanh = " AND (" + string.Join(" OR ", isHoanThanhList) + ") ";
            }

            var phongBanQuery = "";
            if (queryViewModel.PhongBanIds.Count != 0)
            {
                phongBanQuery = $" AND pb.Id IN ({string.Join(", ", queryViewModel.PhongBanIds)}) ";
            }

            var phanTrang = "";
            if (queryViewModel.limit != 0)
            {
                phanTrang = $@" OFFSET {queryViewModel.offset} ROWS 
                    FETCH NEXT {queryViewModel.limit} ROWS ONLY";
            }

            var userIds = "";
            if (queryViewModel.ThanhVienIdList.Count != 0)
            {
                userIds = $" AND u.Id IN ({string.Join(", ", queryViewModel.ThanhVienIdList)}) ";
            }

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();


                string sqlSelect = $@"SELECT
                                u.Id AS ThanhVienId,
                                u.PhongBanId AS PhongBanId,
                                u.Name AS ThanhVien,
                                gv.Id As GiaoViecId,
                                gv.NgayTao As NgayGiaoViec,
                                gv.TenCongViec,
                                gv.NoiDung,
                                gv.GhiChu,
                                gvtv.NoiDungNhiemVu,
                                gvtv.NgayHoanThanh,
                                gvtv.HanXuLy,
                                gvtv.IsHoanThanh,
                                gvtv.Id AS GiaoViecThanhVienId,
                                (SELECT TOP 1 NoiDungXuLy FROM dbo.GiaoViecThanhVienTienTrinh WHERE GiaoViecThanhVienId = gvtv.Id AND (IsDeleted = 0 OR IsDeleted IS NULL) ORDER BY CASE WHEN PhanTramHoanThanh = 100 THEN 0 ELSE 1 END, Id DESC) AS NoiDungBaoCao
                            FROM
                                dbo.PhongBan pb
                            INNER JOIN
                                dbo.Users u ON pb.Id = u.PhongBanId AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                            INNER JOIN
                                dbo.GiaoViecThanhVien gvtv ON u.Id = gvtv.ThanhVienId
                            INNER JOIN
                                dbo.VaiTroXuLy vt ON gvtv.VaiTroXuLyId = vt.Id AND (vt.IsDeleted = 0 OR vt.IsDeleted IS NULL) AND vt.IsThongKeGiaoViec = 1
                            INNER JOIN
                                dbo.GiaoViec gv ON gvtv.GiaoViecId = gv.Id
                            WHERE
                                 (pb.IsDeleted = 0 OR pb.IsDeleted IS NULL)
                                 AND (pb.IsPhongNhanViec IS NULL OR pb.IsPhongNhanViec = 1)
                                {phongBanQuery}
                                AND gvtv.IsDeleted = 0
                                {isHoanThanh}
                                {userIds}
                                AND (gv.IsDeleted = 0 OR gv.IsDeleted IS NULL)
                                AND (gv.IsTuGiao IS NULL OR gv.IsTuGiao = 0)
                                AND gv.NgayTao >= '{DateTimeHelper.BeginningOfDayForSqlQuery(queryViewModel.TuNgay.GetValueOrDefault())}'
                                AND gv.NgayTao <= '{DateTimeHelper.EndOfDayForSqlQuery(queryViewModel.DenNgay.GetValueOrDefault())}'
                            ORDER BY
                                u.Name,
                                gvtv.HanXuLy
                           {phanTrang};";

                string sqlSelectCount = $@"SELECT
                                count(*)
                            FROM
                                dbo.PhongBan pb
                            INNER JOIN
                                dbo.Users u ON pb.Id = u.PhongBanId AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                            INNER JOIN
                                dbo.GiaoViecThanhVien gvtv ON u.Id = gvtv.ThanhVienId
                            INNER JOIN
                                dbo.VaiTroXuLy vt ON gvtv.VaiTroXuLyId = vt.Id AND (vt.IsDeleted = 0 OR vt.IsDeleted IS NULL) AND vt.IsThongKeGiaoViec = 1
                            INNER JOIN
                                dbo.GiaoViec gv ON gvtv.GiaoViecId = gv.Id
                            WHERE
                                (pb.IsDeleted = 0 OR pb.IsDeleted IS NULL)
                                AND (pb.IsPhongNhanViec IS NULL OR pb.IsPhongNhanViec = 1)
                                 {phongBanQuery}
                                AND gvtv.IsDeleted = 0
                                {isHoanThanh}
                                AND (gv.IsDeleted = 0 OR gv.IsDeleted IS NULL)
                                AND (gv.IsTuGiao IS NULL OR gv.IsTuGiao = 0)
                                AND gv.NgayTao >= '{DateTimeHelper.BeginningOfDayForSqlQuery(queryViewModel.TuNgay.GetValueOrDefault())}'
                                AND gv.NgayTao <= '{DateTimeHelper.EndOfDayForSqlQuery(queryViewModel.DenNgay.GetValueOrDefault())}'";

                using (var cmdCount = new SqlCommand(sqlSelectCount, conn))
                {
                    var scalar = cmdCount.ExecuteScalar();
                    total = scalar != null && scalar != DBNull.Value ? Convert.ToInt32(scalar) : 0;
                }

                using (var cmd = new SqlCommand(sqlSelect, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new GiaoViecThanhVienGridVo
                            {
                                ThanhVienId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "ThanhVienId"),
                                ThanhVien = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "ThanhVien"),
                                TenCongViec = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenCongViec"),
                                NoiDung = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "NoiDung"),
                                GhiChu = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "GhiChu"),
                                NoiDungBaoCao = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "NoiDungBaoCao"),
                                NoiDungNhiemVu = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "NoiDungNhiemVu"),
                                HanXuLy = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "HanXuLy"),
                                NgayHoanThanh = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "NgayHoanThanh"),
                                NgayGiaoViec = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "NgayGiaoViec"),
                                IsHoanThanh = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsHoanThanh"),
                                GiaoViecId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "GiaoViecId"),
                                PhongBanId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "PhongBanId"),
                                Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "GiaoViecThanhVienId"),
                            });
                        }
                    }
                }
            }

            return new DataTableResult<GiaoViecThanhVienGridVo>
            {
                rows = result,
                total = total
            };
        }

        public List<ThongKeCongViecTheoPhongBanGridVo> GetThongKeCongViecTheoPhongBan(GiaoViecQueryViewModel queryViewModel)
        {
            var result = new List<ThongKeCongViecTheoPhongBanGridVo>();

            var phongBanQuery = "";
            if (queryViewModel.PhongBanIds != null && queryViewModel.PhongBanIds.Count > 0)
            {
                phongBanQuery = $" AND pb.Id IN ({string.Join(", ", queryViewModel.PhongBanIds)}) ";
            }

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();

                string sql = $@"SELECT
                        pb.Id AS PhongBanId,
                        pb.Ten AS TenPhongBan,
                        -- Hoàn thành sớm hạn: IsHoanThanh = 1 VÀ NgayHoanThanh < BeginningOfDay(HanXuLy)
                        COALESCE(SUM(CASE 
                            WHEN gvtv.IsHoanThanh = 1 
                                AND gvtv.NgayHoanThanh IS NOT NULL 
                                AND gvtv.HanXuLy IS NOT NULL
                                AND CAST(gvtv.NgayHoanThanh AS DATE) < CAST(gvtv.HanXuLy AS DATE) THEN 1 
                            ELSE 0 
                        END), 0) AS SoCongViecHoanThanhSomHonHan,
    
                        -- Hoàn thành đúng hạn: IsHoanThanh = 1 VÀ NgayHoanThanh.Date = HanXuLy.Date
                        COALESCE(SUM(CASE 
                            WHEN gvtv.IsHoanThanh = 1 
                                AND gvtv.NgayHoanThanh IS NOT NULL 
                                AND gvtv.HanXuLy IS NOT NULL
                                AND CAST(gvtv.NgayHoanThanh AS DATE) = CAST(gvtv.HanXuLy AS DATE) THEN 1 
                            ELSE 0 
                        END), 0) AS SoCongViecHoanThanhDungHan,
    
                        -- Chưa hoàn thành còn hạn: IsHoanThanh != true (NULL hoặc 0) VÀ HanXuLy >= BeginningOfDay(DateTime.Now) (hôm nay trở đi)
                        COALESCE(SUM(CASE 
                            WHEN (gvtv.IsHoanThanh IS NULL OR gvtv.IsHoanThanh = 0) 
                                AND gvtv.HanXuLy IS NOT NULL
                                AND gvtv.HanXuLy >= CAST('{DateTimeHelper.BeginningOfDayForSqlQuery(DateTime.Now)}' AS DATETIME) THEN 1 
                            ELSE 0 
                        END), 0) AS SoCongViecChuaHoanThanhConHan,
    
                        -- Hoàn thành trễ hạn: IsHoanThanh = 1 VÀ NgayHoanThanh > HanXuLy
                        COALESCE(SUM(CASE 
                            WHEN gvtv.IsHoanThanh = 1 
                                AND gvtv.NgayHoanThanh IS NOT NULL 
                                AND gvtv.HanXuLy IS NOT NULL
                                AND CAST(gvtv.NgayHoanThanh AS DATE) > CAST(gvtv.HanXuLy AS DATE) THEN 1 
                            ELSE 0 
                        END), 0) AS SoCongViecHoanThanhTreHan,
    
                        -- Chưa hoàn thành trễ hạn: IsHoanThanh != true (NULL hoặc 0) VÀ HanXuLy < BeginningOfDay(DateTime.Now)
                        COALESCE(SUM(CASE 
                            WHEN (gvtv.IsHoanThanh IS NULL OR gvtv.IsHoanThanh = 0) 
                                AND gvtv.HanXuLy IS NOT NULL
                                AND gvtv.HanXuLy < CAST('{DateTimeHelper.BeginningOfDayForSqlQuery(DateTime.Now)}' AS DATETIME) THEN 1 
                            ELSE 0 
                        END), 0) AS SoCongViecChuaHoanThanhTreHan
                    FROM
                        PhongBan pb 
                    LEFT JOIN
                       Users u ON pb.Id = u.PhongBanId AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                    LEFT JOIN
                       GiaoViecThanhVien gvtv ON u.Id = gvtv.ThanhVienId 
                      AND gvtv.IsDeleted = 0 
                        AND gvtv.GiaoViecId IN (SELECT Id 
                                                FROM GiaoViec 
                                                WHERE (IsDeleted = 0 OR IsDeleted IS NULL)
                                                AND (IsTuGiao IS NULL OR IsTuGiao = 0)
                                                AND NgayTao >= '{DateTimeHelper.BeginningOfDayForSqlQuery(queryViewModel.TuNgay.GetValueOrDefault())}' 
                                               AND NgayTao <= '{DateTimeHelper.EndOfDayForSqlQuery(queryViewModel.DenNgay.GetValueOrDefault())}')
                        AND gvtv.VaiTroXuLyId IN (SELECT Id 
                                                  FROM VaiTroXuLy 
                                                  WHERE (IsDeleted = 0 OR IsDeleted IS NULL) 
                                                  AND IsThongKeGiaoViec = 1)
                    WHERE
                       pb.IsDeleted = 0 AND (pb.IsPhongNhanViec IS NULL OR pb.IsPhongNhanViec = 1)
                       {phongBanQuery}
                    GROUP BY
                       pb.Id, pb.Ten, pb.ThuTu
                    ORDER BY
                       ISNULL(pb.ThuTu, 999999) ASC, pb.Ten ASC;
                    ";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new ThongKeCongViecTheoPhongBanGridVo
                        {
                            PhongBanId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "PhongBanId"),
                            TenPhongBan = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenPhongBan"),
                            SoCongViecHoanThanhSomHonHan = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoCongViecHoanThanhSomHonHan"),
                            SoCongViecHoanThanhDungHan = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoCongViecHoanThanhDungHan"),
                            SoCongViecHoanThanhTreHan = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoCongViecHoanThanhTreHan"),
                            SoCongViecChuaHoanThanhConHan = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoCongViecChuaHoanThanhConHan"),
                            SoCongViecChuaHoanThanhTreHan = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoCongViecChuaHoanThanhTreHan"),
                        });
                    }
                }
            }

            return result;
        }

        public List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoNhieuCongViec(GiaoViecQueryViewModel queryViewModel)
        {
            var result = new List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo>();

            var phongBanQuery = "";
            if (queryViewModel.PhongBanIds != null && queryViewModel.PhongBanIds.Count > 0)
            {
                phongBanQuery = $" AND u.PhongBanId IN ({string.Join(", ", queryViewModel.PhongBanIds)}) ";
            }

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();

                string sql = $@"SELECT TOP 10
                        gvtv.ThanhVienId,
                        ISNULL(u.Name, N'Không xác định') AS TenThanhVien,
                        u.PhongBanId,
                        ISNULL(pb.Ten, N'') AS TenPhongBan,
                        COUNT(gvtv.Id) AS SoLuong
                    FROM
                        GiaoViecThanhVien gvtv
                    INNER JOIN
                        VaiTroXuLy vt ON gvtv.VaiTroXuLyId = vt.Id AND (vt.IsDeleted = 0 OR vt.IsDeleted IS NULL) AND vt.IsThongKeGiaoViec = 1
                    INNER JOIN
                        GiaoViec gv ON gvtv.GiaoViecId = gv.Id AND (gv.IsDeleted = 0 OR gv.IsDeleted IS NULL) AND (gv.IsTuGiao IS NULL OR gv.IsTuGiao = 0)
                    LEFT JOIN
                        Users u ON gvtv.ThanhVienId = u.Id AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                    LEFT JOIN
                        PhongBan pb ON u.PhongBanId = pb.Id AND (pb.IsDeleted = 0 OR pb.IsDeleted IS NULL) AND (pb.IsPhongNhanViec IS NULL OR pb.IsPhongNhanViec = 1)
                    WHERE
                        (gvtv.IsDeleted = 0 OR gvtv.IsDeleted IS NULL)
                        AND (u.IsLanhDaoPhong IS NULL OR u.IsLanhDaoPhong = 0)
                        {phongBanQuery}
                        AND gv.NgayTao >= '{DateTimeHelper.BeginningOfDayForSqlQuery(queryViewModel.TuNgay.GetValueOrDefault())}'
                        AND gv.NgayTao <= '{DateTimeHelper.EndOfDayForSqlQuery(queryViewModel.DenNgay.GetValueOrDefault())}'
                    GROUP BY
                        gvtv.ThanhVienId, u.Name, u.PhongBanId, pb.Ten
                    HAVING
                        COUNT(gvtv.Id) > 0
                    ORDER BY
                        SoLuong DESC, u.Name;";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var soLuong = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoLuong");
                        // Chỉ thêm vào kết quả nếu số lượng > 0
                        if (soLuong > 0)
                        {
                            var phongBanId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "PhongBanId");
                            result.Add(new ThongKeTop10ThanhVienCoNhieuCongViecGridVo
                            {
                                ThanhVienId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "ThanhVienId"),
                                TenThanhVien = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenThanhVien"),
                                PhongBanId = phongBanId > 0 ? (int?)phongBanId : null,
                                TenPhongBan = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenPhongBan"),
                                SoLuong = soLuong
                            });
                        }
                    }
                }
            }

            return result;
        }

        public List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoItCongViec(GiaoViecQueryViewModel queryViewModel)
        {
            var result = new List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo>();

            var phongBanQuery = "";
            if (queryViewModel.PhongBanIds != null && queryViewModel.PhongBanIds.Count > 0)
            {
                phongBanQuery = $" AND u.PhongBanId IN ({string.Join(", ", queryViewModel.PhongBanIds)}) ";
            }

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();

                string sql = $@"SELECT TOP 10
                        u.Id AS ThanhVienId,
                        ISNULL(u.Name, N'Không xác định') AS TenThanhVien,
                        u.PhongBanId,
                        ISNULL(pb.Ten, N'') AS TenPhongBan,
                        ISNULL(COUNT(gvtv.Id), 0) AS SoLuong
                    FROM
                        Users u
                    LEFT JOIN
                        PhongBan pb ON u.PhongBanId = pb.Id AND (pb.IsDeleted = 0 OR pb.IsDeleted IS NULL) AND (pb.IsPhongNhanViec IS NULL OR pb.IsPhongNhanViec = 1)
                    LEFT JOIN
                        GiaoViecThanhVien gvtv ON u.Id = gvtv.ThanhVienId 
                        AND (gvtv.IsDeleted = 0 OR gvtv.IsDeleted IS NULL)
                        AND gvtv.GiaoViecId IN (
                            SELECT Id FROM GiaoViec 
                            WHERE (IsDeleted = 0 OR IsDeleted IS NULL)
                            AND (IsTuGiao IS NULL OR IsTuGiao = 0)
                            AND NgayTao >= '{DateTimeHelper.BeginningOfDayForSqlQuery(queryViewModel.TuNgay.GetValueOrDefault())}'
                            AND NgayTao <= '{DateTimeHelper.EndOfDayForSqlQuery(queryViewModel.DenNgay.GetValueOrDefault())}'
                        )
                        AND gvtv.VaiTroXuLyId IN (SELECT Id FROM VaiTroXuLy WHERE (IsDeleted = 0 OR IsDeleted IS NULL) AND IsThongKeGiaoViec = 1)
                    WHERE
                        (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                        AND (u.IsLanhDaoPhong IS NULL OR u.IsLanhDaoPhong = 0)
                        AND (pb.Id IS NOT NULL)
                        {phongBanQuery}
                    GROUP BY
                        u.Id, u.Name, u.PhongBanId, pb.Ten
                    ORDER BY
                        SoLuong ASC, u.Name;";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var phongBanId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "PhongBanId");
                        result.Add(new ThongKeTop10ThanhVienCoNhieuCongViecGridVo
                        {
                            ThanhVienId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "ThanhVienId"),
                            TenThanhVien = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenThanhVien"),
                            PhongBanId = phongBanId > 0 ? (int?)phongBanId : null,
                            TenPhongBan = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenPhongBan"),
                            SoLuong = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoLuong")
                        });
                    }
                }
            }

            return result;
        }

        public List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoNhieuCongViecSomHan(GiaoViecQueryViewModel queryViewModel)
        {
            var result = new List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo>();

            var phongBanQuery = "";
            if (queryViewModel.PhongBanIds != null && queryViewModel.PhongBanIds.Count > 0)
            {
                phongBanQuery = $" AND u.PhongBanId IN ({string.Join(", ", queryViewModel.PhongBanIds)}) ";
            }

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();

                string sql = $@"SELECT TOP 10
                        gvtv.ThanhVienId,
                        ISNULL(u.Name, N'Không xác định') AS TenThanhVien,
                        u.PhongBanId,
                        ISNULL(pb.Ten, N'') AS TenPhongBan,
                        COUNT(gvtv.Id) AS SoLuong
                    FROM
                        GiaoViecThanhVien gvtv
                    INNER JOIN
                        VaiTroXuLy vt ON gvtv.VaiTroXuLyId = vt.Id AND (vt.IsDeleted = 0 OR vt.IsDeleted IS NULL) AND vt.IsThongKeGiaoViec = 1
                    INNER JOIN
                        GiaoViec gv ON gvtv.GiaoViecId = gv.Id AND (gv.IsDeleted = 0 OR gv.IsDeleted IS NULL) AND (gv.IsTuGiao IS NULL OR gv.IsTuGiao = 0)
                    LEFT JOIN
                        Users u ON gvtv.ThanhVienId = u.Id AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                    LEFT JOIN
                        PhongBan pb ON u.PhongBanId = pb.Id AND (pb.IsDeleted = 0 OR pb.IsDeleted IS NULL) AND (pb.IsPhongNhanViec IS NULL OR pb.IsPhongNhanViec = 1)
                    WHERE
                        (gvtv.IsDeleted = 0 OR gvtv.IsDeleted IS NULL)
                        AND (u.IsLanhDaoPhong IS NULL OR u.IsLanhDaoPhong = 0)
                        AND gvtv.IsHoanThanh = 1
                        AND gvtv.NgayHoanThanh IS NOT NULL
                        AND gvtv.HanXuLy IS NOT NULL
                        AND CAST(gvtv.NgayHoanThanh AS DATE) < CAST(gvtv.HanXuLy AS DATE)
                        {phongBanQuery}
                        AND gv.NgayTao >= '{DateTimeHelper.BeginningOfDayForSqlQuery(queryViewModel.TuNgay.GetValueOrDefault())}'
                        AND gv.NgayTao <= '{DateTimeHelper.EndOfDayForSqlQuery(queryViewModel.DenNgay.GetValueOrDefault())}'
                    GROUP BY
                        gvtv.ThanhVienId, u.Name, u.PhongBanId, pb.Ten
                    HAVING
                        COUNT(gvtv.Id) > 0
                    ORDER BY
                        SoLuong DESC, u.Name;";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var soLuong = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoLuong");
                        if (soLuong > 0)
                        {
                            var phongBanId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "PhongBanId");
                            result.Add(new ThongKeTop10ThanhVienCoNhieuCongViecGridVo
                            {
                                ThanhVienId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "ThanhVienId"),
                                TenThanhVien = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenThanhVien"),
                                PhongBanId = phongBanId > 0 ? (int?)phongBanId : null,
                                TenPhongBan = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenPhongBan"),
                                SoLuong = soLuong
                            });
                        }
                    }
                }
            }

            return result;
        }

        public List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo> GetTop10ThanhVienCoNhieuCongViecTreHan(GiaoViecQueryViewModel queryViewModel)
        {
            var result = new List<ThongKeTop10ThanhVienCoNhieuCongViecGridVo>();

            var phongBanQuery = "";
            if (queryViewModel.PhongBanIds != null && queryViewModel.PhongBanIds.Count > 0)
            {
                phongBanQuery = $" AND u.PhongBanId IN ({string.Join(", ", queryViewModel.PhongBanIds)}) ";
            }

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();

                string sql = $@"SELECT TOP 10
                        gvtv.ThanhVienId,
                        ISNULL(u.Name, N'Không xác định') AS TenThanhVien,
                        u.PhongBanId,
                        ISNULL(pb.Ten, N'') AS TenPhongBan,
                        COUNT(gvtv.Id) AS SoLuong
                    FROM
                        GiaoViecThanhVien gvtv
                    INNER JOIN
                        VaiTroXuLy vt ON gvtv.VaiTroXuLyId = vt.Id AND (vt.IsDeleted = 0 OR vt.IsDeleted IS NULL) AND vt.IsThongKeGiaoViec = 1
                    INNER JOIN
                        GiaoViec gv ON gvtv.GiaoViecId = gv.Id AND (gv.IsDeleted = 0 OR gv.IsDeleted IS NULL) AND (gv.IsTuGiao IS NULL OR gv.IsTuGiao = 0)
                    LEFT JOIN
                        Users u ON gvtv.ThanhVienId = u.Id AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                    LEFT JOIN
                        PhongBan pb ON u.PhongBanId = pb.Id AND (pb.IsDeleted = 0 OR pb.IsDeleted IS NULL) AND (pb.IsPhongNhanViec IS NULL OR pb.IsPhongNhanViec = 1)
                    WHERE
                        (gvtv.IsDeleted = 0 OR gvtv.IsDeleted IS NULL)
                        AND (u.IsLanhDaoPhong IS NULL OR u.IsLanhDaoPhong = 0)
                        AND gvtv.HanXuLy IS NOT NULL
                        AND ((gvtv.IsHoanThanh IS NULL OR gvtv.IsHoanThanh = 0)
                            AND gvtv.HanXuLy < CAST('{DateTimeHelper.BeginningOfDayForSqlQuery(DateTime.Now)}' AS DATETIME)
                        OR (gvtv.IsHoanThanh = 1 
                            AND gvtv.NgayHoanThanh IS NOT NULL 
                            AND CAST(gvtv.NgayHoanThanh AS DATE) > CAST(gvtv.HanXuLy AS DATE)))
                        {phongBanQuery}
                        AND gv.NgayTao >= '{DateTimeHelper.BeginningOfDayForSqlQuery(queryViewModel.TuNgay.GetValueOrDefault())}'
                        AND gv.NgayTao <= '{DateTimeHelper.EndOfDayForSqlQuery(queryViewModel.DenNgay.GetValueOrDefault())}'
                    GROUP BY
                        gvtv.ThanhVienId, u.Name, u.PhongBanId, pb.Ten
                    HAVING
                        COUNT(gvtv.Id) > 0
                    ORDER BY
                        SoLuong DESC, u.Name;";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var soLuong = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoLuong");
                        if (soLuong > 0)
                        {
                            var phongBanId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "PhongBanId");
                            result.Add(new ThongKeTop10ThanhVienCoNhieuCongViecGridVo
                            {
                                ThanhVienId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "ThanhVienId"),
                                TenThanhVien = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenThanhVien"),
                                PhongBanId = phongBanId > 0 ? (int?)phongBanId : null,
                                TenPhongBan = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenPhongBan"),
                                SoLuong = soLuong
                            });
                        }
                    }
                }
            }

            return result;
        }
    }
}
