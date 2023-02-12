namespace GUvrs;
using Models;
using Modules;

public partial class MainPage : ContentPage
{
    private readonly GuDebugLog _log;
    private PlayerModel _player;
    private PlayerModel _opponent;

    public MainPage()
    {
        InitializeComponent();

        _log = new GuDebugLog();
        _log.OnBegin += OnBegin;
        _log.OnStart += OnStart;

        PlayerID.GestureRecognizers.Add(new ClickGestureRecognizer()
        {
            Command = new Command(() => OnIDClick(_player?.ID)),
            NumberOfClicksRequired = 1
        });

        PlayerID.GestureRecognizers.Add(new TapGestureRecognizer()
        {
            Command = new Command(() => OnIDClick(_player?.ID))
        });

        OpponentID.GestureRecognizers.Add(new ClickGestureRecognizer()
        {
            Command = new Command(() => OnIDClick(_opponent?.ID)),
            NumberOfClicksRequired = 1
        });

        OpponentID.GestureRecognizers.Add(new TapGestureRecognizer()
        {
            Command = new Command(() => OnIDClick(_opponent?.ID))
        });
    }

    private void OnStart(GameStartModel model)
    {
        GameID.Render(() => GameID.Text = model.GameId);
        CopyGameID.Render(() => CopyGameID.IsVisible = true);
    }

    private void OnBegin(GameBeginModel model)
    {
        _player = model?.Player;
        _opponent = model?.Opponnent;

        PlayerID.Render(() => PlayerID.Text = _player.ID);
        PlayerName.Render(() => PlayerName.Text = _player.Name);
        CopyPlayerID.Render(() => CopyPlayerID.IsVisible = true);

        OpponentID.Render(() => OpponentID.Text = _opponent.ID);
        OpponentName.Render(() => OpponentName.Text = _opponent.Name);
        CopyOpponentID.Render(() => CopyOpponentID.IsVisible = true);
    }

    private void OnIDClick(string id)
    {
        if (string.IsNullOrEmpty(id) || id == "-1")
            return;

        Browser.OpenAsync($"https://gudecks.com/meta/player-stats?userId={id}");
        return;
    }

    private void OnShowHandPointer(object sender, PointerEventArgs e)
    {
#if MACCATALYST
        PlayerID.MacHandCursor(true);
#endif
#if WINDOWS
        PlayerID.WindowsHandCursor(true);
#endif
    }

    private void OnHideHandPointer(object sender, PointerEventArgs e)
    {
#if MACCATALYST
        PlayerID.MacHandCursor(false);
#endif
#if WINDOWS
        PlayerID.WindowsHandCursor(false);
#endif
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        return base.MeasureOverride(100, 100);
    }
}

