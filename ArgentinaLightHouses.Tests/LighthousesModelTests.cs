using ArgentinaLightHouses.Data;
using ArgentinaLightHouses.Models;
using ArgentinaLightHouses.Pages;
using ArgentinaLightHouses.Services;
using Moq;

namespace ArgentinaLightHouses.Tests;

public class LighthousesModelTests
{
    [Fact]
    public async Task OnGetAsync_LoadsLighthousesSuccessfully()
    {
        var expectedCount = LighthouseRepository.GetAll().Count;
        var weatherService = new Mock<IWeatherService>();
        weatherService
            .Setup(s => s.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(new WeatherInfo { TemperatureC = 14.2, WindSpeedKmh = 30.4, WeatherCode = 2 });
        var model = new TestableLighthousesModel(weatherService.Object);

        await model.OnGetAsync();

        Assert.Equal(expectedCount, model.Lighthouses.Count);
        Assert.All(model.Lighthouses, lighthouse => Assert.NotNull(lighthouse.Weather));
    }

    [Fact]
    public async Task OnGetAsync_HandlesNullWeatherGracefully()
    {
        var expectedCount = LighthouseRepository.GetAll().Count;
        var weatherService = new Mock<IWeatherService>();
        weatherService
            .Setup(s => s.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((WeatherInfo?)null);
        var model = new TestableLighthousesModel(weatherService.Object);

        await model.OnGetAsync();

        Assert.Equal(expectedCount, model.Lighthouses.Count);
        Assert.All(model.Lighthouses, lighthouse => Assert.Null(lighthouse.Weather));
    }

    [Fact]
    public async Task OnGetAsync_PopulatesLighthouseListOnModel()
    {
        var lighthouses = new List<Lighthouse>
        {
            new() { Name = "Alpha", Latitude = -40.5, Longitude = -60.5 },
            new() { Name = "Beta", Latitude = -41.5, Longitude = -61.5 }
        };
        var weatherService = new Mock<IWeatherService>();
        weatherService
            .Setup(s => s.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(new WeatherInfo { TemperatureC = 10, WindSpeedKmh = 12, WeatherCode = 3 });
        var model = new TestableLighthousesModel(weatherService.Object, lighthouses);

        await model.OnGetAsync();

        Assert.Collection(model.Lighthouses,
            lighthouse => Assert.Equal("Alpha", lighthouse.Name),
            lighthouse => Assert.Equal("Beta", lighthouse.Name));
    }

    private sealed class TestableLighthousesModel(IWeatherService weatherService, List<Lighthouse>? lighthouses = null)
        : LighthousesModel(weatherService)
    {
        private readonly List<Lighthouse> _lighthouses = lighthouses ?? LighthouseRepository.GetAll();

        protected override List<Lighthouse> GetLighthouses() => _lighthouses;
    }
}
