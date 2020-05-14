using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using AutoSchedule.Dtos.RequestIn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoSchedule.Controllers
{
    public class LogsController : Controller
    {
        private SqlLiteContext _SqlLiteContext;
        private readonly ILogger<LogsController> _logger;
       
        public LogsController(ILogger<LogsController> logger, SqlLiteContext SqlLiteContext)
        {
            _SqlLiteContext = SqlLiteContext;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<LogsModelData> LogsResult(int page,int limit,string value = "")
        {
            var skeyAll = _SqlLiteContext.Logs.AsNoTracking();
            if (!string.IsNullOrEmpty(value))
            {
                skeyAll = _SqlLiteContext.Logs.AsNoTracking().Where(s => s.Level.Contains(value) || s.Message.Contains(value));
            }
            var skey = await skeyAll.OrderByDescending(s => s.TimestampUtc).Skip((page - 1) * limit).Take(limit).ToListAsync();
            //var skey = await _SqlLiteContext.Logs.AsNoTracking().ToListAsync();
            List<LogsModel> data = new List<LogsModel>();
            foreach (var item in skey)
            {
                data.Add(new LogsModel { EventName = item.EventId, Level = item.Level, Message = item.Message, Time = item.TimestampUtc, Id = item.Id });
            }


            return new LogsModelData { msg = "", count = skeyAll.Count(), code = 0, data = data };
        }

        [HttpGet]
        public async Task<string>LogDelete(string logNum)
        {
            if (string.IsNullOrEmpty(logNum))
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "请至少选择一条日志进行删除", code = "1" });
            }
            if (logNum.Contains(","))
            {
                var logs = logNum.Split(',');
                var logList =await  _SqlLiteContext.Logs.AsNoTracking().Where(o => logs.Contains(o.Id)).ToListAsync();
                _SqlLiteContext.Logs.RemoveRange(logList);
            }
            else
            {
                var logDelete = await _SqlLiteContext.Logs.AsNoTracking().Where(o => o.Id == logNum).FirstOrDefaultAsync();

                _SqlLiteContext.Logs.Remove(logDelete);
            }
            

            if (await _SqlLiteContext.SaveChangesAsync() > 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "", code = "0" });
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "删除日志", code = "-1" });
            }
        }

    }
}