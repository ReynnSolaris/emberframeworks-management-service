using Microsoft.AspNetCore.Mvc;

namespace EmberFrameworksService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    [HttpGet("GetToken_Dev")]
    public string GetToken_Dev()
    {
        return "";
    }
}

