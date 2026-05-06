# Decision Drop — Weather Bug Fix

**Author:** Gimli (Backend Dev)
**Date:** 2026-05-06T11:04:20.219-03:00
**Status:** Proposed

---

## What I Found

`IndexModel.OnGetAsync()` fires 61 concurrent `GetWeatherAsync` calls via `Task.WhenAll` — one per lighthouse. The Open-Meteo free tier rate-limits concurrent requests with **HTTP 429 Too Many Requests**. The `WeatherService` broad `catch (Exception ex)` block swallows the resulting `HttpRequestException` and returns `null`, which the frontend renders as "Weather unavailable" for every throttled lighthouse. Because all 61 calls hit the API simultaneously on page load, nearly all of them were being rate-limited.

Verified with a test script: out of 10 concurrent requests, 3 returned 429 immediately.

---

## What I Changed

**File:** `Services/WeatherService.cs`

Added a `static readonly SemaphoreSlim _concurrencyLimiter = new(5, 5)` to the class. Each `GetWeatherAsync` call:
1. Awaits the semaphore before making the HTTP request
2. Releases it in a `finally` block

This caps concurrent Open-Meteo requests at 5 across all callers, eliminating 429 responses without any new dependencies.

---

## Why This Approach

- No new NuGet packages — `SemaphoreSlim` is built-in
- `static` field shares the limiter across all DI-injected instances (correct behaviour for a rate limit)
- `finally` ensures the semaphore is always released even on exception
- 5 concurrent slots is conservative for the free Open-Meteo tier while keeping total page load time acceptable (~2–3s for 61 lighthouses at ~200ms/request)

---

## Alternatives Considered

- **Polly retry with exponential backoff on 429:** Would work but adds a dependency and increases page load time unpredictably on 429 bursts.
- **Sequential requests:** Guaranteed safe but ~12s page load for 61 lighthouses — too slow.
- **Batching in the page model:** Would push concurrency policy into the caller — better kept in the service boundary.
