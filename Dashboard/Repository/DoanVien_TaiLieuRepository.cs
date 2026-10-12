using AutoMapper;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;

namespace Dashboard.Repository
{
    public interface IDoanVien_TaiLieuRepository
    {
        DataTableResult<DoanVien_TaiLieuGridVo> GetAll(QueryParamViewModel queryParams);
        DoanVien_TaiLieuGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(List<HttpPostedFileBase> files);
        bool Xoa(int Id);
    }

    public class DoanVien_TaiLieuRepository : IDoanVien_TaiLieuRepository
    {
        private readonly IContext _context;
        private readonly IDoanVien_TaiLieuValidate _validate;

        public DoanVien_TaiLieuRepository(IContext context, IDoanVien_TaiLieuValidate validate)
        {
            _context = context;
            _validate = validate;
        }

        private readonly string sortStringDefault = "NgayTao desc";
        private readonly List<string> searchCol = new List<string> { "TenFile" };

        public DataTableResult<DoanVien_TaiLieuGridVo> GetAll(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.DoanVien_TaiLieus.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(cv => new DoanVien_TaiLieuGridVo
            {
                Id = cv.Id,
                TenFile = cv.TenFile,
                DuongDan = cv.DuongDan,
                NgayTao = cv.NgayTao,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<DoanVien_TaiLieuGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(List<HttpPostedFileBase> files)
        {
            if (files == null || files.Count == 0)
            {
                return new { Errors = new List<ValidateError> { new ValidateError { Id = "Files", Description = "Vui lòng chọn ít nhất một file." } } };
            }

            var uploadPath = System.Web.HttpContext.Current.Server.MapPath(BienDungChung.UploadsPath);
            var errors = new List<ValidateError>();
            var successCount = 0;

            foreach (var file in files)
            {
                if (file == null || file.ContentLength == 0)
                {
                    continue;
                }

                try
                {
                    var validateError = _validate.ValidateFile(file);
                    if (validateError != null)
                    {
                        errors.Add(validateError);
                        continue;
                    }

                    var duongDan = FileHelper.SaveHttpFilesToUploads(uploadPath, file);
                    var item = new DoanVien_TaiLieu
                    {
                        TenFile = file.FileName,
                        DuongDan = duongDan,
                        NgayTao = DateTime.Now,
                        IsDeleted = false
                    };

                    _context.DoanVien_TaiLieus.Add(item);
                    successCount++;
                }
                catch (ArgumentException ex)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "Files",
                        Description = $"Lỗi khi upload file '{file?.FileName}': {ex.Message}"
                    });
                }
                catch (Exception ex)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "Files",
                        Description = $"Lỗi khi lưu file '{file?.FileName}': {ex.Message}"
                    });
                }
            }

            if (errors.Count > 0 && successCount == 0)
            {
                return new { Errors = errors };
            }

            _context.SaveChanges();

            if (errors.Count > 0)
            {
                return new { Success = true, SuccessCount = successCount, Errors = errors };
            }

            return new { Success = true, SuccessCount = successCount };
        }

        public bool Xoa(int Id)
        {
            var item = _context.DoanVien_TaiLieus.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = true;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool KhoiPhuc(int Id)
        {
            var item = _context.DoanVien_TaiLieus.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public DoanVien_TaiLieuGridVo GetById(int id)
        {
            var item = _context.DoanVien_TaiLieus.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new DoanVien_TaiLieuGridVo
                {
                    Id = item.Id,
                    TenFile = item.TenFile,
                    DuongDan = item.DuongDan,
                    NgayTao = item.NgayTao
                };
            }

            return new DoanVien_TaiLieuGridVo();
        }
    }
}

