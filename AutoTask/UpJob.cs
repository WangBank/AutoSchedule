using BankDbHelper;
using ExcuteInterface;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTask
{
    public class UpJob : IUpJob
    {
        JobLogger _jobLogger;
        public UpJob(JobLogger jobLogger)
        {
            _jobLogger = jobLogger;
        }
        public async Task<int> ExecJob(JobPara jobPara, List<Datas> dsData)
        {
            int result=0;
            switch (jobPara.jobCode)
            {
                case "test":
                    result =await TestAsync(jobPara,dsData);
                    break;
                default:
                    break;
            }
            return  result;
        }

        public async Task<int> TestAsync(JobPara jobPara, List<Datas> dsData)
        {

                for (int i = 0; i < 99; i++)
                {
                 await _jobLogger.WriteLogAsync(LogType.Error, "test", $"error日志{i}");

                //await _jobLogger.WriteLogAsync(LogType.Error, "test", $"error日志{i}");

                // await Task.Run(() => { System.Console.WriteLine(i + ",fsafsdfasd," + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString()); });
                //await Task.Run(() => { _ = GetContext.WriteLogAsync(LogType.Error, "test", $"error日志{i}"); });
                //await GetContext.WriteLogAsync(LogType.Info, "test", $"info日志{i}");
                //await GetContext.WriteLogAsync(LogType.Error, "test", $"error日志{i}");
                //await GetContext.WriteLogAsync(LogType.Warning, "test", $"waring日志{i}");
            }
           
            return 0;
        }
    }
}
