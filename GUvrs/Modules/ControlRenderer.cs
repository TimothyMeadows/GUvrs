namespace GUvrs.Modules;

public static class ControlRenderer
{
    public static void Render(this Label label, Action action)
    {
        if (!MainThread.IsMainThread)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ControlRenderer.Render(label, action);
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
                ControlRenderer.Render(button, action);
            });

            return;
        }

        action?.Invoke();
    }
}