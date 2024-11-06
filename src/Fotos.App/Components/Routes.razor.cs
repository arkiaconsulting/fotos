namespace Fotos.App.Components;
public partial class Routes
{
    private bool _isLoading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Task.Delay(1000);
            _isLoading = false;

            StateHasChanged();
        }
    }
}