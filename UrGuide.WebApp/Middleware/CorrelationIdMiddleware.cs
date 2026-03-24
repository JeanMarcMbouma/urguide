using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UrGuide.WebApp.Middleware;

/// <summary>
/// Middleware that generates or propagates a correlation ID for each request.
/// The correlation ID is read from the X-Correlation-ID request header (if present),
/// or a new GUID is generated. The ID is added to the response header and to the
/// logging scope so that all log entries for the request include it.
/// </summary>
public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        context.Items[CorrelationIdHeader] = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            await _next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var existingId)
            && !string.IsNullOrWhiteSpace(existingId))
        {
            var rawId = existingId.ToString().Trim();

            // Only accept valid GUIDs to prevent log injection and malformed header values.
            // Normalize to canonical "D" format (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).
            if (Guid.TryParse(rawId, out var parsedGuid))
            {
                return parsedGuid.ToString("D");
            }
        }

        return Guid.NewGuid().ToString("D");
    }
}
