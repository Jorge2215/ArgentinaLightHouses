using ArgentinaLightHouses.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ArgentinaLightHouses.Tests;

public class WeatherGridServiceTests
{
    private static WeatherGridService CreateService(string? connectionString)
    {
        var config = new Mock<IConfiguration>();
        config.Setup(c => c["AzureStorageConnection"]).Returns(connectionString);
        return new WeatherGridService(config.Object, NullLogger<WeatherGridService>.Instance);
    }

    [Fact]
    public async Task GetRecentRecordsAsync_WhenConnectionStringIsEmpty_ReturnsEmptyList()
    {
        var service = CreateService(string.Empty);

        var result = await service.GetRecentRecordsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRecentRecordsAsync_WhenConnectionStringIsNull_ReturnsEmptyList()
    {
        var service = CreateService(null);

        var result = await service.GetRecentRecordsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
