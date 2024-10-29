using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Fotos.Client;

internal static class InstrumentationConfigurationExtensions
{
    public static void AddFotosInstrumentation(this WebApplicationBuilder builder)
    {
        var useOtlpExporter = builder.Configuration.GetValue<bool>("Instrumentation:UseOtlpExporter");
        var otlpEndpoint = new Uri("http://localhost:4317");
        builder.Services.AddSingleton<InstrumentationConfig>();
        builder.Logging.ClearProviders();
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService(
                serviceName: builder.Configuration["Instrumentation:ServiceName"]!,
                serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown",
                serviceInstanceId: Environment.MachineName)
            ).WithTracing(traceBuilder =>
            {
                traceBuilder.AddSource(InstrumentationConfig.ActivitySourceName)
                        .SetSampler<AlwaysOnSampler>()
                        .AddAspNetCoreInstrumentation(options => options.RecordException = true);
                if (useOtlpExporter)
                {
                    traceBuilder.AddOtlpExporter(options => options.Endpoint = otlpEndpoint);
                }
                else
                {
                    traceBuilder.AddAzureMonitorTraceExporter(options => options.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
                }
            })
            .WithMetrics(metricsBuilder =>
            {
                metricsBuilder.AddMeter(InstrumentationConfig.MeterName)
                        .SetExemplarFilter(ExemplarFilterType.TraceBased)
                        .AddAspNetCoreInstrumentation();
                if (useOtlpExporter)
                {
                    metricsBuilder.AddOtlpExporter(options => options.Endpoint = otlpEndpoint);
                }
                else
                {
                    metricsBuilder.AddAzureMonitorMetricExporter(options => options.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
                }
            }).WithLogging(loggingBuilder =>
            {
                if (useOtlpExporter)
                {
                    loggingBuilder.AddOtlpExporter(options => options.Endpoint = otlpEndpoint);
                }
            });

        if (!useOtlpExporter)
        {
            builder.Services.Configure<OpenTelemetryLoggerOptions>(options => options.AddAzureMonitorLogExporter(options => options.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]));
        }
    }
}
