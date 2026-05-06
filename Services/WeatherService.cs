using ArgentinaLightHouses.Models;
using System.Text.Json;
namespace ArgentinaLightHouses.Services;
public interface IWeatherService
{
    Task<WeatherInfo?> GetWeatherAsync(double latitude, double longitude);
}
public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;
    public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<WeatherInfo?> GetWeatherAsync(double latitude, double longitude)
    {
        try
        {
            var url = string.Format(
                "https://api.open-meteo.com/v1/forecast?latitude={0}&longitude={1}&current=temperature_2m,wind_speed_10m,weather_code&timezone=auto",
                latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                longitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
            var response = await _httpClient.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);
            var current = doc.RootElement.GetProperty("current");
            return new WeatherInfo
            {
                TemperatureC = current.GetProperty("temperature_2m").GetDouble(),
                WindSpeedKmh = current.GetProperty("wind_speed_10m").GetDouble(),
                WeatherCode = current.GetProperty("weather_code").GetInt32()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch weather for ({Lat}, {Lon})", latitude, longitude);
            return null;
        }
    }
}