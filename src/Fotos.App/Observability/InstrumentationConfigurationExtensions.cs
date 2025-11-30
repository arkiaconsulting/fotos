using Fotos.Application;
using Grafana.OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Fotos.App.Observability;

internal static class InstrumentationConfigurationExtensions
{
    private const string ServiceName = "Fotos";
    private const string ServiceNamespace = "Akc";

    public static void AddFotosInstrumentation(this WebApplicationBuilder builder)
    {
        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

        var otel = builder.Services.AddOpenTelemetry()
            .WithTracing(traceBuilder => traceBuilder
                .AddSource(DiagnosticConfig.AppActivitySource.Name)
                .AddSource("Azure.*")
                .AddProcessor<ServiceBusFilteringProcessor>()
                .AddProcessor<BlazorHubFilteringProcessor>()
                .SetSampler(new TraceIdRatioBasedSampler(1))
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.EnableAspNetCoreSignalRSupport = true;
                })
                .AddHttpClientInstrumentation()
                .SetErrorStatusOnException()
                .UseGrafana(settings => ConfigureGrafana(settings, builder.Configuration)))
            .WithMetrics(metricsBuilder => metricsBuilder
                .AddMeter(DiagnosticConfig.AppActivitySource.Name)
                .SetExemplarFilter(ExemplarFilterType.TraceBased)
                .AddHttpClientInstrumentation()
                .UseGrafana(settings => ConfigureGrafana(settings, builder.Configuration)))
            .WithLogging(loggingBuilder => { },
            options =>
            {
                options.IncludeScopes = true;
                options.IncludeFormattedMessage = true;
                options.UseGrafana(settings => ConfigureGrafana(settings, builder.Configuration));
            });
    }

    private static void ConfigureGrafana(GrafanaOpenTelemetrySettings settings, IConfiguration configuration)
    {
        // extract deployment.environment value from the environment variable OTEL_RESOURCE_ATTRIBUTES=deployment.environment.name=production
        var resourceAttributes = configuration.GetValue("OTEL_RESOURCE_ATTRIBUTES", "deployment.environment=unknown");
        foreach (var attribute in resourceAttributes.Split(','))
        {
            var keyValue = attribute.Split('=');
            if (keyValue.Length == 2 && keyValue[0] == "deployment.environment")
            {
                settings.DeploymentEnvironment = keyValue[1];
            }
        }
    }
}