using Godot;

namespace PlatformerBrackeys;

public partial class Killzone : Area2D
{
    private Timer _timer = null!;

    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        
        // Ensure the timer node exists to avoid runtime errors
        if (_timer == null)
        {
            GD.PrintErr("Timer node not found. Please ensure it exists and is correctly named.");
        }
    }

    public void _on_body_entered(Node2D body)
    {
        GD.Print("You died!");
        Engine.TimeScale = 0.5;
        
        // body is the node that entered the area (in this case, the player)
        body.GetNode<CollisionShape2D>("CollisionShape2D").QueueFree();
        
        _timer.Start();
    }
    
    public void _on_timer_timeout()
    {
        Engine.TimeScale = 1;
        GetTree().ReloadCurrentScene();
    }
}