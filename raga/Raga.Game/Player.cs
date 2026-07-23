using Godot;
using System;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Raga.Game;
using Raga.Game.Libs;
using Raga.Game.Services;

public partial class Player : CharacterBody2D
{
    [Export] public int Speed = 300;
    
    private PlayerClient _playerClient;

    public override void _Ready()
    {
        // Get TextEdit named PlayerName
        _playerClient = new PlayerClient(Constants.GameServerUrl);
        
        Task.Run(async () =>
        {
            var playerName = UiState.Instance.PlayerName.Value;
            GD.Print($"Player name: {playerName}");
            
            var inventory = await _playerClient.GetInventoryAsync(playerName);
            foreach (var item in inventory.Items)
            {
                GD.Print(item);
            }
        });
    }
    
    public override void _PhysicsProcess(double delta)
    {
        var velocity = Vector2.Zero;
    
        if (Input.IsActionPressed("ui_up"))
            velocity.Y -= Speed;
        if (Input.IsActionPressed("ui_down"))
            velocity.Y += Speed;
        if (Input.IsActionPressed("ui_left"))
            velocity.X -= Speed;
        if (Input.IsActionPressed("ui_right"))
            velocity.X += Speed;
    
        Velocity = velocity.Normalized() * Speed;
        MoveAndSlide();
    }
}
