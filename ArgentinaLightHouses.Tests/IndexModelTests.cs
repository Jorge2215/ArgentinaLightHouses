using ArgentinaLightHouses.Data;
using ArgentinaLightHouses.Models;
using ArgentinaLightHouses.Pages;
using ArgentinaLightHouses.Services;
using Moq;

namespace ArgentinaLightHouses.Tests;

public class IndexModelTests
{
    [Fact]
    public async Task OnGetAsync_LoadsLighthousesSuccessfully()
    {
        var expectedCount = LighthouseRepository.GetAll().Count;
        var weatherService = new Mock<IWeatherService>();
        weatherService
            .Setup(s => s.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((double latitude, double longitude) => new WeatherInfo
            {
                TemperatureC = latitude,
                WindSpeedKmh = Math.Abs(longitude),
                WeatherCode = 1
            });
        var model = new TestableIndexModel(weatherService.Object);

        await model.OnGetAsync();

        Assert.Equal(expectedCount, model.Lighthouses.Count);
        Assert.All(model.Lighthouses, lighthouse =>
        {
            Assert.NotNull(lighthouse.Weather);
            Assert.Equal(lighthouse.Latitude, lighthouse.Weather!.TemperatureC);
            Assert.Equal(Math.Abs(lighthouse.Longitude), lighthouse.Weather.WindSpeedKmh);
        });
    }

    [Fact]
    public async Task OnGetAsync_HandlesNullWeatherGracefully()
    {
        var expectedCount = LighthouseRepository.GetAll().Count;
        var weatherService = new Mock<IWeatherService>();
        weatherService
            .Setup(s => s.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((WeatherInfo?)null);
        var model = new TestableIndexModel(weatherService.Object);

        await model.OnGetAsync();

        Assert.Equal(expectedCount, model.Lighthouses.Count);
        Assert.All(model.Lighthouses, lighthouse => Assert.Null(lighthouse.Weather));
    }

    [Fact]
    public async Task OnGetAsync_WithEmptyLighthouseList_LeavesModelEmpty()
    {
        var weatherService = new Mock<IWeatherService>();
        var model = new TestableIndexModel(weatherService.Object, []);

        await model.OnGetAsync();

        Assert.Empty(model.Lighthouses);
        weatherService.Verify(s => s.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
    }

    [Fact]
    public async Task OnGetAsync_InjectsWeatherBackIntoLighthouseObjectsAfterFetch()
    {
        var first = new Lighthouse { Name = "First", Latitude = -40.1, Longitude = -60.2 };
        var second = new Lighthouse { Name = "Second", Latitude = -41.3, Longitude = -61.4 };
        var firstWeather = new WeatherInfo { TemperatureC = 12.5, WindSpeedKmh = 20.1, WeatherCode = 0 };
        var secondWeather = new WeatherInfo { TemperatureC = 8.7, WindSpeedKmh = 35.2, WeatherCode = 95 };
        var weatherByCoordinates = new Dictionary<(double Latitude, double Longitude), WeatherInfo>
        {
            [(first.Latitude, first.Longitude)] = firstWeather,
            [(second.Latitude, second.Longitude)] = secondWeather
        };

        var weatherService = new Mock<IWeatherService>();
        weatherService
            .Setup(s => s.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((double latitude, double longitude) => weatherByCoordinates[(latitude, longitude)]);
        var model = new TestableIndexModel(weatherService.Object, [first, second]);

        await model.OnGetAsync();

        Assert.Same(firstWeather, model.Lighthouses[0].Weather);
        Assert.Same(secondWeather, model.Lighthouses[1].Weather);
    }

    private sealed class TestableIndexModel(IWeatherService weatherService, List<Lighthouse>? lighthouses = null)
        : IndexModel(weatherService)
    {
        private readonly List<Lighthouse> _lighthouses = lighthouses ?? LighthouseRepository.GetAll();

        protected override List<Lighthouse> GetLighthouses() => _lighthouses;
    }
}
