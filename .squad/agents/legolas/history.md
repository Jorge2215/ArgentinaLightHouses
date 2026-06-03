# Legolas — Project History

## Project Seed

- **Project:** ArgentinaLightHouses
- **Stack:** C# / ASP.NET Core (.NET 10) Razor Pages
- **Description:** A web application showcasing Argentina's lighthouses. Razor Pages live under `Pages/` (Index, Lighthouses, Privacy, Shared). Static assets under `wwwroot/`.
- **Key files:** `Pages/Shared/`, `Pages/_ViewImports.cshtml`, `Pages/_ViewStart.cshtml`, `wwwroot/`
- **User:** Jorge2215
- **Team joined:** 2026-05-06

## Learnings

### 2026-05-07 — Popup image support

- **Image section pattern:** Added `imageHtml` variable in the `forEach` loop (mirroring `weatherHtml`). Uses a JS template literal with `lh.imageUrl ? \`...\` : ""` so no image section is rendered at all when the field is null/undefined.
- **`imageUrl` field:** Serialized by Gimli on the backend. On the JS side it is simply falsy when absent — no defensive code needed beyond the ternary.
- **maxWidth bump:** Increased Leaflet popup `maxWidth` from `260` to `300` to give the 180px-tall photo room without horizontal scroll.
- **CSS — `.popup-lighthouse-img`:** `width:100%; max-height:180px; object-fit:cover; border-radius:4px; display:block; margin-bottom:8px`. Lives in the `/* -- Popup image -- */` section of `wwwroot/css/site.css`.
- **CSS — `.popup-attribution`:** `font-size:0.65rem`, `color: var(--alh-pale-blue)`, `margin:0 0 6px 0`. Link inherits the same muted color.
- **Accessibility:** `<img>` carries `alt="${lh.name}"` and `loading="lazy"`.
- **Backward compat:** Existing lighthouses without `imageUrl` render identically to before — no placeholder, no gap.

### 2026-06-02 — WeatherGrid frontend implementation

**Task:** Built the complete frontend for the new Weather Data Grid page.

**Files created:** `Pages/WeatherGrid.cshtml`.  
**Files modified:** `Pages/Shared/_Layout.cshtml` (added "📊 Weather" nav link), `wwwroot/css/site.css` (appended styles under `/* -- Weather Grid page -- */`).

**CSS approach:** All class names follow `alh-*` prefix convention. No new CSS variables — uses existing `:root` palette only. Key additions: `.alh-table` dark-nautical table theming, temperature semantic classes (`.alh-temp-cold`, `.alh-temp-extreme`, `.alh-temp-hot`), `.alh-grid-controls` filter row, `.alh-lighthouse-hero` hero icon, `.alh-alert-warning` error alert.

**JS approach:** Vanilla JS only — no external grid libraries. Three module-level state vars. Pipeline: `applyFilter()` → `applySort()` → `renderTable()`. Data flow: `@Html.Raw(JsonSerializer.Serialize(Model.Records))` — same established pattern as `Index.cshtml`.

**Accessibility:** Column headers have `tabindex="0"`, `role="columnheader"`, `aria-sort` kept in sync. Keyboard sort via Enter/Space. SVG hero `aria-hidden="true" focusable="false"`. Pagination uses `aria-label` and `aria-live="polite"`.

### 2026-05-06 — Map & Weather UI architecture

- **Map page (`Index.cshtml`):** Uses Leaflet (`L.map`). All lighthouse data — including weather — is serialized server-side into a `const lighthouses = [...]` JS variable at page render time via `@Html.Raw(JsonSerializer.Serialize(...))`. There is **no AJAX call on marker click**; data is fully baked into the page.
- **Marker click flow:** Each marker has a Leaflet popup bound at page load via `.bindPopup(popup, ...)`. The popup HTML string is built inline in a `forEach` loop in `Index.cshtml` `@section Scripts`.
- **Weather fallback string (map popup):** Line 63 of `Index.cshtml` — `lh.weather ? <weather html> : <fallback p>`. Was `"Weather unavailable"`, updated to `"Weather data could not be loaded. Check API configuration."`.
- **Weather fallback string (card list):** Line 34 of `Lighthouses.cshtml` — `else` branch of `@if (lh.Weather != null)`. Was `"Weather data unavailable"`, updated to match.
- **Weather icon bug (minor):** The `icon` field was serialized into the JS object but never rendered in the popup. Fixed: icon emoji now prepended to temperature in popup.
- **Root cause of null weather:** `WeatherService.GetWeatherAsync()` in `Services/WeatherService.cs` catches all exceptions and returns `null`. If the Open-Meteo API call fails (network, API down, etc.), `lh.Weather` is null on the model, which propagates to `null` in the JS object. This is a backend concern — Gimli's territory.

