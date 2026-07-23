using Godot;
using System.Collections.Generic;
using Dragonfall.Core;

namespace Dragonfall.Dungeon;

/// <summary>
/// Static utility class for building 3D dungeon geometry from room/corridor data.
/// </summary>
public static class DungeonMeshBuilder
{
	/// <summary>
	/// Builds complete dungeon geometry including rooms, corridors, and collision.
	/// </summary>
	public static Node3D BuildDungeonGeometry(
		List<DungeonRoom> rooms,
		List<Corridor> corridors,
		DungeonTheme theme)
	{
		var root = new Node3D();
		root.Name = "DungeonGeometry";

		var roomsNode = new Node3D();
		roomsNode.Name = "Rooms";
		root.AddChild(roomsNode);

		var corridorsNode = new Node3D();
		corridorsNode.Name = "Corridors";
		root.AddChild(corridorsNode);

		// Build rooms
		foreach (var room in rooms)
		{
			var roomNode = BuildRoom(room, theme);
			roomsNode.AddChild(roomNode);
		}

		// Build corridors
		foreach (var corridor in corridors)
		{
			var corridorNode = BuildCorridor(corridor, theme);
			corridorsNode.AddChild(corridorNode);
		}

		return root;
	}

	/// <summary>
	/// Builds a single room with floor, ceiling, and walls (walls temporarily disabled).
	/// </summary>
	private static Node3D BuildRoom(DungeonRoom room, DungeonTheme theme)
	{
		var roomNode = new Node3D();
		roomNode.Name = $"Room_{room.Type}";

		// Calculate 3D position and size
		Vector3 position = new Vector3(
			room.Bounds.Position.X + room.Bounds.Size.X / 2f,
			0,
			room.Bounds.Position.Y + room.Bounds.Size.Y / 2f
		);

		// Create floor with collision
		var floorBody = new StaticBody3D();
		floorBody.Name = "Floor";

		// SET COLLISION LAYERS - This is critical!
		floorBody.CollisionLayer = 1; // Layer 1 - Environment
		floorBody.CollisionMask = 0;  // Doesn't need to detect anything

		var floor = CreateFloor(room.Bounds, theme);
		floor.Position = Vector3.Zero;
		floorBody.AddChild(floor);

		// Add floor collision (thicker for better collision detection)
		var floorCollision = new CollisionShape3D();
		var floorBox = new BoxShape3D();
		floorBox.Size = new Vector3(room.Bounds.Size.X, 1.0f, room.Bounds.Size.Y); // Much thicker
		floorCollision.Shape = floorBox;
		floorCollision.Position = new Vector3(0, -0.5f, 0); // Position so top is at Y=0
		floorBody.AddChild(floorCollision);

		floorBody.Position = new Vector3(position.X, 0, position.Z);
		roomNode.AddChild(floorBody);

		GD.Print($"Floor collision created - Layer: {floorBody.CollisionLayer}, Size: ({room.Bounds.Size.X}, 1.0, {room.Bounds.Size.Y})");

		// Create ceiling
		var ceiling = CreateCeiling(room.Bounds, theme);
		ceiling.Position = new Vector3(position.X, theme.WallHeight, position.Z);
		roomNode.AddChild(ceiling);

		// Create walls with door openings where corridors connect
		CreateWallsWithDoors(roomNode, room, theme);

		return roomNode;
	}

	/// <summary>
	/// Creates walls with automatic door openings where corridors connect.
	/// </summary>
	private static void CreateWallsWithDoors(Node3D roomNode, DungeonRoom room, DungeonTheme theme)
	{
		var wallsBody = new StaticBody3D();
		wallsBody.Name = "Walls";

		// Set collision layers for walls
		wallsBody.CollisionLayer = 1; // Layer 1 - Environment
		wallsBody.CollisionMask = 0;

		float doorWidth = theme.CorridorWidth * 2.5f; // Door opening size
		Rect2I bounds = room.Bounds;

		// Create walls for each side
		CreateWallSide(wallsBody, room, "north", bounds, theme, doorWidth);
		CreateWallSide(wallsBody, room, "south", bounds, theme, doorWidth);
		CreateWallSide(wallsBody, room, "west", bounds, theme, doorWidth);
		CreateWallSide(wallsBody, room, "east", bounds, theme, doorWidth);

		roomNode.AddChild(wallsBody);
	}

