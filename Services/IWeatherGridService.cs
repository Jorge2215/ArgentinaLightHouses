using ArgentinaLightHouses.Models;

namespace ArgentinaLightHouses.Services;

public interface IWeatherGridService
{
    Task<List<WeatherRecord>> GetRecentRecordsAsync(int hoursBack = 24);
}
