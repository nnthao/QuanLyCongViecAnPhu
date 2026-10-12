using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace Dashboard.Repository
{
    public interface IZaloRepository
    {
        ZaloToken GetToken_FromDB();
        bool ThemOrCapNhat(string AccessToken, string RefreshToken);
        ZaloLienKetResult LienKetSoDienThoai(ZaloLienKetRequest request);
        User LayUserTheoSoDienThoaiDaLienKetZalo(string sdt);
        User CheckSdt(string sdt, string user_zalo_id);
    }

    public class ZaloRepository : IZaloRepository
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ZaloRepository));
        private readonly IContext _context;

        public ZaloRepository(IContext context)
        {
            _context = context;
        }

        public ZaloToken GetToken_FromDB()
        {
            var item = _context.ZaloTokens.AsNoTracking().FirstOrDefault();
            if (item == null)
            {
                return null;
            }
            return item;
        }

        public bool ThemOrCapNhat(string AccessToken, string RefreshToken)
        {
            try
            {
                var entity = _context.ZaloTokens.FirstOrDefault();
                if (entity != null)
                {
                    entity.RefreshToken = RefreshToken;
                    entity.AccessToken = AccessToken;
                }
                else
                {
                    var entity2 = new ZaloToken
                    {
                        RefreshToken = RefreshToken,
                        AccessToken = AccessToken,
                    };
                    _context.ZaloTokens.Add(entity2);
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi ThemOrCapNhat ZaloToken: {ex.Message}", ex);
                return false;
            }
        }

        public ZaloLienKetResult LienKetSoDienThoai(ZaloLienKetRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.SoDienThoai))
            {
                return new ZaloLienKetResult
                {
                    Status = ZaloLienKetStatus.ChuaCoTrenHeThong,
                    Message = "Số điện thoại của bạn chưa được nhập trên hệ thống QLCV, vui lòng liên hệ quản trị để được hỗ trợ"
                };
            }

            var sdtTrim = request.SoDienThoai.Trim();
            var item = _context.Users.FirstOrDefault(n => n.Phone == sdtTrim && (n.IsDeleted == null || n.IsDeleted == false));
            if (item == null)
            {
                return new ZaloLienKetResult
                {
                    Status = ZaloLienKetStatus.ChuaCoTrenHeThong,
                    Message = "Số điện thoại của bạn chưa được nhập trên hệ thống QLCV, vui lòng liên hệ quản trị để được hỗ trợ"
                };
            }

            bool isChuaLienKet = string.IsNullOrWhiteSpace(item.UserZaloId) || item.UserZaloId.Trim().Equals("null", StringComparison.OrdinalIgnoreCase);
            if (!isChuaLienKet)
            {
                return new ZaloLienKetResult
                {
                    Status = ZaloLienKetStatus.DaLienKet,
                    Message = "Số điện thoại đã liên kết với phần mềm giao việc. Vui lòng liên hệ quản trị viên",
                    User = item
                };
            }

            var userZaloIdTrim = request.UserZaloId != null ? request.UserZaloId.Trim() : string.Empty;
            item.UserZaloId = userZaloIdTrim;
            _context.SaveChanges();
            CacheManagerHelper.InitUserCache();

            return new ZaloLienKetResult
            {
                Status = ZaloLienKetStatus.ThanhCong,
                Message = "Cảm ơn bạn đã liên kết số điện thoại thành công với phần mềm QLCV",
                User = item
            };
        }

        public User LayUserTheoSoDienThoaiDaLienKetZalo(string sdt)
        {
            if (string.IsNullOrWhiteSpace(sdt))
            {
                return null;
            }

            var sdtTrim = sdt.Trim();
            var item = _context.Users.FirstOrDefault(n => n.Phone == sdtTrim && (n.IsDeleted == null || n.IsDeleted == false));
            if (item != null)
            {
                bool isCoZaloId = !string.IsNullOrWhiteSpace(item.UserZaloId) && !item.UserZaloId.Trim().Equals("null", StringComparison.OrdinalIgnoreCase);
                if (isCoZaloId)
                {
                    return item;
                }
            }

            return null;
        }

        public User CheckSdt(string sdt, string user_zalo_id)
        {
            if (string.IsNullOrWhiteSpace(sdt))
            {
                return null;
            }

            if (string.IsNullOrEmpty(user_zalo_id))
            {
                return LayUserTheoSoDienThoaiDaLienKetZalo(sdt);
            }

            var result = LienKetSoDienThoai(new ZaloLienKetRequest
            {
                SoDienThoai = sdt,
                UserZaloId = user_zalo_id
            });

            return result.Status == ZaloLienKetStatus.ThanhCong ? result.User : null;
        }
    }
}
