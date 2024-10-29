using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Fotos.Client;

public sealed class InstrumentationConfig : IDisposable
{
    public ActivitySource ActivitySource { get; }

    internal const string ActivitySourceName = "App.Fotos";
    internal const string MeterName = "App.Fotos";
    private readonly Meter _meter;

    public InstrumentationConfig()
    {
        string? version = typeof(InstrumentationConfig).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        _meter = new Meter(MeterName, version);
    }

    public void Dispose()
    {
        ActivitySource.Dispose();
        _meter.Dispose();
    }
}
