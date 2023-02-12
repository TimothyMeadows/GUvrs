namespace GUvrs.Modules;

public static class LabelRenderer
{
    public static void Render(this Label label, Action action)
    {
        if (!MainThread.IsMainThread)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LabelRenderer.Render(label, action);
            });

            return;
        }

        action?.Invoke();
    }
}