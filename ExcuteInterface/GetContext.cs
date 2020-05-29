using System;
using System.Threading;
using System.Threading.Tasks;
using AutoSchedule.Common;
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
    public class JobLogger
    {
        public readonly  ILogger<JobLogger> _logger;
        public JobLogger(ILogger<JobLogger> logger)
        {
            _logger = logger;
        }
        public async Task WriteLogAsync(LogType logType, string name, string info)
        {
            switch (logType)
            {
                case LogType.Info:
                    await Task.Run(() => { _logger.LogInformation("{EventId}:{result}", name, info + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString()); });
                    break;
                //case LogType.Debug:
                //    await Task.Run(() => { _logger.LogDebug("{EventId}:{result}", name, info + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString()); });
                //    break;
                case LogType.Warning:
                    await Task.Run(() => { _logger.LogWarning("{EventId}:{result}", name, info + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString()); });
                    break;
                case LogType.Error:
                    await Task.Run(() => { _logger.LogError("{EventId}:{result}", name, info); });
                    break;
                default:
                    break;
            }
        }
        public void  WriteLog(LogType logType, string name, string info)
        {
            switch (logType)
            {
                case LogType.Info:
                     _logger.LogInformation("{EventId}:{result}", name, info + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString());
                    break;
                //case LogType.Debug:
                //    await Task.Run(() => { _logger.LogDebug("{EventId}:{result}", name, info + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString()); });
                //    break;
                case LogType.Warning:
                    _logger.LogWarning("{EventId}:{result}", name, info + ",ThreadID:" + Thread.CurrentThread.ManagedThreadId.ToString());
                    break;
                case LogType.Error:
                     _logger.LogError("{EventId}:{result}", name, info);
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