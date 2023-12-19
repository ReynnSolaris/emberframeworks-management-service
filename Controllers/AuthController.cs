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

    [HttpGet("dev/GetToken_Dev/{UID}")]
    public async Task<string> GetToken_Dev(string UID)
    {
        return await _firebase.GetUserToken(UID);
    }
    [HttpGet("dev/GetUser_Dev/{UID}")]
    public async Task<UserRecord> GetUser_Dev(string UID)
    {
        return await _firebase.getUser(UID);
    }
    
    [Authorize]
    [HttpGet("dev/UserAuthRequest")]
    public string UserAuthRequest()
    {
        return Request.Headers["Authorization"];
    }

    [HttpGet("UserAuthenticationVerification/")]
    public async Task<bool> UserAuthenticationVerification()
    {
        try
        {
            return await auth.IsAuthenticatedUser(Request);
        }
        catch (Exception e)
        {}

        return false;
    }
    
    
}

