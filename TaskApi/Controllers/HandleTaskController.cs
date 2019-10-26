using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskApi.Common;

namespace TaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandleTaskController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post ()
        {
            return await System.Text.Json.JsonSerializer.SerializeAsync(new ResponseCommon { 
             code = "0",
             msg = ""
            });
        
        }
    }

    
}