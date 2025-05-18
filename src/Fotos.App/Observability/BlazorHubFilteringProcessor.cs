using OpenTelemetry;
using System.Diagnostics;

namespace Fotos.App.Observability;

internal sealed class BlazorHubFilteringProcessor : BaseProcessor<Activity>
{
    public override void OnEnd(Activity data)
    {
        if (data.IsAllDataRequested && data.Kind == ActivityKind.Server)
        {
            if (data.Status != ActivityStatusCode.Error
                && data.GetTagItem("rpc.service") is string rpcService
                && rpcService.Equals("Microsoft.AspNetCore.Components.Server.ComponentHub", StringComparison.OrdinalIgnoreCase))
            {
                // Mark as not recorded to filter it out
                data.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;
            }
        }

        base.OnEnd(data);
    }
}
