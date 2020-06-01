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
            Task<int> testInt;
            switch (jobPara.jobCode)
            {
                case "test":
                    _jobLogger.WriteLog(LogType.Error, "test", $"王振1,当前线程id{Thread.CurrentThread.ManagedThreadId}");
                    testInt = TestAsync(jobPara,dsData);
                    _jobLogger.WriteLog(LogType.Error, "test", $"王振2,当前线程id{Thread.CurrentThread.ManagedThreadId}");
                    break;
                default:
                    break;
            }
            return  result;
        }

        public async Task<int> TestAsync(JobPara jobPara, List<Datas> dsData)
        {
            //await Task.Run(()=> {
            //    for (int i = 0; i < 100000; i++)
            //    {
            //        _jobLogger.WriteLog(LogType.Error, "test", $"error日志{i},当前线程id{Thread.CurrentThread.ManagedThreadId}");
            //    }
            //});
                
            //    for (int i = 0; i < 99; i++)
            //    {
            //     await _jobLogger.WriteLogAsync(LogType.Error, "test", $"error日志{i}");

            //    //await _jobLogger.WriteLogAsync(LogType.Error, "test", $"error日志{i}");

            //    // await Task.Run(() => { System.Console.WriteLine(i + ",fsafsdfasd," + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString()); });
            //    //await Task.Run(() => { _ = GetContext.WriteLogAsync(LogType.Error, "test", $"error日志{i}"); });
            //    //await GetContext.WriteLogAsync(LogType.Info, "test", $"info日志{i}");
            //    //await GetContext.WriteLogAsync(LogType.Error, "test", $"error日志{i}");
            //    //await GetContext.WriteLogAsync(LogType.Warning, "test", $"waring日志{i}");
            //}

            return 0;
        }
    }
}
