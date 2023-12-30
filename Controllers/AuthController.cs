using System.Net;
using EmberFrameworksService.Managers;
using EmberFrameworksService.Managers.Firebase;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace EmberFrameworksService.Controllers;

[EnableCors]
[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private FirebaseManager _firebase = new ();
    private Authentication auth = new();

    [HttpGet("dev/GetToken_Dev/{uid}")]
    public async Task<string> GetToken_Dev(string uid)
    {
        return await _firebase.GetUserToken(uid);
    }
    [HttpGet("dev/GetUser_Dev/{uid}")]
    public async Task<UserRecord> GetUser_Dev(string uid)
    {
        return await _firebase.getUser(uid);
    }
    
    [HttpGet("GetIPAddress")]
    public string? UserAuthRequest()
    {
        return auth.GetClientIPAddress(Request);
    }

    [HttpGet("UserAuthenticationVerification/")]
    public async Task<bool> UserAuthenticationVerification()
    {
        try
        {
            return await auth.IsAuthenticatedUser(Request);
        }
        catch (Exception e)
        {
            throw e;
        }

        return false;
    }
    
    
}

