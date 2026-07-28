using ArgentinaLightHouses.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
namespace ArgentinaLightHouses.Services;
public interface IWeatherService
{
    Task<WeatherInfo?> GetWeatherAsync(double latitude, double longitude);
}
public class WeatherService : IWeatherService
{
    // Caps concurrent Open-Meteo requests to avoid HTTP 429 when many lighthouses load in parallel.
    private static readonly SemaphoreSlim _concurrencyLimiter = new(5, 5);
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(15);

    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;
    private readonly IMemoryCache _cache;

    public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;
    }

    public async Task<WeatherInfo?> GetWeatherAsync(double latitude, double longitude)
    {
        var cacheKey = $"weather:{latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}:{longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

        if (_cache.TryGetValue(cacheKey, out WeatherInfo? cached))
            return cached;

        await _concurrencyLimiter.WaitAsync();
        try
        {
            // Check again after acquiring the semaphore — another thread may have populated the cache.
            if (_cache.TryGetValue(cacheKey, out cached))
                return cached;

            var url = string.Format(
                "https://api.open-meteo.com/v1/forecast?latitude={0}&longitude={1}&current=temperature_2m,wind_speed_10m,weather_code&timezone=auto",
                latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                longitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
            var response = await _httpClient.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);
            var current = doc.RootElement.GetProperty("current");
            var weather = new WeatherInfo
            {
                TemperatureC = current.GetProperty("temperature_2m").GetDouble(),
                WindSpeedKmh = current.GetProperty("wind_speed_10m").GetDouble(),
                WeatherCode = current.GetProperty("weather_code").GetInt32()
            };

            _cache.Set(cacheKey, weather, CacheTtl);
            return weather;
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