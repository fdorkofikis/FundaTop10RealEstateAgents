using Service.Models;

namespace Service.Services;

public interface ICacheService
{
    public IEnumerable<Top10RealEstateAgent>? GetTop10RealEstateAgents(string[] filterParam);
    public void SetTop10RealEstateAgents(string[] filterParam, IEnumerable<Top10RealEstateAgent> agents);
}