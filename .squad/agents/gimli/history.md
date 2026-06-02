# Gimli — Project History

## Project Seed

- **Project:** ArgentinaLightHouses
- **Stack:** C# / ASP.NET Core (.NET 10) Razor Pages
- **Description:** A web application showcasing Argentina's lighthouses. Has Lighthouse model (`Models/Lighthouse.cs`), `Data/LighthouseRepository.cs`, `Services/WeatherService.cs`, and Razor Pages (Index, Lighthouses, Privacy).
- **Key files:** `Program.cs`, `appsettings.json`, `ArgentinaLightHouses.csproj` (net10.0, nullable enabled, implicit usings)
- **User:** Jorge2215
- **Team joined:** 2026-05-06

## Learnings

### 2026-05-07 — Lighthouse Image Support

**What changed:**
Added `public string? ImageUrl { get; set; }` to `Models/Lighthouse.cs`. Populated real Wikimedia Commons image URLs directly in `Data/LighthouseRepository.cs` for each lighthouse entry (static, no runtime API calls). Added `imageUrl` to the JS serialization in `Pages/Index.cshtml` so Legolas can use it in map popups.

**Research method:**
Used Wikimedia Commons category API (`Category:Lighthouses in Argentina by name` and province subcategories) to find per-lighthouse file names, then computed direct `upload.wikimedia.org` URLs locally via MD5 hash of the file title (matching the Wikimedia URL path formula).

**Lighthouses with images (29 of 61):**
- Faro Año Nuevo — `Faro_a%C3%B1o_nuevo.jpg`
- Faro Beauvoir — `Iglesia_Puerto_Deseado_Faro_Beauvoir.jpg`
- Faro Cabo Blanco — `Faro_cabo_blanco.jpg`
- Faro Cabo Dañoso — `Cabo_Da%C3%B1oso_%2C_Faro_argentino.jpg`
- Faro Cabo Domingo — `Faro_Cabo_Domingo.jpg`
- Faro Cabo San Pío — `En_el_fin_del_mundo.JPG`
- Faro Cabo Vírgenes — `Faro_en_Cabo_V%C3%ADrgenes_2719.jpg`
- Faro Campana — `Punta_Mercedes_-_Faro_Campana_-_vista_al_sur.jpg`
- Faro Claromecó — `Faro_Claromeco.jpg`
- Faro El Rincón — `Faro_El_Rinc%C3%B3n.jpg`
- Faro Isla Pingüino — `Faro_-_Isla_Ping%C3%BCino_-_Puerto_Deseado.jpg`
- Faro Les Éclaireurs — `Les_Eclaireurs_Lighthouse.jpg`
- Faro Miramar — `Baliza_Miramar.jpg`
- Faro Punta Delgada — `Peninsula_Valdez_Punta_Delgada.jpg`
- Faro Punta Médanos — `Faro_Punta_M%C3%A9danos_01.jpg`
- Faro Punta Medanosa — `Faro_Punta_Medanosa.jpg`
- Faro Punta Mogotes — `Faro_Punta_Mogotes_2020.jpg`
- Faro Punta Ninfas — `Faro_Punta_Ninfas.jpg`
- Faro Punta Piedras — `Faro_Punta_Piedras%2C_Partido_de_Punta_Indio._Provincia_de_Buenos_Aires%2C_Argentina.jpg`
- Faro Quequén — `Faro-neco-quequen.JPG`
- Faro Querandí — `Faro_Querand%C3%AD_01.jpg`
- Faro Recalada a Bahía Blanca — `Faro_Recalada_(1).JPG`
- Faro Río Negro — `Faro_R%C3%ADo_Negro.jpg`
- Faro San Antonio — `Faro_San_Antonio.jpg`
- Faro San Jorge — `Faro_altura.JPG` (Cabo San Jorge Lighthouse category)
- Faro San Juan de Salvamento — `Faro_Isla_de_los_Estados.jpg`
- Faro San Matías — `Faro_San_Matias_(Argentina).JPG`
- Faro San Sebastián — `Faro_san_sebastian.jpg`
- Faro Segunda Barranca — `Segunda_Barranca.jpg`

**Lighthouses with null (32 of 61):**
No image found on Wikimedia Commons for: Faro 1ro. de Mayo, Faro Almirante Brown, Faro Buen Suceso, Faro Buen Tiempo, Faro Cabo Aristizabal, Faro Cabo Curioso, Faro Cabo Guardian, Faro Cabo Peñas, Faro Cabo Raso, Faro Cabo San Pablo, Faro Chubut, Faro Coig, Faro Esperanza, Faro Guzmán, Faro Isla Rasa, Faro Le Maire, Faro Magallanes, Faro Mar Chiquita, Faro Morro Nuevo, Faro Páramo, Faro Punta Bajos, Faro Punta Colorada, Faro Punta Conscriptos, Faro Punta Lobos, Faro Punta Norte, Faro Punta Tehuelche, Faro San Diego, Faro San Francisco de Paula, Faro San Gonzalo, Faro San Gregorio, Faro San José, Faro Santa Cruz.

**URL pattern:**
`https://upload.wikimedia.org/wikipedia/commons/{a}/{ab}/{encoded_filename}`
where `a` = first char of MD5(canonical_filename), `ab` = first two chars. Canonical = spaces→underscores, first letter uppercase.


**What the WeatherService does:**
`Services/WeatherService.cs` calls the Open-Meteo free API (`https://api.open-meteo.com/v1/forecast`) — no API key required. It requests `current=temperature_2m,wind_speed_10m,weather_code&timezone=auto` for a given latitude/longitude, parses the JSON `current` object, and returns a `WeatherInfo` model. Any exception is caught, logged at Error level, and null is returned (which the frontend renders as "Weather unavailable").

**How it's configured:**
Registered via `builder.Services.AddHttpClient<IWeatherService, WeatherService>()` in `Program.cs`. No API key or base URL in `appsettings.json` — the full URL is built inline in the service using `InvariantCulture` for decimal formatting.

**The bug:**
`IndexModel.OnGetAsync()` fires 61 concurrent `GetWeatherAsync` calls via `Task.WhenAll`. Open-Meteo rate-limits concurrent requests with HTTP 429. The broad `catch (Exception ex)` block silently swallows the `HttpRequestException` and returns null for every throttled lighthouse — all showing "Weather unavailable".

**The fix:**
Added a `static SemaphoreSlim _concurrencyLimiter = new(5, 5)` to `WeatherService`. Each call acquires the semaphore before the HTTP request and releases it in a `finally` block, capping concurrent Open-Meteo requests to 5. All 11 existing tests continue to pass.

### 2026-06-02T16:34:39-03:00 — Azure Function build

- Created projects: ArgentinaLightHouses.Shared and ArgentinaLightHouses.Functions.
- Added CI workflow: .github/workflows/azure-function-deploy.yml.
- Build result: 0 errors.
- Tests: 17/17 passed (existing 11 + 6 image-related tests).

