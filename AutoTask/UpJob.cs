using AutoTask.Model;
using BankDbHelper;
using ExcuteInterface;
using Jdwl.Api;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenApiSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoTask
{
    public class UpJob : IUpJob
    {
        public UpJob()
        {
        }

        public async Task<int> ExecJob(JobPara jobPara, List<Datas> dsData)
        {
            int result=0;
            switch (jobPara.jobCode)
            {
                case "test":
                    result =await TestAsync(jobPara,dsData);
                    break;
                default:
                    break;
            }
            return  result;
        }

        public async Task<int> TestAsync(JobPara jobPara, List<Datas> dsData)
        {

            for (int i = 0; i < 100; i++)
            {
                await GetContext.WriteLogAsync(LogType.Info, "test", $"info日志{i}");
                await GetContext.WriteLogAsync(LogType.Error, "test", $"error日志{i}");
                await GetContext.WriteLogAsync(LogType.Warning, "test", $"waring日志{i}");
            }

            return 0;
        }
    }
}
