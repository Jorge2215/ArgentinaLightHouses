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
}
