using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{
    public class HelloJob : IJob
    {
        private readonly ILogger<HelloJob> _logger;
        private readonly SqlLiteContext _SqlLiteContext;
        public SqlHelper SqlHelper;
        public HelloJob(ILogger<HelloJob> logger,SqlLiteContext SqlLiteContext)
        {
            _logger = logger;
            _SqlLiteContext = SqlLiteContext;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobKey key = context.JobDetail.Key;
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                string jobSays = dataMap.GetString("guid");
                var taskPlan = await _SqlLiteContext.TaskPlan.AsNoTracking().SingleOrDefaultAsync(o => o.GUID == jobSays);
                await Console.Out.WriteLineAsync("任务名称: " + taskPlan.Name + "正在执行！");

                var TaskPlan = await _SqlLiteContext.TaskPlan.AsNoTracking().Where(o => o.GUID == jobSays).FirstOrDefaultAsync();
                string orgCode = TaskPlan.OrgCode;
                var OrgSetting = await _SqlLiteContext.OrgSetting.AsNoTracking().Where(o => o.CODE == orgCode).FirstOrDefaultAsync();
                string connectString = OrgSetting.ConnectingString;
                string orgType = OrgSetting.DBType;

                SqlHelper = new SqlHelper(connectString);
                 //List<DataSource> dataSources = new List<DataSource>();
                 var taskPlanList = await _SqlLiteContext.TaskPlanRelation.AsNoTracking().Where(o => o.TaskPlanGuid == jobSays).ToListAsync();
                for (int i = 0; i < taskPlanList.Count; i++)
                {
                   var dataSource =  await _SqlLiteContext.OpenSql.AsNoTracking().FirstAsync(o => o.GUID == taskPlanList[i].OpenSqlGuid);
                   //执行sql语句 
                   
                }

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("错误信息" + ex.Message);
                throw;
            }
            
            //CommonHelper.HttpPostAsync(,)
        }
    }
}
