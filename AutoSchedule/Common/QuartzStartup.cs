using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{
    public class QuartzStartup
    {
        private static Dictionary<string, JobKey> jobKeys = new Dictionary<string, JobKey>();
        private readonly ILogger<QuartzStartup> _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        private readonly IJobFactory _iocJobfactory;
        private IJobDetail jobDetail;

        public QuartzStartup(IJobFactory iocJobfactory, ILogger<QuartzStartup> logger, ISchedulerFactory schedulerFactory)
        {
            this._logger = logger;
            //1、声明一个调度工厂
            this._schedulerFactory = schedulerFactory;
            this._iocJobfactory = iocJobfactory;
        }

        public async Task<string> Start(List<string> param)
        {
            try
            {
                if (param.Count > 1 && _scheduler != null)
                {
                    await Stop();
                }
                for (int i = 0; i < param.Count; i++)
                {
                    //2、通过调度工厂获得调度器
                    _scheduler = await _schedulerFactory.GetScheduler();
                    _scheduler.JobFactory = this._iocJobfactory;
                    //  替换默认工厂
                    //3、开启调度器
                    _logger.LogInformation("定时任务启动");
                    await _scheduler.Start();
                    //4、创建一个触发器
                    var trigger = TriggerBuilder.Create()
                                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(2).RepeatForever())//每两秒执行一次
                                    .Build();

                    //5、创建任务
                    jobDetail = JobBuilder.Create<HelloJob>()
                                    .WithIdentity(param[i].ToString(), "group")
                                    .UsingJobData("guid", param[i].ToString())
                                    .Build();

                    jobKeys.Add(param[i].ToString(), jobDetail.Key);

                    //6、将触发器和任务器绑定到调度器中

                    await _scheduler.ScheduleJob(jobDetail, trigger);
                }

                //return await Task.FromResult("开启定时调度任务" + ";\r\nJobType:" +  jobDetail.JobType + ";\r\nKey:" +  jobDetail.Key + ";\r\nGetType():" + jobDetail.GetType());
                return await Task.FromResult("开启定时调度任务");
            }
            catch (Exception ex)
            {
                return await Task.FromResult("开启失败，失败原因:" + ex.Message);
            }
        }

        public async Task<string> Stop(string param = "")
        {
            if (jobKeys.Count == 0)
            {
                return "还未开始，怎谈得上关闭呢？";
            }
            if (param != "")
            {
                if (jobKeys.ContainsKey(param))
                {
                    await _scheduler.DeleteJob(jobKeys[param]);
                    jobKeys.Remove(param);
                    return $"定时任务已结束,当前还运行任务数{jobKeys.Count.ToString()}";
                }
                else
                {
                    return "还未开始，怎谈得上关闭呢？";
                }
            }

            await _scheduler.Shutdown();
            jobKeys.Clear();
            return "定时任务已结束";
        }
    }
}