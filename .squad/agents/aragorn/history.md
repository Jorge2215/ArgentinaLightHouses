# Aragorn — Project History

## Project Seed

- **Project:** ArgentinaLightHouses
- **Stack:** C# / ASP.NET Core (.NET 10) Razor Pages
- **Description:** A web application showcasing Argentina's lighthouses. Backend has `LighthouseRepository` and `WeatherService`. Pages include Index, Lighthouses, and Privacy.
- **Testing approach:** xUnit or NUnit for .NET 10; integration testing via ASP.NET Core test host
- **User:** Jorge2215
- **Team joined:** 2026-05-06

## Learnings

### 2026-05-06T21:21:14-03:00 — Test project structure findings

- **Test project file:** ArgentinaLightHouses.Tests.csproj
- **Target framework:** net10.0
- **Test framework:** xUnit (xunit 2.9.3, xunit.runner.visualstudio 3.1.4)
- **Test files:** LighthouseRepositoryTests.cs
- **Project reference:** References main project via <ProjectReference Include="..\ArgentinaLightHouses.csproj" />
- **CI notes:** All required test runner packages present. Main project excludes test files from its build via <Compile Remove="ArgentinaLightHouses.Tests\**" />. No CI-specific issues detected for dotnet test on .NET 10.

### 2026-05-06T01:10:34-03:00 — LighthouseRepository test suite

- **Created** `ArgentinaLightHouses.Tests/` with xUnit (net10.0). Used `dotnet new xunit` because the template correctly sets `<Using Include="Xunit" />` as a global implicit using, which is required for the `[Fact]` attribute to resolve without an explicit `using Xunit;` statement.
- **11 tests** written and passing for `LighthouseRepository.GetAll()`: null/empty guard, exact count (61), non-empty name/location/description for all entries, valid Argentine coordinates, no duplicate (lat, lon) pairs, Patagonia presence, Tierra del Fuego presence.
- **Coordinate range note:** Two lighthouses are in Argentine Antarctic Territory (Faro 1ro. de Mayo at -64.3, Faro Esperanza at -63.4). Latitude range in tests uses [-66, -22] to include them correctly. The task spec of -22 to -56 would have produced false failures.
- **SDK gotcha:** `Microsoft.NET.Sdk.Web` globs `**/*.cs` recursively, so placing the test project as a subdirectory of the main project caused the main project to attempt to compile test files (which reference xunit). Fix: add `<Compile Remove="ArgentinaLightHouses.Tests\**" />` to the main `.csproj`.
- **Solution file:** Updated `ArgentinaLightHouses.slnx` to include the test project path.

### 2026-05-07T21:02:19-03:00 — ImageUrl feature test suite

- **New test file:** `LighthouseImageUrlTests.cs` — 7 tests covering the `ImageUrl` nullable property added to `Lighthouse.cs` by Gimli.
- **Model tests (2):** `Lighthouse` defaults `ImageUrl` to `null` when not set; can hold a valid absolute URL string.
- **Repository tests (3):** At least one lighthouse has a non-null `ImageUrl`; all non-null `ImageUrl` values pass `Uri.TryCreate` + `uri.Scheme == "https"`; no lighthouse has an empty string `ImageUrl`.
- **Edge case (1):** Accessing `ImageUrl = null` does not throw.
- **Data shape confirmed:** 29 of 61 lighthouses have Wikimedia Commons URLs (all `https://upload.wikimedia.org/...`); remaining 32 have `null`. No empty strings found.
- **All 17 tests pass** (11 pre-existing + 6 new image tests) — no bugs found in Gimli's implementation.

### 2026-06-02T21:02:59-03:00 — WeatherGrid test suite

- **New packages:** `Moq 4.20.72` and `Microsoft.Extensions.Logging.Abstractions 10.0.8` added to the test project — now the standard mocking stack.
- **WeatherGridServiceTests.cs (2 tests):** Covers graceful degradation — null and empty `AzureStorageConnection` config both return empty list. Azure Table Storage happy path is not unit tested (no interface wrapping `TableClient`); emulator-based integration tests are a future concern.
- **WeatherGridModelTests.cs (3 tests):** `IWeatherGridService` mocked via Moq; `ILogger<WeatherGridModel>` via `NullLogger`. Tests cover: service returns 3 records → Records has 3 items; service returns empty → Records empty, no error; service throws → ErrorMessage non-empty, Records empty.
- **All 22 tests pass** (17 pre-existing + 5 new WeatherGrid tests). No regressions.
- **Decision note:** `.squad/decisions/inbox/aragorn-weathergrid-tests.md`

### 2026-06-02T16:34:39-03:00 — Update: Azure Function projects added

- New projects added to the solution: ArgentinaLightHouses.Shared and ArgentinaLightHouses.Functions.
- Build verified with 0 errors; tests remain passing (17/17).
- Note for Aragorn: tests that reference shared models may need to be updated to reference the shared class library if required by future refactors.


