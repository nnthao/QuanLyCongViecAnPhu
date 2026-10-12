using Dashboard.Common;
using Dashboard.Models.Validates;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class CauHinhWebConfigController : Controller
    {
        [JwtAuthorize("CauHinhWebConfig")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("CauHinhWebConfig")]
        public JsonResult GetAll()
        {
            var data = WebConfigHelper.GetAllAppSettings();
            return Json(new { Data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("CauHinhWebConfig")]
        public JsonResult Save(string key, string value)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrWhiteSpace(key))
            {
                errors.Add(new ValidateError
                {
                    Id = "Key",
                    Description = "Key không được để trống"
                });
            }

            if (errors.Count > 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                WebConfigHelper.SaveAppSetting(key, value ?? string.Empty);
                return Json(new { Success = true, Message = "Lưu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new
                {
                    Errors = new[]
                    {
                        new ValidateError
                        {
                            Id = "",
                            Description = $"Lỗi khi lưu: {ex.Message}"
                        }
                    }
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [JwtAuthorize("CauHinhWebConfig")]
        public JsonResult Delete(string key)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrWhiteSpace(key))
            {
                errors.Add(new ValidateError
                {
                    Id = "Key",
                    Description = "Key không được để trống"
                });
            }

            if (errors.Count > 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                if (!WebConfigHelper.AppSettingExists(key))
                {
                    return Json(new
                    {
                        Errors = new[]
                        {
                            new ValidateError
                            {
                                Id = "Key",
                                Description = "Key không tồn tại"
                            }
                        }
                    }, JsonRequestBehavior.AllowGet);
                }

                WebConfigHelper.DeleteAppSetting(key);
                return Json(new { Success = true, Message = "Xóa thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new
                {
                    Errors = new[]
                    {
                        new ValidateError
                        {
                            Id = "",
                            Description = $"Lỗi khi xóa: {ex.Message}"
                        }
                    }
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}

