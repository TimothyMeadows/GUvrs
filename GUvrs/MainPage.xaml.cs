namespace GUvrs;

public partial class MainPage : ContentPage
{
	private long PlayerId = 0;
	private long OpponentId = 0;

	public MainPage()
	{
		InitializeComponent();

        PlayerID.GestureRecognizers.Add(new ClickGestureRecognizer()
		{
			Command = new Command(() => OnIDClick(PlayerId)),
			NumberOfClicksRequired = 1
		});

        PlayerID.GestureRecognizers.Add(new TapGestureRecognizer()
        {
            Command = new Command(() => OnIDClick(PlayerId))
        });

        OpponentID.GestureRecognizers.Add(new ClickGestureRecognizer()
        {
            Command = new Command(() => OnIDClick(OpponentId)),
            NumberOfClicksRequired = 1
        });

        OpponentID.GestureRecognizers.Add(new TapGestureRecognizer()
        {
            Command = new Command(() => OnIDClick(OpponentId))
        });
    }

	private void OnResetClick(object sender, EventArgs e)
	{
		Reset.IsEnabled = false;
		// Do Work
		Reset.IsEnabled = true;
	}

	private void OnIDClick(long id)
	{
		if (id == 0 || id == -1)
			return;

		//Browser.OpenAsync("https://www.gudecks.com/");
		return;
	}
}

