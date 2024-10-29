using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Fotos.Client.Adapters;

internal sealed class CustomCircuitHandler : CircuitHandler
{
    public SessionData SessionData { get; private set; } = new([]);
    private readonly SessionDataStorage _sessionDataStorage;

    public CustomCircuitHandler(SessionDataStorage localStorage) => _sessionDataStorage = localStorage;

    public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        SessionData = await _sessionDataStorage.Fetch();

        await base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        await _sessionDataStorage.Save(SessionData);

        await base.OnCircuitClosedAsync(circuit, cancellationToken);
    }
}
