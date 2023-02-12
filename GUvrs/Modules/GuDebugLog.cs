using System;
using System.Threading;
using GUvrs.Models;

namespace GUvrs.Modules;

public class GuDebugLog
{
    private readonly FileSystemWatcher _watcher;

    public delegate void GameStartHandler(GameStartModel model);
    public event GameStartHandler OnStart;
    private bool _onStartFired = false;

    public delegate void GameStopHandler(GameStopModel model);
    public event GameStopHandler OnStop;
    private bool _onStopFired = false;

    public delegate void GameBeginHandler(GameBeginModel model);
    public event GameBeginHandler OnBegin;
    private bool _onBeginFired = false;

    public delegate void GameEndHandler();
    public event GameEndHandler OnEnd;
    private bool _onEndFired = false;

    public GuDebugLog()
    {
        _watcher = new FileSystemWatcher();
        _watcher.Path = CrossPlatform.GuLogPath;
        _watcher.Filter = "*.log";
        _watcher.NotifyFilter = NotifyFilters.Security | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.LastAccess;
        _watcher.EnableRaisingEvents = true;

        _watcher.Changed += Changed;
        _watcher.Deleted += Deleted;
    }

    private void Deleted(object sender, FileSystemEventArgs e)
    {
        OnEnd?.Invoke();
    }

    private void Changed(object sender, FileSystemEventArgs e)
    {
        var file = File.ReadAllText($"{CrossPlatform.GuLogPath}/debug.log");
        var lines = file.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (!_onStartFired && line.Contains("gameID:") && line.Contains("player 0 name:") && line.Contains("player 1 name:"))
            {
                OnStart?.Invoke(new GameStartModel()
                {
                    GameId = line.Extract("gameID: '", "' "),
                    Player0 = line.Extract("player 0 name: '", "',"),
                    Player1 = line.Extract("player 1 name: '", "')")
                });

                _onStartFired = true;
                _onEndFired = false;
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
            }

            if (!_onStopFired & line.Contains("GameNetworkManager.StopClient:"))
            {
                OnStop?.Invoke(new GameStopModel()
                {
                    Concede = line.Contains("ClientAPI.CloseClient"),
                    Won = line.Contains("LocalPlayer won")
                });

                _onStopFired = true;
            }

            if (!_onEndFired && line.Contains("OnLeftGameLoading.Start"))
            {
                _onEndFired = true;
                _onStartFired = false;
                _onBeginFired = false;
                _onStopFired = false;
            }
        }
    }

    private Dictionary<string, string> GetPlayerInfo(string value)
    {
        var playerInfo = new Dictionary<string, string>();
        var properties = value.Split(',', StringSplitOptions.TrimEntries);
        foreach (var property in properties)
        {
            var keyValue = property.Split(':', StringSplitOptions.TrimEntries);
            playerInfo.Add(keyValue[0], keyValue[1]);
        }

        return playerInfo;
    }
}

