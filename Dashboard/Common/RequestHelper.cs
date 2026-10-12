using Dashboard.Models.ViewModel;
using System;
using System.Web;
using UAParser;

namespace Dashboard.Common
{
    public class RequestHelper
    {
        public static string GetIp(HttpRequestBase request)
        {
            try
            {
                var ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                {
                    ip = request.ServerVariables["REMOTE_ADDR"];
                }

                return ip;
            }
            catch (Exception)
            {

                return "";
            }

        }

        public static string GetUserAgent(HttpRequestBase request)
        {
            try
            {
                var userAgent = request.ServerVariables["HTTP_USER_AGENT"];
                if (!string.IsNullOrEmpty(userAgent))
                {
                    return userAgent;
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetBrowser(string userAgent)
        {
            if (!string.IsNullOrEmpty(userAgent))
            {
                var uaParser = Parser.GetDefault();
                var client = uaParser.Parse(userAgent);
                if (client.UA != null)
                {
                    return string.Format("{0} {1}.{2}.{3}", client.UA.Family,
                        client.UA.Major, client.UA.Minor, client.UA.Patch);
                }
            }

            return "";
        }
    }
}