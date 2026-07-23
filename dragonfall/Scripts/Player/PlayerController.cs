using Godot;

namespace Dragonfall.Player;

/// <summary>
/// First-person player controller with WASD movement and mouse look.
/// </summary>
public partial class PlayerController : CharacterBody3D
{
	[ExportGroup("Movement")]
	[Export] public float MoveSpeed { get; set; } = 5.0f;
	[Export] public float SprintMultiplier { get; set; } = 1.5f;
	[Export] public float JumpVelocity { get; set; } = 5.0f;
	[Export] public float Gravity { get; set; } = 20.0f;

	[ExportGroup("Camera")]
	[Export] public float MouseSensitivity { get; set; } = 0.002f;
	[Export] public float MinPitch { get; set; } = -80f;
	[Export] public float MaxPitch { get; set; } = 80f;

	private Camera3D _camera;
	private Node3D _cameraMount;
	private float _cameraPitch;
	private Vector3 _velocity = Vector3.Zero;

	public override void _Ready()
	{
		// Setup camera hierarchy
		_cameraMount = GetNodeOrNull<Node3D>("CameraMount");
		if (_cameraMount == null)
		{
			_cameraMount = new Node3D();
			_cameraMount.Name = "CameraMount";
			AddChild(_cameraMount);
		}

		_camera = _cameraMount.GetNodeOrNull<Camera3D>("Camera3D");
		if (_camera == null)
		{
			_camera = new Camera3D();
			_camera.Name = "Camera3D";
			_camera.Fov = 75f;
			_camera.Near = 0.1f;
			_camera.Far = 1000f;
			_camera.Current = true; // Make sure this camera is active!
			_cameraMount.AddChild(_camera);

			GD.Print($"Camera created - FOV: {_camera.Fov}, Near: {_camera.Near}, Far: {_camera.Far}, Current: {_camera.Current}");
		}
		else
		{
			_camera.Current = true; // Ensure it's the current camera
			GD.Print($"Camera found - Current: {_camera.Current}");
		}

		// Capture mouse
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _PhysicsProcess(double delta)
	{
		// Get input direction
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		// Handle jump
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			_velocity.Y = JumpVelocity;
		}

		// Apply gravity
		if (!IsOnFloor())
		{
			_velocity.Y -= Gravity * (float)delta;
		}
		else
		{
			_velocity.Y = Mathf.Max(_velocity.Y, 0);
		}

		// Calculate horizontal movement speed
		float currentSpeed = MoveSpeed;
		if (Input.IsActionPressed("sprint"))
		{
			currentSpeed *= SprintMultiplier;
		}

		// Move horizontally
		if (direction != Vector3.Zero)
		{
			_velocity.X = direction.X * currentSpeed;
			_velocity.Z = direction.Z * currentSpeed;
		}
		else
		{
			_velocity.X = Mathf.MoveToward(_velocity.X, 0, currentSpeed);
			_velocity.Z = Mathf.MoveToward(_velocity.Z, 0, currentSpeed);
		}

		// Apply velocity
		Velocity = _velocity;
		MoveAndSlide();

		// Update velocity from physics
		_velocity = Velocity;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		// Handle mouse look
		if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			// Horizontal rotation (yaw) - rotate the player body
			RotateY(-mouseMotion.Relative.X * MouseSensitivity);

			// Vertical rotation (pitch) - rotate the camera mount
			_cameraPitch -= mouseMotion.Relative.Y * MouseSensitivity;
			_cameraPitch = Mathf.Clamp(_cameraPitch, Mathf.DegToRad(MinPitch), Mathf.DegToRad(MaxPitch));

			if (_cameraMount != null)
			{
				_cameraMount.Rotation = new Vector3(_cameraPitch, 0, 0);
			}
		}

		// Toggle mouse capture with Escape
		if (@event.IsActionPressed("ui_cancel"))
		{
			if (Input.MouseMode == Input.MouseModeEnum.Captured)
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
			else
			{
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
		}
	}

	/// <summary>
	/// Get the camera's forward direction (useful for weapons).
	/// </summary>
	public Vector3 GetCameraForward()
	{
		return _camera != null ? -_camera.GlobalTransform.Basis.Z : -GlobalTransform.Basis.Z;
	}

	/// <summary>
	/// Get the camera node.
	/// </summary>
	public Camera3D GetCamera()
	{
		return _camera;
	}
}
