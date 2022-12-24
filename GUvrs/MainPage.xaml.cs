using Microsoft.Maui.Platform;

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

        RenderPlayerData();
        Reset.IsEnabled = true;
    }

    private void OnResetClick(object sender, EventArgs e)
    {
        Reset.IsEnabled = false;
        _player = null;
        _opponent = null;

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
}

