using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Service.Config;

namespace Service.Services;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly CacheConfig _options;

    public InMemoryCacheService(IMemoryCache cache, IOptions<CacheConfig> options)
    {
        _cache = cache;
        _options = options.Value;
    }

    public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> factory)
    {
        return (await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.ExpirationInMinutes);
            return await factory();
        }))!;
    }
}