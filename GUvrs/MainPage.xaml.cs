using Microsoft.Maui.Platform;
using MainThread = Microsoft.Maui.ApplicationModel.MainThread;

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
        _log.Change += OnChange;
        
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

    private void OnChange(PlayerModel player, PlayerModel opponent)
    {
        _player = player;
        _opponent = opponent;

        if (!MainThread.IsMainThread)
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Reset.IsEnabled = true;
                Reset.BackgroundColor = Colors.Black;
            });
        else
        {
            Reset.IsEnabled = true;
            Reset.BackgroundColor = Colors.Black;
        }

        RenderPlayerDataOnThread();
    }

    private void OnResetClick(object sender, EventArgs e)
    {
        Reset.IsEnabled = false;
        Reset.BackgroundColor = Colors.Grey;

        _player = null;
        _opponent = null;

        RenderPlayerData();
        _log.Reset();
    }

    private void OnIDClick(long? id)
    {
        if (id is null or 0 or < 0)
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

    private void RenderPlayerDataOnThread()
    {
        if (!MainThread.IsMainThread)
            MainThread.BeginInvokeOnMainThread(RenderPlayerData);
        else
            RenderPlayerData();
    }

    private void RenderPlayerData()
    {

        if (_player == null || _opponent == null)
        {
            PlayerID.Text = string.Empty;
            PlayerName.Text = string.Empty;
            OpponentID.Text = string.Empty;
            OpponentName.Text = string.Empty;
            return;
        }

        PlayerID.Text = _player.ID.ToString();
        PlayerName.Text = _player.Name;
        OpponentID.Text = _opponent.ID.ToString();
        OpponentName.Text = _opponent.Name;
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        return base.MeasureOverride(100, 100);
    }
}

