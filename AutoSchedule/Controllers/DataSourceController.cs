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

        public DataSourceController(ILogger<DataSourceController> logger, SqlLiteContext SqlLiteContext)
        {
            _SqlLiteContext = SqlLiteContext;
            _logger = logger;
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
            var dsdelete = _SqlLiteContext.OpenSql.AsNoTracking().Where(o => o.GUID == GUID).FirstOrDefault();
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