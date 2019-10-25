using AutoSchedule.Dtos.Data;
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
        public HelloJob(ILogger<HelloJob> logger,SqlLiteContext SqlLiteContext)
        {
            _logger = logger;
            _SqlLiteContext = SqlLiteContext;
        }
        public async Task Execute(IJobExecutionContext context)
        {

            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string jobSays = dataMap.GetString("guid");
            var taskPlan = await _SqlLiteContext.TaskPlan.AsNoTracking().SingleOrDefaultAsync(o => o.GUID == jobSays);
            await Console.Out.WriteLineAsync("任务名称: "+ taskPlan.Name +"正在执行！");
        }
    }
}
