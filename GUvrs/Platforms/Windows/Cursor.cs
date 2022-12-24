using System;
using System.Reflection;
using Windows.UI.Core;
using GUvrs.WinUI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;

public static partial class Cursor
{
    public const int IDC_HAND = 32649;
    public const int IDC_ARROW = 32512;

    [DllImport("user32.dll")]
    public static extern int LoadCursor(int hInstance, int lpCursorName);

    [DllImport("user32.dll")]
    public static extern int SetCursor(int hCursor);

    public static void WindowsHandCursor(this Label label, bool active)
    {
        if (active)
        {
            var cursor = LoadCursor(0, IDC_HAND);
            SetCursor(cursor);
        }
        else
        {
            var cursor = LoadCursor(0, IDC_ARROW);
            SetCursor(cursor);
        }
    }

    public static void WindowsHandCursor(this Button button, bool active)
    {
        if (active)
        {
            var cursor = LoadCursor(0, IDC_HAND);
            SetCursor(cursor);
        }
        else
        {
            var cursor = LoadCursor(0, IDC_ARROW);
            SetCursor(cursor);
        }
    }
}

