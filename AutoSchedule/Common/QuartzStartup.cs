using AutoSchedule.Dtos.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewLife.Caching;
using NLog;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{
    public class QuartzStartup
    {
        private static Dictionary<string, JobKey> jobKeys = new Dictionary<string, JobKey>();
        private ILogger<QuartzStartup> _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        public readonly IJobFactory _iocJobfactory;
        private IJobDetail jobDetail;
        private SqlLiteContext _SqlLiteContext;
        public Redis rds;
        private IConfiguration _Configuration;
        private string RedisConnectstring;
        private int RedisDb;

        public QuartzStartup(IJobFactory iocJobfactory, ILogger<QuartzStartup> logger, ISchedulerFactory schedulerFactory, IConfiguration configuration)
        {
            this._logger = logger;
            //1、声明一个调度工厂
            this._schedulerFactory = schedulerFactory;
            _Configuration = configuration;
            this._iocJobfactory = iocJobfactory;
            RedisConnectstring = _Configuration.GetConnectionString("RedisConnectstring");
            RedisDb = int.Parse(_Configuration.GetConnectionString("RedisDb"));
            rds = Redis.Create(RedisConnectstring, RedisDb);
        }

        public async Task<string> Start(List<string> param)
        {
            try
            {
                int Second = 0;
                if (param.Count > 1 && _scheduler != null)
                {
                    await Stop();
                }
                for (int i = 0; i < param.Count; i++)
                {
                    _SqlLiteContext = (SqlLiteContext)GetContext.ServiceProvider.GetService(typeof(SqlLiteContext));
                    //计算出事件的秒数
                    var ts = await _SqlLiteContext.TaskPlan.AsNoTracking().FirstOrDefaultAsync(o => o.GUID == param[i].ToString());
                    switch (ts.FrequencyType)
                    {
                        case "0":
                            Second = int.Parse(ts.Frequency);
                            break;

                        case "1":
                            Second = int.Parse(ts.Frequency) * 60;
                            break;

                        case "2":
                            Second = int.Parse(ts.Frequency) * 3600;
                            break;

                        default:
                            break;
                    }

                    //2、通过调度工厂获得调度器
                    _scheduler = await _schedulerFactory.GetScheduler();
                    _scheduler.JobFactory = this._iocJobfactory;
                    //  替换默认工厂
                    //3、开启调度器
                    _logger.LogInformation("定时任务({EventId})启动", param[i].ToString());
                    await _scheduler.Start();
                    //4、创建一个触发器
                    var trigger = TriggerBuilder.Create()
                                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(Second).RepeatForever())//每两秒执行一次
                                    .Build();
                    //5、创建任务
                    jobDetail = JobBuilder.Create<AutoTaskJob>()
                                    .WithIdentity(param[i].ToString(), "group")
                                    .UsingJobData("guid", param[i].ToString())
                                    .Build();
                    if (rds.ContainsKey(param[i].ToString()))
                    {
                        return await Task.FromResult($"已经开启过任务{ts.Name}不允许重复开启！");
                    }
                    rds.Set(param[i].ToString(), jobDetail.Key);
                    //6、将触发器和任务器绑定到调度器中 

                    await _scheduler.ScheduleJob(jobDetail, trigger);

                    //改变数据库中的状态
                }

                return await Task.FromResult("0");
            }
            catch (Exception ex)
            {
                _logger.LogError($"开启失败，失败原因:{ex.Message}");
                return await Task.FromResult($"开启失败，失败原因:{ex.Message}");
            }
        }

        public async Task<string> Stop(string param = "")
        {
            var taskPlands = await _SqlLiteContext.TaskPlan.AsNoTracking().ToListAsync();
            var tk = await _SqlLiteContext.TaskPlan.AsNoTracking().Where(o => o.GUID == param).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(param))
            {
                if (rds.ContainsKey(param))
                {
                    
                    var closeResult = _scheduler.DeleteJob(rds.Get<JobKey>(param));
                    if (closeResult.Result)
                    {
                        rds.Remove(param);
                        return $"定时任务({ tk.Name})已结束";
                    }
                   
                }
                else
                {
                    return "还未开始，怎谈得上关闭呢？";
                }
            }
            await _scheduler.Shutdown();

            for (int i = 0; i < taskPlands.Count; i++)
            {
                rds.Remove(taskPlands[i].GUID);
            }
            return "定时任务已全部结束";
        }
    }
}