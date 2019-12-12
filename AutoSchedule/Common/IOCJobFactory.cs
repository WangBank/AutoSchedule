using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;

namespace AutoSchedule.Common
{
    public class IOCJobFactory : IJobFactory
    {
        //private readonly IServiceProvider _serviceProvider;
        protected readonly IServiceScope _scope;

        public IOCJobFactory(IServiceProvider serviceProvider)
        {
            //_serviceProvider = serviceProvider;
            _scope = serviceProvider.CreateScope();
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            //return _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
            //从当前scope中获取
              return _scope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
            //从core中默认的调度器中获取
            //return GetContext.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}