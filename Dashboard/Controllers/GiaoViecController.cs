using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dashboard.Services;

namespace Dashboard.Controllers
{
    public class GiaoViecController : Controller
    {
        private readonly IGiaoViecService _service;

        public GiaoViecController(IGiaoViecService service)
        {
            _service = service;
        }

        [JwtAuthorize("GiaoViec")]
        public ActionResult Index(GiaoViecFormQueryViewModel queryParams)
        {
            return View(queryParams ?? new GiaoViecFormQueryViewModel());
        }

        [JwtAuthorize("GiaoViec")]
        public ActionResult Form(GiaoViecFormQueryViewModel queryParams)
        {
            if (queryParams == null)
            {
                queryParams = new GiaoViecFormQueryViewModel { Id = 0 };
            }
            return View(queryParams);
        }

        [JwtAuthorize("GiaoViec")]
        public ActionResult GetListDaGiao(GiaoViecQueryViewModel queryParams)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenCRUDTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenCRUDTatCaPhongBan.ToString());

            if (!isSystemUser && !isQuyenCRUDTatCaPhongBan)
            {
                queryParams.NguoiGiaoId = UserClaimHelper.GetCurrentUserId(identity);
            }

            var data = _service.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("GiaoViec")]
        public JsonResult GetNguoiDuocGiaoLookup()
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

        [JwtAuthorize("GiaoViec")]
        public JsonResult GetById(int id)
        {
            var data = _service.GetById(id);
            return Json(new
            {
                Item = data
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("GiaoViec")]
        public async Task<JsonResult> GetFileAttachFromIoffice(string vanBanId)
        {
            var files = await IofficeFileHelper.GetFileAttachAsync(vanBanId);
            return Json(new { data = files }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("GiaoViec")]
        public ActionResult ThemOrCapNhat(GiaoViecViewModel viewModel)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenCRUDTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenCRUDTatCaPhongBan.ToString());
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);

            if (viewModel.Id > 0)
            {
                var giaoViec = _service.GetById(viewModel.Id);
                if (giaoViec == null || giaoViec.Id == 0)
                {
                    return Json(new
                    {
                        Errors = new List<ValidateError>
                        {
                            new ValidateError { Id = "Id", Description = "Công việc không tồn tại." }
                        }
                    }, JsonRequestBehavior.AllowGet);
                }

                if (!isSystemUser && !isQuyenCRUDTatCaPhongBan && giaoViec.NguoiGiaoId != currentUserId)
                {
                    return Json(new
                    {
                        Errors = new List<ValidateError>
                        {
                            new ValidateError { Id = "Permission", Description = "Bạn không có quyền cập nhật công việc này." }
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            viewModel.CurrentUserId = currentUserId;
            var data = _service.ThemOrCapNhat(viewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("GiaoViec")]
        public ActionResult Xoa(int Id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenCRUDTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenCRUDTatCaPhongBan.ToString());
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);

            var giaoViec = _service.GetById(Id);
            if (giaoViec == null || giaoViec.Id == 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (!isSystemUser && !isQuyenCRUDTatCaPhongBan && giaoViec.NguoiGiaoId != currentUserId)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var data = _service.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public ActionResult DemSoLuongCongViecDaGiao()
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((ClaimsIdentity)User.Identity);
            var congViecDaGiao = _service.DemSoLuongCongViecDaGiao(currentUserId);
            var congViecCanXuLy = _service.DemSoLuongCongViecCanXuLy(currentUserId);
            var congViecDaChuyenGiaoChuaHoanThanh = _service.DemSoLuongCongViecDaChuyenGiaoChuaHoanThanh(currentUserId);
            return Json(new { congViecDaGiao = congViecDaGiao, congViecCanXuLy = congViecCanXuLy, congViecDaChuyenGiaoChuaHoanThanh = congViecDaChuyenGiaoChuaHoanThanh }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("GiaoViec")]
        public ActionResult GuiNhacNho(GuiNhacNhoViewModel viewModel)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenCRUDTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenCRUDTatCaPhongBan.ToString());
            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);

            var giaoViec = _service.GetById(viewModel.GiaoViecId);
            if (giaoViec == null || giaoViec.Id == 0)
            {
                return Json(new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError { Id = "GiaoViecId", Description = "Không tìm thấy công việc." }
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            if (!isSystemUser && !isQuyenCRUDTatCaPhongBan && giaoViec.NguoiGiaoId != currentUserId)
            {
                return Json(new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError { Id = "Permission", Description = "Bạn không có quyền gửi nhắc nhở công việc này." }
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            var data = _service.GuiNhacNho(viewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("GiaoViec")]
        public JsonResult GetGoiYNoiDung(GoiYNoiDungQueryViewModel queryParams)
        {
            if (queryParams == null) queryParams = new GoiYNoiDungQueryViewModel();
            var identity = (ClaimsIdentity)User.Identity;
            queryParams.CurrentUserId = UserClaimHelper.GetCurrentUserId(identity);
            if (!queryParams.Loai.HasValue || queryParams.Loai.Value == 0)
            {
                queryParams.Loai = (int)LoaiGoiYNoiDungEnum.NoiDungCongViec;
            }
            var data = _service.GetListGoiYNoiDung(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
