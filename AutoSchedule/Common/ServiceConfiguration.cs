using AutoSchedule.ServiceBuilder;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;

namespace AutoSchedule.Common
{
    public static class ServiceConfiguration
    {
        //配置入口
        public static void AddExtendService(this IServiceCollection services, Action<ExtendServiceBuilder> configure)
        {
            var builder = new ExtendServiceBuilder(services);
            configure(builder);
        }
    }

   
}