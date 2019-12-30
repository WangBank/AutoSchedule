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
    public class SystemKeyController : Controller
    {
        private SqlLiteContext _SqlLiteContext;
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
        public async Task<string> SystemKeyResult(int page, int limit)
        {
            var skeyAll = _SqlLiteContext.SystemKeys.AsNoTracking();
            var skey = await skeyAll.Skip((page - 1) * limit).Take(limit).ToListAsync();
            List<SystemKeyModel> data = new List<SystemKeyModel>();
            foreach (var item in skey)
            {
                data.Add(new SystemKeyModel { KeyName = item.KeyName, KeyValue = item.KeyValue });
            }
            return System.Text.Json.JsonSerializer.Serialize(new SystemKeyData { msg = "", count = skeyAll.Count(), code = 0, data = data });
        }

        public IActionResult SystemKeyAdd()
        {
            return View();
        }

        [HttpPost]
        //string FType, string GUID, string Name, string IsStart, string MainKey, string GroupSqlString, string SqlString, string AfterSqlString, string AfterSqlstring2
        public async Task<string> SystemKeyAdd([FromBody]SystemKey systemKeyAddIn)
        {
            //判断当前是否有重复
            if (_SqlLiteContext.SystemKeys.AsNoTracking().Contains(systemKeyAddIn))
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "已有重复的项，请检查后重试", code = "-1" });
            }
            Dtos.Models.SystemKey systemKeyAdd = new Dtos.Models.SystemKey
            {
                KeyName = systemKeyAddIn.KeyName,
                KeyValue = systemKeyAddIn.KeyValue
            };
            await _SqlLiteContext.SystemKeys.AddAsync(systemKeyAdd);
            var addresult = await _SqlLiteContext.SaveChangesAsync();
            if (addresult > 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "", code = "0" });
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "新增系统变量失败", code = "-1" });
            }
        }

        public async Task<IActionResult> SystemKeyEdit(string KeyName)
        {
            var ds = await _SqlLiteContext.SystemKeys.AsNoTracking().Where(o => o.KeyName == KeyName).FirstOrDefaultAsync();
            return View(ds);
        }

        [HttpPost]
        public async Task<string> SystemKeyEdit([FromBody]SystemKey systemKeyAddIn)
        {
            var dsdelete = await _SqlLiteContext.SystemKeys.AsNoTracking().Where(o => o.KeyName == systemKeyAddIn.KeyName).FirstOrDefaultAsync();
            _SqlLiteContext.SystemKeys.Remove(dsdelete);
            if (await _SqlLiteContext.SaveChangesAsync() == 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "修改组织机构失败", code = "-1" });
            }
            Dtos.Models.SystemKey systemKeyadd = new SystemKey
            {
                KeyName = systemKeyAddIn.KeyName,
                KeyValue = systemKeyAddIn.KeyValue
            };
            await _SqlLiteContext.SystemKeys.AddAsync(systemKeyadd);
            var addresult = await _SqlLiteContext.SaveChangesAsync();
            if (addresult > 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "", code = "0" });
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "修改组织变量失败", code = "-1" });
            }
        }

        [HttpGet]
        public async Task<string> SystemKeyDelete(string KeyName)
        {
            var dsdelete = _SqlLiteContext.SystemKeys.AsNoTracking().Where(o => o.KeyName == KeyName).FirstOrDefault();
            _SqlLiteContext.SystemKeys.Remove(dsdelete);
            if (await _SqlLiteContext.SaveChangesAsync() > 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "", code = "0" });
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "删除系统变量失败", code = "-1" });
            }
        }
    }
}