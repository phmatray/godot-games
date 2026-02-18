using GdUnit4;
using Godot;
using Dragonfall.Dungeon;
using static GdUnit4.Assertions;

namespace Dragonfall.Tests.TestHelpers;

/// <summary>
/// Custom assertion extensions for dungeon validation.
/// </summary>
public static class DungeonAssertions
{
	/// <summary>
	/// Validates that a dungeon generator has created a valid dungeon structure.
	/// </summary>
	public static void ShouldBeValidDungeon(this DungeonGenerator generator)
	{
		var rooms = generator.GetRooms();
		var corridors = generator.GetCorridors();

		AssertThat(rooms.Count).IsGreater(0);

		foreach (var room in rooms)
		{
			AssertThat(room.Bounds.Size.X).IsGreater(0);
			AssertThat(room.Bounds.Size.Y).IsGreater(0);
		}

		// Corridors should connect rooms (n-1 corridors for n rooms in a tree)
		AssertThat(corridors.Count).IsEqual(rooms.Count - 1);
	}

	/// <summary>
	/// Validates that all rooms in a dungeon are connected via corridors.
	/// </summary>
	public static void ShouldBeConnected(this List<DungeonRoom> rooms, List<Corridor> corridors)
	{
		if (rooms.Count <= 1)
			return;

		// Build adjacency graph
		var graph = new Dictionary<DungeonRoom, List<DungeonRoom>>();
		foreach (var room in rooms)
		{
			graph[room] = new List<DungeonRoom>();
		}

		foreach (var corridor in corridors)
		{
			if (corridor.Room1 != null && corridor.Room2 != null)
			{
				graph[corridor.Room1].Add(corridor.Room2);
				graph[corridor.Room2].Add(corridor.Room1);
			}
		}

		// DFS to check connectivity
		var visited = new HashSet<DungeonRoom>();
		Dfs(rooms[0], graph, visited);

		AssertThat(visited.Count).IsEqual(rooms.Count);
	}

	private static void Dfs(DungeonRoom room, Dictionary<DungeonRoom, List<DungeonRoom>> graph, HashSet<DungeonRoom> visited)
	{
		visited.Add(room);
		foreach (var neighbor in graph[room])
		{
			if (!visited.Contains(neighbor))
			{
				Dfs(neighbor, graph, visited);
			}
		}
	}

	/// <summary>
	/// Validates that no rooms overlap with each other.
	/// </summary>
	public static void ShouldNotOverlap(this List<DungeonRoom> rooms)
	{
		for (int i = 0; i < rooms.Count; i++)
		{
			for (int j = i + 1; j < rooms.Count; j++)
			{
				var overlaps = rooms[i].Intersects(rooms[j]);
				AssertThat(overlaps).IsFalse();
			}
		}
	}

	/// <summary>
	/// Validates that all rooms are within the specified bounds.
	/// </summary>
	public static void ShouldBeWithinBounds(this List<DungeonRoom> rooms, int width, int height)
	{
		foreach (var room in rooms)
		{
			AssertThat(room.Bounds.Position.X).IsGreaterEqual(0);
			AssertThat(room.Bounds.Position.Y).IsGreaterEqual(0);
			AssertThat(room.Bounds.End.X).IsLessEqual(width);
			AssertThat(room.Bounds.End.Y).IsLessEqual(height);
		}
	}

	/// <summary>
	/// Validates room size constraints.
	/// </summary>
	public static void ShouldRespectSizeConstraints(this List<DungeonRoom> rooms, int minSize, int maxSize)
	{
		foreach (var room in rooms)
		{
			AssertThat(room.Bounds.Size.X).IsGreaterEqual(minSize);
			AssertThat(room.Bounds.Size.Y).IsGreaterEqual(minSize);
			AssertThat(room.Bounds.Size.X).IsLessEqual(maxSize);
			AssertThat(room.Bounds.Size.Y).IsLessEqual(maxSize);
		}
	}

	/// <summary>
	/// Validates that room type assignments are correct.
	/// </summary>
	public static void ShouldHaveCorrectTypes(this List<DungeonRoom> rooms)
	{
		if (rooms.Count == 0)
			return;

		// First room should be Start
		AssertThat(rooms[0].Type).IsEqual(RoomType.Start);

		// Last room should be Boss (if more than 1 room)
		if (rooms.Count > 1)
		{
			AssertThat(rooms[^1].Type).IsEqual(RoomType.Boss);
		}

		// Should have at least one treasure room if 3+ rooms
		if (rooms.Count >= 3)
		{
			var treasureCount = rooms.Count(r => r.Type == RoomType.Treasure);
			AssertThat(treasureCount).IsGreater(0);
		}
	}

	/// <summary>
	/// Validates corridor paths are non-empty and connect rooms.
	/// </summary>
	public static void ShouldHaveValidPaths(this List<Corridor> corridors)
	{
		foreach (var corridor in corridors)
		{
			AssertThat(corridor.Path.Count).IsGreater(0);
			AssertThat(corridor.Room1).IsNotNull();
			AssertThat(corridor.Room2).IsNotNull();
		}
	}

	/// <summary>
	/// Checks if a point is near a room (within 2 cells).
	/// </summary>
	public static bool IsNear(Vector2I point, DungeonRoom room)
	{
		var distance = Math.Max(
			Math.Max(room.Bounds.Position.X - point.X, point.X - room.Bounds.End.X),
			Math.Max(room.Bounds.Position.Y - point.Y, point.Y - room.Bounds.End.Y)
		);
		return distance <= 2;
	}
}
