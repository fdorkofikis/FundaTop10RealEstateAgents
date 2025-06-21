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

        List<Task<FundaRealEstateAgentsOffers?>> tasks = [];
        FundaRealEstateAgentsOffers?[] fundaRealEstateAgentsOffers = [];

        for (int i = 2; i < response.Paging.AantalPaginas; i++)
        {
            tasks.Add(_client.GetFundaRealEstateAgentsOffers(Type, filterParam, i, cancellationToken: cancellationToken));
        }
        
        while (true) 
        {
            List<Task<FundaRealEstateAgentsOffers?>> batchedTasks = tasks.Take(50).ToList();
            fundaRealEstateAgentsOffers = await Task.WhenAll(batchedTasks);
            tasks.RemoveRange(0, batchedTasks.Count);

            if (!tasks.Any())
            {
                break;
            }

            Log.Information("Waiting 30 seconds before the next batch, to avoid time out.");
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        }

        foreach (FundaRealEstateAgentsOffers? fundaRealEstateAgentsOffer in fundaRealEstateAgentsOffers)
        {
            if (fundaRealEstateAgentsOffer?.Objects is null) continue;
            
            fundaObjects.AddRange(fundaRealEstateAgentsOffer.Objects);
            Log.Information("Found {objectCount}/{totalObjects} real-estate-agent objects.", fundaObjects.Count, fundaRealEstateAgentsOffer.TotaalAantalObjecten);
        }

        return GetTop10RealEstateAgents(fundaObjects);
    }

    private static IEnumerable<Top10RealEstateAgent> GetTop10RealEstateAgents(List<FundaObject>? fundaObjects)
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