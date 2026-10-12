using log4net;
using Quartz;
using System.Threading.Tasks;

namespace Dashboard.Common
{
    [DisallowConcurrentExecution]
    public class NhacNhoCuocHopTruoc30PhutJob : IJob
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(typeof(NhacNhoCuocHopTruoc30PhutJob));

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.CompletedTask;
        }
    }
}
