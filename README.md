# ArgentinaLightHouses

## Project Overview
ArgentinaLightHouses is a web application that showcases all 61 official lighthouses of Argentina, including their locations, descriptions, live weather data, and photographs. The app provides an interactive map and detailed listings, serving as an educational and reference tool for maritime enthusiasts, travelers, and researchers.

## Tech Stack
- **Backend:** ASP.NET Core 10 (Razor Pages)
- **Frontend:** Razor Pages, Leaflet.js (interactive map), Bootstrap
- **Data:** Static repository (no database); lighthouse data curated from the Argentine Hydrography Service (SHN)
- **Weather:** Open-Meteo API (live weather per lighthouse)
- **Images:** Curated Wikimedia Commons URLs
- **Testing:** xUnit
- **CI/CD:** GitHub Actions, Azure App Service (OIDC deployment)

## Solution Structure
- `ArgentinaLightHouses.slnx` — Solution file
- `ArgentinaLightHouses/` — Main web project
  - `Models/` — Data models (`Lighthouse.cs`, `WeatherInfo`)
  - `Data/` — Static repository (`LighthouseRepository.cs`)
  - `Services/` — Weather service abstraction and implementation (`WeatherService.cs`)
  - `Pages/` — Razor Pages (`Index`, `Lighthouses`, `Privacy`, `Error`)
  - `wwwroot/` — Static assets (JS, CSS, images)
  - `Program.cs` — App startup/configuration
- `ArgentinaLightHouses.Tests/` — xUnit test project

## Key Features
- **Interactive Map:** Leaflet.js map with custom SVG icons for each lighthouse. Clicking a marker shows a popup with photo, description, and live weather.
- **Lighthouse Photos:** Each lighthouse may display a Wikimedia Commons image (if available) with attribution.
- **Live Weather:** Weather data (temperature, wind, condition) fetched from Open-Meteo API per lighthouse.
- **Lighthouse List:** Tabular listing of all lighthouses with weather and details.

## Data Model
- `Lighthouse` entity:
  - `Name` (string)
  - `Location` (string)
  - `Latitude`/`Longitude` (double)
  - `Description` (string)
  - `ImageUrl` (string?, Wikimedia Commons, optional)
  - `Weather` (WeatherInfo?, populated at runtime)
- `WeatherInfo` entity:
  - `TemperatureC` (double)
  - `WindSpeedKmh` (double)
  - `WeatherCode` (int, mapped to description/icon)

## API/Service Dependencies
- **Weather:** [Open-Meteo API](https://open-meteo.com/) (no key required)
- **Images:** Static Wikimedia Commons URLs (no runtime API calls)

## Running Locally
1. **Requirements:** .NET 10 SDK
2. **Restore & Build:**
   ```
   dotnet restore
   dotnet build
   ```
3. **Run:**
   ```
   dotnet run --project ArgentinaLightHouses
   ```
4. Visit [https://localhost:5001](https://localhost:5001) in your browser.

## Testing
- Tests are in `ArgentinaLightHouses.Tests/` (xUnit)
- Run all tests:
  ```
  dotnet test
  ```
- Tests cover:
  - Data integrity (61 lighthouses, valid coordinates, no duplicates)
  - Image URL correctness (well-formed, HTTPS, non-empty)
  - Model defaults and edge cases

## Recent Changes (as of 2026-05-07)
- **Lighthouse photographs:** Added `ImageUrl` to `Lighthouse` model and repository. Popups now show Wikimedia Commons images where available.
- **Weather:** Weather data fetched live from Open-Meteo for each lighthouse.
- **CI/CD:** Azure App Service deployment now uses OIDC (no publish profile secrets).

## Attribution
- Lighthouse data: [Argentine Hydrography Service (SHN)](https://www.hidro.gov.ar/balizamiento/Faros/FarosArgentinos.asp)
- Images: [Wikimedia Commons](https://commons.wikimedia.org/)
- Weather: [Open-Meteo](https://open-meteo.com/)

---

For architecture, decisions, and team history, see `.squad/`.

##Resume
copilot --resume=b2cca19f-c122-495b-a454-c6932f8a1b55