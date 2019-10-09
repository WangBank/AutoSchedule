using AutoSchedule.Common;
using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using AutoSchedule.Dtos.RequestIn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
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
        public async Task<string> OrgResult()
        {
            var org = await _SqlLiteContext.OrgSetting.AsNoTracking().OrderBy(o=>o.CODE).ToListAsync();
            List<OrganizationModel> data = new List<OrganizationModel>();
            foreach (var item in org)
            {
                data.Add(new OrganizationModel { orgName = item.NAME, orgNum = item.CODE });
            }
            return System.Text.Json.JsonSerializer.Serialize(new OrganizationData { msg = "", count = data.Count, code = 0, data = data });
        }



        public IActionResult OrgAdd()
        {
            return View();
        }

        [HttpPost]
        //string FType, string GUID, string Name, string IsStart, string MainKey, string GroupSqlString, string SqlString, string AfterSqlString, string AfterSqlstring2
        public async Task<string> OrgAdd([FromBody]Organization organizationAddIn)
        {
            //判断当前是否有重复
            if (_SqlLiteContext.OrgSetting.AsNoTracking().Contains(organizationAddIn))
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "已有重复的项，请检查后重试", code = "-1" });
            }
            Dtos.Models.Organization organizationAdd = new Dtos.Models.Organization
            {
                CODE = organizationAddIn.CODE,
                NAME = organizationAddIn.NAME,
                DataBaseName = organizationAddIn.DataBaseName,
                DBType = organizationAddIn.DBType,
                Password = organizationAddIn.Password,
                ServerName = organizationAddIn.ServerName,
                UserName = organizationAddIn.UserName
            };
            await _SqlLiteContext.OrgSetting.AddAsync(organizationAdd);
            var addresult = await _SqlLiteContext.SaveChangesAsync();
            if (addresult > 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "", code = "0" });
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "新增组织机构失败", code = "-1" });
            }
        }

        public async Task<IActionResult> OrgEdit(string orgNum)
        {
            var ds = await _SqlLiteContext.OrgSetting.AsNoTracking().Where(o => o.CODE == orgNum).FirstOrDefaultAsync();
            return View(ds);
        }

        [HttpPost]
        public async Task<string> OrgEdit([FromBody]Organization organizationIn)
        {

            var dsdelete = await _SqlLiteContext.OrgSetting.AsNoTracking().Where(o => o.CODE == organizationIn.CODE).FirstOrDefaultAsync();
            _SqlLiteContext.OrgSetting.Remove(dsdelete);
            if (await _SqlLiteContext.SaveChangesAsync() == 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "修改组织机构失败", code = "-1" });
            }
            Dtos.Models.Organization organizationadd = new Organization
            {
                CODE = organizationIn.CODE,
                NAME = organizationIn.NAME,
                DataBaseName = organizationIn.DataBaseName,
                DBType = organizationIn.DBType,
                Password = organizationIn.Password,
                ServerName = organizationIn.ServerName,
                UserName = organizationIn.UserName
            };
            await _SqlLiteContext.OrgSetting.AddAsync(organizationadd);
            var addresult = await _SqlLiteContext.SaveChangesAsync();
            if (addresult > 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "", code = "0" });
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "修改组织机构失败", code = "-1" });
            }
        }

        [HttpGet]
        public async Task<string> OrgDelete(string orgNum)
        {
            var dsdelete = _SqlLiteContext.OrgSetting.AsNoTracking().Where(o => o.CODE == orgNum).FirstOrDefault();
            _SqlLiteContext.OrgSetting.Remove(dsdelete);
            if (await _SqlLiteContext.SaveChangesAsync() > 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "", code = "0" });

            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "删除组织机构失败", code = "-1" });
            }
        }

        [HttpPost]
        //string FType, string GUID, string Name, string IsStart, string MainKey, string GroupSqlString, string SqlString, string AfterSqlString, string AfterSqlstring2
        public async Task<string> OrgTestConnect([FromBody]Organization organizationAddIn)
        {
            //判断当前是否有重复
            //if (_SqlLiteContext.OrgSetting.AsNoTracking().Contains(organizationAddIn))
            //{
            //    return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "已有重复的项，请检查后重试", code = "-1" });
            //}
            string conString = "";
            bool result = false;
            try
            {
                switch (organizationAddIn.DBType)
                {
                    //Oracle 0
                    //SqlServer  1
                    //MySql  2
                    case "0":
                        //User Id=dbo;Password=romens;Data Source=192.168.100.9:1521/NewStddata;
                        conString = $"User Id={organizationAddIn.UserName};Password={organizationAddIn.Password};Data Source={organizationAddIn.ServerName}/{organizationAddIn.DataBaseName};";
                        result = await TestConnect.ConnectOracleAsync(conString);
                        break;
                    case "1":
                        //"data source=*.*.*.*;initial catalog=mcudata;user id=sa;password=sa;"

                        conString = $"data source={organizationAddIn.ServerName.Replace(":", ",")};initial catalog={organizationAddIn.DataBaseName};user id={organizationAddIn.UserName};password={organizationAddIn.Password}";
                        result = await TestConnect.ConnectSqlServerAsync(conString);
                        break;
                    case "2":
                        string name = "";
                        if (!organizationAddIn.ServerName.Contains(':'))
                        {
                            name = organizationAddIn.ServerName + ":3306";
                        }
                        string[] mysqlIp = name.Split(':');
                        //server=192.168.5.7;port=3306;database=beta-user;uid=root;pwd=Wq-.1997315421;CharSet=utf8
                        conString = $"server={mysqlIp[0]};port={mysqlIp[1]};database={organizationAddIn.DataBaseName};uid={organizationAddIn.UserName};pwd={organizationAddIn.Password};CharSet=utf8";
                        result = await TestConnect.ConnectMysqlAsync(conString);
                        break;
                    case "3":
                        break;
                    case "4":
                        break;

                    default:
                        break;
                }
            }
            catch (System.Exception ex)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "测试连接失败，请检查填入的信息后重试！", code = "-1" });
                throw;
            }

            if (result)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "测试连接成功！", code = "0" });
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "测试连接失败，请检查填入的信息后重试！", code = "-1" });
            }

        }
    }
}