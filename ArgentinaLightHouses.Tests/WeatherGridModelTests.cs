using ArgentinaLightHouses.Models;
using ArgentinaLightHouses.Pages;
using ArgentinaLightHouses.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ArgentinaLightHouses.Tests;

public class WeatherGridModelTests
{
    private static WeatherGridModel CreateModel(IWeatherGridService service)
        => new WeatherGridModel(service, NullLogger<WeatherGridModel>.Instance);

    private static List<WeatherRecord> SampleRecords(int count) =>
        Enumerable.Range(1, count).Select(i => new WeatherRecord
        {
            Name = $"Station {i}",
            Date = "2026-06-02",
            Time = $"{i:D2}:00",
            TemperatureCelsius = 10.0 + i,
            WindSpeedKmh = 20.0 + i,
            WindDirectionDegrees = 90.0,
            WindchillCelsius = 8.0 + i
        }).ToList();

    [Fact]
    public async Task OnGetAsync_PopulatesRecords_WhenServiceReturnsData()
    {
        var mockService = new Mock<IWeatherGridService>();
        mockService.Setup(s => s.GetRecentRecordsAsync(24)).ReturnsAsync(SampleRecords(3));
        var model = CreateModel(mockService.Object);

        await model.OnGetAsync();

        Assert.Equal(3, model.Records.Count);
        Assert.Empty(model.ErrorMessage);
    }

    [Fact]
    public async Task OnGetAsync_SetsEmptyRecords_WhenServiceReturnsEmpty()
    {
        var mockService = new Mock<IWeatherGridService>();
        mockService.Setup(s => s.GetRecentRecordsAsync(24)).ReturnsAsync(new List<WeatherRecord>());
        var model = CreateModel(mockService.Object);

        await model.OnGetAsync();

        Assert.Empty(model.Records);
        Assert.Empty(model.ErrorMessage);
    }

    [Fact]
    public async Task OnGetAsync_SetsErrorMessage_WhenServiceThrows()
    {
        var mockService = new Mock<IWeatherGridService>();
        mockService.Setup(s => s.GetRecentRecordsAsync(24))
            .ThrowsAsync(new Exception("Storage unavailable"));
        var model = CreateModel(mockService.Object);

        await model.OnGetAsync();

        Assert.NotEmpty(model.ErrorMessage);
        Assert.Empty(model.Records);
    }
}
