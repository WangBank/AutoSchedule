using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using AutoSchedule.Dtos.RequestIn;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;
using BankDbHelper;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace AutoSchedule.Common
{
    public class AutoTaskJob : IJob
    {
        private ILogger<AutoTaskJob> _logger;
        private SqlLiteContext _SqlLiteContext;
        public ExecSqlHelper _SqlHelper;
        //public IConfiguration _Configuration;
        //public AutoTaskJob(ILogger<AutoTaskJob> logger, SqlLiteContext SqlLiteContext, IConfiguration configuration)
        //{
        //    _logger = logger;
        //    _SqlLiteContext = SqlLiteContext;
        //    _Configuration = configuration;
        //    _services = services;
        //}
        public AutoTaskJob(ILogger<AutoTaskJob> logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobKey key = context.JobDetail.Key;
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                string jobSays = dataMap.GetString("guid");
                _SqlLiteContext = (SqlLiteContext)GetContext.ServiceProvider.GetService(typeof(SqlLiteContext));
                var taskPlan = await  _SqlLiteContext.TaskPlan.AsNoTracking().SingleOrDefaultAsync(o => o.GUID == jobSays);
                _logger.LogInformation("任务名称: " + taskPlan.Name + "正在执行！");
                var TaskPlan = await _SqlLiteContext.TaskPlan.AsNoTracking().Where(o => o.GUID == jobSays).FirstOrDefaultAsync();
                string orgCode = TaskPlan.OrgCode;
                var OrgSetting = await _SqlLiteContext.OrgSetting.AsNoTracking().Where(o => o.CODE == orgCode).FirstOrDefaultAsync();
                string connectString = OrgSetting.ConnectingString;
                string orgType = OrgSetting.DBType;
                switch (orgType)
                {
                    //Oracle 0
                    //SqlServer  1
                    //MySql  2
                    //Sqlite  3
                    case "0":
                        _SqlHelper = new ExecSqlHelper(connectString, DBTypeEnum.Oracle.ToString());
                        break;

                    case "1":
                        _SqlHelper = new ExecSqlHelper(connectString, DBTypeEnum.SqlServer.ToString());
                        break;

                    case "2":
                        _SqlHelper = new ExecSqlHelper(connectString, DBTypeEnum.MySql.ToString());
                        break;

                    case "3":
                        _SqlHelper = new ExecSqlHelper(connectString, DBTypeEnum.Sqlite.ToString());
                        break;

                    default:
                        break;
                }
                _SqlHelper = new ExecSqlHelper(connectString, orgType);
                
                //List<DataSource> dataSources = new List<DataSource>();
                var taskPlanList = await _SqlLiteContext.TaskPlanRelation.AsNoTracking().Where(o => o.TaskPlanGuid == jobSays).FirstOrDefaultAsync();
                string dataJsonData =string.Empty;
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
                var systemKes =await  _SqlLiteContext.SystemKeys.AsNoTracking().Where(o => o.KeyName != "").ToListAsync();
                //计划中的数据源
                var dataSource = await _SqlLiteContext.OpenSql.AsNoTracking().Where(o => o.GUID == taskPlanList.OpenSqlGuid).ToListAsync();
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
                        _logger.LogError("主数据源中不包含关键字" + MainKey);
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
                            _logger.LogInformation("任务名称：" + taskPlan.Name + ",数据源:" + dataSource[j].Name + ",分组sql:" + groupSql + "选择sql:" + sqlString + "成功sql:" + afterSuccess + "失败sql:" + afterFalse);
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

                        string result = await CommonHelper.HttpPostAsync(TaskPlan.TaskUrl, paramJson);
                        _logger.LogInformation("任务名称：" + taskPlan.Name + ",接口地址:" + TaskPlan.TaskUrl + "入参Json" + paramJson + ",返回：" + result);
                        responseCommon = (ResponseCommon)System.Text.Json.JsonSerializer.Deserialize(result, typeof(ResponseCommon));
                        //记录日志
                        if (responseCommon.code == "0")
                        {
                            var afterS = await _SqlHelper.ExecSqlAsync(afterSuccess);

                            //记录日志
                            _logger.LogInformation("任务名称：" + taskPlan.Name + ",数据源:" + dataSource[j].Name + "成功后执行语句为:" + afterSuccess + ",返回" + afterS);
                        }
                        else
                        {
                            var afterF = await _SqlHelper.ExecSqlAsync(afterFalse);
                            //记录日志
                            _logger.LogInformation("任务名称：" + taskPlan.Name + ",数据源:" + dataSource[j].Name + "失败后执行语句为:" + afterFalse + ",返回" + afterF);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogError("错误信息" + ex.Message + ex.StackTrace);
                return;
            }

        }
        public async Task<string> asyncTesr() {
            await Task.Run(() => { 
                
                Thread.Sleep(5000); });
            return "1";
        
        }    
    }
}