using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AutoSchedule.Common;
using FreeSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExcuteInterface
{
    public class GetContext:Singleton<GetContext>
    {
        public  IServiceProvider ServiceProvider { get; set; }
        public  T GetService<T>()
        {
           return ServiceProvider.GetService<T>();
        }

    }
    
    public class JobLogger:IDisposable
    {
        private static readonly object syslock = new object();
        public IFreeSql _SqlLiteContext;
        bool disposed = false;
        SafeHandle handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, true);
        //public IFreeSql GetLogContext()
        //{
        //    if (_SqlLiteContext == null)
        //    {
        //        //lock (syslock)
        //        //{
        //        //    if (_SqlLiteContext == null)
        //        //    {
        //        //        string SqlLiteConn = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Data Source=Db/LogData.dll;" : "Data Source=Db\\LogData.dll;";
        //        //        _SqlLiteContext = new FreeSqlBuilder()
        //        //        .UseConnectionFactory(FreeSql.DataType.Sqlite, () => new System.Data.SQLite.SQLiteConnection(SqlLiteConn))
        //        //         .UseAutoSyncStructure(false)
        //        //         .Build();
        //        //    }
        //        //}


                       
        //    }

        //    return _SqlLiteContext;
        //}
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
        public readonly  ILogger<JobLogger> _logger;
        public JobLogger(ILogger<JobLogger> logger)
        {
             string SqlLiteConn = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Data Source=Db/LogData.dll;" : "Data Source=Db\\LogData.dll;";
            _SqlLiteContext = new FreeSqlBuilder()
            .UseConnectionFactory(FreeSql.DataType.Sqlite, () => new System.Data.SQLite.SQLiteConnection(SqlLiteConn))
             .UseAutoSyncStructure(false)
             .Build();
            _logger = logger;
        }
        public void  WriteLog(LogType logType, string name, string info)
        {
            switch (logType)
            {
                case LogType.Info:
                     _logger.LogInformation("{EventId}:{result}", name, info + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString());
                    break;
                case LogType.Warning:
                    _logger.LogWarning("{EventId}:{result}", name, info + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString());
                    break;
                case LogType.Error:
                     _logger.LogError("{EventId}:{result}", name, info + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString());
                    break;
                default:
                    break;
            }
        }
    }

    public enum LogType
    {
        /// <summary>
        /// 记录一般信息，比如json入参、返回数据等
        /// </summary>
        Info,
        ///// <summary>
        ///// 调试时使用
        ///// </summary>
        //Debug,
        /// <summary>
        /// 可能会出现问题的
        /// </summary>
        Warning,
        /// <summary>
        /// 错误
        /// </summary>
        Error
    }
}