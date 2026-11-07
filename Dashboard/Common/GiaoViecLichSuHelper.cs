using Dashboard.Models.Entities;
using System;
using System.Security.Claims;
using System.Web;

namespace Dashboard.Common
{
    public static class GiaoViecLichSuHelper
    {
        public static GiaoViecLichSu Tao(int giaoViecId, string hanhDong, string noiDung, int? giaoViecThanhVienId = null)
        {
            var identity = HttpContext.Current?.User?.Identity as ClaimsIdentity;
            return new GiaoViecLichSu
            {
                GiaoViecId = giaoViecId,
                GiaoViecThanhVienId = giaoViecThanhVienId,
                NguoiThucHienId = identity == null ? null : (int?)UserClaimHelper.GetCurrentUserId(identity),
                HanhDong = hanhDong,
                NoiDung = noiDung,
                NgayTao = DateTime.Now
            };
        }
    }
}
