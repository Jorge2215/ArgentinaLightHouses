# Gandalf — Project History

## Project Seed

- **Project:** ArgentinaLightHouses
- **Stack:** C# / ASP.NET Core (.NET 10) Razor Pages
- **Description:** A web application showcasing Argentina's lighthouses. Has Lighthouse model, LighthouseRepository, WeatherService, and Razor Pages (Index, Lighthouses, Privacy).
- **User:** Jorge2215
- **Team joined:** 2026-05-06

## Learnings

### 2026-06-02 — WeatherGrid feature architecture decisions

**Task:** Made and recorded architecture decisions for the new Weather Data Grid page.

**Key decisions:**
- `Azure.Data.Tables` NuGet added to main web app project (v12.x, same as Functions project)
- Connection string key: `AzureStorageConnection` (never `AzureWebJobsStorage` — that is Functions-runtime-only)
- Data fetch window: last 24 hours (~1,464 rows max — safe for client-side rendering)
- Client-side grid: vanilla JS only, consistent with project's minimal-dependency philosophy
- Graceful degradation: missing/empty connection string → empty list + informative UI message (mirrors Open-Meteo failure pattern)
- Lighthouse image: inline SVG reused from map page — no external dependency

**Files created:**
- `.squad/decisions/inbox/gandalf-weathergrid-architecture.md`

### 2026-05-07 — Technical documentation overhaul

**Task:** Updated README.md to provide a comprehensive technical overview of the ArgentinaLightHouses web application, reflecting the latest architecture, features (including the new lighthouse photo support), data model, and deployment/testing practices.

**What I found:**
- The project is a .NET 10 Razor Pages app with a static repository of 61 lighthouses, live weather via Open-Meteo, and curated Wikimedia Commons images.
- The solution is cleanly structured: Models, Data, Services, Pages, wwwroot, and a separate xUnit test project.
- The new `ImageUrl` field is present in the model and repository, with tests ensuring URL validity and coverage.
- CI/CD is handled via GitHub Actions and Azure App Service using OIDC.

**Outcome:**
- README.md now documents project purpose, tech stack, structure, features, data model, dependencies, local run/test instructions, and recent changes as of 2026-05-07.
- All information is up to date and reflects current team/architecture decisions.

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

### 2026-05-06 — CI/CD pipeline wired to Azure App Service

**Task:** Create a GitHub Actions workflow to build, test, and deploy the .NET 10 Razor Pages app to Azure App Service using a Publish Profile.

**Key decisions made:**
- Single workflow file `.github/workflows/azure-deploy.yml` with two jobs: `build` → `deploy`.
- Triggers: push to `main` + `workflow_dispatch` for manual runs.
- `build` job: restore → build Release → test (xunit, `--no-build`) → publish → upload artifact `dotnet-app`.
- `deploy` job: downloads artifact, deploys via `azure/webapps-deploy@v3` with `AZURE_WEBAPP_PUBLISH_PROFILE` secret.
- `AZURE_WEBAPP_NAME` and `DOTNET_VERSION` declared as top-level env vars for easy customization.
- Test project confirmed: `ArgentinaLightHouses.Tests/ArgentinaLightHouses.Tests.csproj` (xunit, net10.0).
- Used modern action pins: `checkout@v4`, `setup-dotnet@v4`, `upload-artifact@v4`, `download-artifact@v4`, `webapps-deploy@v3`.
- Publish profile approach chosen over OIDC/service-principal — simpler for a solo project, no Azure AD setup required.

**Files changed:**
- `.github/workflows/azure-deploy.yml` — new workflow file
- `.squad/decisions/inbox/gandalf-azure-deploy.md` — decision recorded

### 2026-05-07 — Architecture assessment: lighthouse photographs in map popup

**Task:** Jorge2215 wants to show a small photograph of each lighthouse inside the Leaflet map popup card. Assessed feasibility and proposed an approach.

**Codebase findings:**
- `Lighthouse` model has: Name, Location, Latitude, Longitude, Description, Weather. No image field.
- `LighthouseRepository.cs` is a static list of 61 hardcoded entries — no database, no dynamic catalogue.
- `Index.cshtml` serializes the lighthouse list to a JS array at render time; popup HTML is built via string concatenation inside a `forEach` loop. Popup `maxWidth` is 260px.
- `WeatherService` is the only external HTTP service; it is registered via `AddHttpClient<IWeatherService, WeatherService>()` in `Program.cs`.
- No existing image infrastructure of any kind.

