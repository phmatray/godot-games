using Godot;

namespace PlatformerBrackeys;

public partial class Coin : Area2D
{
    private GameManager _gameManager = null!;
    private AnimationPlayer _animationPlayer = null!;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("%GameManager");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        
        // Ensure the GameManager node exists to avoid runtime errors
        if (_gameManager == null)
            GD.PrintErr("GameManager node not found. Please ensure it exists and is correctly named.");
        
        // Ensure the AnimationPlayer node exists to avoid runtime errors
        if (_animationPlayer == null)
            GD.PrintErr("AnimationPlayer node not found. Please ensure it exists and is correctly named.");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    
    public void _on_body_entered(Node2D body)
    {
        _gameManager.AddPoint();
        _animationPlayer.Play("pickup");
    }
    
}