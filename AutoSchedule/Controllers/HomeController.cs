using AutoSchedule.Common;
using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.RequestIn;
using AutoSchedule.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LogProvider = Quartz.Logging.LogProvider;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AutoSchedule.Controllers
{
    public class HomeController : Controller
    {
        private SqlLiteContext _SqlLiteContext;
        private readonly ILogger<HomeController> _logger;
        public IScopeTest _scope;
        private QuartzStartup _quartzStartup;

        public HomeController(ILogger<HomeController> logger, SqlLiteContext SqlLiteContext, QuartzStartup quartzStartup, IScopeTest scope)
        {
            //LogProvider.SetCurrentLogProvider(new AutoSchedule.Common.LogProvider());
            _SqlLiteContext = SqlLiteContext;
            _logger = logger;
            _quartzStartup = quartzStartup;
            _scope = scope;
        }

        public IActionResult Index()
        {
            //清空一下无用日志
            _SqlLiteContext.Database.ExecuteSqlRaw("UPDATE TaskPlan  SET Status = '0' where Status = '1'");
            _SqlLiteContext.Database.ExecuteSqlRaw($"DELETE FROM Logs WHERE EventId is null or EventId=''");
            _SqlLiteContext.SaveChanges();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //开启定时任务
        public async Task<string> BeginTaskPlan(string guid = "")
        {
            try
            {
                List<string> guidhssh = new List<string>();
                if (string.IsNullOrEmpty(guid))
                {
                    var tkList = await _SqlLiteContext.TaskPlan.AsNoTracking().Where(o => o.GUID != "").ToListAsync();

                    for (int i = 0; i < tkList.Count; i++)
                    {
                        guidhssh.Add(tkList[i].GUID);
                    }
                }
                else
                {
                    guidhssh.Add(guid);
                }
                string msg = await _quartzStartup.Start(guidhssh);
                if (msg !="0")
                {
                    return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = msg, code = "-1" });
                }
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg= "定时任务已开启！", code = "0" });
            }
            catch (SchedulerException se)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = se.ToString(), code = "-1" });
            }
        }

        //关闭定时任务
        public async Task<string> StopTaskPlan(string guid = "")
        {
            try
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = await _quartzStartup.Stop(guid), code = "0" });
            }
            catch (SchedulerException se)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = se.ToString(), code = "-1" });
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}