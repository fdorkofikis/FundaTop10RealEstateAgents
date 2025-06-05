using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Service.Config;
using Service.Models;

namespace Service.Services;

public class InMemoryCacheService: IRealEstateAgentsService
{
    private readonly IMemoryCache _cache;
    private readonly IRealEstateAgentsService _realEstateAgentsService;
    private  readonly CacheConfig _options;

    public InMemoryCacheService(IMemoryCache cache, IRealEstateAgentsService realEstateAgentsService, IOptions<CacheConfig> options)
    {
        _cache = cache;
        _realEstateAgentsService = realEstateAgentsService;
        _options = options.Value;
    }
    
    public async Task<IEnumerable<Top10RealEstateAgent>> GetTop10RealEstateAgents(string[] filterParam, CancellationToken cancellationToken)
    {
        var cacheKey = string.Join("_", filterParam);

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<Top10RealEstateAgent>? agents))
        {
            agents = await _realEstateAgentsService.GetTop10RealEstateAgents(filterParam, cancellationToken);
            SetTop10RealEstateAgents(filterParam, agents);
        }

        return agents;
    }
    
    private void SetTop10RealEstateAgents(string[] filterParam, IEnumerable<Top10RealEstateAgent> agents)
    {
        var cacheKey = string.Join("_", filterParam);
        
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(_options.ExpirationInMinutes));

        _cache.Set(cacheKey, agents, cacheEntryOptions);
    }
}