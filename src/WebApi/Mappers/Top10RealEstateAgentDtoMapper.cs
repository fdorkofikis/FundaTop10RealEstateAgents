using Service.Models;
using WeApi.Dtos;

namespace WeApi.Mappers;

public static class Top10RealEstateAgentDtoMapper
{
    public static Top10RealEstateAgentDto Map(Top10RealEstateAgent agent)
    {
        return new Top10RealEstateAgentDto
        {
            Id = agent.Id,
            Name = agent.Name,
            TotalObjects = agent.TotalObjects
        };
    }
    
    public static IEnumerable<Top10RealEstateAgentDto> Map(IEnumerable<Top10RealEstateAgent> agents)
    {
        return agents.Select(Map);
    }
}