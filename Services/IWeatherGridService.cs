using ArgentinaLightHouses.Models;

namespace ArgentinaLightHouses.Services;

public interface IWeatherGridService
{
    Task<List<WeatherRecord>> GetRecordsAsync(DateOnly dateFrom, DateOnly dateTo);
}
