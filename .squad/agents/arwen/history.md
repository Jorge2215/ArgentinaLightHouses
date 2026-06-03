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
