using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace AutoSchedule.Common
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection ConfigServies(this IServiceCollection services)
        {

            services.AddTransient<AutoTaskJob>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();//注册ISchedulerFactory的实例。
            services.AddSingleton<IJobFactory, IOCJobFactory>();
            services.AddSingleton<QuartzStartup>();
            return services;
        }
    }
}
