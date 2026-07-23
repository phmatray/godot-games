using Godot;
using System.Collections.Generic;

namespace Dragonfall.Dungeon;

/// <summary>
/// Corridor data structure connecting two rooms.
/// </summary>
public class Corridor
{
	public List<Vector2I> Path { get; set; } = new();
	public DungeonRoom Room1 { get; set; }
	public DungeonRoom Room2 { get; set; }

	public Corridor(DungeonRoom room1, DungeonRoom room2)
	{
		Room1 = room1;
		Room2 = room2;
	}
}

/// <summary>
/// Generates dungeons using Binary Space Partitioning (BSP) algorithm.
/// </summary>
public partial class DungeonGenerator : Node
{
	[ExportGroup("Dungeon Size")]
	[Export] public int DungeonWidth { get; set; } = 100;
	[Export] public int DungeonHeight { get; set; } = 100;

	[ExportGroup("Room Parameters")]
	[Export] public int MinRoomSize { get; set; } = 8;
	[Export] public int MaxRoomSize { get; set; } = 20;
	[Export] public int RoomPadding { get; set; } = 2;

	[ExportGroup("BSP Parameters")]
	[Export] public int MaxDepth { get; set; } = 4;
	[Export] public float MinSplitRatio { get; set; } = 0.4f;
	[Export] public float MaxSplitRatio { get; set; } = 0.6f;

	[ExportGroup("Generation")]
	[Export] public int Seed { get; set; } = -1;
	[Export] public bool DebugVisualization { get; set; } = true;

	private BSPNode _rootNode;
	private List<DungeonRoom> _rooms = new();
	private List<Corridor> _corridors = new();
	private RandomNumberGenerator _rng = new();

	/// <summary>
	/// Gets all generated rooms.
	/// </summary>
	public List<DungeonRoom> GetRooms() => _rooms;

	/// <summary>
	/// Gets all generated corridors.
	/// </summary>
	public List<Corridor> GetCorridors() => _corridors;

	/// <summary>
	/// Gets the root BSP node.
	/// </summary>
	public BSPNode GetRootNode() => _rootNode;

	public override void _Ready()
	{
		// Auto-generate on ready
		GenerateDungeon();
	}

	/// <summary>
	/// Main entry point for dungeon generation.
	/// </summary>
	public void GenerateDungeon(int? customSeed = null)
	{
		// Clear previous data
		_rooms.Clear();
		_corridors.Clear();

		// Initialize RNG
		if (customSeed.HasValue)
		{
			_rng.Seed = (ulong)customSeed.Value;
		}
		else if (Seed >= 0)
		{
			_rng.Seed = (ulong)Seed;
		}
		else
		{
			_rng.Randomize();
		}

		GD.Print($"Generating dungeon with seed: {_rng.Seed}");

		// Create root BSP node
		_rootNode = new BSPNode(new Rect2I(0, 0, DungeonWidth, DungeonHeight), 0);

		// Recursively partition space
		PartitionSpace(_rootNode, 0);

		// Create rooms in leaf nodes
		CreateRooms(_rootNode);

		// Connect rooms with corridors
		ConnectRooms(_rootNode);

		// Assign room types
		AssignRoomTypes();

		GD.Print($"Dungeon generated: {_rooms.Count} rooms, {_corridors.Count} corridors");

		// Visualize if enabled
		if (DebugVisualization)
		{
			VisualizeDebug();
		}
	}

