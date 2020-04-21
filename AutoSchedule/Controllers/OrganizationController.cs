using AutoSchedule.Common;
using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.Models;
using AutoSchedule.Dtos.RequestIn;
using BankDbHelper;
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
        private SqlLiteContext _SqlLiteContext;
        private readonly ILogger<OrganizationController> _logger;
        private string conString = "";

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
        public async Task<OrganizationData> OrgResult(int page, int limit)
        {
            var skeyAll = _SqlLiteContext.OrgSetting.AsNoTracking();
            List<Organization> org;
            if (page == 0 && limit == 0)
            {
                org = await skeyAll.ToListAsync();
            }
            else
            {
                org = await skeyAll.Skip((page - 1) * limit).Take(limit).ToListAsync();
            }
            List<OrganizationModel> data = new List<OrganizationModel>();
            foreach (var item in org)
            {
                data.Add(new OrganizationModel { orgName = item.NAME, orgNum = item.CODE });
            }
            return new OrganizationData { msg = "", count = skeyAll.Count(), code = 0, data = data };
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

            string connecting = GetConnectString(organizationAddIn);

            Dtos.Models.Organization orgCOn = new Dtos.Models.Organization
            {
                CODE = organizationAddIn.CODE,
                NAME = organizationAddIn.NAME,
                DataBaseName = organizationAddIn.DataBaseName,
                DBType = organizationAddIn.DBType,
                Password = organizationAddIn.Password,
                ServerName = organizationAddIn.ServerName,
                UserName = organizationAddIn.UserName,
                ConnectingString = connecting
            };
            await _SqlLiteContext.OrgSetting.AddAsync(orgCOn);
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
            var dsUpdate = await _SqlLiteContext.OrgSetting.Where(o => o.CODE == organizationIn.CODE).FirstOrDefaultAsync();
            string connecting = GetConnectString(organizationIn);
            dsUpdate.NAME = organizationIn.NAME;
            dsUpdate.DataBaseName = organizationIn.DataBaseName;
            dsUpdate.DBType = organizationIn.DBType;
            dsUpdate.Password = organizationIn.Password;
            dsUpdate.ServerName = organizationIn.ServerName;
            dsUpdate.UserName = organizationIn.UserName;
            dsUpdate.ConnectingString = connecting;
            _SqlLiteContext.OrgSetting.Update(dsUpdate);
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

        //删除的时候，移除任务计划中相应的code
        [HttpGet]
        public async Task<string> OrgDelete(string orgNum)
        {
            var orgdelete = await _SqlLiteContext.OrgSetting.AsNoTracking().Where(o => o.CODE == orgNum).FirstOrDefaultAsync();

            var dstk = await _SqlLiteContext.TaskPlan.AsNoTracking().Where(o => o.OrgCode == orgNum).ToListAsync();
            for (int i = 0; i < dstk.Count; i++)
            {
                dstk[i].OrgCode = "";
            }
            _SqlLiteContext.TaskPlan.UpdateRange(dstk);
            _SqlLiteContext.OrgSetting.Remove(orgdelete);

            if (await _SqlLiteContext.SaveChangesAsync() > 0)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "", code = "0" });
            }
            else
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "删除组织机构失败", code = "-1" });
            }
        }

        public string GetConnectString(Organization organizationAddIn)
        {
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

                        break;

                    case "1":
                        //"data source=*.*.*.*;initial catalog=mcudata;user id=sa;password=sa;"

                        conString = $"data source={organizationAddIn.ServerName.Replace(":", ",")};initial catalog={organizationAddIn.DataBaseName};user id={organizationAddIn.UserName};password={organizationAddIn.Password}";

                        break;

                    case "2":
                        string name = organizationAddIn.ServerName;
                        if (!organizationAddIn.ServerName.Contains(':'))
                        {
                            name = organizationAddIn.ServerName + ":3306";
                        }
                        string[] mysqlIp = name.Split(':');
                        //server=192.168.5.7;port=3306;database=beta-user;uid=root;pwd=Wq-.1997315421;CharSet=utf8
                        conString = $"server={mysqlIp[0]};port={mysqlIp[1]};database={organizationAddIn.DataBaseName};uid={organizationAddIn.UserName};pwd={organizationAddIn.Password};CharSet=utf8";

                        break;

                    default:
                        break;
                }
            }
            catch (System.Exception ex)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "测试连接失败，请检查填入的信息后重试！\r\n 错误信息:" + ex.Message, code = "-1" });
                throw;
            }
            return conString;
        }

        [HttpPost]
        //string FType, string GUID, string Name, string IsStart, string MainKey, string GroupSqlString, string SqlString, string AfterSqlString, string AfterSqlstring2
        public async Task<string> OrgSqlHelper([FromBody]Organization organizationAddIn)
        {
            ExecSqlHelper sqlHelper;
            bool result = false;
            string DBType = string.Empty;
            switch (organizationAddIn.DBType)
            {
                //Oracle 0
                //SqlServer  1
                //MySql  2
                case "0":
                    //User Id=dbo;Password=romens;Data Source=192.168.100.9:1521/NewStddata;
                    conString = $"User Id={organizationAddIn.UserName};Password={organizationAddIn.Password};Data Source={organizationAddIn.ServerName}/{organizationAddIn.DataBaseName};";
                    DBType = DBTypeEnum.Oracle.ToString();
                    break;

                case "1":
                    //"data source=*.*.*.*;initial catalog=mcudata;user id=sa;password=sa;"

                    conString = $"data source={organizationAddIn.ServerName.Replace(":", ",")};initial catalog={organizationAddIn.DataBaseName};user id={organizationAddIn.UserName};password={organizationAddIn.Password}";
                    DBType = DBTypeEnum.SqlServer.ToString();
                    break;

                case "2":
                    string name = organizationAddIn.ServerName;
                    if (!organizationAddIn.ServerName.Contains(":"))
                    {
                        name = organizationAddIn.ServerName + ":3306";
                    }
                    string[] mysqlIp = name.Split(':');
                    //server=192.168.5.7;port=3306;database=beta-user;uid=root;pwd=Wq-.1997315421;CharSet=utf8
                    conString = $"server={mysqlIp[0]};port={mysqlIp[1]};database={organizationAddIn.DataBaseName};uid={organizationAddIn.UserName};pwd={organizationAddIn.Password};CharSet=utf8";
                    DBType = DBTypeEnum.MySql.ToString();
                    break;

                default:
                    break;
            }

            sqlHelper = new ExecSqlHelper(conString, DBType);
            try
            {
                result = await sqlHelper.TestConnectionAsync();
            }
            catch (System.Exception ex)
            {
                return System.Text.Json.JsonSerializer.Serialize(new ResponseCommon { msg = "测试连接失败，请检查填入的信息后重试！\r\n 错误信息:" + ex.Message, code = "-1" });
                throw;
            }
            finally
            {
               await sqlHelper.DisposeAsync();
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