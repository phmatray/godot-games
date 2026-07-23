using R3;

namespace Raga.Game.Services;

public sealed class UiState
{
    private static UiState _instance;

    public static UiState Instance
        => _instance ??= new UiState();

    private UiState()
    {
        Counter = new ReactiveProperty<int>(0);

        LabelText = Counter
            .Select(x => $"Counter: {x}")
            .ToReadOnlyReactiveProperty();
        
        PlayerName = new ReactiveProperty<string>("Player1");
    }

    public ReactiveProperty<int> Counter { get; }
    public ReadOnlyReactiveProperty<string> LabelText { get; }
    public ReactiveProperty<string> PlayerName { get; }
}