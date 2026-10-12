using Dashboard.Common;
using Dashboard.Models;
using System.Collections.Generic;
using System.Web;

namespace Dashboard.Models.Validates
{
    public interface IDoanVien_TaiLieuValidate
    {
        ValidateError ValidateFile(HttpPostedFileBase file);
    }

    public class DoanVien_TaiLieuValidate : IDoanVien_TaiLieuValidate
    {
        private readonly IContext _context;

        public DoanVien_TaiLieuValidate(IContext context)
        {
            _context = context;
        }

        public ValidateError ValidateFile(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return new ValidateError
                {
                    Id = "File",
                    Description = "File không được để trống."
                };
            }

            if (string.IsNullOrEmpty(file.FileName))
            {
                return new ValidateError
                {
                    Id = "File",
                    Description = "Tên file không được để trống."
                };
            }

            // Kiểm tra tên file có chứa ký tự nguy hiểm không
            var fileName = file.FileName;
            var dangerousChars = new[] { "..", "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            foreach (var dangerousChar in dangerousChars)
            {
                if (fileName.Contains(dangerousChar))
                {
                    return new ValidateError
                    {
                        Id = "File",
                        Description = $"Tên file chứa ký tự không hợp lệ: {dangerousChar}"
                    };
                }
            }

            try
            {
                FileHelper.ValidateFileExtension(file);
            }
            catch (System.ArgumentException ex)
            {
                return new ValidateError
                {
                    Id = "File",
                    Description = ex.Message
                };
            }

            return null;
        }
    }
}

