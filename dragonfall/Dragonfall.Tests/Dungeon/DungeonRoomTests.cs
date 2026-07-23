using GdUnit4;
using Godot;
using Dragonfall.Dungeon;
using Dragonfall.Tests.TestHelpers;
using static GdUnit4.Assertions;

namespace Dragonfall.Tests.Dungeon;

/// <summary>
/// Tests for DungeonRoom class.
/// Tests cover construction, properties, spatial queries (Contains/Intersects), and collection management.
/// </summary>
[TestSuite]
public class DungeonRoomTests
{
	#region Construction Tests

	[TestCase]
	public void Constructor_WithBounds_InitializesCorrectly()
	{
		// Arrange
		var bounds = new Rect2I(10, 20, 30, 40);

		// Act
		var room = new DungeonRoom(bounds);

		// Assert
		AssertThat(room.Bounds).IsEqual(bounds);
		AssertThat(room.Type).IsEqual(RoomType.Standard);
		AssertThat(room.Doors.Count).IsEqual(0);
		AssertThat(room.CorridorConnections.Count).IsEqual(0);
	}

	[TestCase]
	public void Constructor_WithType_SetsTypeCorrectly()
	{
		// Arrange
		var bounds = new Rect2I(0, 0, 10, 10);

		// Act
		var room = new DungeonRoom(bounds, RoomType.Boss);

		// Assert
		AssertThat(room.Type).IsEqual(RoomType.Boss);
	}

	[TestCase]
	public void Constructor_DefaultType_IsStandard()
	{
		// Arrange & Act
		var room = GodotTestHelper.CreateTestRoom();

		// Assert
		AssertThat(room.Type).IsEqual(RoomType.Standard);
	}

	[TestCase]
	public void Constructor_StartType_SetsCorrectly()
	{
		// Arrange & Act
		var room = GodotTestHelper.CreateTestRoom(type: RoomType.Start);

		// Assert
		AssertThat(room.Type).IsEqual(RoomType.Start);
	}

	[TestCase]
	public void Constructor_TreasureType_SetsCorrectly()
	{
		// Arrange & Act
		var room = GodotTestHelper.CreateTestRoom(type: RoomType.Treasure);

		// Assert
		AssertThat(room.Type).IsEqual(RoomType.Treasure);
	}

	[TestCase]
	public void Constructor_EmptyType_SetsCorrectly()
	{
		// Arrange & Act
		var room = GodotTestHelper.CreateTestRoom(type: RoomType.Empty);

		// Assert
		AssertThat(room.Type).IsEqual(RoomType.Empty);
	}

	[TestCase]
	public void Constructor_EmptyCollections_InitializedCorrectly()
	{
		// Arrange & Act
		var room = GodotTestHelper.CreateTestRoom();

		// Assert
		AssertThat(room.Doors).IsNotNull();
		AssertThat(room.CorridorConnections).IsNotNull();
		AssertThat(room.Doors.Count).IsEqual(0);
		AssertThat(room.CorridorConnections.Count).IsEqual(0);
	}

	[TestCase]
	public void Constructor_MultipleRooms_IndependentCollections()
	{
		// Arrange & Act
		var room1 = GodotTestHelper.CreateTestRoom();
		var room2 = GodotTestHelper.CreateTestRoom(10, 10);

		room1.Doors.Add(new Vector2I(5, 5));

		// Assert - room2's doors should still be empty
		AssertThat(room1.Doors.Count).IsEqual(1);
		AssertThat(room2.Doors.Count).IsEqual(0);
	}

	#endregion

	#region Property Tests

	[TestCase]
	public void Center_EvenDimensions_CalculatesCorrectly()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(10, 20, 30, 40);

		// Act
		var center = room.Center;

