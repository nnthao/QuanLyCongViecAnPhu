using log4net;
using Quartz;
using System.Threading.Tasks;

namespace Dashboard.Common
{
    [DisallowConcurrentExecution]
    public class NhacNhoCuocHopJob : IJob
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(typeof(NhacNhoCuocHopJob));

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.CompletedTask;
        }
    }
}
