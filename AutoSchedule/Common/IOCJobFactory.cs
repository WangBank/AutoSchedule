using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;

namespace AutoSchedule.Common
{
    public class IOCJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public IOCJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}