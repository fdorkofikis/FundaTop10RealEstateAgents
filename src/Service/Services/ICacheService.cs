namespace Service.Services;

public interface ICacheService
{
    public Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> factory);
}