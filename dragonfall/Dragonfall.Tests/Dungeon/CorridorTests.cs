using GdUnit4;
using Godot;
using Dragonfall.Dungeon;
using Dragonfall.Tests.TestHelpers;
using static GdUnit4.Assertions;

namespace Dragonfall.Tests.Dungeon;

/// <summary>
/// Tests for Corridor class.
/// Tests cover construction, path management, and room references.
/// </summary>
[TestSuite]
public class CorridorTests
{
	#region Construction Tests

	[TestCase]
	public void Constructor_WithValidRooms_InitializesCorrectly()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(0, 0, 10, 10);
		var room2 = GodotTestHelper.CreateTestRoom(20, 20, 10, 10);

		// Act
		var corridor = new Corridor(room1, room2);

		// Assert
		AssertThat(corridor.Room1).IsEqual(room1);
		AssertThat(corridor.Room2).IsEqual(room2);
		AssertThat(corridor.Path).IsNotNull();
		AssertThat(corridor.Path.Count).IsEqual(0);
	}

	[TestCase]
	public void Constructor_Room1_StoredCorrectly()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(5, 5, 10, 10);
		var room2 = GodotTestHelper.CreateTestRoom(25, 25, 10, 10);

		// Act
		var corridor = new Corridor(room1, room2);

		// Assert
		AssertThat(corridor.Room1).IsEqual(room1);
		AssertThat(corridor.Room1).IsNotEqual(room2);
	}

	[TestCase]
	public void Constructor_Room2_StoredCorrectly()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(5, 5, 10, 10);
		var room2 = GodotTestHelper.CreateTestRoom(25, 25, 10, 10);

		// Act
		var corridor = new Corridor(room1, room2);

		// Assert
		AssertThat(corridor.Room2).IsEqual(room2);
		AssertThat(corridor.Room2).IsNotEqual(room1);
	}

	[TestCase]
	public void Constructor_Path_InitiallyEmpty()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom();
		var room2 = GodotTestHelper.CreateTestRoom(20, 0);

		// Act
		var corridor = new Corridor(room1, room2);

		// Assert
		AssertThat(corridor.Path.Count).IsEqual(0);
	}

	[TestCase]
	public void Constructor_DifferentRoomTypes_WorksCorrectly()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(type: RoomType.Start);
		var room2 = GodotTestHelper.CreateTestRoom(20, 0, type: RoomType.Boss);

		// Act
		var corridor = new Corridor(room1, room2);

		// Assert
		AssertThat(corridor.Room1.Type).IsEqual(RoomType.Start);
		AssertThat(corridor.Room2.Type).IsEqual(RoomType.Boss);
	}

	#endregion

	#region Path Management Tests

	[TestCase]
	public void Path_CanAddSinglePoint()
	{
		// Arrange
		var corridor = GodotTestHelper.CreateTestCorridor(
			GodotTestHelper.CreateTestRoom(),
			GodotTestHelper.CreateTestRoom(20, 0)
		);
		var point = new Vector2I(10, 10);

		// Act
		corridor.Path.Add(point);

		// Assert
		AssertThat(corridor.Path.Count).IsEqual(1);
		AssertThat(corridor.Path[0]).IsEqual(point);
	}

	[TestCase]
	public void Path_CanAddMultiplePoints()
	{
		// Arrange
		var corridor = GodotTestHelper.CreateTestCorridor(
			GodotTestHelper.CreateTestRoom(),
			GodotTestHelper.CreateTestRoom(20, 0)
		);
		var points = new[]
		{
			new Vector2I(5, 5),
			new Vector2I(10, 5),
			new Vector2I(15, 5),
			new Vector2I(15, 10)
		};

		// Act
		foreach (var point in points)
		{
			corridor.Path.Add(point);
		}

		// Assert
		AssertThat(corridor.Path.Count).IsEqual(4);
	}

	[TestCase]
	public void Path_MaintainsInsertionOrder()
	{
		// Arrange
		var corridor = GodotTestHelper.CreateTestCorridor(
			GodotTestHelper.CreateTestRoom(),
			GodotTestHelper.CreateTestRoom(20, 0)
		);
		var points = new[]
		{
			new Vector2I(1, 1),
			new Vector2I(2, 2),
			new Vector2I(3, 3),
			new Vector2I(4, 4),
			new Vector2I(5, 5)
		};

		// Act
		foreach (var point in points)
		{
			corridor.Path.Add(point);
		}

		// Assert
		for (int i = 0; i < points.Length; i++)
		{
			AssertThat(corridor.Path[i]).IsEqual(points[i]);
		}
	}

	[TestCase]
	public void Path_CanAddDuplicatePoints()
	{
		// Arrange
		var corridor = GodotTestHelper.CreateTestCorridor(
			GodotTestHelper.CreateTestRoom(),
			GodotTestHelper.CreateTestRoom(20, 0)
		);
		var point = new Vector2I(10, 10);

		// Act
		corridor.Path.Add(point);
		corridor.Path.Add(point);
		corridor.Path.Add(point);

		// Assert
		AssertThat(corridor.Path.Count).IsEqual(3);
		AssertThat(corridor.Path[0]).IsEqual(point);
		AssertThat(corridor.Path[1]).IsEqual(point);
		AssertThat(corridor.Path[2]).IsEqual(point);
	}

	[TestCase]
	public void Path_LargePath_HandlesCorrectly()
	{
		// Arrange
		var corridor = GodotTestHelper.CreateTestCorridor(
			GodotTestHelper.CreateTestRoom(),
			GodotTestHelper.CreateTestRoom(100, 100)
		);

		// Act - Create a long path with 1000 points
		for (int i = 0; i < 1000; i++)
		{
			corridor.Path.Add(new Vector2I(i, i));
		}

		// Assert
		AssertThat(corridor.Path.Count).IsEqual(1000);
		AssertThat(corridor.Path[0]).IsEqual(new Vector2I(0, 0));
		AssertThat(corridor.Path[999]).IsEqual(new Vector2I(999, 999));
	}

	[TestCase]
	public void Path_CanAccessByIndex()
	{
		// Arrange
		var corridor = GodotTestHelper.CreateTestCorridor(
			GodotTestHelper.CreateTestRoom(),
			GodotTestHelper.CreateTestRoom(20, 0)
		);
		var points = new[]
		{
			new Vector2I(5, 5),
			new Vector2I(10, 10),
			new Vector2I(15, 15)
		};

		foreach (var point in points)
		{
			corridor.Path.Add(point);
		}

		// Act & Assert
		AssertThat(corridor.Path[0]).IsEqual(points[0]);
		AssertThat(corridor.Path[1]).IsEqual(points[1]);
		AssertThat(corridor.Path[2]).IsEqual(points[2]);
	}

	[TestCase]
	public void Path_CanEnumerate()
	{
		// Arrange
		var corridor = GodotTestHelper.CreateTestCorridor(
			GodotTestHelper.CreateTestRoom(),
			GodotTestHelper.CreateTestRoom(20, 0)
		);
		var points = new[]
		{
			new Vector2I(1, 1),
			new Vector2I(2, 2),
			new Vector2I(3, 3)
		};

		foreach (var point in points)
		{
			corridor.Path.Add(point);
		}

		// Act
		var enumeratedPoints = new List<Vector2I>();
		foreach (var point in corridor.Path)
		{
			enumeratedPoints.Add(point);
		}

		// Assert
		AssertThat(enumeratedPoints.Count).IsEqual(points.Length);
		for (int i = 0; i < points.Length; i++)
		{
			AssertThat(enumeratedPoints[i]).IsEqual(points[i]);
		}
	}

	[TestCase]
	public void Path_WithNegativeCoordinates_WorksCorrectly()
	{
		// Arrange
		var corridor = GodotTestHelper.CreateTestCorridor(
			GodotTestHelper.CreateTestRoom(-20, -20, 10, 10),
			GodotTestHelper.CreateTestRoom(10, 10, 10, 10)
		);
		var points = new[]
		{
			new Vector2I(-10, -10),
			new Vector2I(0, 0),
			new Vector2I(10, 10)
		};

		// Act
		foreach (var point in points)
		{
			corridor.Path.Add(point);
		}

		// Assert
		AssertThat(corridor.Path.Count).IsEqual(3);
		AssertThat(corridor.Path[0]).IsEqual(points[0]);
		AssertThat(corridor.Path[1]).IsEqual(points[1]);
		AssertThat(corridor.Path[2]).IsEqual(points[2]);
	}

	[TestCase]
	public void Path_LShapedPath_CanBeCreated()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(0, 0, 10, 10);
		var room2 = GodotTestHelper.CreateTestRoom(30, 30, 10, 10);
		var corridor = new Corridor(room1, room2);

		// Act - Create L-shaped path using helper
		var path = GodotTestHelper.CreateLShapedPath(
			room1.Center,
			room2.Center,
			horizontalFirst: true
		);
		corridor.Path.AddRange(path);

		// Assert
		AssertThat(corridor.Path.Count).IsGreater(0);
		// Path should start near room1 and end near room2
		var start = corridor.Path[0];
		var end = corridor.Path[^1];
		AssertThat(start.X).IsEqual(room1.Center.X);
		AssertThat(end.X).IsEqual(room2.Center.X);
		AssertThat(end.Y).IsEqual(room2.Center.Y);
	}

	#endregion

	#region Edge Cases

	[TestCase]
	public void Path_EmptyPath_IsValid()
	{
		// Arrange
		var corridor = GodotTestHelper.CreateTestCorridor(
			GodotTestHelper.CreateTestRoom(),
			GodotTestHelper.CreateTestRoom(20, 0)
		);

		// Assert
		AssertThat(corridor.Path).IsNotNull();
		AssertThat(corridor.Path.Count).IsEqual(0);
	}

	[TestCase]
	public void Path_SinglePointPath_IsValid()
	{
		// Arrange
		var corridor = GodotTestHelper.CreateTestCorridor(
			GodotTestHelper.CreateTestRoom(),
			GodotTestHelper.CreateTestRoom(20, 0)
		);
		var point = new Vector2I(10, 10);

		// Act
		corridor.Path.Add(point);

		// Assert
		AssertThat(corridor.Path.Count).IsEqual(1);
	}

	#endregion
}
