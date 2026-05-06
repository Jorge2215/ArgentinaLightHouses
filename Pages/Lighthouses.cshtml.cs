using ArgentinaLightHouses.Data;
using ArgentinaLightHouses.Models;
using ArgentinaLightHouses.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace ArgentinaLightHouses.Pages;
public class LighthousesModel : PageModel
{
    private readonly IWeatherService _weatherService;
    public List<Lighthouse> Lighthouses { get; private set; } = [];
    public LighthousesModel(IWeatherService weatherService) { _weatherService = weatherService; }
    public async Task OnGetAsync()
    {
        var lighthouses = LighthouseRepository.GetAll();
        var tasks = lighthouses.Select(async lh => { lh.Weather = await _weatherService.GetWeatherAsync(lh.Latitude, lh.Longitude); return lh; });
        Lighthouses = [.. await Task.WhenAll(tasks)];
    }
}