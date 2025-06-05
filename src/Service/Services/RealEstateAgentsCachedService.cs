using Service.Models;

namespace Service.Services;

public class RealEstateAgentsCachedService: IRealEstateAgentsService
{
    private readonly ICacheService _cache;
    private readonly IRealEstateAgentsService _realEstateAgentsService;

    public RealEstateAgentsCachedService(ICacheService cache, IRealEstateAgentsService realEstateAgentsService)
    {
        _cache = cache;
        _realEstateAgentsService = realEstateAgentsService;
    }
    
    public async Task<IEnumerable<Top10RealEstateAgent>> GetTop10RealEstateAgents(string[] filterParam, CancellationToken cancellationToken)
    {
        var cacheKey = string.Join("_", filterParam);
        
        return await _cache.GetOrCreateAsync(cacheKey, 
            () => _realEstateAgentsService.GetTop10RealEstateAgents(filterParam, cancellationToken));
    }
}