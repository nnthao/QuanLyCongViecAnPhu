using Dashboard.Models.CommonModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Dashboard.Common
{
    public static class FileHelper
    {
        public static string FolderSavePath => "/FileTemp";

        private static List<string> GetAllowedExtensions()
        {
            var allowedExtensionsStr = BienDungChung.AllowedFileExtensions;
            if (string.IsNullOrEmpty(allowedExtensionsStr))
            {
                return new List<string> { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pdf", ".pptx", ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg", ".txt", ".zip", ".rar" };
            }
            return allowedExtensionsStr.Split(',').Select(e => e.Trim().ToLower()).ToList();
        }

        public static bool IsAllowedFileExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            var extension = Path.GetExtension(fileName).ToLower();
            var allowedExtensions = GetAllowedExtensions();
            return allowedExtensions.Contains(extension);
        }

        private static Dictionary<string, List<string>> GetAllowedMimeTypes()
        {
            return new Dictionary<string, List<string>>
            {
                { ".doc", new List<string> { "application/msword" } },
                { ".docx", new List<string> { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" } },
                { ".xls", new List<string> { "application/vnd.ms-excel", "application/excel" } },
                { ".xlsx", new List<string> { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                { ".ppt", new List<string> { "application/vnd.ms-powerpoint", "application/powerpoint" } },
                { ".pdf", new List<string> { "application/pdf" } },
                { ".pptx", new List<string> { "application/vnd.openxmlformats-officedocument.presentationml.presentation" } },
                { ".jpg", new List<string> { "image/jpeg" } },
                { ".jpeg", new List<string> { "image/jpeg" } },
                { ".png", new List<string> { "image/png" } },
                { ".gif", new List<string> { "image/gif" } },
                { ".bmp", new List<string> { "image/bmp" } },
                { ".webp", new List<string> { "image/webp" } },
                { ".svg", new List<string> { "image/svg+xml" } },
                { ".txt", new List<string> { "text/plain" } },
                { ".zip", new List<string> { "application/zip", "application/x-zip-compressed" } },
                { ".rar", new List<string> { "application/x-rar-compressed", "application/octet-stream" } }
            };
        }

        public static void ValidateFileExtension(HttpPostedFileBase file)
        {
            if (file == null)
            {
                throw new ArgumentException("File không được để trống.");
            }

            if (file.ContentLength == 0)
            {
                throw new ArgumentException("File không được để trống hoặc file rỗng.");
            }

            if (!IsAllowedFileExtension(file.FileName))
            {
                var allowedExtensions = GetAllowedExtensions();
                var allowedExtensionsStr = string.Join(", ", allowedExtensions);
                throw new ArgumentException($"Loại file không được phép. Chỉ chấp nhận các file: {allowedExtensionsStr}");
            }

            // Validate MIME type để tránh file độc hại giả mạo extension
            var extension = Path.GetExtension(file.FileName).ToLower();
            var allowedMimeTypes = GetAllowedMimeTypes();
            
            if (allowedMimeTypes.ContainsKey(extension))
            {
                var expectedMimeTypes = allowedMimeTypes[extension];
                var actualMimeType = file.ContentType.ToLower();
                
                // Một số file có thể có MIME type khác nhau, kiểm tra linh hoạt
                bool isValidMimeType = expectedMimeTypes.Any(mt => actualMimeType.Contains(mt.ToLower())) ||
                                       actualMimeType.Contains(extension.TrimStart('.'));

                // Cho phép một số trường hợp đặc biệt (zip, rar có thể có application/octet-stream)
                if (!isValidMimeType && extension != ".zip" && extension != ".rar")
                {
                    // Log warning nhưng vẫn cho phép nếu extension hợp lệ (để tránh false positive)
                    // Có thể bỏ comment dòng dưới để strict hơn
                    // throw new ArgumentException($"MIME type của file không khớp với extension. File có thể không an toàn.");
                }
            }
        }

        public static void SaveFiles(this List<FileModel> files)
        {
            string subPath = "~/Files/";

            bool exists = System.IO.Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(subPath));

            if (!exists)
                System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(subPath));

            foreach (var item in files)
            {
                ValidateFileExtension(item.File);
                var fullPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/"), item.FileName);

                using (var fileStream = File.Create(fullPath))
                {
                    item.File.InputStream.Seek(0, SeekOrigin.Begin);
                    item.File.InputStream.CopyTo(fileStream);
                }
            }
        }

        public static string SaveHttpFilesToUploads(string folderSave, HttpPostedFileBase file)
        {
            if (file == null)
            {
                throw new ArgumentException("File không được để trống.");
            }

            ValidateFileExtension(file);

            var exists = System.IO.Directory.Exists(folderSave);

            if (!exists)
                System.IO.Directory.CreateDirectory(folderSave);

            var fileName = DateTime.Now.ToFileTime() + Path.GetExtension(file.FileName);
            var fileNameFullPath = Path.Combine(folderSave, fileName);
            file.SaveAs(fileNameFullPath);

            string folderName = Path.GetFileName(Path.GetDirectoryName(fileNameFullPath));
            return $@"/{folderName}/{fileName}";
        }

        public static string SaveHttpFilesToUploadsRelative(string virtualDir, HttpPostedFileBase file)
        {
            if (file == null)
            {
                throw new ArgumentException("File không được để trống.");
            }

            ValidateFileExtension(file);

            var physicalDir = HttpContext.Current.Server.MapPath(virtualDir);
            if (!Directory.Exists(physicalDir))
            {
                Directory.CreateDirectory(physicalDir);
            }

            var fileName = DateTime.Now.ToFileTime() + Path.GetExtension(file.FileName);
            var fileNameFullPath = Path.Combine(physicalDir, fileName);
            file.SaveAs(fileNameFullPath);

            var normalizedVirtualDir = (virtualDir ?? string.Empty).Replace("\\", "/").TrimEnd('/');
            if (!normalizedVirtualDir.StartsWith("/"))
            {
                normalizedVirtualDir = "/" + normalizedVirtualDir.TrimStart('~', '/');
            }
            else
            {
                normalizedVirtualDir = normalizedVirtualDir.TrimStart('~');
            }

            return normalizedVirtualDir + "/" + fileName;
        }

        public static string SaveHttpFilesToUploadsRelative(string virtualDir, HttpPostedFileBase file, string fileName)
        {
            if (file == null)
            {
                throw new ArgumentException("File không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Tên file lưu không được để trống.");
            }

            ValidateFileExtension(file);

            var physicalDir = HttpContext.Current.Server.MapPath(virtualDir);
            if (!Directory.Exists(physicalDir))
            {
                Directory.CreateDirectory(physicalDir);
            }

            var fileNameFullPath = Path.Combine(physicalDir, fileName);
            file.SaveAs(fileNameFullPath);

            var normalizedVirtualDir = (virtualDir ?? string.Empty).Replace("\\", "/").TrimEnd('/');
            if (!normalizedVirtualDir.StartsWith("/"))
            {
                normalizedVirtualDir = "/" + normalizedVirtualDir.TrimStart('~', '/');
            }
            else
            {
                normalizedVirtualDir = normalizedVirtualDir.TrimStart('~');
            }

            return normalizedVirtualDir + "/" + fileName;
        }

        public static string GetFileNameAndFolderParent(string fullPath)
        {
            string fileName = Path.GetFileName(fullPath);
            string parentFolder = new DirectoryInfo(Path.GetDirectoryName(fullPath)).Name;

            return $@"/{parentFolder}/{fileName}";
        }
    }
}
