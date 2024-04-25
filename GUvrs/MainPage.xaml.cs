namespace GUvrs;

using System.Web;
using GUvrs.Components;
using GUvrs.Models.Views;
using GUvrs.Views;
using MemoryCache.NetCore;
using Models;
using System.Text.Json;
using System;
using System.ComponentModel.DataAnnotations;

public partial class MainPage : ContentPage
{
    private static List<FriendModel> Friends;
    private readonly MemoryCache _settings;
    private readonly GuDebugLog _log;
    private string _gameId = string.Empty;
    private int _gameMode = -1;
    private PlayerModel _player;
    private PlayerModel _opponent;
    private readonly MainPageViewModel _defaults;

    public MainPage()
    {
        InitializeComponent();
        _defaults = new MainPageViewModel();

        WebView.Navigating += WebView_Navigating;
        WebView.Source = new HtmlWebViewSource()
        {
            Html = ViewEngine.Render("MainPage.index", _defaults)
        };

        _log = new GuDebugLog();
        _log.OnBegin += OnBegin;
        _log.OnGameMode += OnGameMode;
        _log.OnStart += OnStart;
        _log.OnStop += OnStop;
        _log.OnEnd += OnEnd;

        Friends ??= new List<FriendModel>();
        LoadFriends();

        _settings = new();
        LoadSettings();

        ConcurrentEventListener.Register("load-settings", OnLoadSettings);
        ConcurrentEventListener.Register("save-settings", OnSaveSettings);
        ConcurrentEventListener.Register("save-friend", OnSaveFriend);
        ConcurrentEventListener.Register("open-settings-folder", OnOpenSettingsFolder);
        ConcurrentEventListener.Register("open", OnOpen);
        ConcurrentEventListener.Register("copy", OnCopy);
        ConcurrentEventListener.Register("report-issue", OnReportIssue);
    }

    private void OnEnd()
    {
        _gameId = _defaults.GUVRS_GAME_ID;
        _opponent = null;

        ClearDisplay();
    }

    private void OnStop(GameStopModel model)
    {
        _gameId = _defaults.GUVRS_GAME_ID;
        _opponent = null;

        ClearDisplay();
    }

