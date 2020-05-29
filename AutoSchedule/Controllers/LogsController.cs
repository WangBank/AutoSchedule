using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using AutoSchedule.Dtos.RequestIn;
using FreeSql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoSchedule.Controllers
{
    public class LogsController : Controller
    {
        public FreeSqlFactory _freeSqlFactory;
       // private SqlLiteContext _SqlLiteContext;
        private readonly ILogger<LogsController> _logger;
        IFreeSql _SqlLiteContext;

        //, SqlLiteContext SqlLiteContext
        public LogsController(ILogger<LogsController> logger, FreeSqlFactory freeSqlFactory)
        {
            //_SqlLiteContext = SqlLiteContext;
            _logger = logger;
            _freeSqlFactory = freeSqlFactory;
            _SqlLiteContext = _freeSqlFactory.GetBaseLogSqlLite();
        }
        public IActionResult Index()
        {
            _SqlLiteContext.Ado.ExecuteNonQuery($"DELETE FROM Logs WHERE EventId is null or EventId=''");
            return View();
        }


        [HttpGet]
        public async Task<LogsModelData> LogsResult(int page,int limit,string value = "")
        {
          ISelect<Logs> skeyAll = _SqlLiteContext.Select<Logs>();
            var count = _SqlLiteContext.Select<Logs>().Count().ToInt();
            if (!string.IsNullOrEmpty(value))
            {
                skeyAll = skeyAll.Where(s => s.Level.Contains(value) || s.Message.Contains(value));
            }
          
            var skey = await skeyAll.OrderByDescending(s => s.TimestampUtc).Skip((page - 1) * limit).Take(limit).ToListAsync();
            //var skey = await _SqlLiteContext.Select<Logs>().ToListAsync();
            List<LogsModel> data = new List<LogsModel>();
            foreach (var item in skey)
            {
                data.Add(new LogsModel { EventName = item.EventId, Level = item.Level, Message = item.Message, Time = item.TimestampUtc, Id = item.Id });
            }


            return new LogsModelData { msg = "", count = count, code = 0, data = data };
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
                if (await _SqlLiteContext.Delete<Logs>().Where(o => logs.Contains(o.Id)).ExecuteAffrowsAsync() > 0)
                {
                    return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "", code = "0" });
                }
                else
                {
                    return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "删除日志", code = "-1" });
                }
            }
            else
            {
                if (await _SqlLiteContext.Delete<Logs>().Where(o => o.Id == logNum).ExecuteAffrowsAsync() > 0)
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
}