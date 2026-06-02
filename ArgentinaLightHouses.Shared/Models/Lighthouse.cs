namespace ArgentinaLightHouses.Shared.Models;

public class Lighthouse
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public WeatherInfo? Weather { get; set; }
}
