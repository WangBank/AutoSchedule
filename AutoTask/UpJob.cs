using AutoTask.Models;
using BankDbHelper;
using ExcuteInterface;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
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
        bool disposed = false;
        SafeHandle handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }
            // Free any unmanaged objects here.
            //
            disposed = true;
        }
        public void Dispose()
        {
            // Dispose of unmanaged resources. 
            Dispose(true);

            // Suppress finalization. 
            // 默认Dispose方法会清理所有对象（包括托管），所以GC不再需要调用对象重写的Finalize（析构函数）。因此调用GC.SuppressFinalize可以防止GC调用Finalize（防止调用两次析构函数）。
            GC.SuppressFinalize(this);
        }
        public async Task<int> ExecJob(JobPara jobPara, List<Datas> dsData)
        {
            int result=0;
            Task<int> testInt;
            switch (jobPara.jobCode)
            {
                case "test":
                    _jobLogger.WriteLog(LogType.Error, "test", $"王振1,当前线程id{Thread.CurrentThread.ManagedThreadId}");
                    _ = await TestAsync(jobPara,dsData);
                    _jobLogger.WriteLog(LogType.Error, "test", $"王振2,当前线程id{Thread.CurrentThread.ManagedThreadId}");
                    break;
                default:
                    break;
            }
            return  result;
        }

        public async Task<int> TestAsync(JobPara jobPara, List<Datas> dsData)
        {
            var logs = new List<Logs>();
            for (int i = 0; i < 100000; i++)
            {
                logs.Add(new Logs
                {
                    Application = "test",
                    EventId = "test",
                    Level = "test",
                    Logger = "freesqlLogger",
                    Message = i.ToString() + "当前线程id" + Thread.CurrentThread.ManagedThreadId,
                    TimestampUtc = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

                });
            }
            _ = await _jobLogger._SqlLiteContext.Insert(logs).ExecuteAffrowsAsync();
            _jobLogger.Dispose();
            logs = null;
            //var logs = new List<Logs>();
            //var log = new Logs();
            //for (int i = 0; i < 100000; i++)
            //{
            //    logs.Add(new Logs
            //    {
            //        Application = "test",
            //        EventId = "test",
            //        Level = "test",
            //        Logger = "freesqlLogger",
            //        Message = i.ToString() + "当前线程id" + Thread.CurrentThread.ManagedThreadId,
            //        TimestampUtc = DateTime.Now.ToString("yyyy-MM-dd HH:MM:SS")

            //    });
            //    _ = await _jobLogger.GetLogContext().Insert(logs).ExecuteAffrowsAsync();
            //    // _jobLogger.WriteLog(LogType.Error, "test", "error日志" + i + ",当前线程id" + Thread.CurrentThread.ManagedThreadId);

            //}
            //_ = await _jobLogger.GetLogContext().Insert(logs).ExecuteAffrowsAsync();
            return 0;
        }
    }
}
