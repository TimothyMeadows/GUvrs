using Foundation;

namespace GUvrs;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp()
	{
        var app = MauiProgram.CreateMauiApp();
        Startup.MacCatalyst();

        return app;
    }
}
