namespace GUvrs;

using System.Web;
using GUvrs.Components;
using GUvrs.Models.Views;
using GUvrs.Views;
using Models;

public partial class MainPage : ContentPage
{
    private readonly GuDebugLog _log;
    private string _gameId = string.Empty;
    private PlayerModel _player;
    private PlayerModel _opponent;
    private readonly DefaultMainPageViewModel _defaults;

    public MainPage()
    {
        InitializeComponent();

        _defaults = new DefaultMainPageViewModel();
        WebView.Source = new HtmlWebViewSource()
        {
            Html = ViewEngine.Render("MainPage.index", _defaults)
        };

        _log = new GuDebugLog();
        _log.OnBegin += OnBegin;
        _log.OnStart += OnStart;
        _log.OnStop += OnStop;
        _log.OnEnd += OnEnd;
    }

    private void OnEnd()
    {
        _SetValues(new Dictionary<string, string>
        {
            { "GUVRS_GAME_ID", _defaults.GUVRS_GAME_ID },
            { "GUVRS_OPPONENT_GUID", _defaults.GUVRS_OPPONENT_GUID },
            { "GUVRS_OPPONENT_NAME", _defaults.GUVRS_OPPONENT_NAME }
        });
    }

    private void OnStop(GameStopModel model)
    {
        _SetValues(new Dictionary<string, string>
        {
            { "GUVRS_GAME_ID", _defaults.GUVRS_GAME_ID },
            { "GUVRS_OPPONENT_GUID", _defaults.GUVRS_OPPONENT_GUID },
            { "GUVRS_OPPONENT_NAME", _defaults.GUVRS_OPPONENT_NAME }
        });
    }

    private void OnStart(GameStartModel model)
    {
        _gameId = model?.GameId;

        _SetValue("GUVRS_GAME_ID", _gameId);
    }

    private void OnBegin(GameBeginModel model)
    {
        _player = model?.Player;
        _opponent = model?.Opponnent;

        _SetValues(new Dictionary<string, string>
        {
            { "GUVRS_PLAYER_NAME", _player?.Name },
            { "GUVRS_PLAYER_GUID", _player?.ID },
            { "GUVRS_OPPONENT_NAME", _opponent?.Name },
            { "GUVRS_OPPONENT_GUID", _opponent?.ID }
        });
    }

    private void _SetValues(Dictionary<string, string> values)
    {
        foreach (var current in values)
        {
            _SetValue(current.Key, current.Value);
        }
    }

    private void _SetValue(string name, string value)
    {
        var _name = HttpUtility.HtmlEncode(name);
        var _value = HttpUtility.HtmlEncode(value);

        ControlRenderer.Render(WebView, async () => await WebView.EvaluateJavaScriptAsync($"guvrs_set_value('{_name}', '{_value}');"));
    }

    private void _SetProgress(string name, string value)
    {
        var _name = HttpUtility.HtmlEncode(name);
        var _value = HttpUtility.HtmlEncode(value);

        ControlRenderer.Render(WebView, async () => await WebView.EvaluateJavaScriptAsync($"guvrs_set_progress('{_name}', '{_value}');"));
    }

    private void _SetProgresses(Dictionary<string, string> progresses)
    {
        foreach (var current in progresses)
        {
            _SetProgress(current.Key, current.Value);
        }
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        return base.MeasureOverride(300, 300);
    }
}