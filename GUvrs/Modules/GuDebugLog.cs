using System;
using System.Threading;
using GUvrs.Models;

namespace GUvrs.Modules
{
	public class GuDebugLog
	{
        private readonly FileSystemWatcher _watcher;
        private PlayerModel _player;
        private PlayerModel _opponent;

        public delegate void GameStartHandler(GameStartModel model);
        public event GameStartHandler OnStart;

        public delegate void GameStopHandler(GameStopHandler model);
        public event GameStopHandler OnStop;

        public delegate void GameBeginHandler(PlayerModel player, PlayerModel opponent);
        public event GameBeginHandler OnBegin;

        public delegate void GameEndHandler();
        public event GameEndHandler OnEnd;

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

        public void Reset()
        {
            _player = null;
            _opponent = null;
        }

        private void Deleted(object sender, FileSystemEventArgs e)
        {
            Reset();
            OnBegin?.Invoke(null, null);
        }

        private void Changed(object sender, FileSystemEventArgs e)
        {
            if (_player == null && _opponent == null)
                ReadLog();
        }

        private void ReadLog()
        {
            var timeout = 0;
            while(true)
            {
                var file = File.ReadAllText($"{CrossPlatform.GuLogPath}/debug.log");
                var lines = file.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    if (line.Contains("p:PlayerInfo") && line.Contains("o:PlayerInfo"))
                    {
                        var playerInfo = line.Split(new string[] { "p:PlayerInfo(" }, StringSplitOptions.None)[1].Split(')')[0];
                        var opponentInfo = line.Split(new string[] { "o:PlayerInfo(" }, StringSplitOptions.None)[1].Split(')')[0];

                        if (string.IsNullOrEmpty(playerInfo) || string.IsNullOrEmpty(opponentInfo))
                            continue;
                        else
                        {
                            var playerProperties = playerInfo.Split(',', ' ', StringSplitOptions.RemoveEmptyEntries);
                            _player = new PlayerModel();
                            foreach (var playerProperty in playerProperties)
                            {

                                if (playerProperty.Contains("apolloId"))
                                    _player.ID = Convert.ToInt64(playerProperty.Split(':', StringSplitOptions.TrimEntries)[1]);

                                if (playerProperty.Contains("nickName"))
                                    _player.Name = playerProperty.Split(':', StringSplitOptions.TrimEntries)[1];
                            }

                            var opponentProperties = opponentInfo.Split(',', ' ', StringSplitOptions.RemoveEmptyEntries);
                            _opponent = new PlayerModel();
                            foreach (var opponentProperty in opponentProperties)
                            {
                                if (opponentProperty.Contains("apolloId"))
                                    _opponent.ID = Convert.ToInt64(opponentProperty.Split(':', StringSplitOptions.TrimEntries)[1]);

                                if (opponentProperty.Contains("nickName"))
                                    _opponent.Name = opponentProperty.Split(':', StringSplitOptions.TrimEntries)[1];
                            }
                        }
                    }
                }

                if (_player == null || _opponent == null)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    timeout++;

                    if (timeout <= 320)
                        continue;
                }

                break;
            }

            OnBegin?.Invoke(_player, _opponent);
        }
    }
}

