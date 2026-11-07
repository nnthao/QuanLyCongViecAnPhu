using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class KhoNhiemVuHoanThanhController : Controller
    {
        private readonly IKhoNhiemVuHoanThanhService _service;

        public KhoNhiemVuHoanThanhController(IKhoNhiemVuHoanThanhService service)
        {
            _service = service;
        }

        [JwtAuthorize("KhoNhiemVuHoanThanh")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("KhoNhiemVuHoanThanh")]
        public ActionResult GetList(KhoNhiemVuHoanThanhQueryViewModel queryParams)
        {
            var identity = (ClaimsIdentity)User.Identity;
            queryParams.CurrentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var data = _service.GetListHoanThanh(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("KhoNhiemVuHoanThanh")]
        public ActionResult HuyDanhGia(HuyDanhGiaViewModel viewModel)
        {
            var identity = (ClaimsIdentity)User.Identity;
            viewModel.CurrentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var result = _service.HuyDanhGia(viewModel);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult DemSoLuongHoanThanh()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var count = _service.DemSoLuongHoanThanh(currentUserId);
            return Json(new { count = count }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("KhoNhiemVuHoanThanh")]
        public JsonResult GetNguoiThucHienLookup()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenGiaoViecTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenGiaoViecTatCaPhongBan.ToString());
            var isQuyenCRUDTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenCRUDTatCaPhongBan.ToString());

            var currentPhongBanId = UserClaimHelper.GetPhongBanId(identity);
            var phongBanIds = new List<int>();
            if (!isSystemUser && !isQuyenGiaoViecTatCaPhongBan && !isQuyenCRUDTatCaPhongBan && currentPhongBanId > 0)
            {
                phongBanIds.Add(currentPhongBanId);
                phongBanIds.AddRange(UltiHelper.GetPhongBanChildrenIds(currentPhongBanId));
            }

            var users = CacheManagerHelper.GetListUserFromCacheNotDeleted()
                .Where(o => isSystemUser || isQuyenGiaoViecTatCaPhongBan || isQuyenCRUDTatCaPhongBan || !o.PhongBanId.HasValue || currentPhongBanId == 0 || phongBanIds.Contains(o.PhongBanId.Value))
                .Select(o => new
                {
                    id = o.Id,
                    text = o.Name
                })
                .OrderBy(o => o.text)
                .ToList();

            users.Insert(0, new
            {
                id = 0,
                text = "---Chọn---"
            });

            return Json(users, JsonRequestBehavior.AllowGet);
        }
    }
}

