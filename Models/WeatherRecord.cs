using System.Text.Json.Serialization;

namespace ArgentinaLightHouses.Models;

public class WeatherRecord
{
    public string Name { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public double TemperatureCelsius { get; set; }
    public double WindSpeedKmh { get; set; }
    public double WindDirectionDegrees { get; set; }
    public double WindchillCelsius { get; set; }
    public int WeatherCode { get; set; }

    [JsonIgnore]
    public bool IsFrost => TemperatureCelsius <= 0;

    [JsonIgnore]
    public bool IsHighWind => WindSpeedKmh >= 60;

    [JsonIgnore]
    public bool IsStorm => WeatherCode is >= 80 and <= 82 or >= 95 and <= 99;

    [JsonIgnore]
    public string ExtremeWeatherCssClass => IsStorm
        ? "alh-weather-storm"
        : IsHighWind
            ? "alh-weather-wind"
            : IsFrost
                ? "alh-weather-frost"
                : string.Empty;
}
