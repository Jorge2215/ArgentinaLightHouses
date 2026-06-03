using ArgentinaLightHouses.Models;
using ArgentinaLightHouses.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArgentinaLightHouses.Pages;

public class WeatherGridModel : PageModel
{
    private readonly IWeatherGridService _weatherGridService;
    private readonly ILogger<WeatherGridModel> _logger;

    public WeatherGridModel(IWeatherGridService weatherGridService, ILogger<WeatherGridModel> logger)
    {
        _weatherGridService = weatherGridService;
        _logger = logger;
    }

    public List<WeatherRecord> Records { get; set; } = [];
    public string ErrorMessage { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        try
        {
            Records = await _weatherGridService.GetRecentRecordsAsync(hoursBack: 24);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load weather grid data.");
            ErrorMessage = "Weather data is currently unavailable. Please try again later.";
        }
    }
}
