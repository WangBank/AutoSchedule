using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using AutoSchedule.Dtos.RequestIn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Controllers
{
    public class TaskPlanController : Controller
    {
        SqlLiteContext _SqlLiteContext;
        private readonly ILogger<TaskPlanController> _logger;

        public TaskPlanController(ILogger<TaskPlanController> logger, SqlLiteContext SqlLiteContext)
        {
            _SqlLiteContext = SqlLiteContext;
            _logger = logger;
        }
        public IActionResult TaskPlan()
        {
            return View();
        }

        [HttpGet]
        public async Task<string> TaskPlanResult()
        {
            var skey = await _SqlLiteContext.TaskPlan.AsNoTracking().ToListAsync();
            List<TaskPlanModel> data = new List<TaskPlanModel>();
            string OrgName = "";
            string TaskPlanName = "";
            string FrequencyName = "";
            string FrequencyType = "";
            foreach (var item in skey)
            {
                OrgName = (await _SqlLiteContext.OrgSetting.AsNoTracking().FirstAsync(o => o.CODE == item.OrgCode)).DataBaseName;
                if (item.TaskPlanType == "0")
                {
                    TaskPlanName = "上传";
                }
                else if (item.TaskPlanType == "1")
                {
                    TaskPlanName = "下载";
                }

                if (item.Frequency == "0")
                {
                    FrequencyType = "秒";
                }
                else if (item.Frequency == "1")
                {
                    FrequencyType = "分钟";
                }
                else if (item.Frequency == "2")
                {
                    FrequencyType = "小时";
                }

                FrequencyName = $"每隔{item.Frequency + FrequencyType}执行一次任务";
                data.Add(new TaskPlanModel { GUID = item.GUID, CODE = item.CODE, Name = item.Name, OrgCode = OrgName, TaskPlanType = TaskPlanName, Frequency = FrequencyName });
            }
            return System.Text.Json.JsonSerializer.Serialize(new TaskPlanData { msg = "", count = data.Count, code = 0, data = data });
        }


        [HttpGet]
        //http://localhost:5000/TaskPlan/TaskPlanDetailResult?guid=100&page=1&limit=10
        public async Task<string> TaskPlanDetailResult(TaskPlanRequest taskPlanRequest)
        {
            var skey = await _SqlLiteContext.TaskPlanRelation.AsNoTracking().Where(o => o.TaskPlanGuid == taskPlanRequest.guid).ToListAsync();
            List<TaskPlanDetailModel> data = new List<TaskPlanDetailModel>();
            string dsName = "";
            string dsState = "";
            foreach (var item in skey)
            {
                dsName = (await _SqlLiteContext.OpenSql.AsNoTracking().FirstAsync(o => o.GUID == item.OpenSqlGuid)).Name;
                dsState = (await _SqlLiteContext.OpenSql.AsNoTracking().FirstAsync(o => o.GUID == item.OpenSqlGuid)).IsStart;

                data.Add(new TaskPlanDetailModel { dsGuid = item.OpenSqlGuid, dsName = dsName, dsState = dsState });
            }
            return System.Text.Json.JsonSerializer.Serialize(new TaskPlanDetailData { msg = "", count = data.Count, code = 0, data = data });
        }
    }
}