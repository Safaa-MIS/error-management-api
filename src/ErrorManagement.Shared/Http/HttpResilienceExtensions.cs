using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Timeout;

namespace ErrorManagement.Shared.Http;

public static class HttpResilienceExtensions
{
    /// <summary>
    /// Standard resilience pipeline for all internal API clients.
    /// Retry(3, exponential 300ms) → AttemptTimeout(8s)
    /// </summary>
    public static IHttpClientBuilder AddInternalApiResilience(
        this IHttpClientBuilder builder, string clientName)
    {
        builder.AddResilienceHandler(clientName, pipeline =>
        {
            // ── Total timeout — hard ceiling across all retries ──────────────
          // pipeline.AddTimeout(new HttpTimeoutStrategyOptions
          //     { Timeout = TimeSpan.FromSeconds(25), Name = "TotalTimeout" });

            // ── Retry — 3 attempts, exponential backoff with jitter ──────────

            pipeline.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(300),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutRejectedException>()
                    .HandleResult(r => (int)r.StatusCode is 408 or 429 or 500 or 502 or 503 or 504),
                Name = "Retry"
            });
            // ── Circuit breaker ───────────────────────────────────────────────
            // pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            // {
            //     FailureRatio      = 0.5,
            //     SamplingDuration  = TimeSpan.FromSeconds(15),
            //     MinimumThroughput = 5,
            //     BreakDuration     = TimeSpan.FromSeconds(30),
            //     ShouldHandle      = new PredicateBuilder<HttpResponseMessage>()
            //         .Handle<HttpRequestException>()
            //         .Handle<TimeoutRejectedException>()
            //         .HandleResult(r => (int)r.StatusCode is 500 or 502 or 503 or 504),
            //     Name = "CircuitBreaker"
            // });
            pipeline.AddTimeout(new HttpTimeoutStrategyOptions
            { Timeout = TimeSpan.FromSeconds(8), Name = "AttemptTimeout" });
        });

        return builder; // return the original builder, not the resilience builder
    }
}