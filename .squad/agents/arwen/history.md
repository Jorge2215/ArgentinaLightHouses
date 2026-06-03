# Arwen — Project History

## Project Seed

- **Project:** ArgentinaLightHouses
- **Stack:** C# / ASP.NET Core (.NET 10) Razor Pages
- **Description:** A web application showcasing Argentina's lighthouses with a WeatherGrid page displaying historical weather records from Azure Table Storage.
- **Role:** Frontend Dev — owns UI/UX, Razor templates, CSS, and client-side JavaScript.
- **User:** Jorge2215
- **Team joined:** 2026-06-02

## Learnings

### 2026-06-02T21:52:22-03:00 — WeatherGrid JavaScript property casing bug fix

- **Root cause:** `System.Text.Json.JsonSerializer.Serialize` preserves C# PascalCase property names by default. `WeatherRecord` has `TemperatureCelsius`, `WindSpeedKmh`, `WindDirectionDegrees`, `WindchillCelsius`, etc. The JavaScript in `WeatherGrid.cshtml` accessed these as camelCase (`r.temperatureCelsius`, `r.windSpeedKmh`, etc.) — a mismatch that yielded `undefined` for all numeric fields and caused `undefined.toFixed()` TypeErrors at runtime. The table never rendered; it stayed on "Loading...".
- **Fix applied:** Changed the serialization call on line 71 to use `JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }`. This outputs camelCase keys matching the existing JS.
- **Null safety:** Added `?? 0` guards before all `.toFixed()` calls in `renderTable()` to handle records with missing numeric values (e.g. incomplete Azure Function writes). `tempClass()` calls also guarded.
- **Pattern to remember:** When serializing C# models to inline JS via `@Html.Raw(JsonSerializer.Serialize(...))`, always pass `JsonNamingPolicy.CamelCase` unless the JS explicitly uses PascalCase keys.
- **Decision note:** `.squad/decisions/inbox/arwen-weathergrid-casing-fix.md`
