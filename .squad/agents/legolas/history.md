# Legolas — Project History

## Project Seed

- **Project:** ArgentinaLightHouses
- **Stack:** C# / ASP.NET Core (.NET 10) Razor Pages
- **Description:** A web application showcasing Argentina's lighthouses. Razor Pages live under `Pages/` (Index, Lighthouses, Privacy, Shared). Static assets under `wwwroot/`.
- **Key files:** `Pages/Shared/`, `Pages/_ViewImports.cshtml`, `Pages/_ViewStart.cshtml`, `wwwroot/`
- **User:** Jorge2215
- **Team joined:** 2026-05-06

## Learnings

### 2026-05-06 — Map & Weather UI architecture

- **Map page (`Index.cshtml`):** Uses Leaflet (`L.map`). All lighthouse data — including weather — is serialized server-side into a `const lighthouses = [...]` JS variable at page render time via `@Html.Raw(JsonSerializer.Serialize(...))`. There is **no AJAX call on marker click**; data is fully baked into the page.
- **Marker click flow:** Each marker has a Leaflet popup bound at page load via `.bindPopup(popup, ...)`. The popup HTML string is built inline in a `forEach` loop in `Index.cshtml` `@section Scripts`.
- **Weather fallback string (map popup):** Line 63 of `Index.cshtml` — `lh.weather ? <weather html> : <fallback p>`. Was `"Weather unavailable"`, updated to `"Weather data could not be loaded. Check API configuration."`.
- **Weather fallback string (card list):** Line 34 of `Lighthouses.cshtml` — `else` branch of `@if (lh.Weather != null)`. Was `"Weather data unavailable"`, updated to match.
- **Weather icon bug (minor):** The `icon` field was serialized into the JS object but never rendered in the popup. Fixed: icon emoji now prepended to temperature in popup.
- **Root cause of null weather:** `WeatherService.GetWeatherAsync()` in `Services/WeatherService.cs` catches all exceptions and returns `null`. If the Open-Meteo API call fails (network, API down, etc.), `lh.Weather` is null on the model, which propagates to `null` in the JS object. This is a backend concern — Gimli's territory.

