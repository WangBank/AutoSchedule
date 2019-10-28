using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskApi.Common;

namespace TaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandleTaskController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            return await System.Text.Json.JsonSerializer.SerializeAsync(new ResponseCommon
            {
                code = "0",
                msg = ""
            });
        }
    }
}