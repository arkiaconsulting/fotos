namespace Fotos.App.Components;
public partial class CustomErrorBoundary
{
    protected override Task OnErrorAsync(Exception exception)
    {
        Logger.LogError(exception, "😈 A rotten gremlin got us. Sorry!");

        return Task.CompletedTask;
    }
}