### 2026-06-03T11:20:48-03:00 — Issue #21 verification complete

- Verified the WeatherGrid date range filter workflow after implementation for issue #21.
- Confirmed **22/22 tests passing** and validated that the Azure deployment succeeded.
- Delivery status: live site verification completed and confirmed by Jorgito.

### 2026-07-17T22:31:48.541-03:00 — Full suite regression run
 
- Ran `dotnet test` from the repository root.
- Result: **22 total / 22 passed / 0 failed / 0 skipped**.
- Suite remains green with no regression failures observed.

### 2026-07-17T22:35:08.403-03:00 — Page model and weather service coverage expansion
 
- **New test files:** `IndexModelTests.cs`, `LighthousesModelTests.cs`, and `WeatherServiceTests.cs`.
- **IndexModel coverage (4 tests):** success path loads repository data, null weather results are tolerated, empty lighthouse lists stay empty without calling the service, and fetched `WeatherInfo` instances are written back onto the corresponding `Lighthouse` objects.
- **LighthousesModel coverage (3 tests):** success path loads lighthouses, null weather results are tolerated, and the page model exposes the populated `Lighthouses` list after `OnGetAsync`.
- **WeatherService coverage (8 test cases):** JSON success-path parsing, graceful `HttpClient` failure handling, known/unknown weather description mapping, known weather icon mapping, and invariant-culture URL formatting validated via captured request URI.
- **Patterns used:** Moq remains the standard mocking library; page-model tests use small test-only subclasses to override lighthouse sources, and `NullLogger<T>` is still the logger test double convention for concrete services.
- **Testability seam:** Added protected virtual `GetLighthouses()` methods to `IndexModel` and `LighthousesModel` so empty/custom repository scenarios can be exercised without changing runtime behavior.
- **Verification:** `dotnet test --no-restore` passed with **37 total / 37 passed / 0 failed / 0 skipped**.

### 2026-07-25T20:24:07.610-03:00 — Issue #30 search regression verification

- Reviewed `ArgentinaLightHouses.Tests\IndexModelTests.cs` and `ArgentinaLightHouses.Tests\LighthousesModelTests.cs` to confirm local conventions: xUnit + Moq, page-model subclasses for custom repository input, and `OnGetAsync()`-focused coverage.
- Ran `dotnet test --nologo` before changes: **37 passed / 0 failed / 0 skipped**.
- Added **2 search-adjacent server-side tests**:
  - `IndexModelTests.OnGetAsync_PreservesSearchableNameAndLocationFieldsForClientFiltering` verifies the map page model keeps `Name` and `Location` intact for the client-side search/filter script.
  - `LighthouseRepositoryTests.GetAll_AllLocationsProduceNonEmptyProvinceTokenForMapSearchFilter` verifies every repository location can yield a non-empty province token using the same last-segment rule as the map filter.
- Ran `dotnet test --nologo` after additions: **39 passed / 0 failed / 0 skipped**.
- Verified gap: the actual real-time filtering behavior, clear buttons, “No lighthouses found” state, and Leaflet marker visibility/highlight remain **client-side JavaScript only** and are not directly testable with the current xUnit server-side suite.

### 2026-07-25T21:32:00.174-03:00 — Issue #28 extreme weather regression verification

- Reviewed `Models\WeatherRecord.cs` and `Services\WeatherGridService.cs` to confirm the server-side delta: `WeatherCode` was added to `WeatherRecord`, and `WeatherGridService` now reads `WeatherCode` from Azure Table entities with a `0` fallback when absent.
- Ran baseline `dotnet test`: **39 passed / 0 failed / 0 skipped**.
- Added **23 server-side tests** for the new weather-grid behavior:
  - `WeatherRecordTests.cs` covers frost (`<= 0°C`), high wind (`>= 60 km/h`), storm WMO code boundaries (`80-82`, `95-99`), and severity precedence (`storm > wind > frost`) via computed model properties.
  - `WeatherGridServiceTests.cs` now verifies `MapRecord()` preserves boundary `WeatherCode` values and defaults missing `WeatherCode` to `0`.
- Added a small testability seam only: `WeatherGridService.MapRecord()` / `GetWeatherCode()` plus `InternalsVisibleTo` for the test assembly; runtime behavior is unchanged.
- Ran `dotnet test` after additions: **62 passed / 0 failed / 0 skipped**.
- Verified gap: icon stacking, row-class application inside `renderTable()`, and legend rendering remain client-side JavaScript/CSS behavior and are still not directly exercised by the current xUnit-only server-side suite.

### 2026-07-26T00:58:26Z — Orchestration note

- Ran full test suite and added 23 tests for WeatherRecord and WeatherGridService; verified 62/62 passing.

