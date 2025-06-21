using Serilog;
using Service.Client;
using Service.Models;

namespace Service.Services;

public class RealEstateAgentsService : IRealEstateAgentsService
{
    private readonly ICacheService _cache;
    private readonly IFundaClient _client;

    private const string Type = "koop";

    public RealEstateAgentsService(ICacheService cache, IFundaClient client)
    {
        _cache = cache;
        _client = client;
    }

    public async Task<IEnumerable<Top10RealEstateAgent>> GetTop10RealEstateAgents(string[] filterParam, CancellationToken cancellationToken)
    {
        Log.Information("Getting top 10 real-estate-agents for {filterParam}", string.Join(", ", filterParam));

        var cacheKey = string.Join("_", filterParam);

        return await _cache.GetOrCreateAsync(cacheKey,
            () => GetAllRealEstateAgentsObjects(filterParam, cancellationToken));
        
    }
    
    private async Task<IEnumerable<Top10RealEstateAgent>> GetAllRealEstateAgentsObjects(string[] filterParam, CancellationToken cancellationToken)
    {
        FundaRealEstateAgentsOffers? response = await _client.GetFundaRealEstateAgentsOffers(Type, filterParam, cancellationToken: cancellationToken);
        if (response is null)
        {
            Log.Error("Failed to get objects for first page.");
            return new List<Top10RealEstateAgent>();
        }

        List<FundaObject> fundaObjects = response.Objects;
        Log.Information("Found {objectCount}/{totalObjects} real-estate-agent objects.", fundaObjects.Count, response.TotaalAantalObjecten);

        for (int i = 2; i < response.Paging.AantalPaginas; i++)
        {
            if (i == 100)
            {
                Log.Warning("Reached 100 pages limit, waiting one minute to overwrite the timeout.");
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }

            Log.Information("Getting page {page}/{totalPages} of real-estate-agent objects.", i, response.Paging.AantalPaginas);
            response = await _client.GetFundaRealEstateAgentsOffers(Type, filterParam, i, cancellationToken: cancellationToken);
            if (response?.Objects is null)
            {
                Log.Error("Failed to get objects for page {page}.", i);
                return GetTop10RealEstateAgents(fundaObjects);
            }

            fundaObjects.AddRange(response.Objects);
            Log.Information("Found {objectCount}/{totalObjects} real-estate-agent objects.", fundaObjects.Count, response.TotaalAantalObjecten);
        }

        Log.Information("Successfully retrieved {objectCount}/{totalObjects} real-estate-agent objects from {totalPages} pages.", fundaObjects.Count, response.TotaalAantalObjecten);

        return GetTop10RealEstateAgents(fundaObjects);
    }

    private static IEnumerable<Top10RealEstateAgent> GetTop10RealEstateAgents(List<FundaObject> fundaObjects)
    {
        if (fundaObjects is null)
        {
            return new List<Top10RealEstateAgent>();
        }

        return fundaObjects
            .Where(o => o.MakelaarId > 0)
            .GroupBy(o => o.MakelaarId)
            .Select(g => new Top10RealEstateAgent
            {
                Id = g.Key.GetValueOrDefault(),
                Name = g.FirstOrDefault()?.MakelaarNaam ?? "Unknown",
                TotalObjects = g.Count()
            })
            .OrderByDescending(m => m.TotalObjects)
            .Take(10);
    }
}