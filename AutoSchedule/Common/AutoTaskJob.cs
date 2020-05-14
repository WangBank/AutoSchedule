using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using AutoSchedule.Dtos.RequestIn;
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
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{

    /// <summary>
    /// 调用api方式发送数据
    /// </summary>
    public class AutoTaskJob : IJob
    {
        private ILogger<AutoTaskJob> _logger;
        private SqlLiteContext _SqlLiteContext;
        public ExecSqlHelper _SqlHelper;
        public IHttpClientFactory _httpClientFactory;
        public AutoTaskJob(ILogger<AutoTaskJob> logger, SqlLiteContext SqlLiteContext, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _SqlLiteContext = SqlLiteContext;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {

             
                JobKey key = context.JobDetail.Key;
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                string jobSays = dataMap.GetString("guid");
               
                var taskPlan = await _SqlLiteContext.TaskPlan.AsNoTracking().SingleOrDefaultAsync(o => o.GUID == jobSays);
                _logger.LogDebug("{TaskName}({EventId}):开始执行！", taskPlan.Name, jobSays);
                var TaskPlan = await _SqlLiteContext.TaskPlan.AsNoTracking().Where(o => o.GUID == jobSays).FirstOrDefaultAsync();
                string orgCode = TaskPlan.OrgCode;
                var OrgSetting = await _SqlLiteContext.OrgSetting.AsNoTracking().Where(o => o.CODE == orgCode).FirstOrDefaultAsync();
                string connectString = OrgSetting.ConnectingString;
                string orgType = string.Empty;
            var client = _httpClientFactory.CreateClient();
            try
            {
                switch (OrgSetting.DBType)
                {
                    //Oracle 0
                    //SqlServer  1
                    //MySql  2
                    //Sqlite  3
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
                _SqlHelper = new ExecSqlHelper(connectString, orgType);

                //List<DataSource> dataSources = new List<DataSource>();
                var taskPlanList = await _SqlLiteContext.TaskPlanRelation.AsNoTracking().Where(o => o.TaskPlanGuid == jobSays).FirstOrDefaultAsync();
                string dataJsonData = string.Empty;
                string dataJsonDataDetail = string.Empty;
                //string taskApiUrl = _Configuration.GetSection("TaskApiUrls").GetSection("TaskApiUrl").Value;
                string paramJson = string.Empty;
                ResponseCommon responseCommon = new ResponseCommon();
                string groupSql = string.Empty;
                string[] sqlStrings;
                string sqlString = string.Empty;
                string afterSuccess = string.Empty;
                string afterFalse = string.Empty;
                string MainKey = string.Empty;
                string MainKeyValue = string.Empty;
                var systemKes = await _SqlLiteContext.SystemKeys.AsNoTracking().Where(o => o.KeyName != "").ToListAsync();
                //计划中的数据源
                var dataSource = await _SqlLiteContext.OpenSql.AsNoTracking().Where(o => o.GUID == taskPlanList.OpenSqlGuid && o.IsStart == "1").ToListAsync();
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
                        _logger.LogError("{TaskName}({EventId}):\r\n主数据源中不包含关键字{MainKey}", taskPlan.Name, jobSays, MainKey);
                    }
                    else
                    {
                        //获取当前任务中分组数据
                        var dataMaindt = await _SqlHelper.GetDataTableAsync(groupSql);

                        //MainKeyValue = dataMaindt.Rows[]
                        for (int h = 0; h < dataMaindt.Rows.Count; h++)
                        {
                            List<DataTable> dataTables = new List<DataTable>();
                            DataTable maindetail = dataMaindt.Copy();
                            MainKeyValue = string.Empty;
                            MainKeyValue = dataMaindt.Rows[h][MainKey].ToString();
                            afterSuccess = dataSource[j].AfterSqlString.Replace($"#{MainKey}#", MainKeyValue);
                            afterFalse = dataSource[j].AfterSqlstring2.Replace($"#{MainKey}#", MainKeyValue);
                            sqlStrings = sqlString.Replace($"#{MainKey}#", MainKeyValue).Split(";");
                            //_logger.LogInformation("{TaskName}{EventId}:\r\n数据源:{dataSource[j].Name },\r\n分组sql:{groupSql },\r\n选择sql:{ sqlString },\r\n成功sql:{ afterSuccess },\r\n失败sql:{ afterFalse}", taskPlan.Name, jobSays, dataSource[j].Name, groupSql, sqlString, afterSuccess, afterFalse);
                            dataTables.Clear();
                            for (int k = 0; k < sqlStrings.Length; k++)
                            {
                                dataTables.Add(await _SqlHelper.GetDataTableAsync(sqlStrings[k]));
                            }
                            
                            // var dataDetaildt = await _SqlHelper.GetDataTableAsync(sqlString);
                            var sss = dataMaindt.Rows[h];
                            maindetail.Rows.Clear();
                            maindetail.ImportRow(dataMaindt.Rows[h]);
                            datas.Add(new Datas { DataMain = maindetail, DataDetail = dataTables });
                        }
                        paramJson = JsonConvert.SerializeObject(new DoTaskJson
                        {
                            OpenSqlGuid = dataSource[j].GUID,
                            Data = datas
                        });

                        string result = await CommonHelper.HttpPostAsync(TaskPlan.DllOrUrl, paramJson,client);
                        _logger.LogInformation("{TaskName}({EventId}):\r\n接口地址:{TaskPlan.TaskUrl},\r\n入参Json{paramJson},\r\n返回：{result}", taskPlan.Name, jobSays, TaskPlan.DllOrUrl, paramJson, result);
                        responseCommon = (ResponseCommon)System.Text.Json.JsonSerializer.Deserialize(result, typeof(ResponseCommon));
                        //记录日志
                        if (responseCommon.code == "0")
                        {
                            var afterS = await _SqlHelper.ExecSqlAsync(afterSuccess);

                            //记录日志
                            _logger.LogInformation("{TaskName}({EventId}):\r\n数据源:{dataSource[j].Name },\r\n成功后执行语句为:{afterSuccess}\r\n,返回:{afterS}", taskPlan.Name, jobSays, dataSource[j].Name, afterSuccess, afterS);
                        }
                        else
                        {
                            var afterF = await _SqlHelper.ExecSqlAsync(afterFalse);
                            //记录日志
                            _logger.LogInformation("{TaskName}({EventId}):\r\n数据源:{dataSource[j].Name },\r\n失败后执行语句为:{afterFalse}\r\n,返回:{afterF}", taskPlan.Name, jobSays, dataSource[j].Name, afterFalse, afterF);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.LogError("{TaskName}({EventId})错误信息:{ex.Message},{ex.StackTrace}", taskPlan.Name, jobSays, ex.Message,ex.StackTrace);
                return;
            }
            finally
            {
               await _SqlHelper.DisposeAsync();
            }
        }

        public async Task<string> asyncTesr()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(5000);
            });
            return "1";
        }
    }
}