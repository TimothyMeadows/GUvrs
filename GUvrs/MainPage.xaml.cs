namespace GUvrs;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnResetClick(object sender, EventArgs e)
	{
		Reset.IsEnabled = false;
		// Do Work
		Reset.IsEnabled = true;
	}
}

