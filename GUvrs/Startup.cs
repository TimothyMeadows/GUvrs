using GUvrs.Modules;

namespace GUvrs;

public static class Startup
{
    public static void MacCatalyst()
    {
        CrossPlatform.GuLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library",
            "Logs", "Immutable", "gods");
        if (!Directory.Exists(CrossPlatform.GuLogPath))
            Directory.CreateDirectory(CrossPlatform.GuLogPath);
    }

    public static void Windows()
    {
        CrossPlatform.GuLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData",
            "LocalLow", "Immutable", "gods");
        if (!Directory.Exists(CrossPlatform.GuLogPath))
            Directory.CreateDirectory(CrossPlatform.GuLogPath);
    }
}