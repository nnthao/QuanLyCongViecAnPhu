using Dashboard.Common;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class LuuTruFileController : Controller
    {
        private readonly ILuuTruFileService _luuTruFileService;
        private const long MaxSingleFileUploadBytes = 200L * 1024L * 1024L;

        public LuuTruFileController(ILuuTruFileService luuTruFileService)
        {
            _luuTruFileService = luuTruFileService;
        }

        private int GetOwnerUserId()
        {
            var identity = User.Identity as ClaimsIdentity;
            return UserClaimHelper.GetCurrentUserId(identity);
        }

        [JwtAuthorize("LuuTruFile")]
        public ActionResult Index(int? folderId = null)
        {
            return RedirectToAction("Drive", new { folderId = folderId });
        }

        [JwtAuthorize("LuuTruFile")]
        public ActionResult Drive(int? folderId = null)
        {
            ViewBag.Title = "Drive của tôi";
            ViewBag.IsSharedDrive = false;
            ViewBag.DrivePageUrl = "/LuuTruFile/Drive";
            ViewBag.DriveApiBaseUrl = "/LuuTruFile";
            return View();
        }

        [JwtAuthorize("LuuTruFile")]
        public JsonResult GetDriveList(UserPersonalDriveRequestModel model)
        {
            var ownerId = GetOwnerUserId();
            if (ownerId == 0)
            {
                return Json(new UserPersonalDriveListResult(), JsonRequestBehavior.AllowGet);
            }

            model = model ?? new UserPersonalDriveRequestModel();
            model.NguoiTaoId = ownerId;
            model.IsSharedDrive = false;

            var data = _luuTruFileService.GetDriveList(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("LuuTruFile")]
        public JsonResult CreateFolder(int? parentFolderId)
        {
            var ownerId = GetOwnerUserId();
            if (ownerId == 0)
            {
                return Json(new { ok = false, message = "Không xác định người dùng." }, JsonRequestBehavior.AllowGet);
            }

            var folderName = Request.Form["folderName"];
            if (string.IsNullOrWhiteSpace(folderName))
            {
                folderName = Request.Form["name"];
            }

            if (string.IsNullOrWhiteSpace(folderName))
            {
                return Json(new { ok = false, message = "Thiếu tên thư mục." }, JsonRequestBehavior.AllowGet);
            }

            if (!parentFolderId.HasValue && int.TryParse(Request.Form["parentFolderId"], out int parentFromForm))
            {
                parentFolderId = parentFromForm;
            }

            var ok = _luuTruFileService.CreateFolder(new UserPersonalDriveFolderCreateModel
            {
                NguoiTaoId = ownerId,
                ParentFolderId = parentFolderId,
                Name = folderName.Trim()
            });
            if (!ok)
            {
                return Json(new { ok = false, message = "Không tạo được thư mục." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { ok = true, message = "Đã tạo thư mục." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("LuuTruFile")]
        public JsonResult UploadFile(int? folderId)
        {
            var ownerId = GetOwnerUserId();
            if (ownerId == 0)
            {
                return Json(new { ok = false, message = "Không xác định người dùng." }, JsonRequestBehavior.AllowGet);
            }

            if (!folderId.HasValue)
            {
                return Json(new { ok = false, message = "Chỉ được tải lên file trong thư mục con." }, JsonRequestBehavior.AllowGet);
            }

            var file = Request.Files["file"];
            if (file == null || file.ContentLength <= 0)
            {
                return Json(new { ok = false, message = "Không có file để tải lên." }, JsonRequestBehavior.AllowGet);
            }

            if (file.ContentLength > MaxSingleFileUploadBytes)
            {
                return Json(new { ok = false, message = "Dung lượng file vượt quá 200MB." }, JsonRequestBehavior.AllowGet);
            }

            if (!FileHelper.IsAllowedFileExtension(file.FileName))
            {
                return Json(new
                {
                    ok = false,
                    message = "Định dạng file không được phép. Chỉ hỗ trợ: " + BienDungChung.FileAllowUploads + "."
                }, JsonRequestBehavior.AllowGet);
            }

            var fileId = _luuTruFileService.UploadDriveFile(new UserPersonalDriveRequestModel
            {
                NguoiTaoId = ownerId,
                FolderId = folderId,
                File = file
            });
            if (!fileId.HasValue)
            {
                return Json(new { ok = false, message = "Bạn không có quyền tải file vào thư mục này." }, JsonRequestBehavior.AllowGet);
            }

            var publicUrl = string.Empty;
            var publicFile = _luuTruFileService.GetFileForPublicLink(new UserPersonalDriveRequestModel
            {
                NguoiTaoId = ownerId,
                FileId = fileId.Value
            });
            if (publicFile.IsSuccess
                && !string.IsNullOrEmpty(publicFile.PublicRelativePath)
                && Request.Url != null)
            {
                publicUrl = Request.Url.Scheme + "://" + Request.Url.Authority + publicFile.PublicRelativePath;
            }

            return Json(new { ok = true, fileId = fileId, publicUrl = publicUrl }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("LuuTruFile")]
        public JsonResult GetFolderShareUsers(int id)
        {
            var ownerId = GetOwnerUserId();
            if (ownerId == 0)
            {
                return Json(new { ok = false, message = "Không xác định người dùng." }, JsonRequestBehavior.AllowGet);
            }

            var userIds = _luuTruFileService.GetFolderShareUserIds(new UserPersonalDriveRequestModel
            {
                NguoiTaoId = ownerId,
                FolderId = id
            });

            if (userIds == null)
            {
                return Json(new { ok = false, message = "Không lấy được danh sách chia sẻ." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { ok = true, userIds = userIds }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("LuuTruFile")]
        public JsonResult UpdateFolderShares(int id, List<int> userIds)
        {
            var ownerId = GetOwnerUserId();
            if (ownerId == 0)
            {
                return Json(new { ok = false, message = "Không xác định người dùng." }, JsonRequestBehavior.AllowGet);
            }

            var ok = _luuTruFileService.UpdateFolderShares(new UserPersonalDriveShareUpdateModel
            {
                NguoiTaoId = ownerId,
                FolderId = id,
                UserIds = userIds ?? new List<int>()
            });

            return Json(new
            {
                ok = ok,
                message = ok ? "Đã cập nhật chia sẻ thư mục." : "Không cập nhật được chia sẻ thư mục."
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("LuuTruFile")]
        public JsonResult CreatePublicLinkForFile(int id)
        {
            var ownerId = GetOwnerUserId();
            if (ownerId == 0)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }

            var publicFile = _luuTruFileService.GetFileForPublicLink(new UserPersonalDriveRequestModel
            {
                NguoiTaoId = ownerId,
                FileId = id
            });
            if (!publicFile.IsSuccess
                || string.IsNullOrEmpty(publicFile.PublicRelativePath)
                || Request.Url == null)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }

            var url = Request.Url.Scheme + "://" + Request.Url.Authority + publicFile.PublicRelativePath;
            return Json(new { ok = true, publicUrl = url }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("LuuTruFile")]
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

        [HttpPost]
        [JwtAuthorize("LuuTruFile")]
        public JsonResult DeleteFile(int id)
        {
            var ownerId = GetOwnerUserId();
            if (ownerId == 0)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }

            var ok = _luuTruFileService.DeleteDriveFile(new UserPersonalDriveRequestModel
            {
                NguoiTaoId = ownerId,
                FileId = id
            });
            return Json(new { ok = ok }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("LuuTruFile")]
        public JsonResult DeleteFolder(int id)
        {
            var ownerId = GetOwnerUserId();
            if (ownerId == 0)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }

            var ok = _luuTruFileService.DeleteDriveFolder(new UserPersonalDriveRequestModel
            {
                NguoiTaoId = ownerId,
                FolderId = id
            });
            return Json(new { ok = ok }, JsonRequestBehavior.AllowGet);
        }
    }
}
