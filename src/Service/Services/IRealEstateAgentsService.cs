using Service.Objects;

namespace Service.Services;

public interface IRealEstateAgentsService
{
    Task<IEnumerable<Top10RealEstateAgent>> GetTop10RealEstateAgents(string[] filterParam, CancellationToken cancellationToken);
}