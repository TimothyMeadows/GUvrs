namespace GUvrs;

public partial class MainPage : ContentPage
{
	private long playerId = 0;
	private string playerName = string.Empty;
	private long opponentId = 0;
	private string opponentName = string.Empty;


    public MainPage()
	{
		InitializeComponent();

        PlayerID.GestureRecognizers.Add(new ClickGestureRecognizer()
		{
			Command = new Command(() => OnIDClick(playerId)),
			NumberOfClicksRequired = 1
		});

        PlayerID.GestureRecognizers.Add(new TapGestureRecognizer()
        {
            Command = new Command(() => OnIDClick(playerId))
        });

        OpponentID.GestureRecognizers.Add(new ClickGestureRecognizer()
        {
            Command = new Command(() => OnIDClick(opponentId)),
            NumberOfClicksRequired = 1
        });

        OpponentID.GestureRecognizers.Add(new TapGestureRecognizer()
        {
            Command = new Command(() => OnIDClick(opponentId))
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

