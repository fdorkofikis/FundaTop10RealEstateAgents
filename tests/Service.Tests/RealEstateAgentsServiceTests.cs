using AutoFixture;
using Moq;
using FluentAssertions;
using Service.Client;
using Service.Models;
using Service.Services;

namespace Service.Tests;

public class RealEstateAgentsServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IFundaClient> _fundaClientMock;
    private readonly IRealEstateAgentsService _realEstateAgentsService;
    
    public RealEstateAgentsServiceTests()
    {
        _fixture = new Fixture();
        _fundaClientMock = new Mock<IFundaClient>();
        
        _realEstateAgentsService = new RealEstateAgentsService(_fundaClientMock.Object);
    }
    
    [Fact]
    public async Task GetTop10RealEstateAgents_FundaClientReturnsNull_ReturnEmptyList()
    {
        // Arrange
        _fundaClientMock.Setup(s => s.GetFundaRealEstateAgentsOffers(It.IsAny<string>(),It.IsAny<string[]>(), It.IsAny<int>(),It.IsAny<int>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null)
            .Verifiable(Times.Once());

        // Act
        var result = await _realEstateAgentsService.GetTop10RealEstateAgents(["test"], CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(0);
    }
    
    [Fact]
    public async Task GetTop10RealEstateAgents_FundaRealEstateAgentsOffersHasNullObjects_ReturnEmptyList()
    {
        // Arrange
        _fundaClientMock.Setup(s => s.GetFundaRealEstateAgentsOffers(It.IsAny<string>(),It.IsAny<string[]>(), It.IsAny<int>(),It.IsAny<int>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FundaRealEstateAgentsOffers
            {
                Objects = null,
                Paging = new Paging { AantalPaginas = 1 }
            }).Verifiable(Times.Once());

        // Act
        var result = await _realEstateAgentsService.GetTop10RealEstateAgents(["test"], CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(0);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(25)]
    public async Task GetTop10RealEstateAgents_Success_Returns10Agents(int pages)
    {
        // Arrange
        var fundaObjects = _fixture.CreateMany<FundaObject>(12).ToList();
        
        _fundaClientMock.Setup(s => s.GetFundaRealEstateAgentsOffers(It.IsAny<string>(),It.IsAny<string[]>(), It.IsAny<int>(),It.IsAny<int>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FundaRealEstateAgentsOffers
            {
                Objects = fundaObjects,
                Paging = new Paging { AantalPaginas = pages }
            }).Verifiable(Times.Exactly(pages));

        // Act
        var result = await _realEstateAgentsService.GetTop10RealEstateAgents(["test"], CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(10);
    }
}