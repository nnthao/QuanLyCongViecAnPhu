using Dashboard.Common;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class VnEduController : Controller
    {
        private const string TemplateVirtualPath = "~/Temp/File_Import_hocsinh_vnEdu.xls";
        private const string OutputFilePrefix = "edu_";
        private readonly VnEduImportValidate _validate;
        private readonly VnEduImportHelper _importHelper;

        public VnEduController()
        {
            _validate = new VnEduImportValidate();
            _importHelper = new VnEduImportHelper();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChuyenDoi(VnEduImportRequestViewModel request)
        {
            request = request ?? new VnEduImportRequestViewModel();
            var errors = _validate.Validate(request);
            if (errors.Count > 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }

            var uploadFilePath = string.Empty;
            try
            {
                uploadFilePath = SaveUploadFile(request);
                var templateFilePath = Server.MapPath(TemplateVirtualPath);
                var outputFileName = $"{OutputFilePrefix}{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
                var outputFilePath = Path.Combine(GetOutputFolderPath(), outputFileName);
                var result = _importHelper.ConvertToVnEduFile(uploadFilePath, templateFilePath, outputFilePath, request.Khoi.GetValueOrDefault());
                result.DownloadUrl = GetOutputFileUrl(result.FileName);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "FileImport",
                            Description = ex.Message
                        }
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                DeleteUploadFile(uploadFilePath);
            }
        }

        private string SaveUploadFile(VnEduImportRequestViewModel request)
        {
            var outputFolderPath = GetOutputFolderPath();
            var uploadFileName = $"VnEdu_Source_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
            var uploadFilePath = Path.Combine(outputFolderPath, uploadFileName);
            request.FileImport.SaveAs(uploadFilePath);
            return uploadFilePath;
        }

        private string GetOutputFolderPath()
        {
            var outputFolderPath = Server.MapPath(FileHelper.FolderSavePath);
            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            return outputFolderPath;
        }

        private static string GetOutputFileUrl(string fileName)
        {
            return $"{FileHelper.FolderSavePath}/{fileName}";
        }

        private static void DeleteUploadFile(string uploadFilePath)
        {
            if (!string.IsNullOrWhiteSpace(uploadFilePath) && System.IO.File.Exists(uploadFilePath))
            {
                System.IO.File.Delete(uploadFilePath);
            }
        }
    }
}
