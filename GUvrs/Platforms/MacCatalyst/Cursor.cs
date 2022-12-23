using System;
using System.Reflection;
using AppKit;

namespace GUvrs.Platforms.MacCatalyst
{
	public static class Cursor
	{
        public static void HandCursor(this Button button, bool active)
        {
            if (active)
                NSCursor.OpenHandCursor.Set();
            else
                NSCursor.ArrowCursor.Set();
        }

        public static void HandCursor(this Label label, bool active)
        {
            if (active)
                NSCursor.OpenHandCursor.Set();
            else
                NSCursor.ArrowCursor.Set();
        }
    }
}

