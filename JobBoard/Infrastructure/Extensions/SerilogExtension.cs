using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Grafana.Loki;

namespace JobBoard.Infrastructure.Extensions;

public static class SerilogExtension
{
    public static void AddSerilogWithLokiConfiguration(this IHostBuilder builder)
    {
        builder.UseSerilog(static (context, config) =>
        {
            config.MinimumLevel.Fatal()
                // all other system logs are disable 
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Warning)
                // Only Filters in theese directory will be logged with information level
                .MinimumLevel.Override("JobBoard.Infrastructure.Logger.Middlewares", LogEventLevel.Information)
                .MinimumLevel.Override("GlobalExceptionHandler", LogEventLevel.Information)

                .WriteTo.Console()
                .WriteTo.GrafanaLoki(
                   "http://localhost:3100",
                    labels: [new LokiLabel() { Key = "JobBoardBackend", Value = "v1" }],
                    textFormatter: new RenderedCompactJsonFormatter()
                   );
        });
    }

}
