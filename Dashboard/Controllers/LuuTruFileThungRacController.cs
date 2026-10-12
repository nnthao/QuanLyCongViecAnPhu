using Dashboard.Common;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class LuuTruFileThungRacController : Controller
    {
        private readonly ILuuTruFileService _luuTruFileService;

        public LuuTruFileThungRacController(ILuuTruFileService luuTruFileService)
        {
            _luuTruFileService = luuTruFileService;
        }

        private int GetOwnerUserId()
        {
            var identity = User.Identity as ClaimsIdentity;
            return UserClaimHelper.GetCurrentUserId(identity);
        }

        [JwtAuthorize("LuuTruFileThungRac")]
        public ActionResult Index()
        {
            ViewBag.Title = "Thùng rác Drive";
            return View();
        }

        [JwtAuthorize("LuuTruFileThungRac")]
        public JsonResult GetTrashList()
        {
            var ownerId = GetOwnerUserId();
            if (ownerId == 0)
            {
                return Json(new UserPersonalDriveTrashListResult(), JsonRequestBehavior.AllowGet);
            }

            var data = _luuTruFileService.GetTrashList(new UserPersonalDriveRequestModel
            {
                NguoiTaoId = ownerId
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("LuuTruFileThungRac")]
        public JsonResult RestoreFile(int id)
        {
            var ownerId = GetOwnerUserId();
            if (ownerId == 0)
            {
                return Json(new { ok = false, message = "Không xác định người dùng." }, JsonRequestBehavior.AllowGet);
            }

            var ok = _luuTruFileService.RestoreDriveFile(new UserPersonalDriveRequestModel
            {
                NguoiTaoId = ownerId,
                FileId = id
            });

            return Json(new
            {
                ok = ok,
                message = ok ? "Khôi phục file thành công." : "Không thể khôi phục file."
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("LuuTruFileThungRac")]
        public JsonResult RestoreFolder(int id)
        {
            var ownerId = GetOwnerUserId();
            if (ownerId == 0)
            {
                return Json(new { ok = false, message = "Không xác định người dùng." }, JsonRequestBehavior.AllowGet);
            }

            var ok = _luuTruFileService.RestoreDriveFolder(new UserPersonalDriveRequestModel
            {
                NguoiTaoId = ownerId,
                FolderId = id
            });

            return Json(new
            {
                ok = ok,
                message = ok ? "Khôi phục thư mục thành công." : "Không thể khôi phục thư mục."
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
