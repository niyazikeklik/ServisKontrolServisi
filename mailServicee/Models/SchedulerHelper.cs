using Quartz;
using Quartz.Impl;

namespace mailServicee
{
    public class SchedulerHelper
    {
        public static async void SchedulerSetup()
        {
            var _scheduler = await new StdSchedulerFactory().GetScheduler();
            await _scheduler.Start();

            var showDateTimeJob = JobBuilder.Create<CheckServiceStatus>()
                                            .WithIdentity("CheckServiceStatus")
                                            .Build();
            ITrigger trigger = TriggerBuilder.Create()
                                            .WithIdentity("CheckServiceStatus")
                                            .StartNow()
                                            .WithSimpleSchedule(builder => builder
                                            .WithIntervalInSeconds(AppSettings.Instance.ProgramDonguSuresiSaniye)
                                            .RepeatForever())
                                            .Build();



            if (AppSettings.Instance.IsLogCheckActive)
            {
                var checkRecordLogJobs = JobBuilder.Create<CheckRecordLog>()
                .WithIdentity("CheckRecordLog")
                .Build();
                var trigger2 = TriggerBuilder.Create()
                    .WithIdentity("CheckRecordLog")
                    .StartNow()
                    .WithSimpleSchedule(builder => builder.WithIntervalInSeconds(AppSettings.Instance.ProgramDonguSuresiSaniye + 10).RepeatForever()) 
                    .Build();
                var result2 = await _scheduler.ScheduleJob(checkRecordLogJobs, trigger2);
            }


            var result = await _scheduler.ScheduleJob(showDateTimeJob, trigger);

        }
    }
}
