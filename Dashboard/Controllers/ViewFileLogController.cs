using Dashboard.Common;
using Dashboard.Models.ViewModel;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class ViewFileLogController : Controller
    {
        private static ILog log = log4net.LogManager.GetLogger(typeof(MvcApplication));

        [JwtAuthorize("ViewFileLog")]
        public ActionResult Index()
        {

            var maxRetries = 5;
            var sleepTime = 3000;

            var text = "";
            var folderPath = System.Web.HttpContext.Current.Server.MapPath("/LogFiles");
            var folderPath_Copy = System.Web.HttpContext.Current.Server.MapPath("/LogFiles_Copy");
            if (!Directory.Exists(folderPath_Copy))
            {
                Directory.CreateDirectory(folderPath_Copy);
            }

            if (Directory.Exists(folderPath))
            {
                DirectoryInfo directory = new DirectoryInfo(folderPath);
                var fileLast = directory.GetFiles()
                                    .OrderByDescending(f => f.LastWriteTime)
                                    .FirstOrDefault();
                if (fileLast != null)
                {
                    var fileCopyPath = System.Web.HttpContext.Current.Server.MapPath($"/LogFiles_Copy/{DateTime.Now.ToString("yyyyMMdd_HHmmss")}_{fileLast}");
                    System.IO.File.Copy(fileLast.FullName, fileCopyPath);
                    if (System.IO.File.Exists(fileCopyPath))
                    {
                        var retries = 0;
                        while (true)
                        {
                            try
                            {
                                text = System.IO.File.ReadAllText(fileCopyPath);

                                break;
                            }
                            catch (Exception)
                            {
                                if (++retries > maxRetries)
                                {
                                    break;
                                }

                                Thread.Sleep(sleepTime);
                            }
                        }
                    }
                }
            }

            return View(new ParamControllerToView { Value = text });
        }

        [JwtAuthorize("ViewFileLog")]
        public JsonResult ClearLogTemp()
        {
            var folderPath_Copy = System.Web.HttpContext.Current.Server.MapPath("/LogFiles_Copy");
            if (Directory.Exists(folderPath_Copy))
            {
                DirectoryInfo directory = new DirectoryInfo(folderPath_Copy);
                var files = directory.GetFiles();
                foreach (var file in files)
                {
                    try
                    {
                        System.IO.File.Delete(file.FullName);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DanhSach()
        {
            return View();
        }

        public ActionResult GetListFile()
        {
            var datas = new List<ThongTinFile>();
            var folderPath = System.Web.HttpContext.Current.Server.MapPath("~/LogFiles");
            if (System.IO.Directory.Exists(folderPath))
            {
                datas = Directory.GetFiles(folderPath)
                                            .Select(file => new FileInfo(file))
                                            .OrderByDescending(fileInfo => fileInfo.LastWriteTime)
                                            .Select(fileInfo => new ThongTinFile
                                            {
                                                Ten = fileInfo.Name,
                                                DuongDan = fileInfo.FullName.Replace("\\", "/"),
                                                NgayTao = fileInfo.LastWriteTime,
                                                Link = "/LogFiles/" + fileInfo.Name
                                            })
                                            .ToList();
            }
            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteListFile(string duongDan)
        {
            try
            {
                if (System.IO.File.Exists(duongDan))
                {
                    System.IO.File.Delete(duongDan);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadLogFile(string link)
        {
            // Đường dẫn thư mục chứa file
            string filePath = Server.MapPath(link);

            // Kiểm tra xem file có tồn tại không
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound("File không tồn tại.");
            }

            // Lấy loại nội dung (Content-Type)
            string contentType = MimeMapping.GetMimeMapping(filePath);

            // Trả về file cho client
            return File(filePath, contentType, Path.GetFileName(filePath));
        }

        [HttpPost]
        public ActionResult DeleteListFileFormDate(string dt)
        {
            try
            {
                if (!string.IsNullOrEmpty(dt))
                {
                    var time = DateTimeHelper.ConvertStrToBeginOfDay(dt);
                    var folderPath = System.Web.HttpContext.Current.Server.MapPath("~/LogFiles");
                    if (System.IO.Directory.Exists(folderPath))
                    {
                        var files = Directory.GetFiles(folderPath);

                        foreach (var file in files)
                        {
                            FileInfo fileInfo = new FileInfo(file);

                            if (fileInfo.LastWriteTime < time)
                            {
                                try
                                {
                                    if (System.IO.File.Exists(fileInfo.FullName))
                                    {
                                        System.IO.File.Delete(fileInfo.FullName);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex);
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
    public class ThongTinFile
    {
        public string Ten { get; set; }
        public string DuongDan { get; set; }
        public string Link { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NgayTaoStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(NgayTao.GetValueOrDefault());
            }
        }
    }
}