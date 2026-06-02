using ArgentinaLightHouses.Functions.Services;
using ArgentinaLightHouses.Shared.Data;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ArgentinaLightHouses.Functions.Functions;

public class LighthouseWeatherCollector
{
    private readonly FunctionWeatherService _weatherService;
    private readonly ILogger<LighthouseWeatherCollector> _logger;

    public LighthouseWeatherCollector(
        FunctionWeatherService weatherService,
        ILogger<LighthouseWeatherCollector> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [Function(nameof(LighthouseWeatherCollector))]
    public async Task Run([TimerTrigger("0 0 * * * *")] TimerInfo timerInfo)
    {
        var now = DateTime.UtcNow;
        _logger.LogInformation("LighthouseWeatherCollector started at {Time}", now);

        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
            ?? throw new InvalidOperationException("AzureWebJobsStorage is not configured.");

        var tableClient = new TableClient(connectionString, "LightHousesWeather");
        await tableClient.CreateIfNotExistsAsync();

        var lighthouses = LighthouseRepository.GetAll();

        var tasks = lighthouses.Select(lighthouse => Task.Run(async () =>
        {
            var reading = await _weatherService.FetchWeatherAsync(lighthouse.Latitude, lighthouse.Longitude);
            if (reading is null)
            {
                _logger.LogWarning("No weather data for {Lighthouse} — skipping table write.", lighthouse.Name);
                return;
            }

            // PartitionKey uses URL-encoded name to ensure safe table key characters.
            var partitionKey = Uri.EscapeDataString(lighthouse.Name);
            var rowKey = now.ToString("o");

            var entity = new TableEntity(partitionKey, rowKey)
            {
                ["Name"] = lighthouse.Name,
                ["Latitude"] = lighthouse.Latitude,
                ["Longitude"] = lighthouse.Longitude,
                ["Date"] = now.ToString("yyyy-MM-dd"),
                ["Time"] = now.ToString("HH:mm:ss"),
                ["TemperatureCelsius"] = reading.TemperatureCelsius,
                ["WindSpeedKmh"] = reading.WindSpeedKmh,
                ["WindDirectionDegrees"] = reading.WindDirectionDegrees,
                ["WindchillCelsius"] = reading.WindchillCelsius,
            };

            try
            {
                await tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace);
                _logger.LogInformation("Stored weather for {Lighthouse}", lighthouse.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write table entity for {Lighthouse}", lighthouse.Name);
            }
        }));

        await Task.WhenAll(tasks);

        _logger.LogInformation(
            "LighthouseWeatherCollector finished. Processed {Count} lighthouses.", lighthouses.Count);
    }
}
