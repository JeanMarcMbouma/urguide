using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UrGuide.WebApp.Middleware;

/// <summary>
/// Middleware that logs the duration of every HTTP request.
/// Requests faster than <see cref="SlowRequestThresholdMs"/> are logged at Debug level;
/// requests that exceed the threshold are logged at Warning level so that they
/// can be alerted on in log-aggregation systems.
/// The correlation ID added by <see cref="CorrelationIdMiddleware"/> is included
/// automatically because it lives in the ambient logging scope.
/// </summary>
public class RequestPerformanceMiddleware
{
    private const long SlowRequestThresholdMs = 1000;
    private const string CorrelationIdHeader  = "X-Correlation-ID";

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestPerformanceMiddleware> _logger;

    public RequestPerformanceMiddleware(RequestDelegate next, ILogger<RequestPerformanceMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            var elapsed  = sw.ElapsedMilliseconds;
            var method   = context.Request.Method;
            var path     = context.Request.Path;
            var status   = context.Response.StatusCode;
            var corrId   = context.Items.TryGetValue(CorrelationIdHeader, out var id) ? id?.ToString() : null;

            if (elapsed >= SlowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "PERF: Slow request {Method} {Path} responded {StatusCode} in {ElapsedMs}ms | CorrelationId={CorrelationId}",
                    method, path, status, elapsed, corrId);
            }
            else
            {
                _logger.LogDebug(
                    "PERF: {Method} {Path} responded {StatusCode} in {ElapsedMs}ms | CorrelationId={CorrelationId}",
                    method, path, status, elapsed, corrId);
            }
        }
    }
}
