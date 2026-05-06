# Skill: HTTP Concurrency Throttle with SemaphoreSlim

**Category:** Backend / HTTP / Performance
**Language:** C# / .NET

---

## Problem

When a page or service fires many HTTP requests concurrently (e.g., via `Task.WhenAll`), free-tier or rate-limited APIs respond with **HTTP 429 Too Many Requests**. If the caller uses a broad `catch` block, those 429 responses are silently swallowed and the UI degrades silently (e.g., "Weather unavailable" for all items).

---

## Solution

Add a `static readonly SemaphoreSlim` to the service class to cap concurrent outbound HTTP calls. Use `WaitAsync` / `Release` with a `finally` block.

```csharp
public class MyService : IMyService
{
    // Shared across all DI instances — this is intentional for rate limiting.
    private static readonly SemaphoreSlim _concurrencyLimiter = new(5, 5);

    private readonly HttpClient _httpClient;

    public async Task<Result?> GetDataAsync(...)
    {
        await _concurrencyLimiter.WaitAsync();
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            // parse and return
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "...");
            return null;
        }
        finally
        {
            _concurrencyLimiter.Release();
        }
    }
}
```

---

## Key Points

- **`static`** — the semaphore is shared across all DI-injected instances, which is correct: the rate limit is on the external API, not per-instance.
- **`finally`** — guarantees the semaphore is always released, even when an exception is caught and re-thrown.
- **Slot count** — 5 is a safe default for free-tier REST APIs. Tune based on the target API's documented rate limits.
- **No extra dependencies** — `SemaphoreSlim` is part of the BCL.

---

## When to Use

- A typed `HttpClient` service makes N concurrent calls for a batch of items
- The target API has per-second or per-connection rate limits
- You want to avoid adding Polly/retry complexity for a straightforward throttle

---

## Real Example

`ArgentinaLightHouses / Services/WeatherService.cs` — 61 lighthouse weather calls fired concurrently caused HTTP 429 from Open-Meteo. Fixed with `SemaphoreSlim(5, 5)`.
