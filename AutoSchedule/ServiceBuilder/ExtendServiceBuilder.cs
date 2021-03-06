﻿using AspNetCoreRateLimit;
using AutoSchedule.Common;
using AutoSchedule.Dtos.Data;
using ExcuteInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.ServiceBuilder
{
    public class ExtendServiceBuilder
    {
        public IServiceCollection _services { get; set; }
        public ExtendServiceBuilder(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// 自定义
        /// </summary>
        public void UseIOCJobFactory()
        {
            _services.AddSingleton<IJobFactory, IOCJobFactory>();
            
        }

        public void UseAutoTaskJob()
        {
            _services.AddTransient<AutoTaskJob>();
            _services.AddTransient<AutoTaskJobDll>();
        }

        public void UseISchedulerFactory()
        {
            _services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        }

        public void UseSingleCommon()
        {
            _services.AddSingleton<FreeSqlFactory>();
            _services.AddSingleton<QuartzStartup>();
            
            _services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            _services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }


    }
}