    public void ClearDisplay()
    {
        _SetValues(new()
        {
            { "GUVRS_GAME_ID", _defaults.GUVRS_GAME_ID },
            { "GUVRS_OPPONENT_GUID", _defaults.GUVRS_OPPONENT_GUID },
            { "GUVRS_OPPONENT_NAME", _defaults.GUVRS_OPPONENT_NAME },
            { "GUVRS_OPPONENT_RATING", _defaults.GUVRS_OPPONENT_RATING },
            { "GUVRS_OPPONENT_WINPOINTS", _defaults.GUVRS_OPPONENT_WINPOINTS },
            { "GUVRS_OPPONENT_LOSSPOINTS", _defaults.GUVRS_OPPONENT_LOSSPOINTS },
            { "GUVRS_OPPONENT_SAFELINE", _defaults.GUVRS_OPPONENT_SAFELINE }
        });

        var playerRank = Task.Run(async () => await new GuApi().GetRank(_player.ID)).Result;
        _SetValues(new()
        {
            { "GUVRS_PLAYER_RATING", playerRank.Rating.ToString() },
            { "GUVRS_PLAYER_WINPOINTS", playerRank.WinPoints.ToString() },
            { "GUVRS_PLAYER_LOSSPOINTS", playerRank.LossPoints.ToString() },
            { "GUVRS_PLAYER_SAFELINE", playerRank.SafetyLine.ToString() },
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

        var playerRank = Task.Run(async () => await new GuApi().GetRank(_player.ID)).Result;
        PlayerRankModel opponnentRank = null;
        if (_opponent.ID != "-1")
            opponnentRank = Task.Run(async () => await new GuApi().GetRank(_opponent.ID)).Result;

        if (IsAutoOpen() && _opponent.ID != "-1")
            OpenBrowserWithGameMode(_gameId, _opponent.ID);

        _SetValues(new()
        {
            { "GUVRS_PLAYER_NAME", _player?.Name },
            { "GUVRS_PLAYER_GUID", _player?.ID },
            { "GUVRS_PLAYER_RATING", playerRank.Rating.ToString() },
            { "GUVRS_PLAYER_WINPOINTS", playerRank.WinPoints.ToString() },
            { "GUVRS_PLAYER_LOSSPOINTS", playerRank.LossPoints.ToString() },
            { "GUVRS_PLAYER_SAFELINE", playerRank.SafetyLine.ToString() },
            { "GUVRS_OPPONENT_NAME", _opponent?.Name },
            { "GUVRS_OPPONENT_GUID", _opponent?.ID },
            { "GUVRS_OPPONENT_RATING", opponnentRank == null ? _defaults.GUVRS_OPPONENT_RATING : opponnentRank?.Rating.ToString() },
            { "GUVRS_OPPONENT_WINPOINTS", opponnentRank == null ? _defaults.GUVRS_OPPONENT_WINPOINTS : opponnentRank?.WinPoints.ToString() },
            { "GUVRS_OPPONENT_LOSSPOINTS", opponnentRank == null ? _defaults.GUVRS_OPPONENT_LOSSPOINTS : opponnentRank?.LossPoints.ToString() },
            { "GUVRS_OPPONENT_SAFELINE", opponnentRank == null ? _defaults.GUVRS_OPPONENT_SAFELINE : opponnentRank?.SafetyLine.ToString() },
        });
    }

    private void OnGameMode(int gameMode)
    {
        _gameMode = gameMode;
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

    private string GetSite()
    {
        string site;
        try
        {
            site = ((JsonElement)_settings["site"]).GetString();
        }
        catch (Exception)
        {
            site = string.Empty;
        }

        return site;
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

        string site;
        try
        {
            site = ((JsonElement)_settings["site"]).GetString();
        }
        catch (Exception)
        {
            site = string.Empty;
        }

        if (string.IsNullOrEmpty(theme))
            return;

        EvaluateJavaScriptAsync($"guvrs_set_settings('{theme}', '{autoOpen.ToLower()}', '{site}');");
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

    private void _SetHtml(string name, string html)
    {
       
        EvaluateJavaScriptAsync($"guvrs_set_html('{name}', '{html}');");
    }

    private void SaveFriends()
    {
        var jsonObject = new FriendsModel()
        {
            Friends = Friends
        };

        var json = JsonSerializer.Serialize(jsonObject);
        CrossPlatform.FileSystem.WriteText("friends.json", json);

    }

    private void LoadFriends()
    {
        if (!CrossPlatform.FileSystem.Exists("friends.json"))
            return;

        var json = CrossPlatform.FileSystem.ReadText("friends.json");
        var jsonObject = JsonSerializer.Deserialize<FriendsModel>(json);

        Friends = jsonObject.Friends;
    }

    private void SaveSettings()
    {
        var json = _settings.Save<string>();
        CrossPlatform.FileSystem.WriteText("settings.json", json);
    }

    private void LoadSettings()
    {
        if (!CrossPlatform.FileSystem.Exists("settings.json"))
            return;

        var json = CrossPlatform.FileSystem.ReadText("settings.json");
        _settings.Load<string>(json);
    }

    private void EvaluateJavaScriptAsync(string javascript)
    {
        ControlRenderer.Render(WebView, async () =>
        {
            try
            {
                await WebView.EvaluateJavaScriptAsync(javascript);
            }
            catch (Exception)
            {
                return; // suppress errors in runtime
            }
        });
    }

    private void OpenBrowserWithGameMode(string gameId, string guid)
    {
        if (string.IsNullOrEmpty(gameId))
            return;

        var site = GetSite()?.ToLower();
        switch (site)
        {
            default:
            case "gudecks":
                switch (_gameMode)
                {
                    case 200: // chaos
                    case 13: // ranked
                    case 110: // casual
                    case 7: // sealed
                        Browser.OpenAsync($"https://gudecks.com/meta/player-stats?userId={guid}&gameMode={_gameMode}").Wait(TimeSpan.FromSeconds(3));
                        break;
                    default:
                        Browser.OpenAsync($"https://gudecks.com/meta/player-stats?userId={guid}").Wait(TimeSpan.FromSeconds(3));
                        break;
                }
                break;
            case "gumeta":
                switch (_gameMode)
                {
                    case 7: // sealed
                        Browser.OpenAsync($"https://gumeta.web.app/sealed?userId={guid}").Wait(TimeSpan.FromSeconds(3));
                        break;
                    case 13: // ranked
                    default:
                        Browser.OpenAsync($"https://gumeta.web.app/ranked?userId={guid}").Wait(TimeSpan.FromSeconds(3));
                        break;
                }
                break;
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

    private void OnOpenSettingsFolder(Dictionary<string, string> data)
    {
        CrossPlatform.Open($"{CrossPlatform.FileSystem.AppDataDirectory}");
    }

    private void OnSaveSettings(Dictionary<string, string> data)
    {
        foreach (var setting in data)
            _settings.Write(setting.Key, setting.Value);

        SaveSettings();
        LoadSettings();
    }

    private void OnSaveFriend(Dictionary<string, string> data)
    {
        int guid = 0;
        try
        {
            if (data.ContainsKey("guid"))
                guid = Convert.ToInt32(data["guid"]);
        }
        catch (FormatException)
        {
            _SetHtml("friend-errors", "Guid must be a number.");
            return;
        }

        string name = string.Empty;
        if (data.ContainsKey("name"))
            name = data["name"];

        var model = new FriendModel()
        {
            Guid = guid,
            Name = name
        };
     
        var exists = !(Friends.FirstOrDefault(x => x.Guid == guid) == default(FriendModel));
        if (!exists)

        {
            Friends.Add(model);
            SaveFriends();
        }
        else
        {
            _SetHtml("friend-errors", "Friend already exists.");
        }
    }

    private void OnOpen(Dictionary<string, string> data)
    {
        if (!data.ContainsKey("guid"))
            return;

        var guid = data["guid"];
        if (guid == "-1")
            return;

        OpenBrowserWithGameMode(_gameId, guid);
    }

    private void OnCopy(Dictionary<string, string> data)
    {
        if (!data.ContainsKey("guid"))
            return;

        var guid = data["guid"];
        if (guid == "-1")
            return;

        Clipboard.SetTextAsync(guid).Wait(TimeSpan.FromSeconds(3));
    }

    private void OnReportIssue(Dictionary<string, string> data)
    {
        Browser.OpenAsync($"https://github.com/TimothyMeadows/GUvrs/issues/new");
    }
}