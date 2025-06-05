using Service.Models;

namespace Service.Client;

public interface IFundaClient
{
    public Task<FundaRealEstateAgentsOffers?> GetFundaRealEstateAgentsOffers(string type, string[] filterParam, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default);
}