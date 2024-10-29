using Fotos.Client.Api.Photos;

namespace Fotos.Client.Adapters;

internal class SessionDataStorage
{
    private readonly GetSessionDataFromStore _getSessionData;
    private readonly AddSessionDataToStore _addSessionData;

    public SessionDataStorage(
        GetSessionDataFromStore getSessionData,
        AddSessionDataToStore addSessionData)
    {
        _getSessionData = getSessionData;
        _addSessionData = addSessionData;
    }

    public virtual async Task Save(SessionData sessionData)
    {
        await _addSessionData(Guid.Empty, sessionData);
    }

    public virtual async Task<SessionData> Fetch()
    {
        var sessionData = await _getSessionData(Guid.Empty);

        return sessionData ?? new(new());
    }
}
