using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoSchedule.Controllers
{
    public class SystemKeyController : Controller
    {
        SqlLiteContext _SqlLiteContext;
        private readonly ILogger<SystemKeyController> _logger;

        public SystemKeyController(ILogger<SystemKeyController> logger, SqlLiteContext SqlLiteContext)
        {
            _SqlLiteContext = SqlLiteContext;
            _logger = logger;
        }
        public IActionResult SystemKey()
        {
            return View();
        }

        [HttpGet]
        public async Task<string> SystemKeyResult()
        {
            var skey = await _SqlLiteContext.SystemKeys.AsNoTracking().ToListAsync();
            List<SystemKeyModel> data = new List<SystemKeyModel>();
            foreach (var item in skey)
            {
                data.Add(new SystemKeyModel { KeyName = item.KeyName, KeyValue = item.KeyValue });
            }
            return System.Text.Json.JsonSerializer.Serialize(new SystemKeyData { msg = "", count = data.Count, code = 0, data = data });
        }
    }
}