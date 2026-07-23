using ArgentinaLightHouses.Models;
using Azure.Data.Tables;

namespace ArgentinaLightHouses.Services;

public class WeatherGridService : IWeatherGridService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherGridService> _logger;

    public WeatherGridService(IConfiguration configuration, ILogger<WeatherGridService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<List<WeatherRecord>> GetRecordsAsync(DateOnly dateFrom, DateOnly dateTo)
    {
        var connectionString = _configuration["AzureStorageConnection"];
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogWarning("AzureStorageConnection is not configured. Returning empty weather grid.");
            return [];
        }

        try
        {
            var tableClient = new TableClient(connectionString, "LightHousesWeather");
            var fromKey = dateFrom.ToString("yyyy-MM-dd");
            var toKey = dateTo.ToString("yyyy-MM-dd");
            var filter = $"RowKey ge '{fromKey}' and RowKey le '{toKey}T23:59:59Z'";

            var records = new List<WeatherRecord>();

            await foreach (var entity in tableClient.QueryAsync<TableEntity>(filter: filter))
            {
                records.Add(new WeatherRecord
                {
                    Name = entity.GetString("Name") ?? string.Empty,
                    Date = entity.GetString("Date") ?? string.Empty,
                    Time = entity.GetString("Time") ?? string.Empty,
                    TemperatureCelsius = entity.GetDouble("TemperatureCelsius") ?? 0,
                    WindSpeedKmh = entity.GetDouble("WindSpeedKmh") ?? 0,
                    WindDirectionDegrees = entity.GetDouble("WindDirectionDegrees") ?? 0,
                    WindchillCelsius = entity.GetDouble("WindchillCelsius") ?? 0
                });
            }

            return [.. records.OrderByDescending(r => r.Date).ThenByDescending(r => r.Time)];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query weather grid data from Azure Table Storage.");
            return [];
        }
    }
}
