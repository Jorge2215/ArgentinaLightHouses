using System.Text.Json;
using ArgentinaLightHouses.Functions.Services;
using Microsoft.Extensions.Logging;

namespace ArgentinaLightHouses.Functions.Services;

public record WeatherReading(
    double TemperatureCelsius,
    double WindSpeedKmh,
    double WindDirectionDegrees,
    double WindchillCelsius
);

public class FunctionWeatherService
{
    private static readonly SemaphoreSlim _concurrencyLimiter = new(5, 5);
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FunctionWeatherService> _logger;

    public FunctionWeatherService(IHttpClientFactory httpClientFactory, ILogger<FunctionWeatherService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<WeatherReading?> FetchWeatherAsync(double latitude, double longitude)
    {
        await _concurrencyLimiter.WaitAsync();
        try
        {
            var url = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "https://api.open-meteo.com/v1/forecast?latitude={0}&longitude={1}&current=temperature_2m,wind_speed_10m,wind_direction_10m,apparent_temperature&wind_speed_unit=kmh&timezone=UTC",
                latitude, longitude);

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);
            var current = doc.RootElement.GetProperty("current");

            return new WeatherReading(
                TemperatureCelsius: current.GetProperty("temperature_2m").GetDouble(),
                WindSpeedKmh: current.GetProperty("wind_speed_10m").GetDouble(),
                WindDirectionDegrees: current.GetProperty("wind_direction_10m").GetDouble(),
                WindchillCelsius: current.GetProperty("apparent_temperature").GetDouble()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch weather for ({Lat}, {Lon})", latitude, longitude);
            return null;
        }
        finally
        {
            _concurrencyLimiter.Release();
        }
    }
}
