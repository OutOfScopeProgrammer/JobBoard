using System.Diagnostics;

namespace JobBoard.Shared.EndpointFilters;

public class LogFilter(ILogger<LogFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var result = await next(context);
        stopWatch.Stop();
        LogBuilder(context, stopWatch.ElapsedMilliseconds);
        return result;
    }

    private void LogBuilder(EndpointFilterInvocationContext context, long miliseconds)
    {
        var request = context.HttpContext.Request;
        var route = request.Path.Value;
        var verb = request.Method;
        var statuscode = context.HttpContext.Response.StatusCode;
        logger.LogInformation("{verb} {route} {statusCode}  {time} ms", verb, route, statuscode, miliseconds);
    }
}
