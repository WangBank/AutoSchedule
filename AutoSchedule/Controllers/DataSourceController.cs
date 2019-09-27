using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoSchedule.Controllers
{
    public class DataSourceController : Controller
    {
        SqlLiteContext _SqlLiteContext;
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


        /// <summary>
        /// 数据源
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DataSourceResult()
        {
            var dts = await _SqlLiteContext.OpenSql.AsNoTracking().ToListAsync();
            List<DataSourceModel> data = new List<DataSourceModel>();
            foreach (var item in dts)
            {
                data.Add(new DataSourceModel { GUID = item.GUID, Name = item.Name, AfterSqlString = item.AfterSqlString, AfterSqlstring2 = item.AfterSqlstring2, FType = item.FType, GroupSqlString = item.GroupSqlString, SqlString = item.SqlString, IsStart = item.IsStart, MainKey = item.MainKey });
            }
            string result = System.Text.Json.JsonSerializer.Serialize<DataSourceData>(new DataSourceData { msg = "", count = data.Count, code = 0, data = data });
            return Content(result);
        }
    }
}