		// Assert
		AssertThat(center.X).IsEqual(25); // 10 + 30/2
		AssertThat(center.Y).IsEqual(40); // 20 + 40/2
	}

	[TestCase]
	public void Center_OddDimensions_RoundsDown()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(0, 0, 11, 13);

		// Act
		var center = room.Center;

		// Assert
		AssertThat(center.X).IsEqual(5); // 0 + 11/2 = 5 (integer division)
		AssertThat(center.Y).IsEqual(6); // 0 + 13/2 = 6
	}

	[TestCase]
	public void Center_SingleCell_IsSameAsPosition()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(5, 7, 1, 1);

		// Act
		var center = room.Center;

		// Assert
		AssertThat(center.X).IsEqual(5);
		AssertThat(center.Y).IsEqual(7);
	}

	[TestCase]
	public void Center_LargeRoom_CalculatesCorrectly()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(0, 0, 100, 80);

		// Act
		var center = room.Center;

		// Assert
		AssertThat(center.X).IsEqual(50);
		AssertThat(center.Y).IsEqual(40);
	}

	[TestCase]
	public void Area_CalculatesCorrectly()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(0, 0, 10, 20);

		// Act
		var area = room.Area;

		// Assert
		AssertThat(area).IsEqual(200);
	}

	[TestCase]
	public void Area_SingleCell_ReturnsOne()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(0, 0, 1, 1);

		// Act
		var area = room.Area;

		// Assert
		AssertThat(area).IsEqual(1);
	}

	[TestCase]
	public void Area_SquareRoom_CalculatesCorrectly()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(0, 0, 15, 15);

		// Act
		var area = room.Area;

		// Assert
		AssertThat(area).IsEqual(225);
	}

	[TestCase]
	public void Area_LargeRoom_CalculatesCorrectly()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(0, 0, 100, 100);

		// Act
		var area = room.Area;

		// Assert
		AssertThat(area).IsEqual(10000);
	}

	[TestCase]
	public void Area_RectangularRoom_CalculatesCorrectly()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(5, 10, 8, 12);

		// Act
		var area = room.Area;

		// Assert
		AssertThat(area).IsEqual(96);
	}

	#endregion

	#region Contains Tests

	[TestCase]
	public void Contains_PointInside_ReturnsTrue()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var point = new Vector2I(15, 15);

		// Act
		var contains = room.Contains(point);

		// Assert
		AssertThat(contains).IsTrue();
	}

	[TestCase]
	public void Contains_PointOutside_ReturnsFalse()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var point = new Vector2I(5, 5);

		// Act
		var contains = room.Contains(point);

		// Assert
		AssertThat(contains).IsFalse();
	}

	[TestCase]
	public void Contains_PointOnTopBoundary_ReturnsTrue()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var point = new Vector2I(15, 10);

		// Act
		var contains = room.Contains(point);

		// Assert
		AssertThat(contains).IsTrue();
	}

	[TestCase]
	public void Contains_PointOnBottomBoundary_ReturnsFalse()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var point = new Vector2I(15, 30); // End boundary (exclusive)

		// Act
		var contains = room.Contains(point);

		// Assert
		AssertThat(contains).IsFalse();
	}

	[TestCase]
	public void Contains_PointOnLeftBoundary_ReturnsTrue()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var point = new Vector2I(10, 15);

		// Act
		var contains = room.Contains(point);

		// Assert
		AssertThat(contains).IsTrue();
	}

	[TestCase]
	public void Contains_PointOnRightBoundary_ReturnsFalse()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var point = new Vector2I(30, 15); // End boundary (exclusive)

		// Act
		var contains = room.Contains(point);

		// Assert
		AssertThat(contains).IsFalse();
	}

	[TestCase]
	public void Contains_PointAtTopLeftCorner_ReturnsTrue()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var point = new Vector2I(10, 10);

		// Act
		var contains = room.Contains(point);

		// Assert
		AssertThat(contains).IsTrue();
	}

	[TestCase]
	public void Contains_PointAtBottomRightCorner_ReturnsFalse()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var point = new Vector2I(30, 30); // Exclusive end

		// Act
		var contains = room.Contains(point);

		// Assert
		AssertThat(contains).IsFalse();
	}

	[TestCase]
	public void Contains_PointJustOutsideLeft_ReturnsFalse()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var point = new Vector2I(9, 15);

		// Act
		var contains = room.Contains(point);

		// Assert
		AssertThat(contains).IsFalse();
	}

	[TestCase]
	public void Contains_NegativeCoordinates_WorksCorrectly()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(-10, -10, 20, 20);
		var pointInside = new Vector2I(-5, -5);
		var pointOutside = new Vector2I(-15, -15);

		// Act & Assert
		AssertThat(room.Contains(pointInside)).IsTrue();
		AssertThat(room.Contains(pointOutside)).IsFalse();
	}

	[TestCase]
	public void Contains_LargeCoordinates_WorksCorrectly()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(1000, 1000, 100, 100);
		var pointInside = new Vector2I(1050, 1050);
		var pointOutside = new Vector2I(900, 900);

		// Act & Assert
		AssertThat(room.Contains(pointInside)).IsTrue();
		AssertThat(room.Contains(pointOutside)).IsFalse();
	}

	#endregion

	#region Intersects Tests

	[TestCase]
	public void Intersects_OverlappingRooms_ReturnsTrue()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var room2 = GodotTestHelper.CreateTestRoom(15, 15, 20, 20);

		// Act
		var intersects = room1.Intersects(room2);

		// Assert
		AssertThat(intersects).IsTrue();
	}

	[TestCase]
	public void Intersects_NonOverlappingRooms_ReturnsFalse()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var room2 = GodotTestHelper.CreateTestRoom(40, 40, 20, 20);

		// Act
		var intersects = room1.Intersects(room2);

		// Assert
		AssertThat(intersects).IsFalse();
	}

	[TestCase]
	public void Intersects_AdjacentRooms_ReturnsFalse()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var room2 = GodotTestHelper.CreateTestRoom(30, 10, 20, 20); // Touching at x=30

		// Act
		var intersects = room1.Intersects(room2);

		// Assert
		AssertThat(intersects).IsFalse();
	}

	[TestCase]
	public void Intersects_IdenticalRooms_ReturnsTrue()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var room2 = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);

		// Act
		var intersects = room1.Intersects(room2);

		// Assert
		AssertThat(intersects).IsTrue();
	}

	[TestCase]
	public void Intersects_PartialOverlapFromLeft_ReturnsTrue()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var room2 = GodotTestHelper.CreateTestRoom(5, 10, 10, 20); // Overlaps left edge

		// Act
		var intersects = room1.Intersects(room2);

		// Assert
		AssertThat(intersects).IsTrue();
	}

	[TestCase]
	public void Intersects_PartialOverlapFromRight_ReturnsTrue()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var room2 = GodotTestHelper.CreateTestRoom(25, 10, 10, 20); // Overlaps right edge

		// Act
		var intersects = room1.Intersects(room2);

		// Assert
		AssertThat(intersects).IsTrue();
	}

	[TestCase]
	public void Intersects_PartialOverlapFromTop_ReturnsTrue()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var room2 = GodotTestHelper.CreateTestRoom(10, 5, 20, 10); // Overlaps top edge

		// Act
		var intersects = room1.Intersects(room2);

		// Assert
		AssertThat(intersects).IsTrue();
	}

	[TestCase]
	public void Intersects_PartialOverlapFromBottom_ReturnsTrue()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var room2 = GodotTestHelper.CreateTestRoom(10, 25, 20, 10); // Overlaps bottom edge

		// Act
		var intersects = room1.Intersects(room2);

		// Assert
		AssertThat(intersects).IsTrue();
	}

	[TestCase]
	public void Intersects_OneRoomContainsAnother_ReturnsTrue()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(10, 10, 40, 40);
		var room2 = GodotTestHelper.CreateTestRoom(20, 20, 10, 10); // Inside room1

		// Act
		var intersects = room1.Intersects(room2);

		// Assert
		AssertThat(intersects).IsTrue();
	}

	[TestCase]
	public void Intersects_OneRoomInsideAnother_ReturnsTrue()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(20, 20, 10, 10);
		var room2 = GodotTestHelper.CreateTestRoom(10, 10, 40, 40); // Contains room1

		// Act
		var intersects = room1.Intersects(room2);

		// Assert
		AssertThat(intersects).IsTrue();
	}

	[TestCase]
	public void Intersects_CornerToCornerTouching_ReturnsFalse()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var room2 = GodotTestHelper.CreateTestRoom(30, 30, 20, 20); // Corner touching

		// Act
		var intersects = room1.Intersects(room2);

		// Assert
		AssertThat(intersects).IsFalse();
	}

	[TestCase]
	public void Intersects_TinyOverlap_ReturnsTrue()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(10, 10, 20, 20);
		var room2 = GodotTestHelper.CreateTestRoom(29, 29, 20, 20); // 1 pixel overlap

		// Act
		var intersects = room1.Intersects(room2);

		// Assert
		AssertThat(intersects).IsTrue();
	}

	[TestCase]
	public void Intersects_EdgeCaseOnePixelRooms_ReturnsCorrectly()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(10, 10, 1, 1);
		var room2Overlap = GodotTestHelper.CreateTestRoom(10, 10, 1, 1);
		var room2NoOverlap = GodotTestHelper.CreateTestRoom(11, 11, 1, 1);

		// Act & Assert
		AssertThat(room1.Intersects(room2Overlap)).IsTrue();
		AssertThat(room1.Intersects(room2NoOverlap)).IsFalse();
	}

	[TestCase]
	public void Intersects_NegativeCoordinates_WorksCorrectly()
	{
		// Arrange
		var room1 = GodotTestHelper.CreateTestRoom(-20, -20, 20, 20);
		var room2 = GodotTestHelper.CreateTestRoom(-10, -10, 20, 20);

		// Act
		var intersects = room1.Intersects(room2);

		// Assert
		AssertThat(intersects).IsTrue();
	}

	#endregion

	#region Collection Management Tests

	[TestCase]
	public void Doors_InitiallyEmpty()
	{
		// Arrange & Act
		var room = GodotTestHelper.CreateTestRoom();

		// Assert
		AssertThat(room.Doors.Count).IsEqual(0);
	}

	[TestCase]
	public void Doors_CanAddSingleDoor()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom();
		var door = new Vector2I(10, 15);

		// Act
		room.Doors.Add(door);

		// Assert
		AssertThat(room.Doors.Count).IsEqual(1);
		AssertThat(room.Doors[0]).IsEqual(door);
	}

	[TestCase]
	public void Doors_CanAddMultipleDoors()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom();
		var door1 = new Vector2I(10, 15);
		var door2 = new Vector2I(20, 25);
		var door3 = new Vector2I(30, 35);

		// Act
		room.Doors.Add(door1);
		room.Doors.Add(door2);
		room.Doors.Add(door3);

		// Assert
		AssertThat(room.Doors.Count).IsEqual(3);
		AssertThat(room.Doors[0]).IsEqual(door1);
		AssertThat(room.Doors[1]).IsEqual(door2);
		AssertThat(room.Doors[2]).IsEqual(door3);
	}

	[TestCase]
	public void CorridorConnections_InitiallyEmpty()
	{
		// Arrange & Act
		var room = GodotTestHelper.CreateTestRoom();

		// Assert
		AssertThat(room.CorridorConnections.Count).IsEqual(0);
	}

	[TestCase]
	public void CorridorConnections_CanAddSingle()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom();
		var connection = new Vector2I(15, 20);

		// Act
		room.CorridorConnections.Add(connection);

		// Assert
		AssertThat(room.CorridorConnections.Count).IsEqual(1);
		AssertThat(room.CorridorConnections[0]).IsEqual(connection);
	}

	[TestCase]
	public void CorridorConnections_CanAddMultiple()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom();
		var conn1 = new Vector2I(10, 15);
		var conn2 = new Vector2I(20, 25);

		// Act
		room.CorridorConnections.Add(conn1);
		room.CorridorConnections.Add(conn2);

		// Assert
		AssertThat(room.CorridorConnections.Count).IsEqual(2);
	}

	[TestCase]
	public void Collections_MaintainInsertionOrder()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom();
		var items = new[] { new Vector2I(1, 1), new Vector2I(2, 2), new Vector2I(3, 3) };

		// Act
		foreach (var item in items)
		{
			room.CorridorConnections.Add(item);
		}

		// Assert
		for (int i = 0; i < items.Length; i++)
		{
			AssertThat(room.CorridorConnections[i]).IsEqual(items[i]);
		}
	}

	#endregion

	#region ToString Tests

	[TestCase]
	public void ToString_ContainsRoomType()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(type: RoomType.Boss);

		// Act
		var str = room.ToString();

		// Assert
		AssertThat(str).Contains("Boss");
	}

	[TestCase]
	public void ToString_ContainsPosition()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(10, 20, 30, 40);

		// Act
		var str = room.ToString();

		// Assert
		AssertThat(str).Contains("10");
		AssertThat(str).Contains("20");
	}

	[TestCase]
	public void ToString_ContainsSize()
	{
		// Arrange
		var room = GodotTestHelper.CreateTestRoom(10, 20, 30, 40);

		// Act
		var str = room.ToString();

		// Assert
		AssertThat(str).Contains("30");
		AssertThat(str).Contains("40");
	}

	#endregion
}
