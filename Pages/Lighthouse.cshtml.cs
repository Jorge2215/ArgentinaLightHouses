using ArgentinaLightHouses.Data;
using ArgentinaLightHouses.Models;
using ArgentinaLightHouses.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArgentinaLightHouses.Pages;

public class LighthouseModel : PageModel
{
    private readonly IWeatherService _weatherService;

    public LighthouseModel(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public Lighthouse? LighthouseDetail { get; private set; }

    public async Task<IActionResult> OnGetAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return NotFound();

        LighthouseDetail = LighthouseRepository.GetAll()
            .FirstOrDefault(lh => string.Equals(
                lh.Name, Uri.UnescapeDataString(name),
                StringComparison.OrdinalIgnoreCase));

        if (LighthouseDetail == null)
            return NotFound();

        LighthouseDetail.Weather = await _weatherService.GetWeatherAsync(
            LighthouseDetail.Latitude, LighthouseDetail.Longitude);

        return Page();
    }
}
