# Gandalf — Project History

## Project Seed

- **Project:** ArgentinaLightHouses
- **Stack:** C# / ASP.NET Core (.NET 10) Razor Pages
- **Description:** A web application showcasing Argentina's lighthouses. Has Lighthouse model, LighthouseRepository, WeatherService, and Razor Pages (Index, Lighthouses, Privacy).
- **User:** Jorge2215
- **Team joined:** 2026-05-06

## Learnings

### 2026-05-06 — Lighthouse data sourced from hidro.gov.ar

**Task:** Replace 15 placeholder entries in `Data/LighthouseRepository.cs` with real data from the Argentine Hydrography Service (SHN) at https://www.hidro.gov.ar/balizamiento/Faros/FarosArgentinos.asp

**What I found:**
- The SHN page exposes 61 lighthouses via a POST form (not GET-accessible). Data is retrieved by POSTing `nomFaros=<CODE>` to `RE_FarosArgentinos.asp`.
- Coordinates appear in the page body in the format `Lat: 42°13' S    Long:64°15' W` (inside a `<p>` tag under `<h3>Posición:</h3>`).
- Several coordinate format variants existed on the page:
  - **DM only:** `42°13' S` (most common)
  - **DMS with `''`:** `38°34'03'' S` (5 lighthouses: Quequén, Querandí, Río Negro, San Antonio, Segunda Barranca)
  - **Decimal minutes (`.`):** `41°41.9' S` (Punta Colorada)
  - **Decimal minutes with comma:** `36°53,0' S` (Punta Médanos)
  - **Malformed (° used instead of '):** `65°53° W` (Beauvoir) — parsed as DM
- All 61 lighthouses have valid coordinates after handling all format variants.
- Coordinate patterns for Argentina: latitudes range ~-35 to -55 (mainland+Patagonia), ~-63 to -64 (Antarctic sector). Longitudes range ~-56 to -70.
- The page includes two Antarctic station lighthouses (Faro 1ro. de Mayo at ~64°S and Faro Esperanza at ~63°S).
- Descriptions were extracted from `<h3>Descripción:</h3><p>...</p>` blocks; about 30 lighthouses had no description on the page and received English fallback text.

**Files changed:**
- `Data/LighthouseRepository.cs` — 61 real entries replacing 15 placeholders
- `.squad/decisions/inbox/gandalf-lighthouse-data-source.md` — decision recorded
