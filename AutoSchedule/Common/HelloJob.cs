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
        //  private readonly ICache _cache;
        public HelloJob(ILogger<HelloJob> logger)
        {
            //_cache = cache;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string jobSays = dataMap.GetString("guid");
            await Console.Out.WriteLineAsync("当前执行任务: " + key + " of DumbJob says: " + jobSays);
        }
    }
}
