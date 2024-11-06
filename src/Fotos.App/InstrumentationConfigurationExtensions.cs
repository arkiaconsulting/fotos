using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Fotos.App;

internal static class InstrumentationConfigurationExtensions
{
    public static void AddFotosInstrumentation(this WebApplicationBuilder builder)
    {
        var useOtlpExporter = builder.Configuration.GetValue<bool>("Instrumentation:UseOtlpExporter");
        var azureMonitorConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
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
                        .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                        .AddHttpClientInstrumentation();
                if (useOtlpExporter)
                {
                    traceBuilder.AddOtlpExporter(options => options.Endpoint = otlpEndpoint);
                }
                else if (!string.IsNullOrWhiteSpace(azureMonitorConnectionString))
                {
                    traceBuilder.AddAzureMonitorTraceExporter(options => options.ConnectionString = azureMonitorConnectionString);
                }
            })
            .WithMetrics(metricsBuilder =>
            {
                metricsBuilder.AddMeter(InstrumentationConfig.MeterName)
                        .SetExemplarFilter(ExemplarFilterType.TraceBased)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();
                if (useOtlpExporter)
                {
                    metricsBuilder.AddOtlpExporter(options => options.Endpoint = otlpEndpoint);
                }
                else if (!string.IsNullOrWhiteSpace(azureMonitorConnectionString))
                {
                    metricsBuilder.AddAzureMonitorMetricExporter(options => options.ConnectionString = azureMonitorConnectionString);
                }
            }).WithLogging(loggingBuilder =>
            {
                if (useOtlpExporter)
                {
                    loggingBuilder.AddOtlpExporter(options => options.Endpoint = otlpEndpoint);
                }
            });

        if (!(useOtlpExporter || string.IsNullOrWhiteSpace(azureMonitorConnectionString)))
        {
            builder.Services.Configure<OpenTelemetryLoggerOptions>(options => options.AddAzureMonitorLogExporter(options => options.ConnectionString = azureMonitorConnectionString));
        }
    }
}
