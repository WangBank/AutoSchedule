using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        IFreeSql _fsql;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IFreeSql fsql)
        {
            _fsql = fsql;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();

            int count = _fsql.Update<入库完成回传类>("2b37b7a4-42d6-46f2-996e-29aa1959be97")
                        .Set(a => a.CREATEUSER, "邱继续")
                        .Where(a => a.BILLNO == "0")
                        .ExecuteAffrows();

            //var 影响的行数 = _fsql.Insert<入库完成回传类>().AppendData(new 入库完成回传类
            //{
            //    STATUE = "0",
            //    CLPSORDERCODE = "0",
            //    BILLNO = "0",
            //    CONFIRMTYPE = "0",
            //    CREATEUSER = "0",
            //    GUID = Guid.NewGuid().ToString(),
            //    OPERATETIME = "0",
            //    ORDERTYPE = "0",
            //    OWNERCODE = "0",
            //    POORDERSTATUS = "0",
            //    WAREHOUSECODE = "0"
            //}).ExecuteAffrows();
            //var BILLGUID = new OracleParameter
            //{
            //    ParameterName = "BILLGUID",
            //    OracleDbType = OracleDbType.Varchar2,
            //    Direction = ParameterDirection.Input,
            //    Value = "qiujixuisaaaaaaa",
            //    Size = 50
            //};

            //var RETURNMSG = new OracleParameter
            //{
            //    ParameterName = "RETURNMSG",
            //    OracleDbType = OracleDbType.Varchar2,
            //    Direction = ParameterDirection.Output,
            //    Value = "qiujixuisaaaaaaa",
            //    Size = 50
            //};

            //var RETURNVALUE = new OracleParameter
            //{
            //    ParameterName = "RETURNVALUE",
            //    OracleDbType = OracleDbType.Decimal,
            //    Direction = ParameterDirection.Output,
            //    Value = "1",
            //    Size = 10
            //};
            //var ss =  _fsql.Ado.ExecuteNonQuery(System.Data.CommandType.StoredProcedure, "WMS_JDWMS_STOCKINRETURN", BILLGUID, RETURNMSG, RETURNVALUE);
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
