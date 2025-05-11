namespace Fotos.App.Components;
public partial class Routes
{
    private bool _isLoading = true;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _isLoading = false;

            StateHasChanged();
        }
    }
}