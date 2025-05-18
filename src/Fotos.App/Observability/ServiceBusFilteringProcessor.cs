using OpenTelemetry;
using System.Diagnostics;

namespace Fotos.App.Observability;

internal sealed class ServiceBusFilteringProcessor : BaseProcessor<Activity>
{
    public override void OnEnd(Activity data)
    {
        if (data.IsAllDataRequested && data.Kind == ActivityKind.Client)
        {
            if (data.Status != ActivityStatusCode.Error
                && data.GetTagItem("messaging.system") is string messagingSystem
                && messagingSystem.Equals("servicebus", StringComparison.OrdinalIgnoreCase)
                && data.GetTagItem("messaging.operation") is string messagingOperation
                && messagingOperation.Equals("receive", StringComparison.OrdinalIgnoreCase))
            {
                // Mark as not recorded to filter it out
                data.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;
            }
        }

        base.OnEnd(data);
    }
}
