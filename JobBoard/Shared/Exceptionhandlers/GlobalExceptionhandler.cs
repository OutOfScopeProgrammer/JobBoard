using Microsoft.AspNetCore.Diagnostics;

namespace JobBoard.Shared.ExceptionHandlers;

public static class GlobalExceptionhandler
{
    public static void UseGlobalExceptionhandler(this IApplicationBuilder app)
        => app.UseExceptionHandler(ex =>
            ex.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = exceptionHandlerFeature?.Error;
                var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");
                logger.LogError(exception?.Message);
                await context.Response.WriteAsJsonAsync(new { error = "internal server error" });
            })
        );
}
