namespace GUvrs.Components;

public static class ControlRenderer
{
    // When MAUI matures could be a useful base class to assign this to rather than to each direct control.
    // currently, however, the base control class in MAUI can "change" based on the OS where the direct control
    // class does not.
    public static void Render(this Label label, Action action)
    {
        if (!MainThread.IsMainThread)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                label.Render(action);
            });

            return;
        }

        action?.Invoke();
    }

    public static void Render(this ImageButton button, Action action)
    {
        if (!MainThread.IsMainThread)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                button.Render(action);
            });

            return;
        }

        action?.Invoke();
    }
}