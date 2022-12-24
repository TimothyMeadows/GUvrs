using System;
using System.Reflection;
using Windows.UI.Core;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;

public static partial class Cursor
{
    public static void WindowsHandCursor(this Label label, bool active)
    {
        //Microsoft.UI.Xaml.Window.Current.CoreWindow.PointerCursor = active ? new CoreCursor(CoreCursorType.Hand, 0) : new CoreCursor(CoreCursorType.Arrow, 0);
        return;
    }

    public static void WindowsHandCursor(this Button button, bool active)
    {
        //Microsoft.UI.Xaml.Window.Current.CoreWindow.PointerCursor = active ? new CoreCursor(CoreCursorType.Hand, 0) : new CoreCursor(CoreCursorType.Arrow, 0);
        return;
    }
}

