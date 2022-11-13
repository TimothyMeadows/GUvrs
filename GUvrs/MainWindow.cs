using System.Diagnostics;

namespace GUvrs
{
    public partial class MainWindow : Form
    {
        private readonly string _path;
        private readonly FileSystemWatcher _watcher;

        private string playerId = string.Empty;
        private string playerName = string.Empty;
        private string opponentId = string.Empty;
        private string opponentName = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData",
                "LocalLow", "Immutable", "gods");
            if (string.IsNullOrEmpty(_path) || !Directory.Exists(_path))
                throw new FileNotFoundException(
                    "Unable to locate Gods Unchained install. If you feel this is a mistake. Please report this to Github.");

            _watcher = new FileSystemWatcher();
            _watcher.Path = _path;
            _watcher.Filter = "*.log";
            _watcher.NotifyFilter = NotifyFilters.Security | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.LastAccess;
            _watcher.EnableRaisingEvents = true;
            _watcher.Changed += _watcher_Changed;
            _watcher.Deleted += _watcher_Deleted;

            lblPlayerNameValue.Text = string.Empty;
            lblPlayerIDValue.Text = string.Empty;
            lblOpponentIDValue.Text = string.Empty;
            lblOpponentNameValue.Text = string.Empty;
        }

        private void _watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            lblPlayerNameValue.Text = string.Empty;
            lblPlayerIDValue.Text = string.Empty;
            lblOpponentIDValue.Text = string.Empty;
            lblOpponentNameValue.Text = string.Empty;
        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var file = File.ReadAllText($"{_path}\\debug.log");
            var lines = file.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // TODO: This should be A LOT more optimized, but for now should always grab the most recent game last.
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
                        foreach (var playerProperty in playerProperties)
                        {
                            if (playerProperty.Contains("apolloId"))
                                playerId = playerProperty.Split(':', StringSplitOptions.TrimEntries)[1];

                            if (playerProperty.Contains("nickName"))
                                playerName = playerProperty.Split(':', StringSplitOptions.TrimEntries)[1];
                        }

                        var opponentProperties = opponentInfo.Split(',', ' ', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var opponentProperty in opponentProperties)
                        {
                            if (opponentProperty.Contains("apolloId"))
                                opponentId = opponentProperty.Split(':', StringSplitOptions.TrimEntries)[1];

                            if (opponentProperty.Contains("nickName"))
                                opponentName = opponentProperty.Split(':', StringSplitOptions.TrimEntries)[1];
                        }
                    }
                }
            }

            lblPlayerIDValue.Text = playerId;
            lblPlayerNameValue.Text = playerName;
            lblOpponentIDValue.Text = opponentId;
            lblOpponentNameValue.Text = opponentName;
        }

        private void btnViewPlayerDeck_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo($"https://gudecks.com/meta/player-stats?userId={playerId}")
            {
                UseShellExecute = true
            });
        }

        private void btnViewOpponentDeck_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo($"https://gudecks.com/meta/player-stats?userId={opponentId}")
            {
                UseShellExecute = true
            });
        }
    }
}