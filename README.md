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

## Prompt - Record lighthouses Meteo Statistics 
- Context:
--Our static web app gets all the lighthouses of Argentina's shore with their corresponding Geo coordinates
--Once get then, it shows them geolocalized on a map from Open Street Maps
--Additionally, the app gets the current weather on each lighthouse using Open Meteo API, and shows the weather info on the page,when the user clicks on the lighthouse icon 

- Goal:
- Gather Meteo info through Azure Function triggered by time (1 hour interval) and must include all the lighthouses showed on the Web App 
- Info must contain
-- lighthouse Name`
-- lighthouse coordinates
-- Date
-- Time 
-- Wind Speed (Kms/Hour)
-- Wind Direction
-- Temperature (Celsius)
-- Windchill

- Gathered info must be recorded on an Azure Table contained on an Storage Account
- If an error occurs getting the Meteo info or recording it on the Azure Storage Account Table, info about the error must be recorded on Azure App Insights

-Deliverables:
--An Azure Function App that runs each hour and records the current weather of each lighthouse on an an Azure Storage Account Table
--Deploymen Workflow on Github repo (branch: main) that deploys the Function App everytime, main branch is updated through Pull Request from branc main
--Techinical and architectural documentation of the Function App (with all the azure resoureces involved)

-Azure and Github Technical info
-- Github Repo: 
-- Azure Suscription: 4ffc573f-cffe-48a5-b82a-0f3930ce1700
-- Azure Resource Group: LightHouses_rg
-- Azure Function: LightHousesFunction
-- Azure Storage Account: storagelighthouses
-- Azure Storage Account Key1: NT3eGQtUI3ielsU4VpEyPcGKjq62YwNonX3Tq+3Y52BaaP1PxS9tu87jwhw4ZHvWXitNzejAkdN1+AStyLUGrg==
-- Azure Storage Account Connection String: DefaultEndpointsProtocol=https;AccountName=storagelighthouses;AccountKey=NT3eGQtUI3ielsU4VpEyPcGKjq62YwNonX3Tq+3Y52BaaP1PxS9tu87jwhw4ZHvWXitNzejAkdN1+AStyLUGrg==;EndpointSuffix=core.windows.net
-- Azure Storage Account Table: LightHousesWeather
-- Azure Application Insights: LightHousesFunction
-- Azure Application Insights Connection string: InstrumentationKey=6251c03e-c59f-4b72-bf30-e93bd05f212f;IngestionEndpoint=https://westus3-1.in.applicationinsights.azure.com/;LiveEndpoint=https://westus3.livediagnostics.monitor.azure.com/;ApplicationId=c48eff02-1e41-436f-b8ae-fd175a8779be



##Resume
copilot --resume=b2cca19f-c122-495b-a454-c6932f8a1b55