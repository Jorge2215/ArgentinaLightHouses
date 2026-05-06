namespace ArgentinaLightHouses.Models;

public class Lighthouse
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Description { get; set; } = string.Empty;
    public WeatherInfo? Weather { get; set; }
}

public class WeatherInfo
{
    public double TemperatureC { get; set; }
    public double WindSpeedKmh { get; set; }
    public int WeatherCode { get; set; }
    public string WeatherDescription => WeatherCodeToDescription(WeatherCode);
    public string WeatherIcon => WeatherCodeToIcon(WeatherCode);

    private static string WeatherCodeToDescription(int code) => code switch
    {
        0 => "Clear sky", 1 => "Mainly clear", 2 => "Partly cloudy", 3 => "Overcast",
        45 or 48 => "Foggy", 51 or 53 or 55 => "Drizzle", 61 or 63 or 65 => "Rain",
        71 or 73 or 75 => "Snow", 80 or 81 or 82 => "Rain showers",
        95 => "Thunderstorm", 96 or 99 => "Thunderstorm with hail", _ => "Unknown"
    };

    private static string WeatherCodeToIcon(int code) => code switch
    {
        0 => "☀️", 1 => "🌤️", 2 => "⛅", 3 => "☁️", 45 or 48 => "🌫️",
        51 or 53 or 55 => "🌦️", 61 or 63 or 65 => "🌧️", 71 or 73 or 75 => "❄️",
        80 or 81 or 82 => "🌧️", 95 => "⛈️", 96 or 99 => "⛈️", _ => "🌡️"
    };
}