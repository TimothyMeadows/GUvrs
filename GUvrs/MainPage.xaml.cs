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
        WebView.Navigating += WebView_Navigating;
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

    private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        var url = e.Url;
        if (url.StartsWith("guvrs://"))
        {
            e.Cancel = true;

            var uri = new Uri(url);
            var method = uri.Host;
            var values = new Dictionary<string, string>();
            var valuePairs = uri.Query?.TrimStart('?').Split('&');

            foreach (var valuePart in valuePairs)
            {
                var parts = valuePart.Split('=');
                if (parts.Length == 2)
                    values.Add(Uri.UnescapeDataString(parts[0]), Uri.UnescapeDataString(parts[1]));
            }
        }
    }

    private void OnEnd()
    {
        _SetValues(new()
        {
            { "GUVRS_GAME_ID", _defaults.GUVRS_GAME_ID },
            { "GUVRS_OPPONENT_GUID", _defaults.GUVRS_OPPONENT_GUID },
            { "GUVRS_OPPONENT_NAME", _defaults.GUVRS_OPPONENT_NAME }
        });
    }

    private void OnStop(GameStopModel model)
    {
        _SetValues(new()
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

        //var playerRank = Task.Run(() => new GuApi().GetRank(_player.ID)).Result;
        //var opponnentRank = Task.Run(() => new GuApi().GetRank(_opponent.ID)).Result;

        _SetValues(new()
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
            _SetValue(current.Key, current.Value);
    }

    private void _SetValue(string name, string value)
    {
        var _name = HttpUtility.HtmlEncode(name);
        var _value = HttpUtility.HtmlEncode(value);

        ControlRenderer.Render(WebView, async () => await WebView.EvaluateJavaScriptAsync($"guvrs_set_value('{_name}', '{_value}');"));
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        return base.MeasureOverride(300, 300);
    }
}