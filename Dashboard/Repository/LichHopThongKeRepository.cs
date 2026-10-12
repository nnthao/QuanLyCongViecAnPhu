using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Dashboard.Repository
{
    public interface ILichHopThongKeRepository
    {
        List<ThongKeLyDoCuocHopGridVo> GetThongKeLyDoCuocHop(QueryParamViewModel queryViewModel);
        List<ThongKeNguoiCoNhieuLyDoGridVo> GetTop10NguoiCoNhieuLyDo(QueryParamViewModel queryViewModel);
        DataTableResult<ChiTietLyDoCuocHopGridVo> GetChiTietLyDoCuocHop(QueryParamViewModel queryViewModel);
    }

    public class LichHopThongKeRepository : ILichHopThongKeRepository
    {
        private readonly IContext _context;

        public LichHopThongKeRepository(IContext context)
        {
            _context = context;
        }

        private static List<int> ParsePhongBanFilter(QueryParamViewModel queryViewModel)
        {
            var phongBanFilter = new List<int>();
            if (!string.IsNullOrWhiteSpace(queryViewModel.PhongBanIdsStr))
            {
                phongBanFilter = queryViewModel.PhongBanIdsStr
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(idStr =>
                    {
                        int parsed;
                        return int.TryParse(idStr, out parsed) ? parsed : (int?)null;
                    })
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();
            }
            return phongBanFilter;
        }

        private static string BuildPhongBanCondition(List<int> phongBanFilter)
        {
            if (phongBanFilter == null || phongBanFilter.Count == 0)
            {
                return "";
            }
            return $" AND ch.CoQuanId IN ({string.Join(", ", phongBanFilter)})";
        }

        private static string BuildPhongBanConditionForParticipant(List<int> phongBanFilter)
        {
            if (phongBanFilter == null || phongBanFilter.Count == 0)
            {
                return "";
            }
            return $" AND u.PhongBanId IN ({string.Join(", ", phongBanFilter)})";
        }

        private static List<int> ParseLyDoFilter(QueryParamViewModel queryViewModel)
        {
            var lyDoFilter = new List<int>();
            if (!string.IsNullOrWhiteSpace(queryViewModel.LyDoIdsStr))
            {
                lyDoFilter = queryViewModel.LyDoIdsStr
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(idStr =>
                    {
                        int parsed;
                        return int.TryParse(idStr, out parsed) ? parsed : (int?)null;
                    })
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();
            }
            return lyDoFilter;
        }

        private static string BuildLyDoCondition(List<int> lyDoFilter)
        {
            if (lyDoFilter == null || lyDoFilter.Count == 0)
            {
                return "";
            }
            return $" AND ch.LyDoId IN ({string.Join(", ", lyDoFilter)})";
        }

        public List<ThongKeLyDoCuocHopGridVo> GetThongKeLyDoCuocHop(QueryParamViewModel queryViewModel)
        {
            var result = new List<ThongKeLyDoCuocHopGridVo>();
            var phongBanFilter = ParsePhongBanFilter(queryViewModel);
            var phongBanCondition = BuildPhongBanConditionForParticipant(phongBanFilter);
            var lyDoFilter = ParseLyDoFilter(queryViewModel);
            var lyDoCondition = BuildLyDoCondition(lyDoFilter);

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();

                string sql = $@"WITH ParticipantUsers AS (
                        SELECT ntg.CuocHopId, ntg.UserId
                        FROM NguoiThamGia ntg
                        WHERE ntg.UserId IS NOT NULL
                        UNION
                        SELECT pbtg.CuocHopId, u.Id AS UserId
                        FROM PhongBanThamGiaCuocHop pbtg
                        INNER JOIN Users u ON pbtg.PhongBanId = u.PhongBanId
                            AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                    )
                    SELECT
                        ISNULL(ld.Id, 0) AS LyDoId,
                        ISNULL(ld.Ten, N'Không có lý do') AS TenLyDo,
                        ISNULL(ld.MaMau, '#808080') AS MaMau,
                        COUNT(*) AS SoLuong
                    FROM
                        CuocHop ch
                    INNER JOIN
                        ParticipantUsers ntg ON ch.Id = ntg.CuocHopId
                    LEFT JOIN
                        Users u ON ntg.UserId = u.Id AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                    LEFT JOIN
                        LyDo ld ON ch.LyDoId = ld.Id AND (ld.IsDeleted = 0 OR ld.IsDeleted IS NULL)
                    WHERE
                        (ch.IsDeleted = 0 OR ch.IsDeleted IS NULL)
                        AND ch.LyDoId IS NOT NULL
                        {phongBanCondition}
                        {lyDoCondition}
                        AND ch.ThoiGianBatDau >= '{DateTimeHelper.BeginningOfDayForSqlQuery(queryViewModel.TuNgay.GetValueOrDefault())}'
                        AND ch.ThoiGianBatDau <= '{DateTimeHelper.EndOfDayForSqlQuery(queryViewModel.DenNgay.GetValueOrDefault())}'
                    GROUP BY
                        ld.Id, ld.Ten, ld.MaMau
                    ORDER BY
                        SoLuong DESC, ld.Ten;";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new ThongKeLyDoCuocHopGridVo
                        {
                            LyDoId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "LyDoId"),
                            TenLyDo = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenLyDo"),
                            MaMau = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "MaMau"),
                            SoLuong = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoLuong")
                        });
                    }
                }
            }

            return result;
        }

        public List<ThongKeNguoiCoNhieuLyDoGridVo> GetTop10NguoiCoNhieuLyDo(QueryParamViewModel queryViewModel)
        {
            var result = new List<ThongKeNguoiCoNhieuLyDoGridVo>();
            var phongBanFilter = ParsePhongBanFilter(queryViewModel);
            var phongBanCondition = BuildPhongBanConditionForParticipant(phongBanFilter);
            var lyDoFilter = ParseLyDoFilter(queryViewModel);
            var lyDoCondition = BuildLyDoCondition(lyDoFilter);

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();

                string sql = $@"WITH ParticipantUsers AS (
                        SELECT ntg.CuocHopId, ntg.UserId
                        FROM NguoiThamGia ntg
                        WHERE ntg.UserId IS NOT NULL
                        UNION
                        SELECT pbtg.CuocHopId, u.Id AS UserId
                        FROM PhongBanThamGiaCuocHop pbtg
                        INNER JOIN Users u ON pbtg.PhongBanId = u.PhongBanId
                            AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                    )
                    SELECT TOP 10
                        ntg.UserId,
                        ISNULL(u.Name, N'Không xác định') AS TenNguoi,
                        COUNT(DISTINCT ch.Id) AS SoLuong
                    FROM
                        CuocHop ch
                    INNER JOIN
                        ParticipantUsers ntg ON ch.Id = ntg.CuocHopId
                    LEFT JOIN
                        Users u ON ntg.UserId = u.Id AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                    WHERE
                        (ch.IsDeleted = 0 OR ch.IsDeleted IS NULL)
                        AND ch.LyDoId IS NOT NULL
                        AND ntg.UserId IS NOT NULL
                        {phongBanCondition}
                        {lyDoCondition}
                        AND ch.ThoiGianBatDau >= '{DateTimeHelper.BeginningOfDayForSqlQuery(queryViewModel.TuNgay.GetValueOrDefault())}'
                        AND ch.ThoiGianBatDau <= '{DateTimeHelper.EndOfDayForSqlQuery(queryViewModel.DenNgay.GetValueOrDefault())}'
                    GROUP BY
                        ntg.UserId, u.Name
                    HAVING
                        COUNT(DISTINCT ch.Id) > 0
                    ORDER BY
                        SoLuong DESC, u.Name;";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new ThongKeNguoiCoNhieuLyDoGridVo
                        {
                            NguoiTaoId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "UserId"),
                            TenNguoi = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenNguoi"),
                            SoLuong = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "SoLuong")
                        });
                    }
                }
            }

            return result;
        }

        public DataTableResult<ChiTietLyDoCuocHopGridVo> GetChiTietLyDoCuocHop(QueryParamViewModel queryViewModel)
        {
            var result = new List<ChiTietLyDoCuocHopGridVo>();
            var total = 0;

            var phongBanFilter = ParsePhongBanFilter(queryViewModel);
            var phongBanCondition = BuildPhongBanConditionForParticipant(phongBanFilter);
            var lyDoFilter = ParseLyDoFilter(queryViewModel);
            var lyDoCondition = BuildLyDoCondition(lyDoFilter);

            var phanTrang = "";
            if (queryViewModel.limit != 0)
            {
                phanTrang = $@" OFFSET {queryViewModel.offset} ROWS 
                    FETCH NEXT {queryViewModel.limit} ROWS ONLY";
            }

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();

                string sqlCount = $@"WITH ParticipantUsers AS (
                        SELECT ntg.CuocHopId, ntg.UserId
                        FROM NguoiThamGia ntg
                        WHERE ntg.UserId IS NOT NULL
                        UNION
                        SELECT pbtg.CuocHopId, u.Id AS UserId
                        FROM PhongBanThamGiaCuocHop pbtg
                        INNER JOIN Users u ON pbtg.PhongBanId = u.PhongBanId
                            AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                    )
                    SELECT COUNT(*)
                    FROM
                        CuocHop ch
                    INNER JOIN
                        ParticipantUsers ntg ON ch.Id = ntg.CuocHopId
                    LEFT JOIN
                        Users u ON ntg.UserId = u.Id AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                    WHERE
                        (ch.IsDeleted = 0 OR ch.IsDeleted IS NULL)
                        AND ch.LyDoId IS NOT NULL
                        AND ch.ThoiGianBatDau >= '{DateTimeHelper.BeginningOfDayForSqlQuery(queryViewModel.TuNgay.GetValueOrDefault())}'
                        AND ch.ThoiGianBatDau <= '{DateTimeHelper.EndOfDayForSqlQuery(queryViewModel.DenNgay.GetValueOrDefault())}'
                        {phongBanCondition}
                        {lyDoCondition};";

                using (var cmdCount = new SqlCommand(sqlCount, conn))
                {
                    var scalar = cmdCount.ExecuteScalar();
                    total = scalar != null && scalar != DBNull.Value ? Convert.ToInt32(scalar) : 0;
                }

                string sql = $@"WITH ParticipantUsers AS (
                        SELECT ntg.CuocHopId, ntg.UserId
                        FROM NguoiThamGia ntg
                        WHERE ntg.UserId IS NOT NULL
                        UNION
                        SELECT pbtg.CuocHopId, u.Id AS UserId
                        FROM PhongBanThamGiaCuocHop pbtg
                        INNER JOIN Users u ON pbtg.PhongBanId = u.PhongBanId
                            AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                    )
                    SELECT
                        ch.Id AS CuocHopId,
                        ISNULL(pb.Ten, N'Không xác định') AS TenPhongBan,
                        ISNULL(u.Name, N'Không xác định') AS TenNguoi,
                        ch.ThoiGianBatDau,
                        ISNULL(ld.Ten, N'Không có lý do') AS TenLyDo
                    FROM
                        CuocHop ch
                    INNER JOIN
                        ParticipantUsers ntg ON ch.Id = ntg.CuocHopId
                    LEFT JOIN
                        Users u ON ntg.UserId = u.Id AND (u.IsDeleted = 0 OR u.IsDeleted IS NULL)
                    LEFT JOIN
                        PhongBan pb ON u.PhongBanId = pb.Id AND (pb.IsDeleted = 0 OR pb.IsDeleted IS NULL)
                    LEFT JOIN
                        LyDo ld ON ch.LyDoId = ld.Id AND (ld.IsDeleted = 0 OR ld.IsDeleted IS NULL)
                    WHERE
                        (ch.IsDeleted = 0 OR ch.IsDeleted IS NULL)
                        AND ch.LyDoId IS NOT NULL
                        AND ch.ThoiGianBatDau >= '{DateTimeHelper.BeginningOfDayForSqlQuery(queryViewModel.TuNgay.GetValueOrDefault())}'
                        AND ch.ThoiGianBatDau <= '{DateTimeHelper.EndOfDayForSqlQuery(queryViewModel.DenNgay.GetValueOrDefault())}'
                        {phongBanCondition}
                        {lyDoCondition}
                    ORDER BY
                        ch.ThoiGianBatDau ASC, ISNULL(pb.ThuTu, 999999) ASC, pb.Ten ASC, u.Name ASC
                    {phanTrang};";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var thoiGianBatDau = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "ThoiGianBatDau");
                        result.Add(new ChiTietLyDoCuocHopGridVo
                        {
                            CuocHopId = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "CuocHopId"),
                            TenPhongBan = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenPhongBan"),
                            TenNguoi = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenNguoi"),
                            ThoiGianBatDau = thoiGianBatDau,
                            ThoiGianBatDauStr = thoiGianBatDau.HasValue ? thoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm") : "",
                            TenLyDo = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TenLyDo")
                        });
                    }
                }
            }

            return new DataTableResult<ChiTietLyDoCuocHopGridVo>
            {
                rows = result,
                total = total
            };
        }
    }
}

