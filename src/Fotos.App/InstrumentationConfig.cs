using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Fotos.App;

public sealed class InstrumentationConfig : IDisposable
{
    public ActivitySource ActivitySource { get; }
    public Counter<long> PhotoUploadCounter { get; }

    internal const string ActivitySourceName = "App.Fotos";
    internal const string MeterName = "App.Fotos";
    private readonly Meter _meter;

    public InstrumentationConfig()
    {
        var version = typeof(InstrumentationConfig).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        _meter = new Meter(MeterName, version);
        PhotoUploadCounter = _meter.CreateCounter<long>("fotos.photos.uploaded", "Number of photos uploaded");
    }

    public void Dispose()
    {
        ActivitySource.Dispose();
        _meter.Dispose();
    }
}
