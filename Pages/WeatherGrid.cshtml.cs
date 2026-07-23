using ArgentinaLightHouses.Models;
using ArgentinaLightHouses.Services;
using Microsoft.AspNetCore.Mvc;
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

    [BindProperty(SupportsGet = true)]
    public DateOnly DateFrom { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly DateTo { get; set; }

    public async Task OnGetAsync()
    {
        if (DateFrom == default)
            DateFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        if (DateTo == default)
            DateTo = DateOnly.FromDateTime(DateTime.UtcNow);

        try
        {
            Records = await _weatherGridService.GetRecordsAsync(DateFrom, DateTo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load weather grid data.");
            ErrorMessage = "Weather data is currently unavailable. Please try again later.";
        }
    }
}
