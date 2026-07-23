using Godot;
using R3;
using Raga.Game.Services;

namespace Raga.Game.Scripts;

public partial class HUD : Godot.CanvasLayer
{
    private readonly UiState _uiState = UiState.Instance;
    private readonly CompositeDisposable _subscriptions = [];

    private Label _counterLabel;
    private Button _counterButton;
    private TextEdit _playerNameTextEdit;
    private Label _playerNameLabel;

    public override void _Ready()
    {
        InitializeNodes();
        InitializeSubscriptions();
    }

    private void InitializeNodes()
    {
        _counterLabel = GetNode<Label>("CounterLabel");
        _counterButton = GetNode<Button>("CounterButton");
        _playerNameTextEdit = GetNode<TextEdit>("PlayerNameTextEdit");
        _playerNameLabel = GetNode<Label>("PlayerNameLabel");
    }

    private void InitializeSubscriptions()
    {
        _counterButton.OnPressedAsObservable()
            .Do(_ => _uiState.Counter.Value++)
            .Subscribe()
            .AddTo(_subscriptions);

        _playerNameTextEdit.OnTextChangedAsObservable()
            .Do(_ => _uiState.PlayerName.Value = _playerNameTextEdit.Text)
            .Subscribe()
            .AddTo(_subscriptions);
        
        _uiState.LabelText
            .SubscribeToLabel(_counterLabel)
            .AddTo(_subscriptions);
        
        _uiState.PlayerName
            .SubscribeToLabel(_playerNameLabel)
            .AddTo(_subscriptions);
    }

    public override void _ExitTree()
    {
        _subscriptions.Dispose();
    }
}