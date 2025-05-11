using Fotos.App.Hubs;
using Microsoft.AspNetCore.Components;

namespace Fotos.Tests.Frontend.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
internal sealed class RealTimeServiceFake : RealTimeMessageService
{
    public RealTimeServiceFake(NavigationManager navigationManager)
        : base(navigationManager)
    {
    }

    public override Task StartAsync() => Task.CompletedTask;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2215:Dispose methods should call base class dispose", Justification = "<Pending>")]
    public override async ValueTask DisposeAsync() => await ValueTask.CompletedTask;
}