	/// <summary>
	/// Creates wall segments for one side of a room with door openings.
	/// </summary>
	private static void CreateWallSide(StaticBody3D wallsBody, DungeonRoom room, string side, Rect2I bounds, DungeonTheme theme, float doorWidth)
	{
		float wallHeight = theme.WallHeight;
		float wallThickness = theme.WallThickness;
		float halfHeight = wallHeight / 2f;

		// Find door positions on this wall
		List<float> doorPositions = new List<float>();

		foreach (var connection in room.CorridorConnections)
		{
			float doorPos = GetDoorPositionOnWall(connection, bounds, side);
			if (doorPos >= 0)
			{
				doorPositions.Add(doorPos);
			}
		}

		// Sort door positions
		doorPositions.Sort();

		// Create wall segments between doors
		switch (side)
		{
			case "north":
				CreateHorizontalWallSegments(wallsBody, bounds.Position.X, bounds.End.X,
					bounds.Position.Y - wallThickness / 2, halfHeight, doorPositions, doorWidth,
					wallThickness, wallHeight, theme.WallColor, true);
				break;
			case "south":
				CreateHorizontalWallSegments(wallsBody, bounds.Position.X, bounds.End.X,
					bounds.End.Y + wallThickness / 2, halfHeight, doorPositions, doorWidth,
					wallThickness, wallHeight, theme.WallColor, true);
				break;
			case "west":
				CreateVerticalWallSegments(wallsBody, bounds.Position.Y, bounds.End.Y,
					bounds.Position.X - wallThickness / 2, halfHeight, doorPositions, doorWidth,
					wallThickness, wallHeight, theme.WallColor, false);
				break;
			case "east":
				CreateVerticalWallSegments(wallsBody, bounds.Position.Y, bounds.End.Y,
					bounds.End.X + wallThickness / 2, halfHeight, doorPositions, doorWidth,
					wallThickness, wallHeight, theme.WallColor, false);
				break;
		}
	}

	/// <summary>
	/// Determines if a corridor connection point is on a specific wall and returns its position.
	/// </summary>
	private static float GetDoorPositionOnWall(Vector2I connection, Rect2I bounds, string side)
	{
		float margin = 3f; // Tolerance for connection point

		switch (side)
		{
			case "north":
				if (connection.Y <= bounds.Position.Y + margin)
					return connection.X;
				break;
			case "south":
				if (connection.Y >= bounds.End.Y - margin)
					return connection.X;
				break;
			case "west":
				if (connection.X <= bounds.Position.X + margin)
					return connection.Y;
				break;
			case "east":
				if (connection.X >= bounds.End.X - margin)
					return connection.Y;
				break;
		}
		return -1;
	}

	/// <summary>
	/// Creates horizontal wall segments (for north/south walls).
	/// </summary>
	private static void CreateHorizontalWallSegments(StaticBody3D wallsBody, float startX, float endX,
		float z, float yCenter, List<float> doorPositions, float doorWidth, float thickness,
		float height, Color color, bool isHorizontal)
	{
		float currentX = startX;

		foreach (float doorX in doorPositions)
		{
			float doorStart = doorX - doorWidth / 2;
			float doorEnd = doorX + doorWidth / 2;

			// Create segment before door
			if (doorStart > currentX)
			{
				float segmentWidth = doorStart - currentX;
				CreateWallSegment(wallsBody, new Vector3(segmentWidth, height, thickness),
					new Vector3(currentX + segmentWidth / 2, yCenter, z), color);
			}

			currentX = doorEnd;
		}

		// Create final segment
		if (endX > currentX)
		{
			float segmentWidth = endX - currentX;
			CreateWallSegment(wallsBody, new Vector3(segmentWidth, height, thickness),
				new Vector3(currentX + segmentWidth / 2, yCenter, z), color);
		}
	}

	/// <summary>
	/// Creates vertical wall segments (for east/west walls).
	/// </summary>
	private static void CreateVerticalWallSegments(StaticBody3D wallsBody, float startZ, float endZ,
		float x, float yCenter, List<float> doorPositions, float doorWidth, float thickness,
		float height, Color color, bool isHorizontal)
	{
		float currentZ = startZ;

		foreach (float doorZ in doorPositions)
		{
			float doorStart = doorZ - doorWidth / 2;
			float doorEnd = doorZ + doorWidth / 2;

			// Create segment before door
			if (doorStart > currentZ)
			{
				float segmentLength = doorStart - currentZ;
				CreateWallSegment(wallsBody, new Vector3(thickness, height, segmentLength),
					new Vector3(x, yCenter, currentZ + segmentLength / 2), color);
			}

			currentZ = doorEnd;
		}

		// Create final segment
		if (endZ > currentZ)
		{
			float segmentLength = endZ - currentZ;
			CreateWallSegment(wallsBody, new Vector3(thickness, height, segmentLength),
				new Vector3(x, yCenter, currentZ + segmentLength / 2), color);
		}
	}

