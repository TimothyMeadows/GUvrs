using System;
using GUvrs.Modules;

namespace GUvrs
{
	public static class Startup
	{
		public static void MacCatalyst()
		{
			CrossPlatform.GuLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library",
                "Logs", "Immutable", "gods");
            if (string.IsNullOrEmpty(CrossPlatform.GuLogPath) || !Directory.Exists(CrossPlatform.GuLogPath))
                throw new FileNotFoundException(
                    "Unable to locate Gods Unchained install. If you feel this is a mistake. Please report this to Github.");
        }

		public static void Windows()
		{
            CrossPlatform.GuLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData",
                "LocalLow", "Immutable", "gods");
            if (string.IsNullOrEmpty(CrossPlatform.GuLogPath) || !Directory.Exists(CrossPlatform.GuLogPath))
                throw new FileNotFoundException(
                    "Unable to locate Gods Unchained install. If you feel this is a mistake. Please report this to Github.");
        }
	}
}

