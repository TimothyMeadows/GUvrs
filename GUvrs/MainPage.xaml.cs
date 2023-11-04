namespace GUvrs;

using System.Web;
using Microsoft.Maui.Storage;
using GUvrs.Components;
using GUvrs.Models.Views;
using GUvrs.Views;
using MemoryCache.NetCore;
using Models;
using System.Text.Json;
using System;

public partial class MainPage : ContentPage
{
    private readonly MemoryCache _settings;
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

        _settings = new();
        LoadSettings();

        ConcurrentEventListener.Register("load-settings", OnLoadSettings);
        ConcurrentEventListener.Register("save-settings", OnSaveSettings);
        ConcurrentEventListener.Register("gudecks", OnGudecks);
        ConcurrentEventListener.Register("report-issue", OnReportIssue);
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

        if (IsAutoOpen() && _opponent.ID != "-1")
            Browser.OpenAsync($"https://gudecks.com/meta/player-stats?userId={_opponent.ID}");

        _SetValues(new()
        {
            { "GUVRS_PLAYER_NAME", _player?.Name },
            { "GUVRS_PLAYER_GUID", _player?.ID },
            { "GUVRS_OPPONENT_NAME", _opponent?.Name },
            { "GUVRS_OPPONENT_GUID", _opponent?.ID }
        });
    }

    private bool IsAutoOpen()
    {
        bool autoOpen;
        try
        {
            autoOpen = Convert.ToBoolean(((JsonElement)_settings["auto-open"]).GetString());
        }
        catch (Exception)
        {
            autoOpen = false;
        }

        return autoOpen;
    }

    private void _SetSettings()
    {
        string theme;
        try
        {
            theme = ((JsonElement)_settings["theme"]).GetString();
        } 
        catch (Exception)
        {
            theme = string.Empty;
        }

        string autoOpen;
        try
        {
            autoOpen = ((JsonElement)_settings["auto-open"]).GetString();
        }
        catch (Exception)
        {
            autoOpen = string.Empty;
        }

        if (string.IsNullOrEmpty(theme))
            return;

        EvaluateJavaScriptAsync($"guvrs_set_settings('{theme}', '{autoOpen.ToLower()}');");
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

        EvaluateJavaScriptAsync($"guvrs_set_value('{_name}', '{_value}');");
    }

    private void SaveSettings()
    {
        var json = _settings.Save<string>();
        File.WriteAllText($"{FileSystem.AppDataDirectory}\\settings.json", json);
    }

    private void LoadSettings()
    {
        if (!File.Exists($"{FileSystem.AppDataDirectory}\\settings.json"))
            return;

        var json = File.ReadAllText($"{FileSystem.AppDataDirectory}\\settings.json");
        _settings.Load<string>(json);
    }

    private void EvaluateJavaScriptAsync(string javascript)
    {
        try
        {
            ControlRenderer.Render(WebView, async () => await WebView.EvaluateJavaScriptAsync(javascript));
        } catch(Exception)
        {
            return; // supress errors in runtime
        }
    }

    private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        var url = e.Url;
        if (url.StartsWith("guvrs://"))
        {
            e.Cancel = true;

            var uri = new Uri(url);
            var name = uri.Host;
            var values = new Dictionary<string, string>();
            var valuePairs = uri.Query?.TrimStart('?').Split('&');

            foreach (var valuePart in valuePairs)
            {
                var parts = valuePart.Split('=');
                if (parts.Length == 2)
                    values.Add(Uri.UnescapeDataString(parts[0]), Uri.UnescapeDataString(parts[1]));
            }

            ConcurrentEventListener.Trigger(name, values);
        }
    }

    private void OnLoadSettings(Dictionary<string, string> data)
    {
        _SetSettings();
    }

    private void OnSaveSettings(Dictionary<string, string> data)
    {
        foreach (var setting in data)
            _settings.Write(setting.Key, setting.Value);

        SaveSettings();
    }

    private void OnGudecks(Dictionary<string, string> data)
    {
        if (!data.ContainsKey("guid"))
            return;

        var guid = data["guid"];
        if (guid == "-1")
            return;

        Browser.OpenAsync($"https://gudecks.com/meta/player-stats?userId={guid}");
    }

    private void OnReportIssue(Dictionary<string, string> data)
    {
        Browser.OpenAsync($"https://github.com/TimothyMeadows/GUvrs/issues/new");
    }
}