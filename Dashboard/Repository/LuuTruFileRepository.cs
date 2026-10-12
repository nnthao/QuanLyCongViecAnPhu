using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace Dashboard.Repository
{
    public interface ILuuTruFileRepository
    {
        UserPersonalDriveListResult GetDriveList(UserPersonalDriveRequestModel model);
        UserPersonalDriveTrashListResult GetTrashList(UserPersonalDriveRequestModel model);
        bool CreateFolder(UserPersonalDriveFolderCreateModel model);
        int? AddDriveFile(UserPersonalDriveFileCreateModel model);
        UserPersonalDriveFileAccessResult GetFileForDownload(UserPersonalDriveRequestModel model);
        UserPersonalDriveFileAccessResult GetFileForPublicLink(UserPersonalDriveRequestModel model);
        bool DeleteDriveFile(UserPersonalDriveRequestModel model);
        bool DeleteDriveFolder(UserPersonalDriveRequestModel model);
        bool RestoreDriveFile(UserPersonalDriveRequestModel model);
        bool RestoreDriveFolder(UserPersonalDriveRequestModel model);
        bool CanManageFolder(UserPersonalDriveRequestModel model);
        List<UserPersonalDriveFolder> GetFolderChain(UserPersonalDriveRequestModel model);
        List<int> GetFolderShareUserIds(UserPersonalDriveRequestModel model);
        bool UpdateFolderShares(UserPersonalDriveShareUpdateModel model);
    }

    public class LuuTruFileRepository : ILuuTruFileRepository
    {
        private readonly IContext _context;

        private sealed class DriveFolderListRequest : UserPersonalDriveRequestModel
        {
            public bool CanManage { get; set; }
        }

        private sealed class DriveFolderItemMapRequest
        {
            public int CurrentUserId { get; set; }
            public int FolderId { get; set; }
            public string FolderName { get; set; }
            public DateTime? CreatedAt { get; set; }
            public int OwnerUserId { get; set; }
            public List<UserPersonalDriveNameItem> OwnerNames { get; set; } = new List<UserPersonalDriveNameItem>();
            public List<UserPersonalDriveFolderShareNameItem> SharedWithNamesByFolderId { get; set; } = new List<UserPersonalDriveFolderShareNameItem>();
            public bool IsShared { get; set; }
            public bool CanDelete { get; set; }
            public bool CanShare { get; set; }
        }

        private sealed class DriveFileItemMapRequest
        {
            public int CurrentUserId { get; set; }
            public int FileId { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public DateTime? UploadDate { get; set; }
            public int OwnerUserId { get; set; }
            public int? FolderId { get; set; }
            public List<UserPersonalDriveNameItem> OwnerNames { get; set; } = new List<UserPersonalDriveNameItem>();
            public bool CanManage { get; set; }
        }

        private sealed class DriveTrashItemMapRequest
        {
            public int Id { get; set; }
            public string ItemType { get; set; }
            public string Name { get; set; }
            public string FilePath { get; set; }
            public string ParentName { get; set; }
            public string FileSizeStr { get; set; }
            public string DateDisplay { get; set; }
        }

        public LuuTruFileRepository(IContext context)
        {
            _context = context;
        }

        public UserPersonalDriveListResult GetDriveList(UserPersonalDriveRequestModel model)
        {
            var result = new UserPersonalDriveListResult
            {
                Breadcrumbs = new List<UserPersonalDriveBreadcrumbItem>
                {
                    new UserPersonalDriveBreadcrumbItem { Id = null, Name = "Gốc" }
                }
            };

            if (model == null || model.NguoiTaoId <= 0)
            {
                return result;
            }

            var access = GetFolderAccessContext(new GetFolderAccessContextRequest
            {
                CurrentUserId = model.NguoiTaoId,
                FolderId = model.FolderId
            });
            if (!access.IsAccessible)
            {
                return result;
            }

            if (model.IsSharedDrive)
            {
                if (model.FolderId.HasValue && access.CanManage)
                {
                    return result;
                }
            }
            else if (model.FolderId.HasValue && !access.CanManage)
            {
                return result;
            }

            result.CanManageCurrentFolder = access.CanManage;
            result.CanCreateFolder = !model.IsSharedDrive && (!model.FolderId.HasValue || access.CanManage);
            result.CanUploadFile = !model.IsSharedDrive && model.FolderId.HasValue && access.CanManage;
            result.Breadcrumbs = BuildBreadcrumbs(new BuildBreadcrumbsRequest
            {
                CurrentUserId = model.NguoiTaoId,
                AccessContext = access,
                IsSharedDrive = model.IsSharedDrive
            });

            if (!model.FolderId.HasValue)
            {
                result.Folders = model.IsSharedDrive
                    ? GetSharedRootFolders(model)
                    : GetOwnedRootFolders(model);
                result.Files = model.IsSharedDrive
                    ? new List<UserPersonalDriveFileItem>()
                    : GetFolderFiles(new DriveFolderListRequest
                    {
                        NguoiTaoId = model.NguoiTaoId,
                        FolderId = null,
                        SearchKeyword = model.SearchKeyword,
                        CanManage = true
                    });
                return result;
            }

            result.Folders = GetChildFolders(new DriveFolderListRequest
            {
                NguoiTaoId = model.NguoiTaoId,
                FolderId = model.FolderId,
                SearchKeyword = model.SearchKeyword,
                CanManage = access.CanManage
            });
            result.Files = GetFolderFiles(new DriveFolderListRequest
            {
                NguoiTaoId = model.NguoiTaoId,
                FolderId = model.FolderId,
                SearchKeyword = model.SearchKeyword,
                CanManage = access.CanManage
            });
            return result;
        }

        public UserPersonalDriveTrashListResult GetTrashList(UserPersonalDriveRequestModel model)
        {
            var result = new UserPersonalDriveTrashListResult();
            if (model == null || model.NguoiTaoId <= 0)
            {
                return result;
            }

            var allFolders = _context.UserPersonalDriveFolders.AsNoTracking()
                .Where(o => o.NguoiTaoId == model.NguoiTaoId)
                .Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.ParentFolderId,
                    o.CreatedAt,
                    o.IsDeleted
                })
                .ToList();

            var folderLookup = allFolders.ToDictionary(o => o.Id, o => o);
            result.Folders = allFolders
                .Where(o => o.IsDeleted == true)
                .Where(o => !o.ParentFolderId.HasValue
                    || !folderLookup.ContainsKey(o.ParentFolderId.Value)
                    || folderLookup[o.ParentFolderId.Value].IsDeleted != true)
                .OrderByDescending(o => o.CreatedAt)
                .ThenBy(o => o.Name)
                .Select(o => new UserPersonalDriveTrashFolderItem
                {
                    Id = o.Id,
                    Name = o.Name,
                    ParentFolderName = o.ParentFolderId.HasValue && folderLookup.ContainsKey(o.ParentFolderId.Value)
                        ? folderLookup[o.ParentFolderId.Value].Name
                        : "",
                    CreatedAtStr = o.CreatedAt.HasValue ? o.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm") : ""
                })
                .ToList();

            var deletedFiles = _context.UserPersonalDriveFiles.AsNoTracking()
                .Where(o => o.NguoiTaoId == model.NguoiTaoId && o.IsDeleted == true)
                .Select(o => new
                {
                    o.Id,
                    o.FileName,
                    o.DuongDan,
                    o.NgayTao,
                    o.UserPersonalDriveFolderId
                })
                .ToList();

            result.Files = deletedFiles
                .Where(o => !o.UserPersonalDriveFolderId.HasValue
                    || !folderLookup.ContainsKey(o.UserPersonalDriveFolderId.Value)
                    || folderLookup[o.UserPersonalDriveFolderId.Value].IsDeleted != true)
                .OrderByDescending(o => o.NgayTao)
                .ThenBy(o => o.FileName)
                .Select(o => new UserPersonalDriveTrashFileItem
                {
                    Id = o.Id,
                    FileName = o.FileName,
                    DuongDan = o.DuongDan,
                    FolderName = o.UserPersonalDriveFolderId.HasValue && folderLookup.ContainsKey(o.UserPersonalDriveFolderId.Value)
                        ? folderLookup[o.UserPersonalDriveFolderId.Value].Name
                        : "",
                    FileSizeStr = GetPhysicalFileSizeDisplay(o.DuongDan),
                    UploadDateStr = o.NgayTao.HasValue ? o.NgayTao.Value.ToString("dd/MM/yyyy HH:mm") : ""
                })
                .ToList();

            result.TrashItems = BuildTrashItems(new BuildTrashItemsRequest
            {
                FolderItems = result.Folders,
                FileItems = result.Files
            });

            return result;
        }

        public bool CreateFolder(UserPersonalDriveFolderCreateModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.FolderName))
            {
                return false;
            }

            var name = model.Name.Trim();
            if (model.ParentFolderId.HasValue)
            {
                var parent = _context.UserPersonalDriveFolders.FirstOrDefault(o =>
                    o.Id == model.ParentFolderId.Value
                    && o.NguoiTaoId == model.NguoiTaoId
                    && o.IsDeleted != true);
                if (parent == null)
                {
                    return false;
                }
            }

            var duplicate = _context.UserPersonalDriveFolders.Any(o =>
                o.NguoiTaoId == model.NguoiTaoId
                && o.ParentFolderId == model.ParentFolderId
                && o.Name == name
                && o.IsDeleted != true);
            if (duplicate)
            {
                return false;
            }

            _context.UserPersonalDriveFolders.Add(new UserPersonalDriveFolder
            {
                NguoiTaoId = model.NguoiTaoId,
                ParentFolderId = model.ParentFolderId,
                Name = name,
                FolderName = model.FolderName,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            });
            _context.SaveChanges();
            return true;
        }

        public int? AddDriveFile(UserPersonalDriveFileCreateModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.FileName) || string.IsNullOrWhiteSpace(model.DuongDan))
            {
                return null;
            }

            if (model.UserPersonalDriveFolderId.HasValue)
            {
                var ok = _context.UserPersonalDriveFolders.Any(o =>
                    o.Id == model.UserPersonalDriveFolderId.Value
                    && o.NguoiTaoId == model.NguoiTaoId
                    && o.IsDeleted != true);
                if (!ok)
                {
                    return null;
                }
            }

            var entity = new UserPersonalDriveFile
            {
                NguoiTaoId = model.NguoiTaoId,
                UserPersonalDriveFolderId = model.UserPersonalDriveFolderId,
                FileName = model.FileName,
                DuongDan = model.DuongDan,
                NgayTao = DateTime.Now,
                IsDeleted = false
            };
            _context.UserPersonalDriveFiles.Add(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public UserPersonalDriveFileAccessResult GetFileForDownload(UserPersonalDriveRequestModel model)
        {
            var result = new UserPersonalDriveFileAccessResult();
            if (model == null || !model.FileId.HasValue || model.NguoiTaoId <= 0)
            {
                return result;
            }

            var file = _context.UserPersonalDriveFiles.AsNoTracking()
                .FirstOrDefault(o => o.Id == model.FileId.Value && o.IsDeleted != true);
            if (file == null || string.IsNullOrEmpty(file.DuongDan))
            {
                return result;
            }

            var canAccess = file.NguoiTaoId == model.NguoiTaoId;
            if (!canAccess && file.UserPersonalDriveFolderId.HasValue)
            {
                var access = GetFolderAccessContext(new GetFolderAccessContextRequest
                {
                    CurrentUserId = model.NguoiTaoId,
                    FolderId = file.UserPersonalDriveFolderId
                });
                canAccess = access.IsAccessible;
            }

            if (!canAccess)
            {
                return result;
            }

            result.DisplayName = file.FileName;
            result.FolderId = file.UserPersonalDriveFolderId;
            var path = file.DuongDan.StartsWith("/") ? "~" + file.DuongDan : file.DuongDan;
            result.PhysicalFullPath = HostingEnvironment.MapPath(path);
            result.IsSuccess = File.Exists(result.PhysicalFullPath);
            return result;
        }

        public UserPersonalDriveFileAccessResult GetFileForPublicLink(UserPersonalDriveRequestModel model)
        {
            var result = new UserPersonalDriveFileAccessResult();
            if (model == null || !model.FileId.HasValue || model.NguoiTaoId <= 0)
            {
                return result;
            }

            var file = _context.UserPersonalDriveFiles.AsNoTracking()
                .FirstOrDefault(o => o.Id == model.FileId.Value && o.NguoiTaoId == model.NguoiTaoId && o.IsDeleted != true);
            if (file == null || string.IsNullOrEmpty(file.DuongDan))
            {
                return result;
            }

            result.PublicRelativePath = file.DuongDan.StartsWith("/") ? file.DuongDan : "/" + file.DuongDan;
            result.IsSuccess = true;
            return result;
        }

        public bool DeleteDriveFile(UserPersonalDriveRequestModel model)
        {
            if (model == null || !model.FileId.HasValue)
            {
                return false;
            }

            var file = _context.UserPersonalDriveFiles.FirstOrDefault(o =>
                o.Id == model.FileId.Value
                && o.NguoiTaoId == model.NguoiTaoId);
            if (file == null)
            {
                return false;
            }

            file.IsDeleted = true;
            _context.SaveChanges();
            return true;
        }

        public bool DeleteDriveFolder(UserPersonalDriveRequestModel model)
        {
            if (model == null || !model.FolderId.HasValue)
            {
                return false;
            }

            var folder = _context.UserPersonalDriveFolders.FirstOrDefault(o =>
                o.Id == model.FolderId.Value
                && o.NguoiTaoId == model.NguoiTaoId
                && o.IsDeleted != true);
            if (folder == null)
            {
                return false;
            }

            DeleteFolderRecursive(new DeleteFolderRecursiveRequest
            {
                OwnerUserId = model.NguoiTaoId,
                FolderId = model.FolderId.Value
            });
            _context.SaveChanges();
            return true;
        }

        public bool RestoreDriveFile(UserPersonalDriveRequestModel model)
        {
            if (model == null || !model.FileId.HasValue)
            {
                return false;
            }

            var file = _context.UserPersonalDriveFiles.FirstOrDefault(o =>
                o.Id == model.FileId.Value
                && o.NguoiTaoId == model.NguoiTaoId
                && o.IsDeleted == true);
            if (file == null)
            {
                return false;
            }

            if (file.UserPersonalDriveFolderId.HasValue)
            {
                var folder = _context.UserPersonalDriveFolders.AsNoTracking().FirstOrDefault(o =>
                    o.Id == file.UserPersonalDriveFolderId.Value
                    && o.NguoiTaoId == model.NguoiTaoId
                    && o.IsDeleted != true);
                if (folder == null)
                {
                    return false;
                }
            }

            file.IsDeleted = false;
            _context.SaveChanges();
            return true;
        }

        public bool RestoreDriveFolder(UserPersonalDriveRequestModel model)
        {
            if (model == null || !model.FolderId.HasValue)
            {
                return false;
            }

            var rootFolder = _context.UserPersonalDriveFolders.FirstOrDefault(o =>
                o.Id == model.FolderId.Value
                && o.NguoiTaoId == model.NguoiTaoId
                && o.IsDeleted == true);
            if (rootFolder == null)
            {
                return false;
            }

            if (rootFolder.ParentFolderId.HasValue)
            {
                var parentFolder = _context.UserPersonalDriveFolders.AsNoTracking().FirstOrDefault(o =>
                    o.Id == rootFolder.ParentFolderId.Value
                    && o.NguoiTaoId == model.NguoiTaoId
                    && o.IsDeleted != true);
                if (parentFolder == null)
                {
                    return false;
                }
            }

            var folders = _context.UserPersonalDriveFolders
                .Where(o => o.NguoiTaoId == model.NguoiTaoId)
                .ToList();

            var folderIdsToRestore = GetFolderIdsInTree(new GetFolderIdsInTreeRequest
            {
                Folders = folders,
                RootFolderId = rootFolder.Id
            });
            if (folderIdsToRestore.Count == 0)
            {
                return false;
            }

            foreach (var folder in folders.Where(o => folderIdsToRestore.Contains(o.Id)))
            {
                folder.IsDeleted = false;
            }

            var files = _context.UserPersonalDriveFiles
                .Where(o => o.NguoiTaoId == model.NguoiTaoId
                    && o.UserPersonalDriveFolderId.HasValue
                    && folderIdsToRestore.Contains(o.UserPersonalDriveFolderId.Value))
                .ToList();
            foreach (var file in files)
            {
                file.IsDeleted = false;
            }

            var shares = _context.UserPersonalDriveFolderShares
                .Where(o => folderIdsToRestore.Contains(o.UserPersonalDriveFolderId))
                .ToList();
            foreach (var share in shares)
            {
                share.IsDeleted = false;
            }

            _context.SaveChanges();
            return true;
        }

        public bool CanManageFolder(UserPersonalDriveRequestModel model)
        {
            if (model == null)
            {
                return false;
            }

            if (!model.FolderId.HasValue)
            {
                return true;
            }

            var access = GetFolderAccessContext(new GetFolderAccessContextRequest
            {
                CurrentUserId = model.NguoiTaoId,
                FolderId = model.FolderId
            });
            return access.IsAccessible && access.CanManage;
        }

        public List<UserPersonalDriveFolder> GetFolderChain(UserPersonalDriveRequestModel model)
        {
            var result = new List<UserPersonalDriveFolder>();
            if (model == null || model.NguoiTaoId <= 0 || !model.FolderId.HasValue)
            {
                return result;
            }

            var walk = model.FolderId;
            while (walk.HasValue)
            {
                var folder = _context.UserPersonalDriveFolders.AsNoTracking()
                    .FirstOrDefault(o =>
                        o.Id == walk.Value
                        && o.NguoiTaoId == model.NguoiTaoId
                        && o.IsDeleted != true);
                if (folder == null)
                {
                    break;
                }

                result.Insert(0, folder);
                walk = folder.ParentFolderId;
            }

            return result;
        }

        public List<int> GetFolderShareUserIds(UserPersonalDriveRequestModel model)
        {
            if (model == null || !model.FolderId.HasValue)
            {
                return new List<int>();
            }

            var folder = _context.UserPersonalDriveFolders.AsNoTracking()
                .FirstOrDefault(o =>
                    o.Id == model.FolderId.Value
                    && o.NguoiTaoId == model.NguoiTaoId
                    && o.IsDeleted != true);
            if (folder == null)
            {
                return new List<int>();
            }

            return _context.UserPersonalDriveFolderShares.AsNoTracking()
                .Where(o =>
                    o.UserPersonalDriveFolderId == folder.Id
                    && o.IsDeleted != true)
                .Select(o => o.UserId)
                .Distinct()
                .ToList();
        }

        public bool UpdateFolderShares(UserPersonalDriveShareUpdateModel model)
        {
            if (model == null || model.FolderId <= 0 || model.NguoiTaoId <= 0)
            {
                return false;
            }

            var folder = _context.UserPersonalDriveFolders.FirstOrDefault(o =>
                o.Id == model.FolderId
                && o.NguoiTaoId == model.NguoiTaoId
                && o.IsDeleted != true);
            if (folder == null)
            {
                return false;
            }

            var userIds = (model.UserIds ?? new List<int>())
                .Where(o => o > 0 && o != model.NguoiTaoId)
                .Distinct()
                .ToList();

            var validUserIds = _context.Users.AsNoTracking()
                .Where(o => userIds.Contains(o.Id) && o.IsDeleted != true)
                .Select(o => o.Id)
                .ToList();

            var existingShares = _context.UserPersonalDriveFolderShares
                .Where(o => o.UserPersonalDriveFolderId == model.FolderId)
                .ToList();

            foreach (var share in existingShares)
            {
                share.IsDeleted = !validUserIds.Contains(share.UserId);
            }

            foreach (var userId in validUserIds)
            {
                var existing = existingShares.FirstOrDefault(o => o.UserId == userId);
                if (existing == null)
                {
                    _context.UserPersonalDriveFolderShares.Add(new UserPersonalDriveFolderShare
                    {
                        UserPersonalDriveFolderId = model.FolderId,
                        UserId = userId,
                        SharedByUserId = model.NguoiTaoId,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    });
                    continue;
                }

                existing.IsDeleted = false;
                existing.SharedByUserId = model.NguoiTaoId;
            }

            _context.SaveChanges();
            return true;
        }

        private List<UserPersonalDriveFolderItem> GetOwnedRootFolders(UserPersonalDriveRequestModel request)
        {
            if (request == null)
            {
                return new List<UserPersonalDriveFolderItem>();
            }

            var keyword = NormalizeKeyword(request.SearchKeyword);
            var ownedFolders = _context.UserPersonalDriveFolders.AsNoTracking()
                .Where(o =>
                    o.NguoiTaoId == request.NguoiTaoId
                    && o.ParentFolderId == null
                    && o.IsDeleted != true)
                .Where(o => string.IsNullOrEmpty(keyword) || o.Name.Contains(keyword))
                .Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.CreatedAt,
                    o.NguoiTaoId
                })
                .ToList();

            var all = ownedFolders
                .Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.CreatedAt,
                    o.NguoiTaoId,
                    IsShared = false,
                    CanDelete = true,
                    CanShare = true
                })
                .OrderBy(o => o.Name)
                .ToList();

            var ownerIds = all.Select(o => o.NguoiTaoId).Distinct().ToList();
            var ownerNames = GetUserNames(ownerIds);
            var sharedWithNamesByFolderId = GetSharedWithNamesByFolderIds(all.Select(o => o.Id).ToList());

            return all.Select(o => CreateFolderItem(new DriveFolderItemMapRequest
            {
                CurrentUserId = request.NguoiTaoId,
                FolderId = o.Id,
                FolderName = o.Name,
                CreatedAt = o.CreatedAt,
                OwnerUserId = o.NguoiTaoId,
                OwnerNames = ownerNames,
                SharedWithNamesByFolderId = sharedWithNamesByFolderId,
                IsShared = o.IsShared,
                CanDelete = o.CanDelete,
                CanShare = o.CanShare
            })).ToList();
        }

        private List<UserPersonalDriveFolderItem> GetSharedRootFolders(UserPersonalDriveRequestModel request)
        {
            if (request == null)
            {
                return new List<UserPersonalDriveFolderItem>();
            }

            var keyword = NormalizeKeyword(request.SearchKeyword);
            var sharedFolders = (from share in _context.UserPersonalDriveFolderShares.AsNoTracking()
                                 join folder in _context.UserPersonalDriveFolders.AsNoTracking()
                                     on share.UserPersonalDriveFolderId equals folder.Id
                                 where share.UserId == request.NguoiTaoId
                                     && share.IsDeleted != true
                                     && folder.IsDeleted != true
                                     && folder.NguoiTaoId != request.NguoiTaoId
                                     && (string.IsNullOrEmpty(keyword) || folder.Name.Contains(keyword))
                                 select new
                                 {
                                     folder.Id,
                                     folder.Name,
                                     folder.CreatedAt,
                                     folder.NguoiTaoId
                })
                .ToList()
                .GroupBy(o => o.Id)
                .Select(o => o.First())
                .ToList();

            var all = sharedFolders.Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.CreatedAt,
                    o.NguoiTaoId,
                    IsShared = true,
                    CanDelete = false,
                    CanShare = false
                })
                .OrderBy(o => o.Name)
                .ToList();

            var ownerIds = all.Select(o => o.NguoiTaoId).Distinct().ToList();
            var ownerNames = GetUserNames(ownerIds);
            var sharedWithNamesByFolderId = GetSharedWithNamesByFolderIds(all.Select(o => o.Id).ToList());

            return all.Select(o => CreateFolderItem(new DriveFolderItemMapRequest
            {
                CurrentUserId = request.NguoiTaoId,
                FolderId = o.Id,
                FolderName = o.Name,
                CreatedAt = o.CreatedAt,
                OwnerUserId = o.NguoiTaoId,
                OwnerNames = ownerNames,
                SharedWithNamesByFolderId = sharedWithNamesByFolderId,
                IsShared = o.IsShared,
                CanDelete = o.CanDelete,
                CanShare = o.CanShare
            })).ToList();
        }

        private List<UserPersonalDriveFolderItem> GetChildFolders(DriveFolderListRequest request)
        {
            if (request == null || !request.FolderId.HasValue)
            {
                return new List<UserPersonalDriveFolderItem>();
            }

            var keyword = NormalizeKeyword(request.SearchKeyword);
            var folders = _context.UserPersonalDriveFolders.AsNoTracking()
                .Where(o => o.ParentFolderId == request.FolderId.Value && o.IsDeleted != true)
                .Where(o => string.IsNullOrEmpty(keyword) || o.Name.Contains(keyword))
                .Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.CreatedAt,
                    o.NguoiTaoId
                })
                .OrderBy(o => o.Name)
                .ToList();

            var ownerNames = GetUserNames(folders.Select(o => o.NguoiTaoId).Distinct().ToList());
            var sharedWithNamesByFolderId = GetSharedWithNamesByFolderIds(folders.Select(o => o.Id).ToList());

            return folders.Select(o => CreateFolderItem(new DriveFolderItemMapRequest
            {
                CurrentUserId = request.NguoiTaoId,
                FolderId = o.Id,
                FolderName = o.Name,
                CreatedAt = o.CreatedAt,
                OwnerUserId = o.NguoiTaoId,
                OwnerNames = ownerNames,
                SharedWithNamesByFolderId = sharedWithNamesByFolderId,
                IsShared = !request.CanManage,
                CanDelete = request.CanManage && o.NguoiTaoId == request.NguoiTaoId,
                CanShare = request.CanManage && o.NguoiTaoId == request.NguoiTaoId
            })).ToList();
        }

        private List<UserPersonalDriveFileItem> GetFolderFiles(DriveFolderListRequest request)
        {
            if (request == null)
            {
                return new List<UserPersonalDriveFileItem>();
            }

            var keyword = NormalizeKeyword(request.SearchKeyword);
            var query = _context.UserPersonalDriveFiles.AsNoTracking()
                .Where(o => o.IsDeleted != true);

            if (request.FolderId.HasValue)
            {
                query = query.Where(o => o.UserPersonalDriveFolderId == request.FolderId);
            }
            else
            {
                query = query.Where(o => o.UserPersonalDriveFolderId == null && o.NguoiTaoId == request.NguoiTaoId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(o => o.FileName.Contains(keyword));
            }

            var files = query
                .OrderBy(o => o.FileName)
                .Select(o => new
                {
                    o.Id,
                    o.FileName,
                    o.DuongDan,
                    o.NgayTao,
                    o.NguoiTaoId,
                    o.UserPersonalDriveFolderId
                })
                .ToList();

            var ownerNames = GetUserNames(files.Select(o => o.NguoiTaoId).Distinct().ToList());

            return files.Select(o => CreateFileItem(new DriveFileItemMapRequest
            {
                CurrentUserId = request.NguoiTaoId,
                FileId = o.Id,
                FileName = o.FileName,
                FilePath = o.DuongDan,
                UploadDate = o.NgayTao,
                OwnerUserId = o.NguoiTaoId,
                FolderId = o.UserPersonalDriveFolderId,
                OwnerNames = ownerNames,
                CanManage = request.CanManage
            })).ToList();
        }

        private List<UserPersonalDriveBreadcrumbItem> BuildBreadcrumbs(BuildBreadcrumbsRequest request)
        {
            if (request == null)
            {
                return new List<UserPersonalDriveBreadcrumbItem>();
            }

            var access = request.AccessContext;
            if (request.IsSharedDrive)
            {
                if (access == null || access.Folder == null)
                {
                    return new List<UserPersonalDriveBreadcrumbItem>();
                }

                var sharedChain = GetFolderChainInternal(access.Folder.Id);
                if (access.SharedRootFolderId.HasValue)
                {
                    sharedChain = sharedChain.SkipWhile(o => o.Id != access.SharedRootFolderId.Value).ToList();
                }

                return sharedChain.Select(o => new UserPersonalDriveBreadcrumbItem
                {
                    Id = o.Id,
                    Name = o.Name
                }).ToList();
            }

            var result = new List<UserPersonalDriveBreadcrumbItem>
            {
                new UserPersonalDriveBreadcrumbItem { Id = null, Name = "Gốc" }
            };

            if (access == null || access.Folder == null)
            {
                return result;
            }

            var chain = GetFolderChainInternal(access.Folder.Id);
            result.AddRange(chain.Select(o => new UserPersonalDriveBreadcrumbItem
            {
                Id = o.Id,
                Name = o.Name
            }));
            return result;
        }

        private void DeleteFolderRecursive(DeleteFolderRecursiveRequest request)
        {
            if (request == null)
            {
                return;
            }

            var children = _context.UserPersonalDriveFolders
                .Where(o => o.ParentFolderId == request.FolderId && o.NguoiTaoId == request.OwnerUserId && o.IsDeleted != true)
                .ToList();
            foreach (var child in children)
            {
                DeleteFolderRecursive(new DeleteFolderRecursiveRequest
                {
                    OwnerUserId = request.OwnerUserId,
                    FolderId = child.Id
                });
            }

            var files = _context.UserPersonalDriveFiles
                .Where(o => o.NguoiTaoId == request.OwnerUserId && o.UserPersonalDriveFolderId == request.FolderId && o.IsDeleted != true)
                .ToList();
            foreach (var file in files)
            {
                file.IsDeleted = true;
            }

            var shares = _context.UserPersonalDriveFolderShares
                .Where(o => o.UserPersonalDriveFolderId == request.FolderId && o.IsDeleted != true)
                .ToList();
            foreach (var share in shares)
            {
                share.IsDeleted = true;
            }

            var folder = _context.UserPersonalDriveFolders.First(o => o.Id == request.FolderId);
            folder.IsDeleted = true;
        }

        private static List<int> GetFolderIdsInTree(GetFolderIdsInTreeRequest request)
        {
            var result = new List<int>();
            if (request == null || request.Folders == null || request.Folders.Count == 0)
            {
                return result;
            }

            var folders = request.Folders;
            var queue = new Queue<int>();
            queue.Enqueue(request.RootFolderId);
            while (queue.Count > 0)
            {
                var folderId = queue.Dequeue();
                if (result.Contains(folderId))
                {
                    continue;
                }

                result.Add(folderId);
                var childIds = folders
                    .Where(o => o.ParentFolderId == folderId)
                    .Select(o => o.Id)
                    .ToList();
                foreach (var childId in childIds)
                {
                    queue.Enqueue(childId);
                }
            }

            return result;
        }

        private UserPersonalDriveFolderAccessContext GetFolderAccessContext(GetFolderAccessContextRequest request)
        {
            if (request == null)
            {
                return new UserPersonalDriveFolderAccessContext();
            }

            if (!request.FolderId.HasValue)
            {
                return new UserPersonalDriveFolderAccessContext
                {
                    IsAccessible = true,
                    CanManage = true
                };
            }

            var folder = _context.UserPersonalDriveFolders.AsNoTracking()
                .FirstOrDefault(o => o.Id == request.FolderId.Value && o.IsDeleted != true);
            if (folder == null)
            {
                return new UserPersonalDriveFolderAccessContext();
            }

            if (folder.NguoiTaoId == request.CurrentUserId)
            {
                return new UserPersonalDriveFolderAccessContext
                {
                    Folder = folder,
                    IsAccessible = true,
                    CanManage = true
                };
            }

            var sharedRootFolderId = GetSharedRootFolderId(new GetSharedRootFolderIdRequest
            {
                CurrentUserId = request.CurrentUserId,
                FolderId = folder.Id
            });
            return new UserPersonalDriveFolderAccessContext
            {
                Folder = folder,
                IsAccessible = sharedRootFolderId.HasValue,
                CanManage = false,
                SharedRootFolderId = sharedRootFolderId
            };
        }

        private int? GetSharedRootFolderId(GetSharedRootFolderIdRequest request)
        {
            if (request == null)
            {
                return null;
            }

            var chain = GetFolderChainInternal(request.FolderId);
            if (chain.Count == 0)
            {
                return null;
            }

            var chainIds = chain.Select(o => o.Id).ToList();
            var sharedIds = _context.UserPersonalDriveFolderShares.AsNoTracking()
                .Where(o =>
                    o.UserId == request.CurrentUserId
                    && o.IsDeleted != true
                    && chainIds.Contains(o.UserPersonalDriveFolderId))
                .Select(o => o.UserPersonalDriveFolderId)
                .Distinct()
                .ToList();

            return chain.FirstOrDefault(o => sharedIds.Contains(o.Id))?.Id;
        }

        private List<UserPersonalDriveFolder> GetFolderChainInternal(int folderId)
        {
            var result = new List<UserPersonalDriveFolder>();
            var walk = (int?)folderId;
            while (walk.HasValue)
            {
                var folder = _context.UserPersonalDriveFolders.AsNoTracking()
                    .FirstOrDefault(o => o.Id == walk.Value && o.IsDeleted != true);
                if (folder == null)
                {
                    break;
                }

                result.Insert(0, folder);
                walk = folder.ParentFolderId;
            }

            return result;
        }

        private List<UserPersonalDriveNameItem> GetUserNames(List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return new List<UserPersonalDriveNameItem>();
            }

            return _context.Users.AsNoTracking()
                .Where(o => userIds.Contains(o.Id))
                .Select(o => new UserPersonalDriveNameItem
                {
                    Id = o.Id,
                    Name = o.Name ?? ""
                })
                .ToList()
                .OrderBy(o => o.Name)
                .ToList();
        }

        private List<UserPersonalDriveFolderShareNameItem> GetSharedWithNamesByFolderIds(List<int> folderIds)
        {
            if (folderIds == null || folderIds.Count == 0)
            {
                return new List<UserPersonalDriveFolderShareNameItem>();
            }

            var shares = _context.UserPersonalDriveFolderShares.AsNoTracking()
                .Where(o => folderIds.Contains(o.UserPersonalDriveFolderId) && o.IsDeleted != true)
                .Select(o => new
                {
                    o.UserPersonalDriveFolderId,
                    o.UserId
                })
                .ToList();

            if (shares.Count == 0)
            {
                return new List<UserPersonalDriveFolderShareNameItem>();
            }

            var userNames = GetUserNames(shares.Select(o => o.UserId).Distinct().ToList());

            return shares
                .GroupBy(o => o.UserPersonalDriveFolderId)
                .Select(o => new UserPersonalDriveFolderShareNameItem
                {
                    FolderId = o.Key,
                    SharedWithNames = string.Join(", ", o.Select(x => GetOwnerName(new GetOwnerNameRequest { OwnerNames = userNames, UserId = x.UserId }))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct()
                        .OrderBy(x => x))
                })
                .OrderBy(o => o.FolderId)
                .ToList();
        }

        private static string GetSharedWithNames(GetSharedWithNamesRequest request)
        {
            if (request == null || request.SharedWithNamesByFolderId == null)
            {
                return "";
            }

            return request.SharedWithNamesByFolderId.FirstOrDefault(o => o.FolderId == request.FolderId)?.SharedWithNames ?? "";
        }

        private static string GetOwnerName(GetOwnerNameRequest request)
        {
            if (request == null || request.OwnerNames == null)
            {
                return "";
            }

            return request.OwnerNames.FirstOrDefault(o => o.Id == request.UserId)?.Name ?? "";
        }

        private static UserPersonalDriveFolderItem CreateFolderItem(DriveFolderItemMapRequest request)
        {
            if (request == null)
            {
                return new UserPersonalDriveFolderItem();
            }

            return new UserPersonalDriveFolderItem
            {
                Id = request.FolderId,
                Name = request.FolderName,
                OwnerName = request.OwnerUserId == request.CurrentUserId ? "Tôi" : GetOwnerName(new GetOwnerNameRequest
                {
                    OwnerNames = request.OwnerNames,
                    UserId = request.OwnerUserId
                }),
                CreatedAtStr = GetDateTimeDisplay(request.CreatedAt),
                SharedWithNames = GetSharedWithNames(new GetSharedWithNamesRequest
                {
                    SharedWithNamesByFolderId = request.SharedWithNamesByFolderId,
                    FolderId = request.FolderId
                }),
                IsShared = request.IsShared,
                CanDelete = request.CanDelete,
                CanShare = request.CanShare
            };
        }

        private static UserPersonalDriveFileItem CreateFileItem(DriveFileItemMapRequest request)
        {
            if (request == null)
            {
                return new UserPersonalDriveFileItem();
            }

              return new UserPersonalDriveFileItem
              {
                  Id = request.FileId,
                  FileName = request.FileName,
                  DuongDan = request.FilePath,
                  OwnerName = request.OwnerUserId == request.CurrentUserId ? "Tôi" : GetOwnerName(new GetOwnerNameRequest
                  {
                      OwnerNames = request.OwnerNames,
                      UserId = request.OwnerUserId
                  }),
                FileSizeStr = GetPhysicalFileSizeDisplay(request.FilePath),
                UploadDateStr = GetDateTimeDisplay(request.UploadDate),
                CanDelete = request.CanManage && request.OwnerUserId == request.CurrentUserId,
                CanDownload = request.FolderId.HasValue,
                CanCopyLink = request.OwnerUserId == request.CurrentUserId
            };
        }

        private static string GetDateTimeDisplay(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd/MM/yyyy HH:mm") : "";
        }

        private static string NormalizeKeyword(string keyword)
        {
            return string.IsNullOrWhiteSpace(keyword) ? "" : keyword.Trim();
        }

        private static List<UserPersonalDriveTrashItem> BuildTrashItems(BuildTrashItemsRequest request)
        {
            var result = new List<UserPersonalDriveTrashItem>();
            if (request == null)
            {
                return result;
            }

            if (request.FolderItems != null)
            {
                result.AddRange(request.FolderItems.Select(o => CreateTrashItem(new DriveTrashItemMapRequest
                {
                    Id = o.Id,
                    ItemType = "folder",
                    Name = o.Name,
                    ParentName = o.ParentFolderName,
                    FileSizeStr = "",
                    DateDisplay = o.CreatedAtStr
                })));
            }

            if (request.FileItems != null)
            {
                result.AddRange(request.FileItems.Select(o => CreateTrashItem(new DriveTrashItemMapRequest
                {
                    Id = o.Id,
                    ItemType = "file",
                    Name = o.FileName,
                    FilePath = o.DuongDan,
                    ParentName = o.FolderName,
                    FileSizeStr = o.FileSizeStr,
                    DateDisplay = o.UploadDateStr
                })));
            }

            return result
                .OrderByDescending(o => ParseDisplayDateTime(o.DateDisplay))
                .ThenBy(o => o.ItemTypeDisplay)
                .ThenBy(o => o.Name)
                .ToList();
        }

        private static UserPersonalDriveTrashItem CreateTrashItem(DriveTrashItemMapRequest request)
        {
            if (request == null)
            {
                return new UserPersonalDriveTrashItem();
            }

            return new UserPersonalDriveTrashItem
              {
                  Id = request.Id,
                  ItemType = request.ItemType,
                  ItemTypeDisplay = request.ItemType == "folder" ? "Thư mục" : "Tệp",
                  Name = request.Name,
                  DuongDan = request.FilePath,
                  ParentName = request.ParentName,
                  FileSizeStr = request.FileSizeStr,
                  DateDisplay = request.DateDisplay
              };
          }

        private static DateTime? ParseDisplayDateTime(string value)
        {
            DateTime parsedValue;
            return DateTime.TryParseExact(value ?? "", "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out parsedValue)
                ? parsedValue
                : (DateTime?)null;
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            var order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }

        private static string GetPhysicalFileSizeDisplay(string duongDan)
        {
            var bytes = GetPhysicalFileSize(duongDan);
            return bytes > 0 ? FormatFileSize(bytes) : "";
        }

        private static long GetPhysicalFileSize(string duongDan)
        {
            if (string.IsNullOrWhiteSpace(duongDan))
            {
                return 0;
            }

            try
            {
                var path = duongDan.StartsWith("/") ? "~" + duongDan : duongDan;
                var full = HostingEnvironment.MapPath(path);
                if (File.Exists(full))
                {
                    return new FileInfo(full).Length;
                }
            }
            catch
            {
            }

            return 0;
        }
    }
}
