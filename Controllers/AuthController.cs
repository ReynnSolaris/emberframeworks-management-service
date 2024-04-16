using System.Net;
using EmberFrameworksService.Managers;
using EmberFrameworksService.Managers.Firebase;
using EmberFrameworksService.SMTP;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace EmberFrameworksService.Controllers;

[EnableCors]
[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private FirebaseManager _firebase = new ();
    private Authentication auth;
    private SendMail mail;

    public AuthController(IConfiguration config)
    {
        auth = new(config);
        mail = new(config);
    }

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

    [HttpGet("dev/SendEmail/{emailTo}")]
    public IActionResult SendEmailTo(string emailTo)
    {
        var smtp = mail.createClient();
        try
        {
            smtp.Send(
                "no-reply@emberframeworks.xyz", 
                emailTo != null ? emailTo : "rainn@emberframeworks.xyz", 
                "EmberFrameworks LLC",
                @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin pharetra, urna sit amet varius semper, ipsum dolor pellentesque est, ut eleifend diam risus et ipsum. Nam pellentesque magna lectus, at consectetur ipsum ultrices vitae. Phasellus vehicula sed tellus eu tristique. Curabitur feugiat eros a mi varius, id convallis neque imperdiet. Curabitur tempus arcu magna, sed pulvinar neque tempor ac. Sed eget tellus hendrerit lorem tempus porttitor eu id leo. Cras suscipit urna leo, quis pulvinar eros rhoncus nec. Donec id nibh at nisl scelerisque laoreet. Sed quis lectus tincidunt, convallis erat eu, sagittis magna.

Morbi tincidunt ac turpis ut bibendum. Curabitur in hendrerit nisi, nec mollis elit. Etiam rutrum consectetur dolor, nec ultricies nibh tempor rhoncus. Proin vel aliquet neque. Suspendisse nec tortor sodales, vulputate lectus sit amet, iaculis nibh. Donec eu pellentesque urna. Mauris quis dolor ac urna vestibulum mattis. Quisque viverra, odio eu tempus commodo, nisl lectus suscipit ante, eu egestas dui felis id tellus. Interdum et malesuada fames ac ante ipsum primis in faucibus. Donec eleifend cursus quam, eu scelerisque elit rutrum et. In interdum est sit amet velit gravida, ut varius arcu pellentesque. Nam convallis sapien arcu, nec facilisis nunc lacinia convallis. Suspendisse vel turpis fringilla, scelerisque justo eu, facilisis massa. Vestibulum finibus cursus eleifend. Praesent accumsan, nibh sit amet aliquet tempor, metus enim fringilla leo, eget ultricies libero mi at dolor."
            );
        } catch(Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.StackTrace);
        }
        return Ok();
    }

    [HttpGet("GetIPAddress")]
    public string? UserAuthRequest()
    {
        return auth.GetClientIPAddress(Request);
    }

    [HttpGet("Verification/")]
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

