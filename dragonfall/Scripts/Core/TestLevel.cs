using Godot;
using Dragonfall.Player;

namespace Dragonfall.Core;

/// <summary>
/// Simple test level to verify player controller functionality.
/// </summary>
public partial class TestLevel : Node3D
{
	public override void _Ready()
	{
		// Create ground plane
		var ground = MeshBuilder.CreatePlane(new Vector2(50, 50), new Color(0.3f, 0.5f, 0.3f), 10);
		var groundBody = new StaticBody3D();
		var groundShape = new CollisionShape3D();
		var boxShape = new BoxShape3D();
		boxShape.Size = new Vector3(50, 0.1f, 50);
		groundShape.Shape = boxShape;
		groundShape.Position = new Vector3(0, -0.05f, 0);

		groundBody.AddChild(ground);
		groundBody.AddChild(groundShape);
		AddChild(groundBody);

		// Create some test cubes for spatial reference
		for (int i = 0; i < 5; i++)
		{
			var cube = MeshBuilder.CreateCube(new Vector3(1, 2, 1), new Color(0.8f, 0.2f, 0.2f));
			var cubeBody = new StaticBody3D();
			var cubeShape = new CollisionShape3D();
			var cubeBox = new BoxShape3D();
			cubeBox.Size = new Vector3(1, 2, 1);
			cubeShape.Shape = cubeBox;

			cubeBody.Position = new Vector3(i * 4 - 8, 1, 5);
			cubeBody.AddChild(cube);
			cubeBody.AddChild(cubeShape);
			AddChild(cubeBody);
		}

		// Create player
		var player = new PlayerController();
		player.Position = new Vector3(0, 2, 0);
		AddChild(player);

		// Add collision shape to player
		var playerShape = new CollisionShape3D();
		var capsule = new CapsuleShape3D();
		capsule.Radius = 0.4f;
		capsule.Height = 1.8f;
		playerShape.Shape = capsule;
		player.AddChild(playerShape);

		// Create directional light
		var light = new DirectionalLight3D();
		light.Rotation = new Vector3(Mathf.DegToRad(-45), Mathf.DegToRad(30), 0);
		light.ShadowEnabled = true;
		AddChild(light);

		// Create ambient environment
		var env = new WorldEnvironment();
		var environment = new Environment();
		environment.BackgroundMode = Environment.BGMode.Sky;
		var sky = new Sky();
		sky.SkyMaterial = new ProceduralSkyMaterial();
		environment.Sky = sky;
		environment.AmbientLightSource = Environment.AmbientSource.Sky;
		env.Environment = environment;
		AddChild(env);

		GD.Print("Test level created. Use WASD to move, mouse to look, Space to jump, Shift to sprint.");
	}
}
