# Azure Function Architecture — LighthouseWeatherCollector

## 1. Overview
The LighthouseWeatherCollector Azure Function collects hourly weather readings for each lighthouse in the ArgentinaLightHouses dataset and writes them to Azure Table Storage. It exists to provide a lightweight telemetry store of historical weather at lighthouse locations for reporting, analytics and monitoring. The Function is implemented as an isolated-worker .NET 10 Function and is deployed from GitHub Actions using OIDC.

## 2. Architecture diagram
Mermaid (renderers that understand mermaid can draw this; ASCII is provided below):

Mermaid:

```mermaid
flowchart LR
  subgraph WebApp
    A[ArgentinaLightHouses (Razor Pages)]
  end
  subgraph Function
    F[LightHousesFunction\nLighthouseWeatherCollector]\n    FS[FunctionWeatherService]\n  end
  A -->|Shared models| F
  F -->|HTTP calls| OpenMeteo[Open-Meteo API]
  F -->|writes| Table[Azure Table Storage\nLightHousesWeather]
  F -->|traces| AppInsights[Application Insights]
  GitHub[GitHub Actions (OIDC)] -->|deploy| F

  classDef infra fill:#f9f,stroke:#333,stroke-width:1px;
  class Table,AppInsights,Github infra;
```

ASCII (quick view):

Web App (Razor Pages)  <-->  Shared Library
      |
      v
Function App (LighthouseWeatherCollector) -- HTTP --> Open-Meteo API
Function App -- writes --> Azure Table Storage (LightHousesWeather)
Function App -- telemetry --> Application Insights (OpenTelemetry)
GitHub Actions (OIDC) -- deploys --> Function App

## 3. Solution structure
- ArgentinaLightHouses.Shared/ (class library)
  - Models/Lighthouse.cs
  - Models/WeatherInfo.cs
  - Data/LighthouseRepository.cs  (single source-of-truth for 61 lighthouses)

- ArgentinaLightHouses.Functions/ (Azure Functions isolated worker, .NET 10)
  - Functions/LighthouseWeatherCollector.cs  (Timer-triggered collector)
  - Services/FunctionWeatherService.cs (Open-Meteo HTTP client with concurrency limiter)
  - Program.cs (Functions host, DI, OpenTelemetry)
  - host.json (OpenTelemetry + App Insights sampling)
  - local.settings.json (gitignored template for local dev)

- .github/workflows/azure-function-deploy.yml  (OIDC-based CI/CD for the Functions project)

Dependencies:
- Shared library used by Functions project for Lighthouse model and repository.
- Azure.Data.Tables package used for TableClient interactions.
- Microsoft.Azure.Functions.Worker (isolated worker SDK).
- OpenTelemetry + Azure Monitor exporter configured in Program.cs.

## 4. Azure Resources
- Subscription: 4ffc573f-cffe-48a5-b82a-0f3930ce1700
- Resource Group: LightHouses_rg — contains Function App, Storage, App Insights
- Function App: LightHousesFunction
  - Runtime: .NET 10 (isolated worker)
  - OS: Linux
  - Plan: App Service Plan `ASP-LightHousesrg-9f88`
  - App Settings required: `AzureWebJobsStorage`, `APPLICATIONINSIGHTS_CONNECTION_STRING`
- Storage Account: storagelighthouses
  - Used for: Table Storage (LightHousesWeather) and Function runtime storage
- Table: LightHousesWeather (Azure Table Storage)
  - PartitionKey: URL-encoded Lighthouse Name
  - RowKey: ISO 8601 UTC timestamp (ToString("o"))
- Application Insights: LightHousesFunction
  - Receives telemetry exported via OpenTelemetry

## 5. Function design
Trigger
- TimerTrigger cron: `0 0 * * * *` — hourly at the top of the hour (UTC). Implemented via [TimerTrigger("0 0 * * * *")] attribute.

Concurrency model
- Function uses FunctionWeatherService which enforces a SemaphoreSlim(5,5) to limit concurrent outbound calls to Open-Meteo. The collector fans out per-lighthouse tasks via Task.Run and then awaits Task.WhenAll; per-call concurrency is controlled by the Semaphore in the service.

Failure isolation
- Per-lighthouse failures are isolated: if FetchWeatherAsync returns null (or throws), that lighthouse is skipped and the collector logs a warning or error. The batch continues for remaining lighthouses.

Storage writes
- Each successful reading is upserted to the `LightHousesWeather` table using TableClient.UpsertEntityAsync with TableUpdateMode.Replace. This ensures idempotency for a given partition and timestamp.

Telemetry & observability
- OpenTelemetry is configured in Program.cs and wired to Azure Monitor exporter. host.json enables sampling for Application Insights.
- The Function logs start/finish and per-lighthouse storage success/failure.

Data flow summary
1. Timer triggers function.
2. Function reads static lighthouse list from Shared.LighthouseRepository.GetAll().
3. For each lighthouse, the FunctionWeatherService fetches current observations from Open-Meteo.
4. If successful, a TableEntity is built and UpsertEntityAsync writes it to LightHousesWeather.
5. Function awaits all tasks and logs summary.

