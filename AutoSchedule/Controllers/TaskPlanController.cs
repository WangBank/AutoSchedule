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
                if (string.IsNullOrEmpty(item.OrgCode))
                {
                    OrgName = "";
                }
                else
                {
                    OrgName = (await _SqlLiteContext.OrgSetting.AsNoTracking().FirstAsync(o => o.CODE == item.OrgCode)).NAME;
                }
                if (item.TaskPlanType == "0")
                {
                    TaskPlanName = "上传";
                }
                else if (item.TaskPlanType == "1")
                {
                    TaskPlanName = "下载";
                }

                if (item.FrequencyType == "0")
                {
                    FrequencyType = "秒";
                }
                else if (item.FrequencyType == "1")
                {
                    FrequencyType = "分钟";
                }
                else if (item.FrequencyType == "2")
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



        public IActionResult TaskPlanAdd()
        {
            return View();
        }

        [HttpPost]
        //string FType, string GUID, string Name, string IsStart, string MainKey, string GroupSqlString, string SqlString, string AfterSqlString, string AfterSqlstring2
        public async Task<string> TaskPlanAdd([FromBody]TaskPlanExGuidCode TaskPlanAddIn)
        {
            string guid = "";
            string code = "";
            var isExist = await _SqlLiteContext.TaskPlan.AsNoTracking().OrderByDescending(o=>o.GUID).FirstOrDefaultAsync();
            if (isExist ==null)
            {
                guid = "100";
                code = guid;
            }
            else
            {
                guid = (int.Parse(isExist.GUID) + 1).ToString();
                code = guid;
            }
            Dtos.Models.TaskPlan TaskPlanAdd = new Dtos.Models.TaskPlan
            {
                CODE = code,
                Frequency = TaskPlanAddIn.Frequency,
                FrequencyType = TaskPlanAddIn.FrequencyType,
                GUID = guid,
                Name = TaskPlanAddIn.Name,
                OrgCode = TaskPlanAddIn.OrgCode,
                TaskPlanType = TaskPlanAddIn.TaskPlanType
            };
            await _SqlLiteContext.TaskPlan.AddAsync(TaskPlanAdd);
            var addresult = await _SqlLiteContext.SaveChangesAsync();
            if (addresult > 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "", code = "0" });
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "新增任务计划失败", code = "-1" });
            }
        }

    }
}