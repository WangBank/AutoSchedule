using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AutoScheduleApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("api/identity")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "admin")]
        public object GetUserClaims()
        {
            return User.Claims.Select(r => new { r.Type, r.Value });
        }
    }
}
