using EmberFrameworksService.Managers;
using EmberFrameworksService.Managers.SquareAPI;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmberFrameworksService.Controllers
{
    [EnableCors]
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class SquareController : ControllerBase
    {
        SquareManager sq;
        private Authentication _authentication;


        public SquareController(IConfiguration config)
        {
            sq = new SquareManager(config);
            _authentication = new(config);
        }

        [HttpGet("CatalogItems")]
        public async Task<IActionResult> getCatalog()
        {
            bool authenticated = _authentication.IsAuthenticatedAPIRequest(Request);
            if (!authenticated)
            {
                return Unauthorized();
            }
            try
            {
                return Ok((await sq.GetCatalog()).Objects);
            } catch(Exception e)
            {
                return BadRequest();
            }
            return BadRequest();
        }
    }
}
