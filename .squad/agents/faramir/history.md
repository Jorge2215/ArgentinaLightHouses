# Faramir — History

## Project Context
- **Project:** ArgentinaLightHouses — ASP.NET Core (.NET 10) Razor Pages web app
- **Stack:** C#, Razor Pages, Azure App Service (Linux), Azure Functions, Azure Table Storage
- **User:** Jorge2215 (Jorgito)
- **Joined:** 2026-06-02

## Learnings

### 2026-06-02 — Opened PR #20 for WeatherGrid contrast fix
- PR: https://github.com/Jorge2215/ArgentinaLightHouses/pull/20
- Scope: dev → main for Arwen's CSS-only WeatherGrid table contrast fix (`2112225`)
- Included explicit dark backgrounds on `.alh-table`, `tbody tr`, `tbody td`, `.table-responsive`, zebra striping, and stronger hover feedback
- Validation noted in PR: build clean and 22/22 tests passing

### 2026-06-02 — wwwroot/lib static assets not committed to git
- Root cause: `dist/` pattern in `.gitignore` was silently excluding `wwwroot/lib/bootstrap/dist/` and `wwwroot/lib/jquery/dist/` files
- Fix: force-staged 56 files via `git add -f wwwroot/lib/` and committed as `b480468`
- Impact: Bootstrap and jQuery were 404ing on Azure because GH Actions checkout lacked them
- Rule: always verify `git ls-files wwwroot/lib` includes .min.css/.min.js after any lib update

### 2026-06-02 — Azure startup crash diagnosis
- "Worker process failed to start within allotted time" can be a false failure
- Azure retries container starts automatically; second attempt succeeded (~40s warmup)
- `az webapp deploy` exits with error code 1 if first container attempt times out, even if retry succeeds
- Docker logs available via: `az webapp log download --name lighthouses-app --resource-group LightHouses_rg`
- Certificate update step (`update-ca-certificates`) in Linux container takes ~22s — accounts for warmup delay

### 2026-06-03T11:20:48-03:00 — Issue #21 delivery and production rollout

- Committed the WeatherGrid date range filter feature for issue #21 and pushed it to dev.
- Merged PR #22 into main and confirmed the Azure deployment completed successfully.
- Delivery status: production is live with the date filter and the rollout was verified by Jorgito.