## 6. Data schema (LightHousesWeather table)
- PartitionKey (string): URL-encoded lighthouse Name (Uri.EscapeDataString)
- RowKey (string): Timestamp in ISO 8601 UTC format (e.g., 2026-06-02T15:00:00.0000000Z)
- Name (string): Lighthouse display name
- Latitude (double): Decimal latitude (invariant culture)
- Longitude (double): Decimal longitude (invariant culture)
- Date (string): YYYY-MM-DD (UTC date of reading)
- Time (string): HH:mm:ss (UTC time of reading)
- TemperatureCelsius (double): temperature_2m from Open-Meteo
- WindSpeedKmh (double): wind_speed_10m (km/h)
- WindDirectionDegrees (double): wind_direction_10m (degrees)
- WindchillCelsius (double): apparent_temperature

Notes:
- All numeric fields are stored as native numeric types (TableEntity infers types from the CLR values).
- Timestamp consistency: the code uses DateTime.UtcNow captured once per run to provide a consistent RowKey across entries in the same batch.

## 7. GitHub Actions workflow
- Workflow: `.github/workflows/azure-function-deploy.yml`
- Triggers: push to `main` and workflow_dispatch. Path filter: `ArgentinaLightHouses.Functions/**` and `ArgentinaLightHouses.Shared/**` to avoid unnecessary runs.
- Build: restore, build (Release), publish and upload artifact.
- Deploy job uses OIDC (azure/login@v2) with repository secrets `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID` present.
- Permissions: `id-token: write`, `contents: read` are required so GitHub can mint OIDC tokens.
- Deploy step uses `Azure/functions-action@v1` to push the zipped publish output.

Important notes:
- Ensure the Azure AD service principal referenced by `AZURE_CLIENT_ID` has a federated credential with subject `repo:Jorge2215/ArgentinaLightHouses:ref:refs/heads/main` and the required role (Contributor or specific scope) on the Function App or Resource Group.

## 8. Azure Portal configuration (manual steps)
1. App Settings (Function App -> Configuration) — add or verify:
   - `AzureWebJobsStorage` = connection string for `storagelighthouses` (used by runtime and table writes)
   - `APPLICATIONINSIGHTS_CONNECTION_STRING` = App Insights connection string
   - Do NOT hardcode secrets in code. Use App Settings for all runtime secrets.

2. Create/Open the federated credential in Azure AD for the workload identity used by GitHub Actions (one-time):
   - Create a service principal for CI/CD (if not present).
   - Add a federated credential with issuer `https://token.actions.githubusercontent.com` and subject `repo:Jorge2215/ArgentinaLightHouses:ref:refs/heads/main`.
   - Assign Role (Contributor or a scoped role) to the service principal on the resource group `LightHouses_rg` or specific Function App.

3. Application Insights sampling
   - host.json enables sampling. Review in App Insights to tune ingestion/sampling if needed to reduce cost.

## 9. Local development
Prerequisites:
- .NET 10 SDK (required by solution)
- Azure Functions Core Tools (for local Functions runtime) — recommended
- Azurite (local Storage emulator) or a real Storage connection string

Steps:
1. Start Azurite (or ensure storage emulator is available):
   - Using npm: `npx azurite` or run Visual Studio / VS Code Azurite extension.
2. Copy `local.settings.json` from the Functions project template and set:
   - `AzureWebJobsStorage` = `UseDevelopmentStorage=true` (Azurite)
   - `APPLICATIONINSIGHTS_CONNECTION_STRING` = (optional for local)
3. From solution root, run either:
   - `func start --script-root ArgentinaLightHouses.Functions` (requires Functions Core Tools)
   - or `dotnet run --project ArgentinaLightHouses.Functions` (isolated worker will run the host)
4. The function will create the `LightHousesWeather` table automatically on first write.

Notes:
- The Function relies on shared code `ArgentinaLightHouses.Shared` so ensure the project reference is valid and `dotnet restore` has run.

## 10. Data retention strategy (12 months)
Storage accounts do not offer native time-based automatic deletion for Table entities the same way Blob lifecycle policies do. Options to enforce 12-month retention:

1. Lifecycle / Service-managed (not available for Table entities): NOT APPLICABLE

2. Storage account-level solution for Table data:
   - Implement a separate Azure Function (timer-triggered, e.g., daily) that scans `LightHousesWeather` partitions and deletes entities older than 365 days. This function can be deployed alongside the collector and share the same storage credentials.

   Pseudocode:
   - Query entities with RowKey (timestamp) or a Date property and filter `Date < DateTime.UtcNow.AddDays(-365)`.
   - DeleteMatchingEntities using Batch operations per PartitionKey where possible.

3. Azure Automation / Logic Apps:
   - Use a scheduled Logic App to call an API or run a script that deletes old table entries.

Recommendation: Implement an additional Timer-triggered cleanup Function placed in the same Functions project. It can reuse the Shared models and TableClient and be scheduled once per day. Keep it lightweight and idempotent.

## Appendix: Operational notes and maintenance
- Keep Open-Meteo usage polite — SemaphoreSlim(5) is configured to avoid 429s. Monitor function logs for repeated 429/429-like failures and lower concurrency if needed.
- Monitor Application Insights for errors and cold-starts. If the Function experiences high cold-start rates, consider switching to a Consumption Plan with Premium features or a dedicated App Service Plan as configured.
- Backups: Table Storage snapshots are not available for Table entities; consider exporting data periodically to Blob Storage or a managed analytics store for long-term retention beyond 12 months.


---
Generated by Gandalf on 2026-06-02 — documents implemented Azure Function architecture, telemetry, storage schema and operational guidance.
