using System;
using System.Reflection;

public static class Cursor
{
    public static void WindowsHandCursor(this UIElement uiElement, bool active)
    {
        Type type = typeof(UIElement);

        if (active)
            type.InvokeMember("ProtectedCursor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, uiElement, new object[] { InputSystemCursor.Create(InputSystemCursorShape.Hand) });
        else
            type.InvokeMember("ProtectedCursor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, uiElement, new object[] { InputSystemCursor.Create(InputSystemCursorShape.Arrow) });
    }
}

