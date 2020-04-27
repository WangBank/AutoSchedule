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
        private SqlLiteContext _SqlLiteContext;
        public ExecSqlHelper _SqlHelper;
        public IHttpClientFactory _httpClientFactory;
       
        public AutoTaskJobDll(ILogger<AutoTaskJobDll> logger, SqlLiteContext SqlLiteContext, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _SqlLiteContext = SqlLiteContext;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            
            CommonHelper commonHelper = new CommonHelper();
           
            JobKey key = context.JobDetail.Key;
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                string jobSays = dataMap.GetString("guid");
                //从asp netcore中重新获取sqlcontext
               // _SqlLiteContext = (SqlLiteContext)GetContext.ServiceProvider.GetService(typeof(SqlLiteContext));
                var taskPlan = await _SqlLiteContext.TaskPlan.AsNoTracking().SingleOrDefaultAsync(o => o.GUID == jobSays);
                _logger.LogInformation("{EventId}:开始执行！", taskPlan.Name);
                var TaskPlan = await _SqlLiteContext.TaskPlan.AsNoTracking().Where(o => o.GUID == jobSays).FirstOrDefaultAsync();
                string orgCode = TaskPlan.OrgCode;
                var OrgSetting = await _SqlLiteContext.OrgSetting.AsNoTracking().Where(o => o.CODE == orgCode).FirstOrDefaultAsync();
                string connectString = OrgSetting.ConnectingString;
                string orgType = string.Empty;
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
                var taskPlanList = await _SqlLiteContext.TaskPlanRelation.AsNoTracking().Where(o => o.TaskPlanGuid == jobSays).ToListAsync();
                string[] openSqls = new string[taskPlanList.Count];
                for (int i = 0; i < openSqls.Length; i++)
                {
                    openSqls[i] = taskPlanList[i].OpenSqlGuid;
                }
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
                var dataSource = await _SqlLiteContext.OpenSql.AsNoTracking().Where(o => openSqls.Contains(o.GUID)  && o.IsStart == "1").ToListAsync();
                //反射dll
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
                        _logger.LogError("{EventId}:\r\n主数据源中不包含关键字{MainKey}", taskPlan.Name, MainKey);
                        return;
                    }
                    else
                    {
                        //获取当前任务中分组数据 ,主表
                        var dataMaindt = await _SqlHelper.GetDataTableAsync(groupSql);
                        if (dataMaindt.Rows.Count==0)
                        {
                            _logger.LogError("{EventId}:\r\n查询无数据：{groupSql}", taskPlan.Name, groupSql);
                            return;
                        }
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
                        JobPara jobPara = new JobPara();
                        string allResult =  upJob.ExecJob(new JobPara {connString = connectString,dbType = orgType, jobCode = dataSource[j].GUID}, datas,out string result);
                       
                        //记录日志
                        if (result == "0")
                        {
                            var afterS = await _SqlHelper.ExecSqlAsync(afterSuccess);

                            //记录日志
                            _logger.LogInformation("{EventId}:\r\n调用接口返回结果:{result}数据源:{dataSource[j].Name },\r\n成功后执行语句为:{afterSuccess}\r\n", taskPlan.Name, allResult, dataSource[j].Name, afterSuccess);
                        }
                        else
                        {
                            var afterF = await _SqlHelper.ExecSqlAsync(afterFalse);
                            //记录日志
                            _logger.LogError("{EventId}:\r\n调用接口返回结果:{result}数据源:{dataSource[j].Name },\r\n失败后执行语句为:{afterFalse}\r\n", taskPlan.Name,result, dataSource[j].Name, afterFalse);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.LogError("{EventId}错误信息:{ex.Message},{ex.StackTrace}", taskPlan.Name, ex.Message,ex.StackTrace);
               
                return;
            }
            finally
            {
               await _SqlHelper.DisposeAsync();
            }
        }

        public async Task<string> AsyncTesr()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(5000);
            });
            return "1";
        }
    }
}