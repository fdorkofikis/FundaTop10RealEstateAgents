using AutoFixture;
using Moq;
using FluentAssertions;
using Service.Models;
using Service.Services;

namespace Service.Tests;

public class RealEstateAgentsCachedServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly Mock<IRealEstateAgentsService> _serviceMock;

    private readonly IRealEstateAgentsService _realEstateAgentsService;

    public RealEstateAgentsCachedServiceTests()
    {
        _fixture = new Fixture();
        _cacheMock = new Mock<ICacheService>();
        _serviceMock = new Mock<IRealEstateAgentsService>();

        _realEstateAgentsService = new RealEstateAgentsCachedService(_cacheMock.Object, _serviceMock.Object);
    }

    [Fact]
    public async Task GetTop10RealEstateAgents_IfCacheHasValue_ReturnFromCache()
    {
        // Arrange
        var agents = _fixture.CreateMany<Top10RealEstateAgent>(10).ToList();

        _cacheMock.Setup(m => m.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<Top10RealEstateAgent>>>>()))
            .ReturnsAsync(agents);

        // Act
        var result = await _realEstateAgentsService.GetTop10RealEstateAgents(["test"], CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(10);
        _serviceMock.Verify(x => x.GetTop10RealEstateAgents(It.IsAny<string[]>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    //Did use the help of ChatGPT to write this test
    [Fact]
    public async Task GetTop10RealEstateAgents_IfCacheMiss_CallsServiceAndCachesValue()
    {
        // Arrange
        var agents = _fixture.CreateMany<Top10RealEstateAgent>(10).ToList();

        _cacheMock
            .Setup(m => m.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<Top10RealEstateAgent>>>>()))
            .Returns<string, Func<Task<IEnumerable<Top10RealEstateAgent>>>>(async (key, factory) => await factory());

        _serviceMock
            .Setup(s => s.GetTop10RealEstateAgents(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(agents);

        // Act
        var result = await _realEstateAgentsService.GetTop10RealEstateAgents(["test"], CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(10);
        _serviceMock.Verify(x => x.GetTop10RealEstateAgents(It.IsAny<string[]>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}