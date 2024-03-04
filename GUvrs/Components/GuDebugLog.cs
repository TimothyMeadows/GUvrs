using System.IO;
using System.Text;
using GUvrs.Models;

namespace GUvrs.Components;

public class GuDebugLog
{
    private readonly FileSystemWatcher _watcher;
    private Timer _timer;
    private bool _onTick = false;

    public delegate void GameStartHandler(GameStartModel model);
    public event GameStartHandler OnStart;
    private bool _onStartFired = false;

    public delegate void GameModeHandler(int gameMode);
    public event GameModeHandler OnGameMode;
    private bool _onGameModeFired = false;

    public delegate void GameStopHandler(GameStopModel model);
    public event GameStopHandler OnStop;
    private bool _onStopFired = false;

    public delegate void GameBeginHandler(GameBeginModel model);
    public event GameBeginHandler OnBegin;
    private bool _onBeginFired = false;

    public delegate void GameEndHandler();
    public event GameEndHandler OnEnd;

    public GuDebugLog()
    {
        _timer = new Timer(Tick, null, 0, 1000);

        _watcher = new FileSystemWatcher();
        _watcher.Path = CrossPlatform.GuLogPath;
        _watcher.Filter = "*.log";
        _watcher.NotifyFilter = NotifyFilters.Security | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.LastAccess;
        _watcher.EnableRaisingEvents = true;

        _watcher.Changed += Changed;
    }

    private void Changed(object sender, FileSystemEventArgs e)
    {
        if (e.Name?.ToLower() == "debug.log")
            _onTick = true;
    }

    private void Tick(object sender)
    {
        if (!_onTick)
            return;

        _onTick = false;
        var debugPath = $"{CrossPlatform.GuLogPath}/debug.log";
        if (!File.Exists(debugPath))
            return;

        var (result, file) = ReadAllText(debugPath);
        if (!result)
            return;

        var lines = file.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (!_onStartFired && line.Contains("gameID:"))
            {
                var gameId = line.Extract("gameID: '", "' ");
                if (gameId == "TODO")
                    gameId = "SOLO";

                OnStart?.Invoke(new GameStartModel()
                {
                    GameId = gameId
                });

                _onStartFired = true;
                continue;
            }

            if (!_onGameModeFired && line.Contains("Game mode is "))
            {
                var gameMode = line.Extract("Game mode is '", "'");

                OnGameMode?.Invoke(Convert.ToInt32(gameMode));

                _onGameModeFired = true;
                continue;
            }

            if (!_onBeginFired && line.Contains("p:PlayerInfo") && line.Contains("o:PlayerInfo"))
            {
                var playerInfoLine = line.Extract("p:PlayerInfo(", "),");
                var playerInfo = GetPlayerInfo(playerInfoLine);
                var opponentInfoLine = line.Extract("o:PlayerInfo(", "))");
                var opponentInfo = GetPlayerInfo(opponentInfoLine);

                OnBegin?.Invoke(new GameBeginModel()
                {
                    Player = new PlayerModel()
                    {
                        ID = playerInfo.GetValueOrDefault("apolloId"),
                        Name = playerInfo.GetValueOrDefault("nickName")
                    },
                    Opponnent = new PlayerModel()
                    {
                        ID = opponentInfo.GetValueOrDefault("apolloId"),
                        Name = opponentInfo.GetValueOrDefault("nickName")
                    }
                });

                _onBeginFired = true;
                continue;
            }

            if (!_onStopFired && line.Contains("GameNetworkManager.StopClient"))
            {
                OnStop?.Invoke(new GameStopModel()
                {
                    Concede = line.Contains("ClientAPI.CloseClient"),
                    Won = line.Contains("LocalPlayer won")
                });

                _onStopFired = true;
                continue;
            }

            if (line.Contains("Ending the game") || line.Contains("Send Terminate Message"))
            {
                OnEnd?.Invoke();

                _onStartFired = false;
                _onBeginFired = false;
                _onStopFired = false;
                _onGameModeFired = false;

                _onTick = false;
                return;
            }
        }
    }

    private (bool, string) ReadAllText(string path)
    {
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var buffer = new byte[stream.Length];
            _ = stream.Read(buffer, 0, (int)stream.Length);

            var text = string.Empty;
            if (buffer.Length > 0)
                text = Encoding.UTF8.GetString(buffer);

            return (true, text);
        }
        catch (Exception)
        {
            return (false, string.Empty);
        }
    }

    private Dictionary<string, string> GetPlayerInfo(string value)
    {
        var playerInfo = new Dictionary<string, string>();

        var propertyValue = value.Extract("nickName:", "isOnServer:")?.Trim();
        if (!string.IsNullOrEmpty(propertyValue))
            propertyValue = propertyValue[..^1];
        playerInfo.Add("nickName", propertyValue);

        propertyValue = value.Extract("apolloId:", "netId:")?.Trim();
        if (!string.IsNullOrEmpty(propertyValue))
            propertyValue = propertyValue[..^1];
        playerInfo.Add("apolloId", propertyValue);

        return playerInfo;
    }
}