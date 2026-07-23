using Godot;

namespace PlatformerBrackeys;

public partial class Player : CharacterBody2D
{
    public const float Speed = 130.0f;
    public const float JumpVelocity = -300.0f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    
    private AnimatedSprite2D _animatedSprite = null!;

    public override void _Ready()
    {
        // Initialize the animated sprite by finding the node
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        // Ensure the animated sprite node exists to avoid runtime errors
        if (_animatedSprite == null)
        {
            GD.PrintErr("AnimatedSprite2D node not found. Please ensure it exists and is correctly named.");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Apply gravity if the player is not on the floor
        if (!IsOnFloor())
        {
            velocity.Y += Gravity * (float)delta;
        }

        // Handle jumping
        if (Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction: 0, -1, or 1.
        var direction = Input.GetAxis("move_left", "move_right");

        // Handle horizontal movement
        HandleMovement(direction, ref velocity);

        // Handle animations
        HandleAnimations(direction);

        Velocity = velocity;
        MoveAndSlide();
    }

    private void HandleMovement(float direction, ref Vector2 velocity)
    {
        // Move the player.
        if (direction != 0)
        {
            velocity.X = direction * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }
    }

    private void HandleAnimations(float direction)
    {
        // Flip the sprite.
        if (direction != 0)
        {
            _animatedSprite.FlipH = direction < 0;
        }
        
        // Play the animation.
        if (IsOnFloor())
        {
            // Idle or Run animation.
            _animatedSprite.Play(direction == 0 ? "idle" : "run");
        }
        else
        {
            // Jump animation.
            _animatedSprite.Play("jump");
        }
    }
}