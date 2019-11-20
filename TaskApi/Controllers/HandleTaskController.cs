using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using TaskApi.Common;

namespace TaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandleTaskController : ControllerBase
    {
        [HttpPost]
        public async Task<string> Post()
        {
            //var jObject =  HttpContext.Request.ContentType;
            //return jObject;
            //if (doTaskJson.OpenSqlGuid == "10009")
            //{

            //    return JsonConvert.SerializeObject
            //    (new ResponseCommon
            //    {
            //        code = "0",
            //        msg = ""
            //    });
            //}

            return JsonConvert.SerializeObject
                (new ResponseCommon
                {
                    code = "-1",
                    msg = "没有对应计划的处理逻辑"
                });
        }
    }
}