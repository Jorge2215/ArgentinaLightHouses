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
    public async Task GetRecordsAsync_WhenConnectionStringIsEmpty_ReturnsEmptyList()
    {
        var service = CreateService(string.Empty);
        var dateFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var dateTo = DateOnly.FromDateTime(DateTime.UtcNow);

        var result = await service.GetRecordsAsync(dateFrom, dateTo);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRecordsAsync_WhenConnectionStringIsNull_ReturnsEmptyList()
    {
        var service = CreateService(null);
        var dateFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var dateTo = DateOnly.FromDateTime(DateTime.UtcNow);

        var result = await service.GetRecordsAsync(dateFrom, dateTo);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
