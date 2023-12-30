using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Helpers;
using EmberFrameworksService.Managers;
using EmberFrameworksService.Managers.SQL;
using EmberFrameworksService.Models.UserInformation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EmberFrameworksService.Controllers
{
    [EnableCors]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _config;
        private string ConnectionString;
        private MySqlManager _mySqlManager = new();
        private UserManager _userManager = new();
        private Authentication _authentication = new();

        public UserController(IConfiguration config)
        {
            _config = config;
            ConnectionString = config.GetConnectionString("main");
        }
        [Produces("application/json")]
        [HttpGet("GetUserInformation")]
        public async Task<IActionResult> GetUserInformation()
        {
            bool authenticated = await _authentication.IsAuthenticatedUser(Request);
            if (!authenticated)
            {
                return Unauthorized();
            }

            Dictionary<string, string> userReq = _authentication._getUserReq(Request);
            Dictionary<int, Dictionary<int, Object>> userInformation = _mySqlManager.ExecuteQuery(
                @"SELECT * FROM ambidextrous.users WHERE Id = @param1 LIMIT 1", 
                new []{ userReq["UID"] }, 
                    ConnectionString
                );
            User user = new();
            if (userInformation.Count == 0)
            {
                return Ok(user);
            }
            User userResult = _userManager.castSQLToUser(userInformation[0]);
            
            return Ok(userResult);
        }
    }
}
