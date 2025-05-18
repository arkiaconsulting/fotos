using Grafana.OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Fotos.App;

internal static class InstrumentationConfigurationExtensions
{
    private const string ServiceName = "Fotos";
    private const string ServiceNamespace = "Akc";

    public static void AddFotosInstrumentation(this WebApplicationBuilder builder)
    {
        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

        builder.Services.AddSingleton<InstrumentationConfig>();

        builder.Services.AddOpenTelemetry()
            .WithTracing(traceBuilder => traceBuilder
                .AddSource(InstrumentationConfig.AppActivitySourceName)
                .AddSource("Azure.*")
                .SetSampler(new TraceIdRatioBasedSampler(.2))
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.EnableAspNetCoreSignalRSupport = true;
                })
                .AddHttpClientInstrumentation()
                .SetErrorStatusOnException()
                .UseGrafana(ConfigureGrafana))
            .WithMetrics(metricsBuilder => metricsBuilder
                .AddMeter(InstrumentationConfig.AppMeterName)
                .SetExemplarFilter(ExemplarFilterType.TraceBased)
                .AddHttpClientInstrumentation()
                .UseGrafana(ConfigureGrafana))
            .WithLogging(loggingBuilder => loggingBuilder.ConfigureResource(ConfigureResourceForLogging),
            options =>
            {
                options.IncludeScopes = true;
                options.IncludeFormattedMessage = true;
                options.UseGrafana(ConfigureGrafana);
            });
    }

    private static void ConfigureResourceForLogging(ResourceBuilder builder)
    {
        builder.AddService(ServiceName)
        .AddAttributes(new Dictionary<string, object>
        {
            { "service.namespace", ServiceNamespace },
        });
    }

    private static void ConfigureGrafana(GrafanaOpenTelemetrySettings settings)
    {
        settings.ServiceName = ServiceName;
        settings.ResourceAttributes.Add("service.namespace", ServiceNamespace);
    }
}