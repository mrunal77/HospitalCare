using Serilog;

namespace HospitalCare.Api.Middleware;

public static class SerilogMiddleware
{
    public static IHostBuilder UseSerilogLogging(this IHostBuilder hostBuilder, string mongoConnectionString, string mongoDatabaseName)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.MongoDBBson(
                $"{mongoConnectionString}/{mongoDatabaseName}",
                collectionName: "logs",
                batchPostingLimit: 50,
                period: TimeSpan.FromSeconds(2),
                cappedMaxSizeMb: 1024,
                cappedMaxDocuments: 100000)
            .CreateLogger();

        hostBuilder.UseSerilog();
        
        return hostBuilder;
    }

    public static IApplicationBuilder UseSerilogRequestLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms - {UserId}";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("UserId", httpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Anonymous");
                diagnosticContext.Set("UserEmail", httpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Anonymous");
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            };
        });

        return app;
    }

    public static void CloseAndFlushSerilog()
    {
        Log.CloseAndFlush();
    }
}
