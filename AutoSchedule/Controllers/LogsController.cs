using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoSchedule.Controllers
{
    public class LogsController : Controller
    {
        private SqlLiteContext _SqlLiteContext;
        private readonly ILogger<DataSourceController> _logger;
       
        public LogsController(ILogger<DataSourceController> logger, SqlLiteContext SqlLiteContext)
        {
            _SqlLiteContext = SqlLiteContext;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<string> LogsResult(int page,int limit,string value = "")
        {
            var skeyAll = _SqlLiteContext.Logs.AsNoTracking();
            if (!string.IsNullOrEmpty(value))
            {
                skeyAll = _SqlLiteContext.Logs.AsNoTracking().Where(s => s.Level.Contains(value) || s.Message.Contains(value));
            }
            var skey = await skeyAll.Skip((page - 1) * limit).Take(limit).ToListAsync();
            //var skey = await _SqlLiteContext.Logs.AsNoTracking().ToListAsync();
            List<LogsModel> data = new List<LogsModel>();
            string EventName = "";
            foreach (var item in skey)
            {
                string state = string.Empty;
                if (string.IsNullOrEmpty(item.EventId))
                {
                    EventName = "";
                }
                else
                {
                    EventName = (await _SqlLiteContext.TaskPlan.AsNoTracking().FirstAsync(o => o.GUID == item.EventId)).Name;
                }
                
                
                data.Add(new LogsModel { EventName = EventName, Level = item.Level, Message = item.Message, Time = item.TimestampUtc });
            }


            return System.Text.Json.JsonSerializer.Serialize(new LogsModelData { msg = "", count = skeyAll.Count(), code = 0, data = data});
        }
    }
}