namespace GUvrs;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        return base.MeasureOverride(100, 100);
    }
}