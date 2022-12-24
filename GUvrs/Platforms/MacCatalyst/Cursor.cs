using System;
using System.Reflection;
using AppKit;

public static class Cursor
{
    public static void MacHandCursor(this Button button, bool active)
    {
        if (active)
            NSCursor.OpenHandCursor.Set();
        else
            NSCursor.ArrowCursor.Set();
    }

    public static void MacHandCursor(this Label label, bool active)
    {
        if (active)
            NSCursor.OpenHandCursor.Set();
        else
            NSCursor.ArrowCursor.Set();
    }
}

