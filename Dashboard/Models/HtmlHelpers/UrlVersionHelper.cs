using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;


namespace Dashboard.Models.HtmlHelpers
{
    public static class UrlVersionHelper
    {
        private static Cache cache = HttpContext.Current.Cache;

        public static IHtmlString VersionedContent(this UrlHelper urlHelper, string contentPath)
        {
            string key = "ver_" + contentPath;
            string version = cache[key] as string;

            if (version == null)
            {
                var absolutePath = HttpContext.Current.Server.MapPath(contentPath);
                if (File.Exists(absolutePath))
                {
                    var lastWrite = File.GetLastWriteTime(absolutePath);
                    version = lastWrite.Ticks.ToString();
                    cache.Insert(
                         key,
                         version,
                         null,
                         Cache.NoAbsoluteExpiration,
                         Cache.NoSlidingExpiration
                     );
                    //cache.Set(key, version, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(10) });
                }
                else
                {
                    version = "0";
                }
            }

            return new HtmlString(urlHelper.Content(contentPath) + "?v=" + version);
        }
    }
}