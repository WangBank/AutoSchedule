using AutoSchedule.Dtos.Data;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return _scope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();

        }
    }
}
