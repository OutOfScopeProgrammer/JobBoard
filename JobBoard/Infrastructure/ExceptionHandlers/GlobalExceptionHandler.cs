using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.ExceptionHandlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/json";
            logger.LogError(ex.Message);
        }
        catch (Exception ex)
        {
            Log(context, ex);
            await context.Response.WriteAsJsonAsync(new { error = "internal server error" });

        }
    }

    private void Log(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        logger.LogError(ex.Message);
    }
}
