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
		}

		public static void Windows()
		{
            CrossPlatform.GuLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData",
                "LocalLow", "Immutable", "gods");
		}
	}
}

