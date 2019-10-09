using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Controllers
{
    public class OrganizationController : Controller
    {
        SqlLiteContext _SqlLiteContext;
        private readonly ILogger<OrganizationController> _logger;

        public OrganizationController(ILogger<OrganizationController> logger, SqlLiteContext SqlLiteContext)
        {
            _SqlLiteContext = SqlLiteContext;
            _logger = logger;
        }
        public IActionResult Organization()
        {
            return View();
        }

        /// <summary>
        /// 组织机构
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> OrgResult()
        {
            var org = await _SqlLiteContext.OrgSetting.AsNoTracking().OrderBy(o=>o.CODE).ToListAsync();
            List<OrganizationModel> data = new List<OrganizationModel>();
            foreach (var item in org)
            {
                data.Add(new OrganizationModel { orgName = item.NAME, orgNum = item.CODE });
            }
            string result = System.Text.Json.JsonSerializer.Serialize<OrganizationData>(new OrganizationData { msg = "", count = data.Count, code = 0, data = data });
            return Content(result);
        }



        public IActionResult OrgAdd()
        {
            return View();
        }
    }
}