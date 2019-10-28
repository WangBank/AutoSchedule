using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using BankDbHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{
    public class AutoTaskJob : IJob
    {
        //private readonly ILogger<HelloJob> _logger;
        private readonly SqlLiteContext _SqlLiteContext;

        public ExecSqlHelper _SqlHelper;

        public AutoTaskJob(ILogger<AutoTaskJob> logger, SqlLiteContext SqlLiteContext)
        {
            //_logger = logger;
            _SqlLiteContext = SqlLiteContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobKey key = context.JobDetail.Key;
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                string jobSays = dataMap.GetString("guid");
                var taskPlan = await _SqlLiteContext.TaskPlan.AsNoTracking().SingleOrDefaultAsync(o => o.GUID == jobSays);
                await Console.Out.WriteLineAsync("任务名称: " + taskPlan.Name + "正在执行！");

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
                var taskPlanList = await _SqlLiteContext.TaskPlanRelation.AsNoTracking().Where(o => o.TaskPlanGuid == jobSays).ToListAsync();
                for (int i = 0; i < taskPlanList.Count; i++)
                {
                    var dataSource = await _SqlLiteContext.OpenSql.AsNoTracking().FirstAsync(o => o.GUID == taskPlanList[i].OpenSqlGuid);
                    //执行sql语句
                    await _SqlHelper.ExecSqlAsync(dataSource.GroupSqlString);
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("错误信息" + ex.Message);
                throw;
            }

            //CommonHelper.HttpPostAsync(,)
        }
    }
}