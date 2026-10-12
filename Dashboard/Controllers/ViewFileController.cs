using Dashboard.Common;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class ViewFileController : Controller
    {
        [JwtAuthorize("Public")]
        public ActionResult ViewFile(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return HttpNotFound("URL không hợp lệ.");
            }

            // Loại bỏ dấu / ở đầu nếu có
            if (url.StartsWith("/"))
            {
                url = url.Substring(1);
            }

            // Map path từ URL
            string filePath = Server.MapPath("~/" + url);

            // Kiểm tra file có tồn tại không
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound("File không tồn tại.");
            }

            // Lấy extension để xác định loại file
            string extension = Path.GetExtension(filePath).ToLower();
            string contentType = MimeMapping.GetMimeMapping(filePath);

            // Trả về file với content type phù hợp
            return File(filePath, contentType);
        }
    }
}

