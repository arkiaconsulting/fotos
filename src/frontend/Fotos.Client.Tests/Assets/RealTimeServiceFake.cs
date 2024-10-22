using Fotos.Client.Hubs;
using Microsoft.AspNetCore.Components;

namespace Fotos.Client.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
internal sealed class RealTimeServiceFake : RealTimeMessageService
{
    public RealTimeServiceFake(NavigationManager navigationManager) : base(navigationManager)
    {
    }

    public override Task StartAsync() => Task.CompletedTask;
}