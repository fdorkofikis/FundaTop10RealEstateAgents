using Microsoft.AspNetCore.Mvc;

namespace WeApi.Controllers;

[ApiController]
[Route("api/makelaars")]
public class RealEstateAgentsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTop10RealEstateAgents(CancellationToken cancellationToken)
    {
        return Ok();
    }
}