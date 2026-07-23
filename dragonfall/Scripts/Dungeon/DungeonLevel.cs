using Godot;
using Dragonfall.Player;

namespace Dragonfall.Dungeon;

/// <summary>
/// Manages a complete dungeon level with generation, geometry, player, and lighting.
/// </summary>
public partial class DungeonLevel : Node3D
{
	[Export] public bool BuildGeometry { get; set; } = true;
	[Export] public bool AddLighting { get; set; } = true;
	[Export] public bool SpawnPlayer { get; set; } = true;
	[Export] public DungeonTheme Theme { get; set; }

	private DungeonGenerator _generator;
	private Node3D _geometryNode;
	private PlayerController _player;

	public override void _Ready()
	{
		// Use default theme if none provided
		if (Theme == null)
		{
			Theme = DungeonTheme.CreateDefault();
		}

		// Find or create generator
		_generator = GetNodeOrNull<DungeonGenerator>("DungeonGenerator");
		if (_generator == null)
		{
			_generator = new DungeonGenerator();
			_generator.Name = "DungeonGenerator";
			_generator.DebugVisualization = false; // Disable debug, we'll use real geometry
			AddChild(_generator);
		}

		// Generate dungeon
		_generator.GenerateDungeon();

		// Build geometry if enabled
		if (BuildGeometry)
		{
			BuildDungeonGeometry();
		}

		// Spawn player in the start room
		if (SpawnPlayer)
		{
			SpawnPlayerInStartRoom();
		}

		// Add environmental elements
		AddEnvironment();

		// Run diagnostics
		Core.DungeonDiagnostics.ValidateDungeon(this);

		GD.Print("Dungeon level ready!");
	}

	/// <summary>
	/// Builds the 3D geometry for the dungeon.
	/// </summary>
	private void BuildDungeonGeometry()
	{
		var rooms = _generator.GetRooms();
		var corridors = _generator.GetCorridors();

		_geometryNode = DungeonMeshBuilder.BuildDungeonGeometry(rooms, corridors, Theme);
		AddChild(_geometryNode);

		// Add lighting to rooms if enabled
		if (AddLighting)
		{
			var roomNodes = _geometryNode.GetNode("Rooms").GetChildren();
			for (int i = 0; i < rooms.Count && i < roomNodes.Count; i++)
			{
				if (roomNodes[i] is Node3D roomNode)
				{
					DungeonMeshBuilder.AddRoomLighting(roomNode, rooms[i], Theme);
				}
			}
		}

		GD.Print($"Dungeon geometry built: {rooms.Count} rooms");
	}

	/// <summary>
	/// Spawns the player in the start room.
	/// </summary>
	private void SpawnPlayerInStartRoom()
	{
		var rooms = _generator.GetRooms();
		if (rooms.Count == 0)
		{
			GD.PrintErr("Cannot spawn player: no rooms generated");
			return;
		}

		// Find start room
		DungeonRoom startRoom = rooms[0];
		foreach (var room in rooms)
		{
			if (room.Type == RoomType.Start)
			{
				startRoom = room;
				break;
			}
		}

		// Create player
		_player = new PlayerController();

		// SET COLLISION LAYERS - Critical for collision detection!
		_player.CollisionLayer = 2; // Layer 2 - Player
		_player.CollisionMask = 1;  // Collides with Layer 1 (Environment)

		_player.Position = new Vector3(
			startRoom.Center.X,
			2.0f, // Spawn higher to ensure we fall onto floor
			startRoom.Center.Y
		);

		// Add collision shape (capsule center is at player position)
		var playerShape = new CollisionShape3D();
		var capsule = new CapsuleShape3D();
		capsule.Radius = 0.4f;
		capsule.Height = 1.8f;
		playerShape.Shape = capsule;
		_player.AddChild(playerShape);

		AddChild(_player);

		GD.Print($"Player spawned at {_player.Position}");
		GD.Print($"Player CollisionLayer: {_player.CollisionLayer}, CollisionMask: {_player.CollisionMask}");
	}

	/// <summary>
	/// Adds environmental elements like lighting and atmosphere.
	/// </summary>
	private void AddEnvironment()
	{
		// Add bright directional light
		var dirLight = new DirectionalLight3D();
		dirLight.Rotation = new Vector3(Mathf.DegToRad(-45), Mathf.DegToRad(30), 0);
		dirLight.LightEnergy = 1.5f; // Very bright
		dirLight.ShadowEnabled = false;
		AddChild(dirLight);

		// Add simple world environment
		var worldEnv = new WorldEnvironment();
		var env = new Environment();
		env.BackgroundMode = Environment.BGMode.Color;
		env.BackgroundColor = new Color(0.2f, 0.2f, 0.25f);
		env.AmbientLightSource = Environment.AmbientSource.Color;
		env.AmbientLightColor = new Color(0.5f, 0.5f, 0.6f);
		env.AmbientLightEnergy = 1.0f;
		env.FogEnabled = false; // Disable fog for now

		worldEnv.Environment = env;
		AddChild(worldEnv);

		GD.Print("Environment added - DirectionalLight energy: 1.5, Ambient energy: 1.0");
	}

	/// <summary>
	/// Regenerates the dungeon with a new seed.
	/// </summary>
	public void RegenerateDungeon(int? newSeed = null)
	{
		// Clear existing geometry
		if (_geometryNode != null)
		{
			_geometryNode.QueueFree();
			_geometryNode = null;
		}

		// Remove player
		if (_player != null)
		{
			_player.QueueFree();
			_player = null;
		}

		// Generate new dungeon
		_generator.GenerateDungeon(newSeed);

		// Rebuild
		if (BuildGeometry)
		{
			BuildDungeonGeometry();
		}

		if (SpawnPlayer)
		{
			SpawnPlayerInStartRoom();
		}

		GD.Print("Dungeon regenerated!");
	}

	public override void _Input(InputEvent @event)
	{
		// Press F5 to regenerate dungeon
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.F5)
		{
			GD.Print("Regenerating dungeon (F5)...");
			RegenerateDungeon();
		}
	}
}
