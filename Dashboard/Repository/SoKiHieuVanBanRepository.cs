using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface ISoKiHieuVanBanRepository
    {
        DataTableResult<SoKiHieuVanBanGridVo> GetAll(SoKiHieuVanBanQueryViewModel queryParams);
        dynamic LaySo(int userId, int nam, int hinhThucVanBanId);
        bool Xoa(int id, int userId);
        SoKiHieuVanBanGridVo LaySoGanNhat();
        dynamic KhoiTaoSoKiHieuBanDau(int soKiHieu, int? userId);
    }

    public class SoKiHieuVanBanRepository : ISoKiHieuVanBanRepository
    {
        private readonly IContext _context;

        public SoKiHieuVanBanRepository(IContext context)
        {
            _context = context;
        }

        private readonly string sortStringDefault = "NgayLay desc";
        private readonly List<string> searchCol = new List<string> { "SoKiHieu" };

        public DataTableResult<SoKiHieuVanBanGridVo> GetAll(SoKiHieuVanBanQueryViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.SoKiHieuVanBans.AsNoTracking();

            if (queryParams.UserIdFilter.HasValue && queryParams.UserIdFilter.Value > 0)
            {
                query = query.Where(o => o.UserId == queryParams.UserIdFilter.Value);
            }

            var data = query.Select(cv => new SoKiHieuVanBanGridVo
            {
                Id = cv.Id,
                SoKiHieu = cv.SoKiHieu,
                UserId = cv.UserId,
                NgayLay = cv.NgayLay,
                Nam = cv.Nam,
                HinhThucVanBanId = cv.HinhThucVanBanId
            }).Where(queryParams.searchString);

            var total = data.Count();
            var result = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();

            if (result.Count > 0)
            {
                foreach (var item in result)
                {
                    if (item.UserId.HasValue && item.UserId.Value > 0)
                    {
                        var user = CacheManagerHelper.GetUserByIdFromCache(item.UserId.Value);
                        if (user != null)
                        {
                            item.TenNguoiLay = user.Name;
                        }
                    }

                    if (item.HinhThucVanBanId.HasValue && item.HinhThucVanBanId.Value > 0)
                    {
                        var hinhThuc = CacheManagerHelper.GetHinhThucVanBanCache()
                            .FirstOrDefault(h => h.Id == item.HinhThucVanBanId.Value);
                        if (hinhThuc != null)
                        {
                            item.TenHinhThucVanBan = hinhThuc.Ten;
                        }
                    }
                }
            }

            return new DataTableResult<SoKiHieuVanBanGridVo>
            {
                rows = result,
                total = total
            };
        }

        public dynamic LaySo(int userId, int nam, int hinhThucVanBanId)
        {
            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int soKiHieuMoi = 1;

                        // Sử dụng UPDLOCK + ROWLOCK để chỉ lock các row liên quan, không lock toàn bảng
                        string sqlGetMax = @"
                            SELECT MAX(SoKiHieu) 
                            FROM SoKiHieuVanBan WITH (UPDLOCK, ROWLOCK, HOLDLOCK)
                            WHERE Nam = @Nam AND HinhThucVanBanId = @HinhThucVanBanId";
                        
                        using (var cmdGetMax = new SqlCommand(sqlGetMax, conn, transaction))
                        {
                            cmdGetMax.Parameters.AddWithValue("@Nam", nam);
                            cmdGetMax.Parameters.AddWithValue("@HinhThucVanBanId", hinhThucVanBanId);
                            
                            var result = cmdGetMax.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                soKiHieuMoi = Convert.ToInt32(result) + 1;
                            }
                        }

                        string sqlInsert = @"
                            INSERT INTO SoKiHieuVanBan (SoKiHieu, UserId, NgayLay, Nam, HinhThucVanBanId) 
                            OUTPUT INSERTED.Id
                            VALUES (@SoKiHieu, @UserId, @NgayLay, @Nam, @HinhThucVanBanId)";

                        int newId = 0;
                        using (var cmdInsert = new SqlCommand(sqlInsert, conn, transaction))
                        {
                            cmdInsert.Parameters.AddWithValue("@SoKiHieu", soKiHieuMoi);
                            cmdInsert.Parameters.AddWithValue("@UserId", userId > 0 ? (object)userId : DBNull.Value);
                            cmdInsert.Parameters.AddWithValue("@NgayLay", DateTime.Now);
                            cmdInsert.Parameters.AddWithValue("@Nam", nam);
                            cmdInsert.Parameters.AddWithValue("@HinhThucVanBanId", hinhThucVanBanId);

                            newId = (int)cmdInsert.ExecuteScalar();
                        }

                        transaction.Commit();

                        return new { Id = newId, SoKiHieu = soKiHieuMoi };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Lỗi khi lấy số: " + ex.Message);
                    }
                }
            }
        }

        public bool Xoa(int id, int userId)
        {
            var item = _context.SoKiHieuVanBans.FirstOrDefault(o => o.Id == id && o.UserId == userId);
            if (item == null)
            {
                return false;
            }

            _context.SoKiHieuVanBans.Remove(item);
            _context.SaveChanges();

            return true;
        }

        public SoKiHieuVanBanGridVo LaySoGanNhat()
        {
            var item = _context.SoKiHieuVanBans.AsNoTracking()
                .OrderByDescending(o => o.NgayLay)
                .FirstOrDefault();

            if (item == null)
            {
                return null;
            }

            var result = new SoKiHieuVanBanGridVo
            {
                Id = item.Id,
                SoKiHieu = item.SoKiHieu,
                UserId = item.UserId,
                NgayLay = item.NgayLay,
                Nam = item.Nam,
                HinhThucVanBanId = item.HinhThucVanBanId
            };

            if (item.UserId.HasValue && item.UserId.Value > 0)
            {
                var user = CacheManagerHelper.GetUserByIdFromCache(item.UserId.Value);
                if (user != null)
                {
                    result.TenNguoiLay = user.Name;
                }
            }

            return result;
        }

        public dynamic KhoiTaoSoKiHieuBanDau(int soKiHieu, int? userId)
        {
            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlTruncate = @"TRUNCATE TABLE SoKiHieuVanBan";
                        using (var cmdTruncate = new SqlCommand(sqlTruncate, conn, transaction))
                        {
                            cmdTruncate.ExecuteNonQuery();
                        }

                        string sqlInsert = @"INSERT INTO SoKiHieuVanBan (SoKiHieu, UserId, NgayLay) 
                                           OUTPUT INSERTED.Id
                                           VALUES (@SoKiHieu, @UserId, @NgayLay)";

                        int newId = 0;
                        using (var cmdInsert = new SqlCommand(sqlInsert, conn, transaction))
                        {
                            cmdInsert.Parameters.AddWithValue("@SoKiHieu", soKiHieu);
                            cmdInsert.Parameters.AddWithValue("@UserId", DBNull.Value);
                            cmdInsert.Parameters.AddWithValue("@NgayLay", DateTime.Now);

                            newId = (int)cmdInsert.ExecuteScalar();
                        }

                        transaction.Commit();

                        return new { Id = newId, SoKiHieu = soKiHieu };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Lỗi khi khởi tạo số ký hiệu ban đầu: " + ex.Message);
                    }
                }
            }
        }
    }
}

