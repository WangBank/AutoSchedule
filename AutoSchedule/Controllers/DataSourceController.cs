using AutoSchedule.Common;
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
    public class DataSourceController : Controller
    {
        private SqlLiteContext _SqlLiteContext;
        private readonly ILogger<DataSourceController> _logger;
        private IFreeSql _sqliteFSql;
        public DataSourceController(ILogger<DataSourceController> logger, SqlLiteContext SqlLiteContext, FreeSqlFactory freeSqlFactory)
        {
            _SqlLiteContext = SqlLiteContext;
            _logger = logger;
            _sqliteFSql = freeSqlFactory.GetBaseSqlLite();
        }

        public IActionResult DataSource()
        {
            return View();
        }

        public async Task<IActionResult> DataSourceEdit(string guid)
        {

            var ds = await _SqlLiteContext.OpenSql.AsNoTracking().Where(o => o.GUID == guid).FirstOrDefaultAsync();
            return View(ds);
        }

        [HttpPost]
        public async Task<string> DataSourceEdit([FromBody]DataSource dataSourceAddIn)
        {
            var nowRunningTP = await _sqliteFSql.Select<TaskPlan>().Where(o => o.Status == "1").ToListAsync();
            List<string> tPGuids = new List<string>();
            foreach (var item in nowRunningTP)
            {
                tPGuids.Add(item.GUID);
            }

            var nowRuningTPRE = await _sqliteFSql.Select<TaskPlanDetail>().Where(o => tPGuids.Contains(o.TaskPlanGuid) && o.OpenSqlGuid == dataSourceAddIn.GUID).ToListAsync();

            if (nowRuningTPRE.Count != 0)
            {
                tPGuids.Clear();
                return new ResponseCommon { msg = "此数据源正在执行，不允许修改！", code = "1" }.ToJsonCommon();
            }



            var dsUpdate = await _SqlLiteContext.OpenSql.AsNoTracking().Where(o => o.GUID == dataSourceAddIn.GUID).FirstOrDefaultAsync();
            dsUpdate.AfterSqlString = dataSourceAddIn.AfterSqlString;
            dsUpdate.AfterSqlstring2 = dataSourceAddIn.AfterSqlstring2;
            dsUpdate.GroupSqlString = dataSourceAddIn.GroupSqlString;
            dsUpdate.IsStart = string.IsNullOrEmpty(dataSourceAddIn.IsStart) ? "0" : "1";
            dsUpdate.MainKey = dataSourceAddIn.MainKey;
            dsUpdate.SqlString = dataSourceAddIn.SqlString;
            dsUpdate.Name = dataSourceAddIn.Name;
            _SqlLiteContext.OpenSql.Update(dsUpdate);
            var addresult = await _SqlLiteContext.SaveChangesAsync();
            if (addresult > 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "", code = "0" });
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "修改数据源失败", code = "-1" });
            }
        }

        public IActionResult DataSourceAdd()
        {
            return View();
        }

        [HttpPost]
        public async Task<string> DataSourceAdd([FromBody]DataSource dataSourceAddIn)
        {
            var ds = await _SqlLiteContext.OpenSql.AsNoTracking().OrderByDescending(o => o.GUID).FirstOrDefaultAsync();
            Dtos.Models.DataSource dataSource = new DataSource
            {
                GUID = dataSourceAddIn.GUID,
                AfterSqlString = dataSourceAddIn.AfterSqlString,
                AfterSqlstring2 = dataSourceAddIn.AfterSqlstring2,
                GroupSqlString = dataSourceAddIn.GroupSqlString,
                IsStart = string.IsNullOrEmpty(dataSourceAddIn.IsStart) ? "0" : "1",
                MainKey = dataSourceAddIn.MainKey,
                SqlString = dataSourceAddIn.SqlString,
                Name = dataSourceAddIn.Name
            };
            await _SqlLiteContext.OpenSql.AddAsync(dataSource);
            var addresult = await _SqlLiteContext.SaveChangesAsync();
            if (addresult > 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "", code = "0" });
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "新增数据源失败", code = "-1" });
            }
        }

        [HttpGet]
        public async Task<string> DataSourceResult(int page, int limit)
        {
            var skeyAll = _SqlLiteContext.OpenSql.AsNoTracking();
            List<DataSource> dts;
            if (page ==0 ||
                
                limit ==0)
            {
                dts = await skeyAll.ToListAsync();
            }
            else
            {
                dts = await skeyAll.Skip((page - 1) * limit).Take(limit).ToListAsync();
            }
            List<DataSourceModel> data = new List<DataSourceModel>();
            foreach (var item in dts)
            {
                data.Add(new DataSourceModel
                {
                    Name = item.Name,
                    AfterSqlString = item.AfterSqlString,
                    AfterSqlstring2 = item.AfterSqlstring2,
                    GroupSqlString = item.GroupSqlString,
                    SqlString = item.SqlString,
                    IsStart = item.IsStart,
                    MainKey = item.MainKey,
                    GUID = item.GUID
                });
            }
            return System.Text.Json.JsonSerializer.Serialize(new DataSourceData { msg = "", count = skeyAll.Count(), code = 0, data = data });
        }

        [HttpGet]
        public async Task<string> DataSourceDelete(string GUID)
        {
            //如果当前删除的在正在运行的任务中，不允许删除
            //当前正在运行的任务
            var nowRunningTP =await _sqliteFSql.Select<TaskPlan>().Where(o => o.Status == "1").ToListAsync();
            List<string> tPGuids = new List<string>();
            foreach (var item in nowRunningTP)
            {
                tPGuids.Add(item.GUID);
            }

            var nowRuningTPRE = await _sqliteFSql.Select<TaskPlanDetail>().Where(o => tPGuids.Contains(o.TaskPlanGuid) && o.OpenSqlGuid==GUID ).ToListAsync();

            if (nowRuningTPRE.Count !=0)
            {
                tPGuids.Clear();
                return new ResponseCommon { msg = "此数据源正在执行，不允许删除！", code = "1" }.ToJsonCommon();
            }

            var dsdelete = _SqlLiteContext.OpenSql.AsNoTracking().Where(o => o.GUID == GUID).FirstOrDefault();
            _sqliteFSql.Delete<TaskPlanDetail>()
                .Where(b => b.OpenSqlGuid == GUID)
                .ExecuteAffrows();

            _SqlLiteContext.OpenSql.Remove(dsdelete);

            if (await _SqlLiteContext.SaveChangesAsync() > 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "", code = "0" });
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "删除数据源失败", code = "-1" });
            }
        }
    }
}