using Godot;

namespace PlatformerBrackeys;

public partial class Slime : Node2D
{
    public const int Speed = 60;
    public int Direction = 1;
    
    private RayCast2D _rayCastRight = null!;
    private RayCast2D _rayCastLeft = null!;
    private AnimatedSprite2D _animatedSprite = null!;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _rayCastRight = GetNode<RayCast2D>("RayCastRight");
        _rayCastLeft = GetNode<RayCast2D>("RayCastLeft");
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        
        // Ensure all nodes are properly initialized to avoid null reference issues
        if (_rayCastRight == null || _rayCastLeft == null || _animatedSprite == null)
        {
            GD.PrintErr("One or more required nodes not found. Please ensure RayCastRight, RayCastLeft, and AnimatedSprite2D nodes exist.");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_rayCastRight.IsColliding())
        {
            ChangeDirection(-1, true);
        }
        
        if (_rayCastLeft.IsColliding())
        {
            ChangeDirection(1, false);
        }
        
        MoveSlime((float)delta);
    }

    // Method to change the direction of the slime
    private void ChangeDirection(int newDirection, bool flipSprite)
    {
        Direction = newDirection;
        _animatedSprite.FlipH = flipSprite;
    }

    // Method to move the slime
    private void MoveSlime(float delta)
    {
        var x = Direction * Speed * delta;
        Position += new Vector2(x, 0);
    }
}
