using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using AutoSchedule.Dtos.RequestIn;
using AutoSchedule.TaskDlls;
using BankDbHelper;
using ExcuteInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{

    /// <summary>
    /// 调用dll方式发送数据
    /// </summary>
    public class AutoTaskJobDll : IJob
    {
        private ILogger<AutoTaskJobDll> _logger;
        public IHttpClientFactory _httpClientFactory;
        public FreeSqlFactory _freeSqlFactory;
        public AutoTaskJobDll(ILogger<AutoTaskJobDll> logger, IHttpClientFactory httpClientFactory, FreeSqlFactory freeSqlFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _freeSqlFactory = freeSqlFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {

            var _SqlLiteContext = _freeSqlFactory.GetBaseSqlLite();
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string jobSays = dataMap.GetString("guid");
            var TaskPlan = await _SqlLiteContext.Select<TaskPlan>().Where(o => o.GUID == jobSays).FirstAsync();
            _logger.LogInformation("{EventId}:开始执行！", TaskPlan.Name);
            string orgCode = TaskPlan.OrgCode;
            var OrgSetting = await _SqlLiteContext.Select<Organization>().Where(o => o.CODE == orgCode).FirstAsync();
            string connectString = OrgSetting.ConnectingString;
            string orgType = string.Empty;
            try
            {
                switch (OrgSetting.DBType)
                {
                    case "0":
                        orgType = DBTypeEnum.Oracle.ToString();
                        break;
                    case "1":
                        orgType = DBTypeEnum.SqlServer.ToString();
                        break;
                    case "2":
                        orgType = DBTypeEnum.MySql.ToString();
                        break;
                    case "3":
                        orgType = DBTypeEnum.Sqlite.ToString();
                        break;
                    default:
                        break;
                }
                var _SqlHelper = new ExecSqlHelper(connectString, orgType);
                var taskPlanList = await _SqlLiteContext.Select<TaskPlanDetail>().Where(o => o.TaskPlanGuid == jobSays).ToListAsync();
                string[] openSqls = new string[taskPlanList.Count];
                for (int i = 0; i < openSqls.Length; i++)
                {
                    openSqls[i] = taskPlanList[i].OpenSqlGuid;
                }
                string dataJsonData = string.Empty;
                string dataJsonDataDetail = string.Empty;
                string paramJson = string.Empty;
                string groupSql = string.Empty;
                string[] sqlStrings;
                string sqlString = string.Empty;
                string afterSuccess = string.Empty;
                string afterFalse = string.Empty;
                string MainKey = string.Empty;
                string MainKeyValue = string.Empty;
                var systemKes = await _SqlLiteContext.Select<SystemKey>().Where(o => o.KeyName != "").ToListAsync();
                //计划中的数据源
                var dataSource = await _SqlLiteContext.Select<DataSource>().Where(o => openSqls.Contains(o.GUID)  && o.IsStart == "1").ToListAsync();
                string startupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string dllPath = string.Empty;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    dllPath = "TaskDlls/" + TaskPlan.DllOrUrl.Split(',')[0];
                }
                else
                {
                    dllPath = "TaskDlls\\" +TaskPlan.DllOrUrl.Split(',')[0];
                }
                object obj = ReflectorDllHelper.ReturnObjType(dllPath, TaskPlan.DllOrUrl);
                IUpJob upJob = obj as IUpJob;
                for (int j = 0; j < dataSource.Count; j++)
                {
                    MainKey = dataSource[j].MainKey;
                    groupSql = dataSource[j].GroupSqlString;
                    sqlString = dataSource[j].SqlString;
                    foreach (var item in systemKes)
                    {
                        groupSql = groupSql.Replace($"[{item.KeyName}]", item.KeyValue);
                        sqlString = sqlString.Replace($"[{item.KeyName}]", item.KeyValue);
                    }
                    List<Datas> datas = new List<Datas>();
                    if (!groupSql.Contains(MainKey))
                    {
                        //主数据源中不包含关键字MainKey
                        _logger.LogError("{EventId}:\r\n主数据源中不包含关键字{MainKey}", TaskPlan.Name, MainKey);
                        return;
                    }
                    else
                    {
                        //获取当前任务中分组数据 ,主表
                        var dataMaindt = await _SqlHelper.GetDataTableAsync(groupSql);
                        if (dataMaindt.Rows.Count==0)
                        {
                            _logger.LogError("{EventId}:\r\n查询无数据：{groupSql}", TaskPlan.Name, groupSql);
                            return;
                        }
                        for (int h = 0; h < dataMaindt.Rows.Count; h++)
                        {
                            List<DataTable> dataTables = new List<DataTable>();
                            DataTable maindetail = dataMaindt.Copy();
                            MainKeyValue = string.Empty;
                            MainKeyValue = dataMaindt.Rows[h][MainKey].ToString();
                            afterSuccess = dataSource[j].AfterSqlString.Replace($"#{MainKey}#", MainKeyValue);
                            afterFalse = dataSource[j].AfterSqlstring2.Replace($"#{MainKey}#", MainKeyValue);
                            sqlStrings = sqlString.Replace($"#{MainKey}#", MainKeyValue).Split(";");
                            dataTables.Clear();
                            for (int k = 0; k < sqlStrings.Length; k++)
                            {
                                dataTables.Add(await _SqlHelper.GetDataTableAsync(sqlStrings[k]));
                            }
                            maindetail.Rows.Clear();
                            maindetail.ImportRow(dataMaindt.Rows[h]);
                            datas.Add(new Datas { DataMain = maindetail, DataDetail = dataTables });
                        }
                        int allResult = upJob.ExecJob(new JobPara {connString = connectString,dbType = orgType, jobCode = dataSource[j].GUID}, datas);
                        if (allResult == 0)
                        {
                            await _SqlHelper.ExecSqlAsync(afterSuccess);
                            _logger.LogInformation("{EventId}:\r\n数据源:{dataSource[j].Name },\r\n成功后执行语句为:{afterSuccess}\r\n", TaskPlan.Name, dataSource[j].Name, afterSuccess);
                        }
                        else
                        {
                            await _SqlHelper.ExecSqlAsync(afterFalse);
                            _logger.LogError("{EventId}:\r\n数据源:{dataSource[j].Name },\r\n失败后执行语句为:{afterFalse}\r\n", TaskPlan.Name, dataSource[j].Name, afterFalse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{EventId}错误信息:{ex.Message},{ex.StackTrace}", TaskPlan.Name, ex.Message,ex.StackTrace);
                return;
            }
        }
    }
}