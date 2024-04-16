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
using Newtonsoft.Json.Linq;

class errResp
{
    public int error { get; set; }
    public string msg { get; set; }
}
class uidReq
{
    public string uid { get; set; }
    public string biography { get; set; }
}

namespace EmberFrameworksService.Controllers
{
    [EnableCors]
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _config;
        private string ConnectionString;
        private MySqlManager _mySqlManager = new();
        private UserManager _userManager = new();
        private Authentication _authentication;

        public UserController(IConfiguration config)
        {
            _config = config;
            _authentication = new(config);
            ConnectionString = config.GetConnectionString("main");
        }
        [HttpGet("GetInformation")]
        public async Task<IActionResult> GetUserInformation()
        {
            bool authenticated = await _authentication.IsAuthenticatedUser(Request);
            if (!authenticated)
            {
                return Ok(
                    returnErrorStatus("User isn't authenticated.")
                );
            }

            Dictionary<string, string> userReq = _authentication._getUserReq(Request);
            Dictionary<int, Dictionary<string, Object>> userInformation = _mySqlManager.ExecuteQuery(
                @"SELECT * FROM ambidextrous.users WHERE Id = @param1 LIMIT 1", 
                new []{ userReq["UID"] }, 
                    ConnectionString
                );
            User user = new();
            if (userInformation.Count == 0)
            {
                _mySqlManager.ExecuteNonQuery(@"INSERT INTO ambidextrous.users 
                                                        (Id, Biography, GeoLocation, CreateTime) VALUES
                                                         (@param1, 'Default biography given to the user by the system upon account creation.', @param2, current_timestamp())",
                    new []{ userReq["UID"], _authentication.GetClientIPAddress(Request) }!, ConnectionString);
                var d = _mySqlManager.ExecuteQuery("SELECT * FROM ambidextrous.users WHERE Id = @param1 LIMIT 1", new[] { userReq["UID"] }, ConnectionString);
                
                return Ok(d);
            }
            User userResult = _userManager.castSQLToUser(userInformation[0]);
            
            return Ok(userResult);
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> UserRegistration([FromBody] string json)
        {
            bool authenticated = await _authentication.IsAuthenticatedUser(Request);
            if (!authenticated)
            {
                return Ok(
                    returnErrorStatus("User isn't authenticated.")
                );
            }

            Dictionary<string, string> usr = _authentication._getUserReq(Request);
            var w = JsonConvert.DeserializeObject<uidReq>(json);
            if (w == null)
                w = new uidReq() { uid = "", biography = ""};
            w.uid = usr["UID"];
            var d = _mySqlManager.ExecuteQuery("SELECT * FROM ambidextrous.users WHERE Id = @param1 LIMIT 1", new[] { w.uid }, ConnectionString);
            if (d.Count == 0)
            {
                _mySqlManager.ExecuteNonQuery(@"INSERT INTO ambidextrous.users 
                                                        (Id, Biography, GeoLocation, CreateTime) VALUES
                                                         (@param1, 'Default biography given to the user by the system upon account creation.', @param2, current_timestamp())",
                    new []{ w.uid, _authentication.GetClientIPAddress(Request) }!, ConnectionString);
                d = _mySqlManager.ExecuteQuery("SELECT * FROM ambidextrous.users WHERE Id = @param1 LIMIT 1", new[] { w.uid }, ConnectionString);
                
                return Ok(d);
            }
            return Ok(d);
        }

        private errResp returnErrorStatus(string msg)
        {
            return new errResp()
            {
                error = 401,
                msg = msg
            };
        }
    }
}
