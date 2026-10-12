using log4net;
using Quartz;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dashboard.Common
{
    [DisallowConcurrentExecution]
    public class ScheduledUrlJob : IJob
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(typeof(ScheduledUrlJob));

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var url = BienDungChung.ScheduledUrl;
                if (string.IsNullOrEmpty(url))
                {
                    log.Warn("ScheduledUrl không được cấu hình trong Web.config");
                    return;
                }

                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(5);
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        log.Info($"Đã gọi URL thành công: {url} - Status: {response.StatusCode}");
                    }
                    else
                    {
                        log.Warn($"Gọi URL không thành công: {url} - Status: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi gọi URL: {ex.Message}", ex);
            }
        }
    }
}

