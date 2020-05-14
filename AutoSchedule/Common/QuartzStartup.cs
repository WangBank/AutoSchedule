using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.MessageModel;
using AutoSchedule.Dtos.Models;
using FreeSql;
using Jdwl.Api.Message;
using Jdwl.Api.Message.Protocol;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewLife.Caching;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{
    public class QuartzStartup
    {
        private TaskFactory taskFactory = new TaskFactory();
        private static Dictionary<string, JobKey> jobKeys = new Dictionary<string, JobKey>();
        private readonly ILogger<QuartzStartup> _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        public readonly IJobFactory _iocJobfactory;
        private IJobDetail jobDetail;
        public Redis rds;
        private IConfiguration _Configuration;
        private string RedisConnectstring;
        private readonly int RedisDb;
        private IFreeSql _sqliteFSql;
        public QuartzStartup(IJobFactory iocJobfactory, ILogger<QuartzStartup> logger, ISchedulerFactory schedulerFactory, IConfiguration configuration, IServiceProvider serviceProvider, FreeSqlFactory freeSqlFactory)
        {
            string SqlLiteConn = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                SqlLiteConn = configuration.GetConnectionString("SqlLiteLinux");
            }
            else
            {
                SqlLiteConn = configuration.GetConnectionString("SqlLiteWin");
            }
          
            this._logger = logger;
            //1、声明一个调度工厂
            this._schedulerFactory = schedulerFactory;
            _Configuration = configuration;
            this._iocJobfactory = iocJobfactory;
            RedisConnectstring = _Configuration.GetConnectionString("RedisConnectstring");
            RedisDb = int.Parse(_Configuration.GetConnectionString("RedisDb"));
            string redispwd = _Configuration.GetConnectionString("RedisPwd");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                rds = new Redis(RedisConnectstring, redispwd, RedisDb);
            }
            _sqliteFSql = freeSqlFactory.GetBaseSqlLite();
            _sqliteFSql.Update<TaskPlan>()
            .Set(a => a.Status, "0")
            .Where(a => a.Status == "1")
            .ExecuteAffrows();
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

                    //计算出事件的秒数
                    var ts = await _sqliteFSql.Select<TaskPlan>().Where(o => o.GUID == param[i]).FirstAsync();
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
                    _logger.LogInformation("定时任务({EventId})启动", ts.Name);
                    await _scheduler.Start();
                    //4、创建一个触发器
                    var trigger = TriggerBuilder.Create()
                                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(Second).RepeatForever())
                                    .Build();
                    //5、创建任务 0是dll 模式 1是api模式
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        if (rds.ContainsKey(param[i].ToString()))
                        {
                            return await Task.FromResult($"已经开启过任务{ts.Name}不允许重复开启！");
                        }

                        if (ts.WorkType == "0")
                        {
                            jobDetail = JobBuilder.Create<AutoTaskJobDll>()
                                       .WithIdentity(param[i].ToString(), "group")
                                       .UsingJobData("guid", param[i].ToString())
                                       .Build();

                        }
                        else
                        {
                            jobDetail = JobBuilder.Create<AutoTaskJob>()
                                       .WithIdentity(param[i].ToString(), "group")
                                       .UsingJobData("guid", param[i].ToString())
                                       .Build();
                        }

                        rds.Set(param[i].ToString(), jobDetail.Key);
                    }
                    else
                    {
                        if (ts.Status == "1" || jobKeys.ContainsKey(param[i].ToString()))
                        {
                            return await Task.FromResult($"已经开启过任务{ts.Name}不允许重复开启！");
                        }
                        else
                        {

                            if (ts.WorkType == "0")
                            {
                                jobDetail = JobBuilder.Create<AutoTaskJobDll>()
                                           .WithIdentity(param[i].ToString(), "group")
                                           .UsingJobData("guid", param[i].ToString())
                                           .Build();
                            }
                            else
                            {
                                jobDetail = JobBuilder.Create<AutoTaskJob>()
                                           .WithIdentity(param[i].ToString(), "group")
                                           .UsingJobData("guid", param[i].ToString())
                                           .Build();
                            }

                            ts.Status = "1";

                            _sqliteFSql.Update<TaskPlan>()
                                .Set(a => a.Status, ts.Status)
              .Where(a => a.GUID == ts.GUID)
              .ExecuteAffrows();
                            jobKeys.Add(param[i].ToString(), jobDetail.Key);
                        }

                    }
                    //6、将触发器和任务器绑定到调度器中 

                    await _scheduler.ScheduleJob(jobDetail, trigger);

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
            try
            {
                var taskPlands = await _sqliteFSql.Select<TaskPlan>().ToListAsync();

                if (!string.IsNullOrEmpty(param))
                {
                    var tk = await _sqliteFSql.Select<TaskPlan>().Where(o => o.GUID == param).FirstAsync();
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        if (rds.ContainsKey(param))
                        {
                            if (await _scheduler.DeleteJob(rds.Get<JobKey>(param)))
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
                    else
                    {
                        if (tk.Status == "1" && jobKeys.ContainsKey(param))
                        {
                            if (await _scheduler.DeleteJob(jobKeys.GetValueOrDefault(param)))
                            {
                                jobKeys.Remove(param);
                                tk.Status = "0";
                                _sqliteFSql.Update<TaskPlan>()
             .Set(a => a.Status, tk.Status)
             .Where(a => a.GUID == tk.GUID)
             .ExecuteAffrows();
                                return await Task.FromResult($"定时任务({ tk.Name})已结束");
                            }

                        }
                        else
                        {
                            tk.Status = "0";
                            _sqliteFSql.Update<TaskPlan>()
              .Set(a => a.Status, tk.Status)
              .Where(a => a.GUID == tk.GUID)
              .ExecuteAffrows();
                            return "还未开始，怎谈得上关闭呢？";
                        }

                    }

                }
                await _scheduler.Shutdown();

                if (jobKeys.Count==0&& taskPlands.Where(o=>o.Status=="1").Count()==0)
                {
                    return "还未开始，怎谈得上关闭呢？";

                }
                for (int i = 0; i < taskPlands.Count; i++)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        rds.Remove(taskPlands[i].GUID);
                    }
                    else
                    {
                        jobKeys.Remove(taskPlands[i].GUID);
                        taskPlands[i].Status = "0";
                        _sqliteFSql.Update<TaskPlan>()
             .Set(a => a.Status, taskPlands[i].Status)
             .Where(a => a.GUID == taskPlands[i].GUID)
             .ExecuteAffrows();
                    }
                }

                return "定时任务已全部结束";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }

    }
}