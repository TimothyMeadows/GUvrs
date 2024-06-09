using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Microsoft.Maui.Platform;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GUvrs.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
            handler.PlatformView.GetAppWindow().Resize(new SizeInt32(1024, 768));
        });
    }

    protected override MauiApp CreateMauiApp()
    {
        var app = MauiProgram.CreateMauiApp();
        Startup.Windows();

        return app;
    }
}