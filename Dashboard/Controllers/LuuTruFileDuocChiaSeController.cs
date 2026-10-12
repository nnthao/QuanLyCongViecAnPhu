using Dashboard.Common;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class LuuTruFileDuocChiaSeController : Controller
    {
        private readonly ILuuTruFileService _luuTruFileService;

        public LuuTruFileDuocChiaSeController(ILuuTruFileService luuTruFileService)
        {
            _luuTruFileService = luuTruFileService;
        }

        private int GetOwnerUserId()
        {
            var identity = User.Identity as ClaimsIdentity;
            return UserClaimHelper.GetCurrentUserId(identity);
        }

        [JwtAuthorize("LuuTruFileDuocChiaSe")]
        public ActionResult Index()
        {
            ViewBag.Title = "Drive được chia sẻ";
            ViewBag.IsSharedDrive = true;
            ViewBag.DrivePageUrl = "/LuuTruFileDuocChiaSe";
            ViewBag.DriveApiBaseUrl = "/LuuTruFileDuocChiaSe";
            return View();
        }

        [JwtAuthorize("LuuTruFileDuocChiaSe")]
        public JsonResult GetDriveList(UserPersonalDriveRequestModel model)
        {
            var ownerId = GetOwnerUserId();
            if (ownerId == 0)
            {
                return Json(new UserPersonalDriveListResult(), JsonRequestBehavior.AllowGet);
            }

            model = model ?? new UserPersonalDriveRequestModel();
            model.NguoiTaoId = ownerId;
            model.IsSharedDrive = true;

            var data = _luuTruFileService.GetDriveList(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("LuuTruFileDuocChiaSe")]
        public ActionResult DownloadFile(int id)
        {
            var ownerId = GetOwnerUserId();
            if (ownerId == 0)
            {
                return new HttpStatusCodeResult(403);
            }

            var downloadFile = _luuTruFileService.GetFileForDownload(new UserPersonalDriveRequestModel
            {
                NguoiTaoId = ownerId,
                FileId = id
            });
            if (!downloadFile.IsSuccess)
            {
                return HttpNotFound();
            }

            if (!downloadFile.FolderId.HasValue)
            {
                return new HttpStatusCodeResult(403);
            }

            return File(downloadFile.PhysicalFullPath, "application/octet-stream", downloadFile.DisplayName);
        }
    }
}
