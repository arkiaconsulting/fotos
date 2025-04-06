using Fotos.App.Domain;
using Fotos.App.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.ObjectModel;

namespace Fotos.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
internal sealed class FakeHubContext : IHubContext<PhotosHub>
{
    private readonly Collection<(string, PhotoId)> _messages;

    public FakeHubContext(Collection<(string, PhotoId)> messages) => _messages = messages;

    public IHubClients Clients => new FakeHubClient(_messages);
    public IGroupManager Groups { get; } = default!;
}

internal sealed class FakeHubClient : IHubClients
{
    private readonly Collection<(string, PhotoId)> _messages;

    public FakeHubClient(Collection<(string, PhotoId)> messages) => _messages = messages;

    public IClientProxy All => new FakeClientProxy(_messages);

    public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => throw new NotImplementedException();
    public IClientProxy Client(string connectionId) => throw new NotImplementedException();
    public IClientProxy Clients(IReadOnlyList<string> connectionIds) => throw new NotImplementedException();
    public IClientProxy Group(string groupName) => throw new NotImplementedException();
    public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => throw new NotImplementedException();
    public IClientProxy Groups(IReadOnlyList<string> groupNames) => throw new NotImplementedException();
    public IClientProxy User(string userId) => throw new NotImplementedException();
    public IClientProxy Users(IReadOnlyList<string> userIds) => throw new NotImplementedException();
}

internal sealed class FakeClientProxy : IClientProxy
{
    private readonly Collection<(string, PhotoId)> _messages;

    public FakeClientProxy(Collection<(string, PhotoId)> messages) => _messages = messages;

    public Task SendCoreAsync(string method, object?[]? args, CancellationToken cancellationToken = default)
    {
        _messages.Add((method, (PhotoId)args![0]!));

        return Task.CompletedTask;
    }

    public Task SendAsync(string method, object?[]? args, CancellationToken cancellationToken = default) => SendCoreAsync(method, args, cancellationToken);
}