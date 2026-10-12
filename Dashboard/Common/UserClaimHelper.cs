using Dashboard.AdoNetHelper;
using Dashboard.Models.ViewModel;
using Newtonsoft.Json;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Claims;

namespace Dashboard.Common
{
    public class UserClaimHelper
    {
        public static int GetCurrentUserId(ClaimsIdentity identity)
        {
            try
            {
                var infoClaim = identity.FindFirst(BienDungChung.JwtClaimInfoKey);
                if (infoClaim != null)
                {
                    var infoEncode = infoClaim.Value;
                    if (!string.IsNullOrEmpty(infoEncode))
                    {
                        var infoDecode = JsonConvert.DeserializeObject<LoginAuthenViewModel>(EncryptionHelper.Decrypt(infoEncode));
                        if (infoDecode != null)
                        {
                            if (infoDecode.Id != 0)
                            {
                                return infoDecode.Id;
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
            }

            return 0;
        }

        public static int GetCurrentVaiTroId(ClaimsIdentity identity)
        {
            try
            {
                var infoClaim = identity.FindFirst(BienDungChung.JwtClaimInfoKey);
                if (infoClaim != null)
                {
                    var infoEncode = infoClaim.Value;
                    if (!string.IsNullOrEmpty(infoEncode))
                    {
                        var infoDecode = JsonConvert.DeserializeObject<LoginAuthenViewModel>(EncryptionHelper.Decrypt(infoEncode));
                        if (infoDecode != null)
                        {
                            if (infoDecode.VaiTroId != 0)
                            {
                                return infoDecode.VaiTroId.GetValueOrDefault();
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
            }

            return 0;
        }

        public static string GetCurrentQuyenOfUser(ClaimsIdentity identity)
        {
            try
            {
                var infoClaim = identity.FindFirst(BienDungChung.JwtClaimInfoKey);
                if (infoClaim != null)
                {
                    var infoEncode = infoClaim.Value;
                    if (!string.IsNullOrEmpty(infoEncode))
                    {
                        var infoDecode = JsonConvert.DeserializeObject<LoginAuthenViewModel>(EncryptionHelper.Decrypt(infoEncode));
                        if (infoDecode != null)
                        {
                            if (!string.IsNullOrEmpty(infoDecode.Quyen))
                            {
                                return infoDecode.Quyen;
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
            }

            return "";
        }

        public static bool IsQuyenAllow(ClaimsIdentity identity, string quyen)
        {
            var quyens = GetCurrentQuyenOfUser(identity);
            if (!string.IsNullOrEmpty(quyens))
            {
                var quyenArr = quyens.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => o.Trim()).ToList();
                if (quyenArr.Count != 0 && quyenArr.Contains(quyen))
                {
                    return true;
                }
            }

            var currentUserId = GetCurrentUserId(identity);
            if (currentUserId > 0)
            {
                var user = CacheManagerHelper.GetUserByIdFromCache(currentUserId);
                if (user != null)
                {
                    var vaiTroCoQuyenIds = VaiTroAdoNetHelper.GetVaiTriIdCoQuyen(quyen);
                    if (user.VaiTroId.HasValue && vaiTroCoQuyenIds.Contains(user.VaiTroId.Value))
                    {
                        return true;
                    }
                    if (!string.IsNullOrEmpty(user.NhomVaiTroId))
                    {
                        var nhomIds = user.NhomVaiTroId.Split(',')
                            .Where(o => !string.IsNullOrEmpty(o))
                            .Select(o => { int.TryParse(o, out int id); return id; })
                            .Where(id => id > 0).ToList();
                        if (nhomIds.Any(id => vaiTroCoQuyenIds.Contains(id)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool GetIsSystemUser(ClaimsIdentity identity)
        {
            try
            {
                var infoClaim = identity.FindFirst(BienDungChung.JwtClaimInfoKey);
                if (infoClaim != null)
                {
                    var infoEncode = infoClaim.Value;
                    if (!string.IsNullOrEmpty(infoEncode))
                    {
                        var infoDecode = JsonConvert.DeserializeObject<LoginAuthenViewModel>(EncryptionHelper.Decrypt(infoEncode));
                        if (infoDecode != null)
                        {
                            if (infoDecode.IsSystemUser != null && infoDecode.IsSystemUser == true)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
            }

            return false;
        }

        public static int GetPhongBanId(ClaimsIdentity identity)
        {
            try
            {
                var infoClaim = identity.FindFirst(BienDungChung.JwtClaimInfoKey);
                if (infoClaim != null)
                {
                    var infoEncode = infoClaim.Value;
                    if (!string.IsNullOrEmpty(infoEncode))
                    {
                        var infoDecode = JsonConvert.DeserializeObject<LoginAuthenViewModel>(EncryptionHelper.Decrypt(infoEncode));
                        if (infoDecode != null)
                        {
                            if (infoDecode.PhongBanId != null)
                            {
                                return infoDecode.PhongBanId.GetValueOrDefault();
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
            }

            return 0;
        }

        public static bool CheckIsTaiXe(ClaimsIdentity identity)
        {
            try
            {
                var infoClaim = identity.FindFirst(BienDungChung.JwtClaimInfoKey);
                if (infoClaim != null)
                {
                    var infoEncode = infoClaim.Value;
                    if (!string.IsNullOrEmpty(infoEncode))
                    {
                        var infoDecode = JsonConvert.DeserializeObject<LoginAuthenViewModel>(EncryptionHelper.Decrypt(infoEncode));
                        if (infoDecode != null)
                        {
                            if (infoDecode.IsTaiXe != null && infoDecode.IsTaiXe == true)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
            }

            return false;
        }

        public static string GetTelegramIdOfCurrentUser(ClaimsIdentity identity)
        {
            try
            {
                var currentUserId = UserClaimHelper.GetCurrentUserId(identity);
                if (currentUserId != 0)
                {
                    var user = CacheManagerHelper.GetUserByIdFromCache(currentUserId);
                    if (user != null && user.Id != 0 && !string.IsNullOrEmpty(user.TelegramId))
                    {
                        return user.TelegramId;
                    }
                }
            }
            catch (System.Exception)
            {
            }

            return "";
        }

    }
}