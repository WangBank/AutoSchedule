using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExcuteInterface
{
    public  class GetContext
    {
        private static ILogger _logger;
        private static readonly object locker = new object();
            

        public static IServiceProvider ServiceProvider { get; set; }
        public static T GetService<T>()
        {
           return ServiceProvider.GetService<T>();
        }

        public async static Task WriteLogAsync(LogType logType,string name,string info)    
        {
            
            if (_logger == null)
            {
                lock (locker)
                {
                    if (_logger == null)
                    {
                        _logger = new JobLogger()._logger;
                    }
                }
            }

            switch (logType)
            {
                case LogType.Info:
                    await Task.Run(()=> { _logger.LogInformation("{EventId}:{result}", name, info + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString()); });
                    break;
                //case LogType.Debug:
                //    await Task.Run(() => { _logger.LogDebug("{EventId}:{result}", name, info + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString()); });
                //    break;
                case LogType.Warning:
                    await Task.Run(() => { _logger.LogWarning("{EventId}:{result}", name, info + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString()); });
                    break;
                case LogType.Error:
                    await Task.Run(() => { _logger.LogError("{EventId}:{result}", name, info + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString()); });
                    break;
                default:
                    break;
            }
        }
    }

    public class JobLogger
    {
        public ILogger _logger;
        public JobLogger()
        {
            _logger = GetContext.GetService<ILoggerProvider>().CreateLogger("JobLogger");
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