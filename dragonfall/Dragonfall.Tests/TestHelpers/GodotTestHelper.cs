using Godot;
using Dragonfall.Dungeon;

namespace Dragonfall.Tests.TestHelpers;

/// <summary>
/// Helper class for creating test instances and common test setup.
/// </summary>
public static class GodotTestHelper
{
	/// <summary>
	/// Creates a test room with specified bounds.
	/// </summary>
	public static DungeonRoom CreateTestRoom(
		int x = 0,
		int y = 0,
		int width = 10,
		int height = 10,
		RoomType type = RoomType.Standard)
	{
		return new DungeonRoom(new Rect2I(x, y, width, height), type);
	}

	/// <summary>
	/// Creates a test BSP node with specified bounds and depth.
	/// </summary>
	public static BSPNode CreateTestBSPNode(
		int x = 0,
		int y = 0,
		int width = 50,
		int height = 50,
		int depth = 0)
	{
		return new BSPNode(new Rect2I(x, y, width, height), depth);
	}

	/// <summary>
	/// Creates a test dungeon generator with default configuration.
	/// </summary>
	public static DungeonGenerator CreateTestGenerator(
		int width = 100,
		int height = 100,
		int maxDepth = 4,
		int minRoomSize = 8,
		int maxRoomSize = 20,
		int roomPadding = 2)
	{
		var generator = new DungeonGenerator
		{
			DungeonWidth = width,
			DungeonHeight = height,
			MaxDepth = maxDepth,
			MinRoomSize = minRoomSize,
			MaxRoomSize = maxRoomSize,
			RoomPadding = roomPadding,
			DebugVisualization = false
		};
		return generator;
	}

	/// <summary>
	/// Creates a test corridor connecting two rooms.
	/// </summary>
	public static Corridor CreateTestCorridor(DungeonRoom room1, DungeonRoom room2)
	{
		return new Corridor(room1, room2);
	}

	/// <summary>
	/// Creates a simple L-shaped corridor path between two points.
	/// </summary>
	public static List<Vector2I> CreateLShapedPath(Vector2I start, Vector2I end, bool horizontalFirst = true)
	{
		var path = new List<Vector2I>();

		if (horizontalFirst)
		{
			// Move horizontally first
			int x = start.X;
			while (x != end.X)
			{
				path.Add(new Vector2I(x, start.Y));
				x += x < end.X ? 1 : -1;
			}

			// Then vertically
			int y = start.Y;
			while (y != end.Y)
			{
				path.Add(new Vector2I(end.X, y));
				y += y < end.Y ? 1 : -1;
			}
		}
		else
		{
			// Move vertically first
			int y = start.Y;
			while (y != end.Y)
			{
				path.Add(new Vector2I(start.X, y));
				y += y < end.Y ? 1 : -1;
			}

			// Then horizontally
			int x = start.X;
			while (x != end.X)
			{
				path.Add(new Vector2I(x, end.Y));
				x += x < end.X ? 1 : -1;
			}
		}

		path.Add(end);
		return path;
	}
}
