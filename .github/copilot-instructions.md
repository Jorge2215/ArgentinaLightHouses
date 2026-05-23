# Copilot Instructions — ArgentinaLightHouses

## Build, Test & Run

```bash
dotnet restore
dotnet build
dotnet run --project ArgentinaLightHouses

# Run all tests
dotnet test

# Run a single test class
dotnet test --filter "FullyQualifiedName~LighthouseRepositoryTests"

# Run a single test by name
dotnet test --filter "DisplayName~GetAll_ReturnsExactly61Lighthouses"
```

Target framework: **net10.0**. Requires .NET 10 SDK.

## Architecture

**No database.** All 61 lighthouse records live in `Data/LighthouseRepository.cs` as a static in-memory list returned by `LighthouseRepository.GetAll()`. To add or modify lighthouses, edit that file directly.

**Two pages share the same pattern** — both `IndexModel` (map) and `LighthousesModel` (card list) call `LighthouseRepository.GetAll()`, then fan out weather fetches via `Task.WhenAll`, injecting results back into each `Lighthouse.Weather`. There is no caching; weather is fetched fresh on every page load.

**Weather** is fetched from the Open-Meteo API (no API key needed) via `IWeatherService` / `WeatherService`. A `SemaphoreSlim(5, 5)` caps concurrent requests to avoid HTTP 429s. Failures are swallowed and logged — `Weather` stays `null`, and both the map popup and card template have fallback UI for this.

**Frontend map** (Index page): Leaflet.js renders a custom SVG lighthouse icon defined inline in `Index.cshtml`. Lighthouse data is serialized to JSON via `Html.Raw(JsonSerializer.Serialize(...))` and passed to client-side JS in the Scripts section.

**CSS theming**: All custom styles use `alh-*` prefixed CSS variables and class names defined in `wwwroot/css/site.css`. The color palette is dark-nautical blues (`--alh-dark-blue`, `--alh-mid-blue`, `--alh-light-blue`, `--alh-pale-blue`, `--alh-accent`).

## Key Conventions

**`ImageUrl` must be `null` or a valid HTTPS URL — never an empty string.** Tests enforce this. When adding lighthouse data, omit `ImageUrl` entirely (defaults to `null`) rather than setting it to `""`. All images are Wikimedia Commons URLs.

**Lighthouse coordinates** use `InvariantCulture` when formatted for the Open-Meteo API URL. Latitude range: −66.0 to −22.0; longitude range: −74.0 to −53.0 (covers mainland + Antarctic territory).

**Weather codes** (WMO standard) are decoded in `WeatherInfo.WeatherCodeToDescription` and `WeatherCodeToIcon` using C# switch expressions. Add new codes there when extending weather display.

**`IWeatherService` is injected via `AddHttpClient<IWeatherService, WeatherService>()`** in `Program.cs`. Tests that exercise weather must mock this interface.

**Tests assert exactly 61 lighthouses.** `LighthouseRepositoryTests.GetAll_ReturnsExactly61Lighthouses` will fail if you add or remove entries. Update this count when the dataset changes intentionally.

## CI/CD

GitHub Actions workflows in `.github/workflows/`:
- `azure-deploy.yml` — deploys to Azure App Service using OIDC (no publish profile secrets).
- Squad automation workflows (`squad-*.yml`, `sync-squad-labels.yml`) manage issue routing via `squad:*` labels — do not remove these labels from issues.
