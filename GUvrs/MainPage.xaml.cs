namespace GUvrs;

using GUvrs.Components;
using Models;

public partial class MainPage : ContentPage
{
    private readonly GuDebugLog _log;
    private string _gameId = string.Empty;
    private PlayerModel _player;
    private PlayerModel _opponent;

    public MainPage()
    {
        InitializeComponent();

        _log = new GuDebugLog();
        _log.OnBegin += OnBegin;
        _log.OnStart += OnStart;
        _log.OnStop += OnStop;
        _log.OnEnd += OnEnd;
    }

    private void OnEnd()
    {
    }

    private void OnStop(GameStopModel model)
    {
    }

    private void OnStart(GameStartModel model)
    {
        _gameId = model?.GameId;
    }

    private void OnBegin(GameBeginModel model)
    {
        _player = model?.Player;
        _opponent = model?.Opponnent;
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        return base.MeasureOverride(100, 100);
    }
}