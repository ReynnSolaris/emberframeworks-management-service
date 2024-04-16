using System.Net.Http.Headers;
using EmberFrameworksService.Managers;
using EmberFrameworksService.Managers.SQL;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EmberFrameworksService.Controllers;

[EnableCors]
[Produces("application/json")]
[Route("[controller]")]
[ApiController]
public class RobloxController: ControllerBase
{
    private IConfiguration _config;
    private string ConnectionString;
    private MySqlManager _mySqlManager = new();
    private UserManager _userManager = new();
    private Authentication _authentication;
    private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("https://api.blox.link/v4/public/guilds/1025474328390807684"),
    };
    
    public RobloxController(IConfiguration config)
    {
        _config = config;
        _authentication = new(config);
        ConnectionString = config.GetConnectionString("rbx_data");
        
    }
    
    async Task<string> GetAsync(HttpClient httpClient, int uid)
    {
        using (var requestMessage =
               new HttpRequestMessage(HttpMethod.Get, $"https://api.blox.link/v4/public/guilds/1025474328390807684/roblox-to-discord/{uid}"))
        {
            requestMessage.Headers.TryAddWithoutValidation("Authorization", _config["bloxlink_key"]);
    
            var response = await httpClient.SendAsync(requestMessage);
            
            return await response.Content.ReadAsStringAsync();
        }
    }

    [HttpGet("mailbox/{uid}")]
    public IActionResult UserMailboxCreation(int uid)
    {
        bool authenticated = _authentication.IsAuthenticatedAPIRequest(Request);
        if (!authenticated)
        {
            return Unauthorized();
        }
        
        if (uid == null)
            return BadRequest("Invalid user id specified.");

        var data = _mySqlManager.ExecuteQuery("SELECT * FROM v_mailbox WHERE roblox_user_id = @param1 and DateDeleted is null limit 1;", new[] { uid.ToString() }, ConnectionString);
        if (data.Count == 0)
        {
            _mySqlManager.ExecuteNonQuery("INSERT INTO v_mailbox (roblox_user_id, unread_mail_count, DateCreated) VALUES (@param1, 0, current_timestamp());", new []{ uid.ToString() }, ConnectionString);
            data = _mySqlManager.ExecuteQuery("SELECT * FROM v_mailbox WHERE roblox_user_id = @param1 and DateDeleted is null limit 1;", new[] { uid.ToString() }, ConnectionString);
        }
        
        return Ok(data);
    }
    
    [HttpGet("mailbox/{uid}/mail")]
    public IActionResult UserMailbox(int uid)
    {
        bool authenticated = _authentication.IsAuthenticatedAPIRequest(Request);
        if (!authenticated)
        {
            return Unauthorized();
        }
        
        if (uid == null)
            return BadRequest("Invalid user id specified.");

        var data = _mySqlManager.ExecuteQuery("SELECT * FROM v_mailbox WHERE roblox_user_id = @param1 and DateDeleted is null limit 1;", new[] { uid.ToString() }, ConnectionString);
        if (data.Count == 0)
        {
            return BadRequest("User doesn't own a mailbox in db.");
        }

        var mail = _mySqlManager.ExecuteQuery(
            @"SELECT md.*, receiver.roblox_user_id 'Receiver', sender.roblox_user_id 'Sender' FROM mail_data md 
                        inner join v_mailbox receiver on receiver.Id = md.to_mailbox and receiver.DateDeleted is null
                        inner join v_mailbox sender on sender.Id = md.from_mailbox and sender.DateDeleted is null
                        where md.to_mailbox = @param1 and md.DateDeleted is null order by md.DateSent",
            new[] { data[0]["Id"].ToString() }!,
            ConnectionString
        );
        
        return Ok(mail);
    }

    [HttpGet("verify/{uid}")]
    [Produces("application/json")]
    public async Task<IActionResult> UserVerify(int uid)
    {
        bool authenticated = _authentication.IsAuthenticatedAPIRequest(Request);
        if (!authenticated)
        {
            return Unauthorized();
        }
        
        if (uid == null)
            return BadRequest("Invalid user id specified.");

        var verification = await GetAsync(sharedClient, uid);
        return Ok(verification);
    }
}