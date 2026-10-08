using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UserManagementAPI.Middleware
{
    public sealed class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var sw = Stopwatch.StartNew();
            var request = context.Request;
            var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            _logger.LogInformation("Started {Method} {Path}{QueryString} from {RemoteIp}",
                request.Method, request.Path, request.QueryString, remoteIp);

            try
            {
                await _next(context);
                sw.Stop();

                _logger.LogInformation("Finished {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                    request.Method, request.Path, context.Response.StatusCode, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Unhandled exception processing {Method} {Path} after {ElapsedMs}ms",
                    request.Method, request.Path, sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}