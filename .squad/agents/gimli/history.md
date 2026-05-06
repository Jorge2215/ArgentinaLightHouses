# Gimli — Project History

## Project Seed

- **Project:** ArgentinaLightHouses
- **Stack:** C# / ASP.NET Core (.NET 10) Razor Pages
- **Description:** A web application showcasing Argentina's lighthouses. Has Lighthouse model (`Models/Lighthouse.cs`), `Data/LighthouseRepository.cs`, `Services/WeatherService.cs`, and Razor Pages (Index, Lighthouses, Privacy).
- **Key files:** `Program.cs`, `appsettings.json`, `ArgentinaLightHouses.csproj` (net10.0, nullable enabled, implicit usings)
- **User:** Jorge2215
- **Team joined:** 2026-05-06

## Learnings

### 2026-05-06 — Weather Service & "Weather Unavailable" Bug

**What the WeatherService does:**
`Services/WeatherService.cs` calls the Open-Meteo free API (`https://api.open-meteo.com/v1/forecast`) — no API key required. It requests `current=temperature_2m,wind_speed_10m,weather_code&timezone=auto` for a given latitude/longitude, parses the JSON `current` object, and returns a `WeatherInfo` model. Any exception is caught, logged at Error level, and null is returned (which the frontend renders as "Weather unavailable").

**How it's configured:**
Registered via `builder.Services.AddHttpClient<IWeatherService, WeatherService>()` in `Program.cs`. No API key or base URL in `appsettings.json` — the full URL is built inline in the service using `InvariantCulture` for decimal formatting.

**The bug:**
`IndexModel.OnGetAsync()` fires 61 concurrent `GetWeatherAsync` calls via `Task.WhenAll`. Open-Meteo rate-limits concurrent requests with HTTP 429. The broad `catch (Exception ex)` block silently swallows the `HttpRequestException` and returns null for every throttled lighthouse — all showing "Weather unavailable".

**The fix:**
Added a `static SemaphoreSlim _concurrencyLimiter = new(5, 5)` to `WeatherService`. Each call acquires the semaphore before the HTTP request and releases it in a `finally` block, capping concurrent Open-Meteo requests to 5. All 11 existing tests continue to pass.