	/// <summary>
	/// Recursively partitions space using BSP algorithm.
	/// </summary>
	private void PartitionSpace(BSPNode node, int depth)
	{
		// Stop if max depth reached
		if (depth >= MaxDepth)
			return;

		// Stop if node is too small to split
		if (node.Bounds.Size.X < MinRoomSize * 2 || node.Bounds.Size.Y < MinRoomSize * 2)
			return;

		// Decide split direction based on aspect ratio
		bool canSplitVertically = node.Bounds.Size.X >= MinRoomSize * 2;
		bool canSplitHorizontally = node.Bounds.Size.Y >= MinRoomSize * 2;

		if (!canSplitVertically && !canSplitHorizontally)
			return;

		bool splitVertically;
		if (canSplitVertically && !canSplitHorizontally)
			splitVertically = true;
		else if (!canSplitVertically && canSplitHorizontally)
			splitVertically = false;
		else
			splitVertically = _rng.Randf() > 0.5f;

		node.IsVerticalSplit = splitVertically;

		// Calculate split position
		int splitPos;
		if (splitVertically)
		{
			int minPos = node.Bounds.Position.X + (int)(node.Bounds.Size.X * MinSplitRatio);
			int maxPos = node.Bounds.Position.X + (int)(node.Bounds.Size.X * MaxSplitRatio);
			splitPos = _rng.RandiRange(minPos, maxPos);
			node.SplitPosition = new Vector2I(splitPos, node.Bounds.Position.Y);

			// Create left and right children
			var leftBounds = new Rect2I(
				node.Bounds.Position.X,
				node.Bounds.Position.Y,
				splitPos - node.Bounds.Position.X,
				node.Bounds.Size.Y
			);

			var rightBounds = new Rect2I(
				splitPos,
				node.Bounds.Position.Y,
				node.Bounds.End.X - splitPos,
				node.Bounds.Size.Y
			);

			node.Left = new BSPNode(leftBounds, depth + 1);
			node.Right = new BSPNode(rightBounds, depth + 1);
		}
		else
		{
			int minPos = node.Bounds.Position.Y + (int)(node.Bounds.Size.Y * MinSplitRatio);
			int maxPos = node.Bounds.Position.Y + (int)(node.Bounds.Size.Y * MaxSplitRatio);
			splitPos = _rng.RandiRange(minPos, maxPos);
			node.SplitPosition = new Vector2I(node.Bounds.Position.X, splitPos);

			// Create top and bottom children
			var topBounds = new Rect2I(
				node.Bounds.Position.X,
				node.Bounds.Position.Y,
				node.Bounds.Size.X,
				splitPos - node.Bounds.Position.Y
			);

			var bottomBounds = new Rect2I(
				node.Bounds.Position.X,
				splitPos,
				node.Bounds.Size.X,
				node.Bounds.End.Y - splitPos
			);

			node.Left = new BSPNode(topBounds, depth + 1);
			node.Right = new BSPNode(bottomBounds, depth + 1);
		}

		// Recursively partition children
		PartitionSpace(node.Left, depth + 1);
		PartitionSpace(node.Right, depth + 1);
	}

	/// <summary>
	/// Creates rooms in all leaf nodes.
	/// </summary>
	private void CreateRooms(BSPNode node)
	{
		if (node.IsLeaf)
		{
			CreateRoomInLeaf(node);
			return;
		}

		if (node.Left != null)
			CreateRooms(node.Left);
		if (node.Right != null)
			CreateRooms(node.Right);
	}

	/// <summary>
	/// Creates a room within a leaf node.
	/// </summary>
	private void CreateRoomInLeaf(BSPNode leaf)
	{
		// Calculate room size (smaller than partition bounds)
		int roomWidth = _rng.RandiRange(
			Mathf.Min(MinRoomSize, leaf.Bounds.Size.X - RoomPadding * 2),
			Mathf.Min(MaxRoomSize, leaf.Bounds.Size.X - RoomPadding * 2)
		);

		int roomHeight = _rng.RandiRange(
			Mathf.Min(MinRoomSize, leaf.Bounds.Size.Y - RoomPadding * 2),
			Mathf.Min(MaxRoomSize, leaf.Bounds.Size.Y - RoomPadding * 2)
		);

		// Random position within partition (with padding)
		int roomX = _rng.RandiRange(
			leaf.Bounds.Position.X + RoomPadding,
			leaf.Bounds.End.X - roomWidth - RoomPadding
		);

		int roomY = _rng.RandiRange(
			leaf.Bounds.Position.Y + RoomPadding,
			leaf.Bounds.End.Y - roomHeight - RoomPadding
		);

		var roomBounds = new Rect2I(roomX, roomY, roomWidth, roomHeight);
		var room = new DungeonRoom(roomBounds);
		leaf.Room = room;
		_rooms.Add(room);
	}

	/// <summary>
	/// Connects rooms with corridors.
	/// </summary>
	private void ConnectRooms(BSPNode node)
	{
		if (node.IsLeaf)
			return;

		// Recursively connect children
		if (node.Left != null)
			ConnectRooms(node.Left);
		if (node.Right != null)
			ConnectRooms(node.Right);

		// Connect sibling rooms
		if (node.Left != null && node.Right != null)
		{
			var leftRoom = node.Left.GetRoom();
			var rightRoom = node.Right.GetRoom();

			if (leftRoom != null && rightRoom != null)
			{
				CreateCorridor(leftRoom, rightRoom);
			}
		}
	}

