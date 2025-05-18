using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Fotos.App;

public static class DiagnosticConfig
{
    internal const string AppActivitySourceName = "Akc.Fotos";
    public static readonly ActivitySource AppActivitySource = new(AppActivitySourceName);

    private static readonly Meter _meter = new(AppActivitySourceName);

    public static readonly Counter<long> PhotoUploadCounter =
        _meter.CreateCounter<long>("fotos.photos.uploaded", description: "Number of photos uploaded");

    public static Activity? StartUserActivity(string name)
    {
        Activity.Current = null;

        return AppActivitySource.StartActivity(name, ActivityKind.Client);
    }
}
