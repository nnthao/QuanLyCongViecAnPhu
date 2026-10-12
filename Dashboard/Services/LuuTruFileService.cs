using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Dashboard.Services
{
    public interface ILuuTruFileService
    {
        UserPersonalDriveListResult GetDriveList(UserPersonalDriveRequestModel model);
        UserPersonalDriveTrashListResult GetTrashList(UserPersonalDriveRequestModel model);
        bool CreateFolder(UserPersonalDriveFolderCreateModel model);
        int? UploadDriveFile(UserPersonalDriveRequestModel model);
        UserPersonalDriveFileAccessResult GetFileForDownload(UserPersonalDriveRequestModel model);
        UserPersonalDriveFileAccessResult GetFileForPublicLink(UserPersonalDriveRequestModel model);
        bool DeleteDriveFile(UserPersonalDriveRequestModel model);
        bool DeleteDriveFolder(UserPersonalDriveRequestModel model);
        bool RestoreDriveFile(UserPersonalDriveRequestModel model);
        bool RestoreDriveFolder(UserPersonalDriveRequestModel model);
        List<int> GetFolderShareUserIds(UserPersonalDriveRequestModel model);
        bool UpdateFolderShares(UserPersonalDriveShareUpdateModel model);
    }

    public class LuuTruFileService : ILuuTruFileService
    {
        private readonly ILuuTruFileRepository _repository;

        public LuuTruFileService(ILuuTruFileRepository repository)
        {
            _repository = repository;
        }

        public UserPersonalDriveListResult GetDriveList(UserPersonalDriveRequestModel model)
        {
            return _repository.GetDriveList(model);
        }

        public UserPersonalDriveTrashListResult GetTrashList(UserPersonalDriveRequestModel model)
        {
            return _repository.GetTrashList(model);
        }

        public bool CreateFolder(UserPersonalDriveFolderCreateModel model)
        {
            var storageFolderName = "F_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            model.FolderName = storageFolderName;
            if (!_repository.CreateFolder(model))
            {
                return false;
            }

            EnsurePhysicalFolder(model.NguoiTaoId, model.ParentFolderId, storageFolderName);
            return true;
        }

        public int? UploadDriveFile(UserPersonalDriveRequestModel model)
        {
            if (model == null || model.File == null || model.File.ContentLength == 0)
            {
                return null;
            }

            if (!FileHelper.IsAllowedFileExtension(model.File.FileName))
            {
                return null;
            }

            if (!_repository.CanManageFolder(model))
            {
                return null;
            }

            var virtualDir = BuildVirtualUploadDir(model.FolderId, model.NguoiTaoId);
            var generatedFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(model.File.FileName);
            var webPath = FileHelper.SaveHttpFilesToUploadsRelative(virtualDir, model.File, generatedFileName);
            return _repository.AddDriveFile(new UserPersonalDriveFileCreateModel
            {
                NguoiTaoId = model.NguoiTaoId,
                UserPersonalDriveFolderId = model.FolderId,
                FileName = Path.GetFileName(model.File.FileName),
                DuongDan = webPath
            });
        }

        public UserPersonalDriveFileAccessResult GetFileForDownload(UserPersonalDriveRequestModel model)
        {
            return _repository.GetFileForDownload(model);
        }

        public UserPersonalDriveFileAccessResult GetFileForPublicLink(UserPersonalDriveRequestModel model)
        {
            return _repository.GetFileForPublicLink(model);
        }

        public bool DeleteDriveFile(UserPersonalDriveRequestModel model)
        {
            return _repository.DeleteDriveFile(model);
        }

        public bool DeleteDriveFolder(UserPersonalDriveRequestModel model)
        {
            return _repository.DeleteDriveFolder(model);
        }

        public bool RestoreDriveFile(UserPersonalDriveRequestModel model)
        {
            return _repository.RestoreDriveFile(model);
        }

        public bool RestoreDriveFolder(UserPersonalDriveRequestModel model)
        {
            return _repository.RestoreDriveFolder(model);
        }

        public List<int> GetFolderShareUserIds(UserPersonalDriveRequestModel model)
        {
            return _repository.GetFolderShareUserIds(model);
        }

        public bool UpdateFolderShares(UserPersonalDriveShareUpdateModel model)
        {
            return _repository.UpdateFolderShares(model);
        }

        private void EnsurePhysicalFolder(int ownerUserId, int? parentFolderId, string storageFolderName)
        {
            var parentVirtualDir = BuildVirtualUploadDir(parentFolderId, ownerUserId);
            var parentPhysicalDir = HttpContext.Current.Server.MapPath(parentVirtualDir);
            var currentPhysicalDir = Path.Combine(parentPhysicalDir, storageFolderName);
            if (!Directory.Exists(currentPhysicalDir))
            {
                Directory.CreateDirectory(currentPhysicalDir);
            }
        }

        private string BuildVirtualUploadDir(int? folderId, int ownerUserId)
        {
            var virtualDir = "~/Uploads";
            var chain = _repository.GetFolderChain(new UserPersonalDriveRequestModel
            {
                NguoiTaoId = ownerUserId,
                FolderId = folderId
            });
            if (chain == null || chain.Count == 0)
            {
                return virtualDir;
            }

            return virtualDir + "/" + string.Join("/", GetPhysicalFolderNames(chain));
        }

        private static IEnumerable<string> GetPhysicalFolderNames(IEnumerable<UserPersonalDriveFolder> chain)
        {
            foreach (var folder in chain)
            {
                if (!string.IsNullOrWhiteSpace(folder.FolderName))
                {
                    yield return folder.FolderName;
                }
            }
        }

    }
}