	/// <summary>
	/// Creates an L-shaped corridor between two rooms.
	/// </summary>
	private void CreateCorridor(DungeonRoom room1, DungeonRoom room2)
	{
		var corridor = new Corridor(room1, room2);

		Vector2I start = room1.Center;
		Vector2I end = room2.Center;

		// Create L-shaped path
		if (_rng.Randf() > 0.5f)
		{
			// Horizontal then vertical
			for (int x = Mathf.Min(start.X, end.X); x <= Mathf.Max(start.X, end.X); x++)
			{
				corridor.Path.Add(new Vector2I(x, start.Y));
			}
			for (int y = Mathf.Min(start.Y, end.Y); y <= Mathf.Max(start.Y, end.Y); y++)
			{
				corridor.Path.Add(new Vector2I(end.X, y));
			}
		}
		else
		{
			// Vertical then horizontal
			for (int y = Mathf.Min(start.Y, end.Y); y <= Mathf.Max(start.Y, end.Y); y++)
			{
				corridor.Path.Add(new Vector2I(start.X, y));
			}
			for (int x = Mathf.Min(start.X, end.X); x <= Mathf.Max(start.X, end.X); x++)
			{
				corridor.Path.Add(new Vector2I(x, end.Y));
			}
		}

		_corridors.Add(corridor);

		// Mark connection points
		room1.CorridorConnections.Add(start);
		room2.CorridorConnections.Add(end);
	}

	/// <summary>
	/// Assigns special types to rooms.
	/// </summary>
	private void AssignRoomTypes()
	{
		if (_rooms.Count == 0)
			return;

		// First room is the start room
		_rooms[0].Type = RoomType.Start;

		// Last room is potentially a boss room
		if (_rooms.Count > 1)
		{
			_rooms[_rooms.Count - 1].Type = RoomType.Boss;
		}

		// Randomly assign some treasure rooms
		int treasureCount = Mathf.Max(1, _rooms.Count / 5);
		for (int i = 0; i < treasureCount && i < _rooms.Count - 2; i++)
		{
			int idx = _rng.RandiRange(1, _rooms.Count - 2);
			if (_rooms[idx].Type == RoomType.Standard)
			{
				_rooms[idx].Type = RoomType.Treasure;
			}
		}
	}

	/// <summary>
	/// Visualizes the dungeon layout using debug geometry.
	/// </summary>
	private void VisualizeDebug()
	{
		// Clear previous visualization
		foreach (Node child in GetChildren())
		{
			if (child.Name.ToString().StartsWith("Debug"))
			{
				child.QueueFree();
			}
		}

		// Visualize rooms
		foreach (var room in _rooms)
		{
			Color roomColor = room.Type switch
			{
				RoomType.Start => new Color(0, 1, 0, 0.5f),
				RoomType.Boss => new Color(1, 0, 0, 0.5f),
				RoomType.Treasure => new Color(1, 1, 0, 0.5f),
				_ => new Color(0.5f, 0.5f, 0.5f, 0.5f)
			};

			var visual = Core.MeshBuilder.CreatePlane(
				new Vector2(room.Bounds.Size.X, room.Bounds.Size.Y),
				roomColor,
				1
			);
			visual.Name = $"DebugRoom_{room.Type}";
			visual.Position = new Vector3(
				room.Bounds.Position.X + room.Bounds.Size.X / 2f,
				0.1f,
				room.Bounds.Position.Y + room.Bounds.Size.Y / 2f
			);
			AddChild(visual);
		}

		// Visualize corridors
		foreach (var corridor in _corridors)
		{
			foreach (var point in corridor.Path)
			{
				var visual = Core.MeshBuilder.CreateCube(
					new Vector3(1, 0.1f, 1),
					new Color(0, 0, 1, 0.5f)
				);
				visual.Name = "DebugCorridor";
				visual.Position = new Vector3(point.X, 0.05f, point.Y);
				AddChild(visual);
			}
		}

		GD.Print("Debug visualization created");
	}
}
