using GUvrs.Components;

namespace GUvrs;

public static class Startup
{
    public static void MacCatalyst()
    {
        CrossPlatform.FileSystem.GuLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library",
            "Logs", "Immutable", "gods");

        CrossPlatform.FileSystem.CreateIfNotExists(CrossPlatform.FileSystem.AppDataDirectory);
        CrossPlatform.FileSystem.CreateIfNotExists(CrossPlatform.FileSystem.GuLogPath);
    }

    public static void Windows()
    {
        CrossPlatform.FileSystem.GuLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData",
            "LocalLow", "Immutable", "gods");

        CrossPlatform.FileSystem.CreateIfNotExists(CrossPlatform.FileSystem.AppDataDirectory);
        CrossPlatform.FileSystem.CreateIfNotExists(CrossPlatform.FileSystem.GuLogPath);
    }

    public static void Android()
    {
        CrossPlatform.FileSystem.GuLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "data",
            "com.immutable.godsunchained", "files");

        CrossPlatform.FileSystem.CreateIfNotExists(CrossPlatform.FileSystem.AppDataDirectory);
        CrossPlatform.FileSystem.CreateIfNotExists(CrossPlatform.FileSystem.GuLogPath);
    }
}