	/// <summary>
	/// Creates a single wall segment with visual mesh and collision.
	/// </summary>
	private static void CreateWallSegment(StaticBody3D wallsBody, Vector3 size, Vector3 position, Color color)
	{
		// Skip if segment is too small
		if (size.X < 0.5f || size.Z < 0.5f)
			return;

		// Create visual mesh
		var wallMesh = MeshBuilder.CreateCube(size, color);
		wallMesh.Position = position;
		wallsBody.AddChild(wallMesh);

		// Create collision
		var collision = new CollisionShape3D();
		var box = new BoxShape3D();
		box.Size = size;
		collision.Shape = box;
		collision.Position = position;
		wallsBody.AddChild(collision);
	}

	/// <summary>
	/// Creates a floor mesh for a room.
	/// </summary>
	private static MeshInstance3D CreateFloor(Rect2I bounds, DungeonTheme theme)
	{
		return MeshBuilder.CreatePlane(
			new Vector2(bounds.Size.X, bounds.Size.Y),
			theme.FloorColor
		);
	}

	/// <summary>
	/// Creates a ceiling mesh for a room.
	/// </summary>
	private static MeshInstance3D CreateCeiling(Rect2I bounds, DungeonTheme theme)
	{
		var ceiling = MeshBuilder.CreatePlane(
			new Vector2(bounds.Size.X, bounds.Size.Y),
			theme.CeilingColor
		);
		ceiling.RotationDegrees = new Vector3(180, 0, 0);
		return ceiling;
	}

	/// <summary>
	/// Builds a corridor with floor and ceiling.
	/// </summary>
	private static Node3D BuildCorridor(Corridor corridor, DungeonTheme theme)
	{
		var corridorNode = new Node3D();
		corridorNode.Name = "Corridor";

		if (corridor.Path.Count == 0)
			return corridorNode;

		// Make corridors wider
		float corridorWidth = theme.CorridorWidth * 2f; // Double the width

		// Create floor tiles along the path
		foreach (var point in corridor.Path)
		{
			// Create floor with collision
			var floorBody = new StaticBody3D();
			floorBody.Name = "CorridorFloor";

			// Set collision layers
			floorBody.CollisionLayer = 1; // Layer 1 - Environment
			floorBody.CollisionMask = 0;

			var floorTile = MeshBuilder.CreatePlane(
				new Vector2(corridorWidth, corridorWidth),
				theme.CorridorColor
			);
			floorBody.AddChild(floorTile);

			// Add collision (thicker for better detection)
			var floorCollision = new CollisionShape3D();
			var floorBox = new BoxShape3D();
			floorBox.Size = new Vector3(corridorWidth, 1.0f, corridorWidth); // Much thicker
			floorCollision.Shape = floorBox;
			floorCollision.Position = new Vector3(0, -0.5f, 0); // Position so top is at Y=0
			floorBody.AddChild(floorCollision);

			floorBody.Position = new Vector3(point.X, 0, point.Y);
			corridorNode.AddChild(floorBody);

			// Add ceiling tile
			var ceilingTile = MeshBuilder.CreatePlane(
				new Vector2(corridorWidth, corridorWidth),
				theme.CeilingColor
			);
			ceilingTile.Position = new Vector3(point.X, theme.WallHeight, point.Y);
			ceilingTile.RotationDegrees = new Vector3(180, 0, 0);
			corridorNode.AddChild(ceilingTile);
		}

		return corridorNode;
	}

	/// <summary>
	/// Adds point lights to rooms.
	/// </summary>
	public static void AddRoomLighting(Node3D roomNode, DungeonRoom room, DungeonTheme theme)
	{
		var light = new OmniLight3D();
		light.Position = new Vector3(
			room.Bounds.Position.X + room.Bounds.Size.X / 2f,
			theme.WallHeight - 0.5f,
			room.Bounds.Position.Y + room.Bounds.Size.Y / 2f
		);
		light.OmniRange = Mathf.Max(room.Bounds.Size.X, room.Bounds.Size.Y) + 5f; // Increased range
		light.LightEnergy = 2.0f; // Much brighter
		light.LightColor = new Color(1.0f, 0.95f, 0.85f); // Warmer light
		light.ShadowEnabled = false;

		roomNode.AddChild(light);

		GD.Print($"Added light to {room.Type} room at {light.Position}");
	}
}
