using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoSchedule.Dtos.Data;
using Microsoft.AspNetCore.Mvc;
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
    }
}