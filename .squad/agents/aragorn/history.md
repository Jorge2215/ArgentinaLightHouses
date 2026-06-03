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

