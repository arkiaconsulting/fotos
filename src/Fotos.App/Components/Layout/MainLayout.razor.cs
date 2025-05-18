using MudBlazor;

namespace Fotos.App.Components.Layout;
public partial class MainLayout
{
    private bool _isDarkMode;
    private MudThemeProvider _mudThemeProvider = default!;
    private readonly MudTheme _fotosTheme = new()
    {
        PaletteLight = new()
        {
            AppbarBackground = Colors.BlueGray.Lighten1,
            AppbarText = Colors.BlueGray.Lighten3,
            Background = Colors.BlueGray.Lighten5,
            Primary = Colors.Gray.Darken4,
            Secondary = Colors.Teal.Lighten1,
            Surface = Colors.BlueGray.Lighten4,
            TextPrimary = Colors.BlueGray.Darken1,
        },
        PaletteDark = new()
        {
            AppbarBackground = Colors.Gray.Darken4,
            Background = Colors.Gray.Darken3,
            Primary = Colors.Gray.Darken4,
            Secondary = Colors.Teal.Darken1,
            Surface = Colors.Gray.Darken4,
        }
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _isDarkMode = await _mudThemeProvider.GetSystemPreference();

            StateHasChanged();
        }
    }
}