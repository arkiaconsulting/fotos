using Fotos.App.Application.User;

namespace Fotos.Tests.Frontend.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
internal sealed class LocalStorageServiceFake : SessionDataStorage
{
    public LocalStorageServiceFake()
        : base(default!, default!)
    {
    }

    public override Task Save(SessionData sessionData) => Task.CompletedTask;

    public override Task<SessionData> Fetch() => Task.FromResult(new SessionData([]));
}