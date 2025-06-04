using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Service.Config;
using Service.Objects;

namespace Service.Services;

public class InMemoryCacheService: ICacheService
{
    private readonly IMemoryCache _cache;
    private  readonly CacheConfig _options;

    public InMemoryCacheService(IMemoryCache cache, IOptions<CacheConfig> options)
    {
        _cache = cache;
        _options = options.Value;
    }
    
    public IEnumerable<Top10RealEstateAgent>? GetTop10RealEstateAgents(string[] filterParam)
    {
        var cacheKey = string.Join("_", filterParam);
        
        return _cache.TryGetValue(cacheKey, out IEnumerable<Top10RealEstateAgent>? agents) ? agents : null;
    }
    
    public void SetTop10RealEstateAgents(string[] filterParam, IEnumerable<Top10RealEstateAgent> agents)
    {
        var cacheKey = string.Join("_", filterParam);
        
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(_options.ExpirationInMinutes));

        _cache.Set(cacheKey, agents, cacheEntryOptions);
    }
}