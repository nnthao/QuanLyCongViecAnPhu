using log4net;
using Quartz;
using Quartz.Impl;
using System;

namespace Dashboard.Common
{
    public class QuartzSchedulerHelper
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(typeof(QuartzSchedulerHelper));
        private static IScheduler _scheduler;

        public static void Start()
        {
            try
            {
                var factory = new StdSchedulerFactory();
                _scheduler = factory.GetScheduler().Result;
                _scheduler.Start().Wait();

                var intervalMinutes = BienDungChung.ScheduledIntervalMinutes;
                if (intervalMinutes <= 0)
                {
                    intervalMinutes = 10;
                }

                ScheduleWakeUpJob(intervalMinutes);
                ScheduleThongKeCongViecHangNgayJob();

                log.Info($"Quartz Scheduler da duoc khoi dong thanh cong voi interval: {intervalMinutes} phut");
            }
            catch (Exception ex)
            {
                log.Error($"Loi khi khoi dong Quartz Scheduler: {ex.Message}", ex);
            }
        }

        public static void Stop()
        {
            try
            {
                if (_scheduler != null && !_scheduler.IsShutdown)
                {
                    _scheduler.Shutdown().Wait();
                    log.Info("Quartz Scheduler da duoc dung");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Loi khi dung Quartz Scheduler: {ex.Message}", ex);
            }
        }

        private static void ScheduleWakeUpJob(int intervalMinutes)
        {
            try
            {
                var job = JobBuilder.Create<ScheduledUrlJob>()
                    .WithIdentity("ScheduledUrlJob", "DefaultGroup")
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("ScheduledUrlTrigger", "DefaultGroup")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(intervalMinutes)
                        .RepeatForever())
                    .Build();

                _scheduler.ScheduleJob(job, trigger).Wait();
                log.Info($"Da dang ky ScheduledUrlJob chay moi {intervalMinutes} phut");
            }
            catch (Exception ex)
            {
                log.Error($"Loi khi dang ky ScheduledUrlJob: {ex.Message}", ex);
            }
        }

        private static void ScheduleThongKeCongViecHangNgayJob()
        {
            try
            {
                var thongKeJob = JobBuilder.Create<ThongKeCongViecHangNgayJob>()
                    .WithIdentity("ThongKeCongViecHangNgayJob", "DefaultGroup")
                    .Build();

                var thongKeTrigger = TriggerBuilder.Create()
                    .WithIdentity("ThongKeCongViecHangNgayTrigger", "DefaultGroup")
                    .WithCronSchedule("0 30 7,13 * * ?")
                    .Build();

                _scheduler.ScheduleJob(thongKeJob, thongKeTrigger).Wait();
                log.Info("Da dang ky ThongKeCongViecHangNgayJob chay vao 7h30 va 13h30 hang ngay");
            }
            catch (Exception ex)
            {
                log.Error($"Loi khi dang ky ThongKeCongViecHangNgayJob: {ex.Message}", ex);
            }
        }
    }
}
