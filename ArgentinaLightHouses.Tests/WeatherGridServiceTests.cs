using Azure.Data.Tables;
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

    [Theory]
    [InlineData(80)]
    [InlineData(82)]
    [InlineData(95)]
    [InlineData(99)]
    public void MapRecord_PreservesWeatherCodeBoundaries(int weatherCode)
    {
        var entity = new TableEntity
        {
            ["Name"] = "Faro Test",
            ["Date"] = "2026-07-25",
            ["Time"] = "12:00",
            ["TemperatureCelsius"] = -1.0,
            ["WindSpeedKmh"] = 64.0,
            ["WindDirectionDegrees"] = 180.0,
            ["WindchillCelsius"] = -5.0,
            ["WeatherCode"] = weatherCode
        };

        var record = WeatherGridService.MapRecord(entity);

        Assert.Equal(weatherCode, record.WeatherCode);
    }

    [Fact]
    public void MapRecord_DefaultsWeatherCodeToZero_WhenMissing()
    {
        var entity = new TableEntity
        {
            ["Name"] = "Faro Test",
            ["Date"] = "2026-07-25",
            ["Time"] = "12:00"
        };

        var record = WeatherGridService.MapRecord(entity);

        Assert.Equal(0, record.WeatherCode);
    }
}
