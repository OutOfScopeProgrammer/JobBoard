
using System.Diagnostics;

namespace JobBoard.Infrastructure.Logger.Middlewares;

public class LogMiddleware(ILogger<LogMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        await next(context);
        stopWatch.Stop();
        Log(context, stopWatch.ElapsedMilliseconds);

    }

    private void Log(HttpContext context, long milliseconds)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;
        var statusCode = context.Response.StatusCode;
        logger.LogInformation("{method} {path} {statusCode} {milliseconds} ms", method, path, statusCode, milliseconds);
    }

}
