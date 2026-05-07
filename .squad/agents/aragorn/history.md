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
