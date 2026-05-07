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
