namespace GUvrs;
using Models;
using Modules;

public partial class MainPage : ContentPage
{
    private readonly GuDebugLog _log;
    private string _gameId = string.Empty;
    private PlayerModel _player;
    private PlayerModel _opponent;

    public MainPage()
    {
        InitializeComponent();

        _log = new GuDebugLog();
        _log.OnBegin += OnBegin;
        _log.OnStart += OnStart;
        _log.OnEnd += OnEnd;

        CopyGameID.GestureRecognizers.Add(new ClickGestureRecognizer()
        {
            Command = new Command(() => OnCopyIDClick(_gameId)),
            NumberOfClicksRequired = 1
        });

        CopyGameID.GestureRecognizers.Add(new TapGestureRecognizer()
        {
            Command = new Command(() => OnCopyIDClick(_gameId))
        });

        PlayerID.GestureRecognizers.Add(new ClickGestureRecognizer()
        {
            Command = new Command(() => OnIDClick(_player?.ID)),
            NumberOfClicksRequired = 1
        });

        PlayerID.GestureRecognizers.Add(new TapGestureRecognizer()
        {
            Command = new Command(() => OnIDClick(_player?.ID))
        });

        CopyPlayerID.GestureRecognizers.Add(new ClickGestureRecognizer()
        {
            Command = new Command(() => OnCopyIDClick(_player?.ID)),
            NumberOfClicksRequired = 1
        });

        CopyPlayerID.GestureRecognizers.Add(new TapGestureRecognizer()
        {
            Command = new Command(() => OnCopyIDClick(_player?.ID))
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

        CopyOpponentID.GestureRecognizers.Add(new ClickGestureRecognizer()
        {
            Command = new Command(() => OnCopyIDClick(_opponent?.ID)),
            NumberOfClicksRequired = 1
        });

        CopyOpponentID.GestureRecognizers.Add(new TapGestureRecognizer()
        {
            Command = new Command(() => OnCopyIDClick(_opponent?.ID))
        });

        Github.GestureRecognizers.Add(new ClickGestureRecognizer()
        {
            Command = new Command(() => OnGithubClick()),
            NumberOfClicksRequired = 1
        });

        Github.GestureRecognizers.Add(new TapGestureRecognizer()
        {
            Command = new Command(() => OnGithubClick())
        });
    }

    private void OnEnd()
    {
        GameID.Render(() => GameID.Text = string.Empty);
        CopyGameID.Render(() => CopyGameID.IsVisible = false);

        OpponentID.Render(() => OpponentID.Text = string.Empty);
        OpponentName.Render(() => OpponentName.Text = string.Empty);
        CopyOpponentID.Render(() => CopyOpponentID.IsVisible = false);
    }

    private void OnStart(GameStartModel model)
    {
        _gameId = model.GameId;
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

    private void OnGithubClick()
    {
        Browser.OpenAsync("https://github.com/TimothyMeadows/GUvrs/issues/new");
    }

    private void OnIDClick(string id)
    {
        if (string.IsNullOrEmpty(id) || id == "-1")
            return;

        Browser.OpenAsync($"https://gudecks.com/meta/player-stats?userId={id}");
        return;
    }

    private void OnCopyIDClick(string id)
    {
        if (string.IsNullOrEmpty(id) || id == "-1")
            return;

        Clipboard.SetTextAsync(id);
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

