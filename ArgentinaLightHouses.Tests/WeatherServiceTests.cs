using System.Globalization;
using System.Net;
using System.Text;
using ArgentinaLightHouses.Models;
using ArgentinaLightHouses.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace ArgentinaLightHouses.Tests;

public class WeatherServiceTests
{
    private static IMemoryCache NewCache() =>
        new MemoryCache(new MemoryCacheOptions());

    private static WeatherService BuildService(HttpMessageHandler handler, IMemoryCache? cache = null) =>
        new(new HttpClient(handler), NullLogger<WeatherService>.Instance, cache ?? NewCache());

    [Fact]
    public async Task GetWeatherAsync_SuccessfulFetch_ReturnsPopulatedWeatherInfo()
    {
        const string json = """
            {
              "current": {
                "temperature_2m": 13.7,
                "wind_speed_10m": 24.9,
                "weather_code": 61
              }
            }
            """;
        var originalCulture = CultureInfo.CurrentCulture;
        Uri? requestedUri = null;
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => requestedUri = request.RequestUri)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        var service = BuildService(handler.Object);

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("es-AR");

            var result = await service.GetWeatherAsync(-38.2833, -57.8333);

            Assert.NotNull(result);
            Assert.Equal(13.7, result!.TemperatureC);
            Assert.Equal(24.9, result.WindSpeedKmh);
            Assert.Equal(61, result.WeatherCode);
            Assert.NotNull(requestedUri);
            Assert.Contains("latitude=-38.2833", requestedUri!.Query);
            Assert.Contains("longitude=-57.8333", requestedUri.Query);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public async Task GetWeatherAsync_HttpErrorResponse_ReturnsNullGracefully()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var service = BuildService(handler.Object);

        var exception = await Record.ExceptionAsync(() => service.GetWeatherAsync(-40.0, -60.0));
        var result = await service.GetWeatherAsync(-40.0, -60.0);

        Assert.Null(exception);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetWeatherAsync_SecondCall_ReturnsCachedResultWithoutHttpRequest()
    {
        const string json = """
            {
              "current": {
                "temperature_2m": 10.0,
                "wind_speed_10m": 15.0,
                "weather_code": 0
              }
            }
            """;
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        var service = BuildService(handler.Object);

        var first = await service.GetWeatherAsync(-34.0, -58.0);
        var second = await service.GetWeatherAsync(-34.0, -58.0);

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.Same(first, second);  // exact same cached object reference
        // HTTP handler called exactly once — second result came from cache
        handler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Theory]
    [InlineData(0, "Clear sky")]
    [InlineData(95, "Thunderstorm")]
    public void WeatherDescription_ReturnsExpectedStringForKnownCodes(int weatherCode, string expected)
    {
        var weather = new WeatherInfo { WeatherCode = weatherCode };

        Assert.Equal(expected, weather.WeatherDescription);
    }

    [Fact]
    public void WeatherDescription_UnknownCode_ReturnsUnknown()
    {
        var weather = new WeatherInfo { WeatherCode = 999 };

        Assert.Equal("Unknown", weather.WeatherDescription);
    }

    [Theory]
    [InlineData(0, "☀️")]
    [InlineData(45, "🌫️")]
    [InlineData(95, "⛈️")]
    public void WeatherIcon_ReturnsExpectedIconForKnownCodes(int weatherCode, string expected)
    {
        var weather = new WeatherInfo { WeatherCode = weatherCode };

        Assert.Equal(expected, weather.WeatherIcon);
    }
}
