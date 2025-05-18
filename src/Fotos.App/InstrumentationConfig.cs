using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Fotos.App;

public sealed class InstrumentationConfig : IDisposable
{
    public ActivitySource AppActivitySource { get; }
    public Counter<long> PhotoUploadCounter { get; }

    internal const string AppActivitySourceName = "Akc.Fotos";
    internal const string AppMeterName = "Akc.Fotos";
    private readonly Meter _meter;

    public InstrumentationConfig()
    {
        var version = typeof(InstrumentationConfig).Assembly.GetName().Version?.ToString();
        AppActivitySource = new ActivitySource(AppActivitySourceName, version);
        _meter = new Meter(AppMeterName, version);
        PhotoUploadCounter = _meter.CreateCounter<long>("fotos.photos.uploaded", "Number of photos uploaded");
    }

    public void Dispose()
    {
        AppActivitySource.Dispose();
        _meter.Dispose();
    }
}
