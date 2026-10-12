using Dashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Web;

namespace Dashboard.Models.ViewModel
{
    public class UserPersonalDriveListResult
    {
        public List<UserPersonalDriveFolderItem> Folders { get; set; } = new List<UserPersonalDriveFolderItem>();
        public List<UserPersonalDriveFileItem> Files { get; set; } = new List<UserPersonalDriveFileItem>();
        public List<UserPersonalDriveBreadcrumbItem> Breadcrumbs { get; set; } = new List<UserPersonalDriveBreadcrumbItem>();
        public bool CanCreateFolder { get; set; }
        public bool CanUploadFile { get; set; }
        public bool CanManageCurrentFolder { get; set; }
    }

    public class UserPersonalDriveFolderItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string CreatedAtStr { get; set; }
        public string SharedWithNames { get; set; }
        public bool IsShared { get; set; }
        public bool CanDelete { get; set; }
        public bool CanShare { get; set; }
    }

    public class UserPersonalDriveFileItem
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string DuongDan { get; set; }
        public string OwnerName { get; set; }
        public string FileSizeStr { get; set; }
        public string UploadDateStr { get; set; }
        public bool CanDelete { get; set; }
        public bool CanDownload { get; set; }
        public bool CanCopyLink { get; set; }
    }

    public class UserPersonalDriveBreadcrumbItem
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }

    public class UserPersonalDriveFolderCreateModel
    {
        public int NguoiTaoId { get; set; }
        public int? ParentFolderId { get; set; }
        public string Name { get; set; }
        public string FolderName { get; set; }
    }

    public class UserPersonalDriveFileCreateModel
    {
        public int NguoiTaoId { get; set; }
        public int? UserPersonalDriveFolderId { get; set; }
        public string FileName { get; set; }
        public string DuongDan { get; set; }
    }

    public class UserPersonalDriveRequestModel
    {
        public int NguoiTaoId { get; set; }
        public int? FolderId { get; set; }
        public int? FileId { get; set; }
        public bool IsSharedDrive { get; set; }
        public string SearchKeyword { get; set; }
        public HttpPostedFileBase File { get; set; }
    }

    public class UserPersonalDriveShareUpdateModel
    {
        public int NguoiTaoId { get; set; }
        public int FolderId { get; set; }
        public List<int> UserIds { get; set; } = new List<int>();
    }

    public class UserPersonalDriveFileAccessResult
    {
        public bool IsSuccess { get; set; }
        public int? FolderId { get; set; }
        public string PhysicalFullPath { get; set; }
        public string DisplayName { get; set; }
        public string PublicRelativePath { get; set; }
    }

    public class UserPersonalDriveTrashListResult
    {
        public List<UserPersonalDriveTrashFolderItem> Folders { get; set; } = new List<UserPersonalDriveTrashFolderItem>();
        public List<UserPersonalDriveTrashFileItem> Files { get; set; } = new List<UserPersonalDriveTrashFileItem>();
        public List<UserPersonalDriveTrashItem> TrashItems { get; set; } = new List<UserPersonalDriveTrashItem>();
    }

    public class UserPersonalDriveTrashFolderItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ParentFolderName { get; set; }
        public string CreatedAtStr { get; set; }
    }

    public class UserPersonalDriveTrashFileItem
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string DuongDan { get; set; }
        public string FolderName { get; set; }
        public string FileSizeStr { get; set; }
        public string UploadDateStr { get; set; }
    }

    public class UserPersonalDriveTrashItem
    {
        public int Id { get; set; }
        public string ItemType { get; set; }
        public string ItemTypeDisplay { get; set; }
        public string Name { get; set; }
        public string DuongDan { get; set; }
        public string ParentName { get; set; }
        public string FileSizeStr { get; set; }
        public string DateDisplay { get; set; }
    }

    public class BuildTrashItemsRequest
    {
        public List<UserPersonalDriveTrashFolderItem> FolderItems { get; set; } = new List<UserPersonalDriveTrashFolderItem>();
        public List<UserPersonalDriveTrashFileItem> FileItems { get; set; } = new List<UserPersonalDriveTrashFileItem>();
    }

    public class UserPersonalDriveFolderAccessContext
    {
        public UserPersonalDriveFolder Folder { get; set; }
        public bool IsAccessible { get; set; }
        public bool CanManage { get; set; }
        public int? SharedRootFolderId { get; set; }
    }

    public class UserPersonalDriveNameItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserPersonalDriveFolderShareNameItem
    {
        public int FolderId { get; set; }
        public string SharedWithNames { get; set; }
    }

    // Parameter classes for Repository methods
    public class BuildBreadcrumbsRequest
    {
        public int CurrentUserId { get; set; }
        public UserPersonalDriveFolderAccessContext AccessContext { get; set; }
        public bool IsSharedDrive { get; set; }
    }

    public class DeleteFolderRecursiveRequest
    {
        public int OwnerUserId { get; set; }
        public int FolderId { get; set; }
    }

    public class GetFolderIdsInTreeRequest
    {
        public List<UserPersonalDriveFolder> Folders { get; set; }
        public int RootFolderId { get; set; }
    }

    public class GetFolderAccessContextRequest
    {
        public int CurrentUserId { get; set; }
        public int? FolderId { get; set; }
    }

    public class GetSharedRootFolderIdRequest
    {
        public int CurrentUserId { get; set; }
        public int FolderId { get; set; }
    }

    public class GetSharedWithNamesRequest
    {
        public List<UserPersonalDriveFolderShareNameItem> SharedWithNamesByFolderId { get; set; }
        public int FolderId { get; set; }
    }

    public class GetOwnerNameRequest
    {
        public List<UserPersonalDriveNameItem> OwnerNames { get; set; }
        public int UserId { get; set; }
    }
}