**Options evaluated:**
1. Wikimedia Commons API at startup (on-demand per lighthouse, server-side) — free, no key, ~40–60% coverage, medium complexity.
2. Flickr API — requires key, unreliable coverage, complex licensing. Rejected.
3. Curated `ImageUrl` in the data model (Wikimedia Commons static URLs) — free, permanent URLs, 100% reliable for populated entries, low code complexity, one-time curation effort. **Recommended.**
4. Backend proxy / on-demand search per popup interaction — adds latency and a new controller surface for no benefit over static curation. Rejected.

**Decision:** Option 3. The lighthouse list is static; on-demand image search adds complexity with no functional gain. Add `string? ImageUrl` to `Lighthouse`, populate with curated Wikimedia Commons URLs in the repository, render `<img>` in popup when non-null with CC attribution.

**Files created:**
- `.squad/decisions/inbox/gandalf-lighthouse-images.md` — full decision with work item breakdown

### 2026-06-02 — Azure Function architecture decomposition (Weather Statistics)

**Task:** PRD received for an Azure Function that records hourly weather stats for all 61 lighthouses to Azure Table Storage.

**Codebase findings:**
- Solution uses `.slnx` format with 2 projects: web app (net10.0) + tests. No shared library exists.
- `LighthouseRepository.cs` is a static class in `Data/` returning 61 hardcoded `Lighthouse` objects. The Function needs this same data.
- `WeatherService` fetches `temperature_2m`, `wind_speed_10m`, `weather_code` from Open-Meteo. The Function needs additional fields: `wind_direction_10m` and `apparent_temperature`.
- OIDC deployment is already configured in `azure-deploy.yml` with path-filtered triggers and `az webapp deploy`.
- No Azure Function project exists in the solution yet.

**Key architecture decisions:**
1. New `ArgentinaLightHouses.Functions/` project inside existing solution (code sharing)
2. New `ArgentinaLightHouses.Shared/` class library for `Lighthouse` model + `LighthouseRepository`
3. Target .NET 8 LTS for the Function (safe default; .NET 10 Functions support unconfirmed)
4. Table Storage: PartitionKey = LighthouseName, RowKey = ISO 8601 UTC timestamp
5. SemaphoreSlim(5,5) concurrency pattern reused from existing WeatherService
6. Separate GitHub Actions workflow with OIDC auth and path-scoped triggers

**Open questions flagged:**
- .NET version confirmation for Function App runtime
- OIDC federated credential scope (does it cover Function App deployment?)
- Storage connection string app setting name
- Timer cron offset preference
- Data retention policy

**Files created:**
- `.squad/decisions/inbox/gandalf-azure-function-architecture.md`

### 2026-05-06 — Switched deployment auth from Publish Profile to OIDC

**Task:** Azure App Service blocks Basic Auth / Publish Profile. Migrate the workflow to OIDC (Workload Identity Federation) so deployments can succeed.

**Key decisions made:**
- Publish Profile approach dropped — Azure has Basic Auth disabled on this App Service, making `publish-profile` unusable.
- Switched to `azure/login@v2` OIDC flow: no long-lived secrets, uses short-lived GitHub OIDC tokens tied to the `main` branch federated credential.
- Added `permissions: id-token: write / contents: read` to the `deploy` job — mandatory for GitHub to issue OIDC tokens.
- Added three top-level env vars (`AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`) backed by repository secrets.
- `AZURE_WEBAPP_PUBLISH_PROFILE` secret is now obsolete and can be removed.
- Requires one-time Azure AD setup: create service principal, add a federated credential scoped to `Jorge2215/ArgentinaLightHouses` branch `main`.

**Files changed:**
- `.github/workflows/azure-deploy.yml` — OIDC workflow replacing Publish Profile
- `.squad/decisions/inbox/gandalf-oidc-deploy.md` — decision recorded

### 2026-06-02 — Azure Function documentation

**Task:** Wrote technical and architectural documentation for the new Azure Function at `docs/azure-function-architecture.md`.

**Files created:**
- `docs/azure-function-architecture.md` — technical & architectural docs for LighthouseWeatherCollector

**Key patterns documented:**
- Timer-triggered collector and hourly cron schedule
- Table Storage schema and partitioning (PartitionKey = URL-encoded Name, RowKey = ISO 8601 UTC timestamp)
- Concurrency control via SemaphoreSlim(5,5) and per-lighthouse failure isolation
- OIDC-based GitHub Actions deployment and necessary Azure AD federated credential steps
- Data retention guidance (12 months) and recommended cleanup approaches
