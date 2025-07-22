
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace JobBoard.Shared.EndpointFilters;

public class LogginFilter(ILogger<LogginFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var result = await next(context);
        stopWatch.Stop();
        var logMessage = LogBuilder(context, stopWatch.ElapsedMilliseconds);
        logger.LogInformation(logMessage);
        return result;
    }

    private string LogBuilder(EndpointFilterInvocationContext context, long miliseconds)
    {
        var sb = new StringBuilder();
        var request = context.HttpContext.Request;
        var route = request.Path.Value;
        var verb = request.Method;
        sb.Append(verb.ToUpper())
        .Append(" ")
        .Append(route)
        .Append("  ")
        .Append("time:")
        .Append(miliseconds)
        .Append(" ms.");

        return sb.ToString();
    }
}
