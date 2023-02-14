using AppKit;

public static partial class Cursor
{
    public static void MacHandCursor(this Button button, bool active)
    {
        if (active)
            NSCursor.PointingHandCursor.Set();
        else
            NSCursor.ArrowCursor.Set();
    }

    public static void MacHandCursor(this Label label, bool active)
    {
        if (active)
            NSCursor.PointingHandCursor.Set();
        else
            NSCursor.ArrowCursor.Set();
    }
}

