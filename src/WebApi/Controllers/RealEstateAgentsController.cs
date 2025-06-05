using Microsoft.AspNetCore.Mvc;
using Service.Services;
using WeApi.Mappers;

namespace WeApi.Controllers;

[ApiController]
[Route("api/real-estate-agents")]
public class RealEstateAgentsController : ControllerBase
{
    private readonly IRealEstateAgentsService _realEstateAgentsService;

    public RealEstateAgentsController(IRealEstateAgentsService realEstateAgentsService)
    {
        _realEstateAgentsService = realEstateAgentsService;
    }
    
    [HttpGet("/amsterdam/top10")]
    public async Task<IActionResult> GetTop10RealEstateAgentsAmsterdam(CancellationToken cancellationToken)
    {
        var result = await _realEstateAgentsService.GetTop10RealEstateAgents(["amsterdam"], cancellationToken);
        return Ok(Top10RealEstateAgentDtoMapper.Map(result));
    }
    
    [HttpGet("/amsterdam/garden/top10")]
    public async Task<IActionResult> GetTop10RealEstateAgentsAmsterdamGarden(CancellationToken cancellationToken)
    {
        var result = await _realEstateAgentsService.GetTop10RealEstateAgents(["amsterdam", "tuin"], cancellationToken);
        return Ok(Top10RealEstateAgentDtoMapper.Map(result));
    }
}