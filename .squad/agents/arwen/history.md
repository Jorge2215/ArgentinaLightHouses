# Arwen — Project History

## Project Seed

- **Project:** ArgentinaLightHouses
- **Stack:** C# / ASP.NET Core (.NET 10) Razor Pages
- **Description:** A web application showcasing Argentina's lighthouses with a WeatherGrid page displaying historical weather records from Azure Table Storage.
- **Role:** Frontend Dev — owns UI/UX, Razor templates, CSS, and client-side JavaScript.
- **User:** Jorge2215
- **Team joined:** 2026-06-02

## Learnings

### 2026-06-02T22:10:05-03:00 — WeatherGrid table contrast dark-theme fix

- **Root cause:** Bootstrap's `.table` styles apply light backgrounds directly on table sections and cells, so our `background: transparent` on `.alh-table tbody tr` did not preserve the dark nautical theme.
- **Fix applied:** Set explicit `background-color` values on `.alh-table`, `.alh-table tbody tr`, `.alh-table tbody td`, and `.table-responsive`, plus added a subtle even-row stripe and stronger hover state for readability.
- **Pattern to remember:** When restyling Bootstrap tables into the ALH dark theme, do not rely on transparent inheritance; override Bootstrap with explicit background-color on the table and the body cells that render the visible surface.

### 2026-06-02T21:52:22-03:00 — WeatherGrid JavaScript property casing bug fix

- **Root cause:** `System.Text.Json.JsonSerializer.Serialize` preserves C# PascalCase property names by default. `WeatherRecord` has `TemperatureCelsius`, `WindSpeedKmh`, `WindDirectionDegrees`, `WindchillCelsius`, etc. The JavaScript in `WeatherGrid.cshtml` accessed these as camelCase (`r.temperatureCelsius`, `r.windSpeedKmh`, etc.) — a mismatch that yielded `undefined` for all numeric fields and caused `undefined.toFixed()` TypeErrors at runtime. The table never rendered; it stayed on "Loading...".
- **Fix applied:** Changed the serialization call on line 71 to use `JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }`. This outputs camelCase keys matching the existing JS.
- **Null safety:** Added `?? 0` guards before all `.toFixed()` calls in `renderTable()` to handle records with missing numeric values (e.g. incomplete Azure Function writes). `tempClass()` calls also guarded.
- **Pattern to remember:** When serializing C# models to inline JS via `@Html.Raw(JsonSerializer.Serialize(...))`, always pass `JsonNamingPolicy.CamelCase` unless the JS explicitly uses PascalCase keys.
- **Decision note:** `.squad/decisions/inbox/arwen-weathergrid-casing-fix.md`

### 2026-06-02T22:12:35-03:00 — Table Dark Theme decision merged

- **Source:** `.squad/decisions/inbox/arwen-table-dark-theme.md` (merged into `.squad/decisions/decisions.md`)
- **Note:** Decision recorded in `.squad/decisions/decisions.md`. The CSS fix aligns with the project's dark nautical theme and documents the pattern to override Bootstrap table defaults.


### 2026-06-03T11:20:48-03:00 — Issue #21 date range filter delivered

- Added **Date From** and **Date To** pickers to Pages/WeatherGrid.cshtml for WeatherGrid filtering.
- Added .alh-date-input styling in wwwroot/css/site.css to match the ALH dark nautical theme.
- Delivery status: implementation merged in PR #22, Azure deployment is live, and Jorgito confirmed live verification.

### 2026-07-25T20:22:34-03:00 — Issue #30 lighthouse search UX delivered

- Added real-time name search to `Pages/Lighthouses.cshtml` with a DOM-filtering pattern that toggles card visibility using `hidden` on the existing rendered card columns.
- Added a matching search control to `Pages/Index.cshtml`; province and name filters now combine, and matching visible markers receive an `.alh-map-marker-match` highlight state.
- Added reusable frontend search styles in `wwwroot/css/site.css`: `.alh-search-control`, `.alh-search-input`, `.alh-search-clear`, and `.alh-map-marker-match`.
- Reinforced Arwen's inline JSON pattern on the map page by using `JsonNamingPolicy.CamelCase` and guarding numeric popup formatting with `?? 0` before `.toFixed()`.

### 2026-07-25T21:29:54-03:00 — Issue #28 extreme weather highlighting delivered

- Added Weather Grid row severity highlighting in `Pages/WeatherGrid.cshtml` for frost (`temperatureCelsius <= 0`), high winds (`windSpeedKmh >= 60`), and storm/heavy-rain weather codes (`80-82`, `95-99`), with severity precedence `storm > wind > frost`.
- Added inline alert icons beside lighthouse names plus a legend under the table so operators can quickly interpret highlighted rows without leaving the grid.
- Extended `WeatherRecord` / `WeatherGridService` to include `WeatherCode` from Azure Table Storage and added ALH-themed highlight styles in `wwwroot/css/site.css`.

### 2026-07-26T00:58:26Z — Orchestration note

- Implemented extreme-weather highlights (frost/wind/storm thresholds), added client-side icons and legend; integrated WeatherCode into WeatherRecord and WeatherGridService. 
- Build: clean. Tests: 62/62 passing.

