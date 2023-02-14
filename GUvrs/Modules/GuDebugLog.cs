using System.Text;
using GUvrs.Models;

namespace GUvrs.Modules;

public class GuDebugLog
{
    private readonly FileSystemWatcher _watcher;
    private Timer _timer;
    private bool _onTick = false;

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

    public GuDebugLog()
    {
        _timer = new Timer(Tick, null, 0, 3000);

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

        var (result, file) = CopyAndReadAllText(debugPath);
        if (!result)
            return;

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

            if (file.Contains("Ending the game"))
            {
                if (File.Exists($"{debugPath}.guvrs"))
                    File.Delete($"{debugPath}.guvrs");

                OnEnd?.Invoke();

                _onStartFired = false;
                _onBeginFired = false;
                _onStopFired = false;

                _onTick = false;
                return;
            }
        }
    }

    private (bool, string) CopyAndReadAllText(string path)
    {
        try
        {
            var copyPath = $"{path}.guvrs";
            File.Copy(path, copyPath, true);
            var file = File.ReadAllText(copyPath, Encoding.UTF8);
            return (true, file);
        }
        catch (IOException)
        {
            return (false, string.Empty);
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