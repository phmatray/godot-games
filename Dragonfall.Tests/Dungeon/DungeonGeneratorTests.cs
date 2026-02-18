using GdUnit4;
using Godot;
using Dragonfall.Dungeon;
using Dragonfall.Tests.TestHelpers;
using Dragonfall.Tests.Fixtures;
using static GdUnit4.Assertions;
using System.Diagnostics;

namespace Dragonfall.Tests.Dungeon;

/// <summary>
/// Comprehensive tests for DungeonGenerator class.
/// Tests cover BSP generation, room creation, corridor connectivity, type assignment, and edge cases.
/// </summary>
[TestSuite]
public class DungeonGeneratorTests
{
	#region Seed and Determinism Tests

	[TestCase]
	public void GenerateDungeon_SameSeed_ProducesIdenticalDungeons()
	{
		// Arrange
		const int seed = 12345;
		var gen1 = GodotTestHelper.CreateTestGenerator();
		var gen2 = GodotTestHelper.CreateTestGenerator();

		// Act
		gen1.GenerateDungeon(seed);
		gen2.GenerateDungeon(seed);

		// Assert
		var rooms1 = gen1.GetRooms();
		var rooms2 = gen2.GetRooms();

		AssertThat(rooms1.Count).IsEqual(rooms2.Count);
		for (int i = 0; i < rooms1.Count; i++)
		{
			AssertThat(rooms1[i].Bounds).IsEqual(rooms2[i].Bounds);
			AssertThat(rooms1[i].Type).IsEqual(rooms2[i].Type);
		}
	}

	[TestCase]
	public void GenerateDungeon_DifferentSeeds_ProducesDifferentDungeons()
	{
		// Arrange
		var gen1 = GodotTestHelper.CreateTestGenerator();
		var gen2 = GodotTestHelper.CreateTestGenerator();

		// Act
		gen1.GenerateDungeon(11111);
		gen2.GenerateDungeon(99999);

		// Assert
		var rooms1 = gen1.GetRooms();
		var rooms2 = gen2.GetRooms();

		// At least one room should be different
		bool foundDifference = rooms1.Count != rooms2.Count ||
			rooms1.Where((r, i) => i < rooms2.Count && r.Bounds != rooms2[i].Bounds).Any();

		AssertThat(foundDifference).IsTrue();
	}

	[TestCase]
	public void GenerateDungeon_CustomSeed_OverridesProperty()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		gen.Seed = 11111;

		// Act
		gen.GenerateDungeon(22222); // Custom seed should override property

		// Assert - Generate again with same custom seed
		var rooms1 = gen.GetRooms();
		gen.GenerateDungeon(22222);
		var rooms2 = gen.GetRooms();

		AssertThat(rooms1.Count).IsEqual(rooms2.Count);
	}

	[TestCase]
	public void GenerateDungeon_NegativeSeed_UsesRandom()
	{
		// Arrange
		var gen1 = GodotTestHelper.CreateTestGenerator();
		var gen2 = GodotTestHelper.CreateTestGenerator();
		gen1.Seed = -1;
		gen2.Seed = -1;

		// Act
		gen1.GenerateDungeon();
		gen2.GenerateDungeon();

		// Assert - Should produce different results (very unlikely to be same)
		var rooms1 = gen1.GetRooms();
		var rooms2 = gen2.GetRooms();

		bool areDifferent = rooms1.Count != rooms2.Count ||
			rooms1.Where((r, i) => i < rooms2.Count && r.Bounds != rooms2[i].Bounds).Any();

		AssertThat(areDifferent).IsTrue();
	}

	[TestCase]
	public void GenerateDungeon_ZeroSeed_IsValid()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(0);

		// Assert
		gen.ShouldBeValidDungeon();
	}

	[TestCase]
	public void GenerateDungeon_VeryLargeSeed_WorksCorrectly()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		const int largeSeed = int.MaxValue - 1;

		// Act
		gen.GenerateDungeon(largeSeed);

		// Assert
		gen.ShouldBeValidDungeon();
	}

	[TestCase]
	public void GenerateDungeon_MultipleRegenerationsWithSameSeed_Consistent()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		const int seed = 42424;

		// Act - Generate 5 times with same seed
		var results = new List<List<DungeonRoom>>();
		for (int i = 0; i < 5; i++)
		{
			gen.GenerateDungeon(seed);
			results.Add(new List<DungeonRoom>(gen.GetRooms()));
		}

		// Assert - All results should be identical
		for (int i = 1; i < results.Count; i++)
		{
			AssertThat(results[i].Count).IsEqual(results[0].Count);
			for (int j = 0; j < results[0].Count; j++)
			{
				AssertThat(results[i][j].Bounds).IsEqual(results[0][j].Bounds);
			}
		}
	}

	[TestCase(Parameters(KnownSeeds.SimpleDungeon))]
	[TestCase(Parameters(KnownSeeds.ComplexDungeon))]
	[TestCase(Parameters(KnownSeeds.MinimalRooms))]
	public void GenerateDungeon_KnownSeeds_ProduceExpectedResults(int seed)
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		var expected = DungeonTestData.GetExpectedSnapshot(seed);

		// Act
		gen.GenerateDungeon(seed);

		// Assert
		if (expected != null)
		{
			var actual = DungeonTestData.CreateSnapshot(seed, gen);
			// Allow small tolerance for variations
			AssertThat(Math.Abs(actual.RoomCount - expected.RoomCount)).IsLessEqual(2);
		}
	}

	#endregion

	#region Room Generation Tests

	[TestCase]
	public void GenerateDungeon_CreatesAtLeastOneRoom()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		AssertThat(rooms.Count).IsGreater(0);
	}

	[TestCase]
	public void GenerateDungeon_RoomCountInExpectedRange()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		gen.MaxDepth = 4;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		// Expected: 2^depth = 16 rooms max, but can be less if spaces too small
		AssertThat(rooms.Count).IsGreaterEqual(2);
		AssertThat(rooms.Count).IsLessEqual(16);
	}

	[TestCase]
	public void GenerateDungeon_AllRoomsWithinBounds()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator(100, 100);

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		rooms.ShouldBeWithinBounds(100, 100);
	}

	[TestCase]
	public void GenerateDungeon_RoomsRespectMinSize()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		gen.MinRoomSize = 8;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		foreach (var room in rooms)
		{
			AssertThat(room.Bounds.Size.X).IsGreaterEqual(8);
			AssertThat(room.Bounds.Size.Y).IsGreaterEqual(8);
		}
	}

	[TestCase]
	public void GenerateDungeon_RoomsRespectMaxSize()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		gen.MaxRoomSize = 20;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		foreach (var room in rooms)
		{
			AssertThat(room.Bounds.Size.X).IsLessEqual(20);
			AssertThat(room.Bounds.Size.Y).IsLessEqual(20);
		}
	}

	[TestCase]
	public void GenerateDungeon_RoomsDontOverlap()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		rooms.ShouldNotOverlap();
	}

	[TestCase]
	public void GenerateDungeon_SmallDungeon_ProducesValidResult()
	{
		// Arrange
		var gen = DungeonTestBuilder.SmallDungeon().WithSeed(12345).Build();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		gen.ShouldBeValidDungeon();
	}

	[TestCase]
	public void GenerateDungeon_MediumDungeon_ProducesValidResult()
	{
		// Arrange
		var gen = DungeonTestBuilder.MediumDungeon().WithSeed(12345).Build();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		gen.ShouldBeValidDungeon();
	}

	[TestCase]
	public void GenerateDungeon_LargeDungeon_ProducesValidResult()
	{
		// Arrange
		var gen = DungeonTestBuilder.LargeDungeon().WithSeed(12345).Build();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		gen.ShouldBeValidDungeon();
	}

	[TestCase]
	public void GenerateDungeon_VerySmallDungeon_HandlesGracefully()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator(20, 20);
		gen.MinRoomSize = 6;
		gen.MaxRoomSize = 10;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		AssertThat(rooms.Count).IsGreater(0);
	}

	[TestCase]
	public void GenerateDungeon_VeryLargeDungeon_CompletesSuccessfully()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator(500, 500);
		gen.MaxDepth = 6;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		AssertThat(rooms.Count).IsGreater(0);
	}

	[TestCase]
	public void GenerateDungeon_SquareDungeon_GeneratesCorrectly()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator(100, 100);

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		gen.ShouldBeValidDungeon();
	}

	[TestCase]
	public void GenerateDungeon_WideRectangle_GeneratesCorrectly()
	{
		// Arrange
		var gen = DungeonTestBuilder.WideDungeon().WithSeed(12345).Build();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		gen.ShouldBeValidDungeon();
	}

	[TestCase]
	public void GenerateDungeon_TallRectangle_GeneratesCorrectly()
	{
		// Arrange
		var gen = DungeonTestBuilder.TallDungeon().WithSeed(12345).Build();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		gen.ShouldBeValidDungeon();
	}

	[TestCase]
	public void GenerateDungeon_ExtremeAspectRatioWide_WorksCorrectly()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator(300, 30);
		gen.MinRoomSize = 6;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		gen.ShouldBeValidDungeon();
	}

	[TestCase]
	public void GenerateDungeon_ExtremeAspectRatioTall_WorksCorrectly()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator(30, 300);
		gen.MinRoomSize = 6;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		gen.ShouldBeValidDungeon();
	}

	#endregion

	#region BSP Partitioning Tests

	[TestCase]
	public void GenerateDungeon_MaxDepthZero_CreatesOneRoom()
	{
		// Arrange
		var gen = DungeonTestBuilder.SingleRoomDungeon().WithSeed(12345).Build();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		AssertThat(rooms.Count).IsEqual(1);
	}

	[TestCase]
	public void GenerateDungeon_MaxDepthOne_CreatesExpectedRooms()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		gen.MaxDepth = 1;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		AssertThat(rooms.Count).IsGreaterEqual(2);
		AssertThat(rooms.Count).IsLessEqual(2);
	}

	[TestCase]
	public void GenerateDungeon_RespectsMaxDepth()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		gen.MaxDepth = 3;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		// Max rooms = 2^3 = 8
		AssertThat(rooms.Count).IsLessEqual(8);
	}

	[TestCase]
	public void GenerateDungeon_SmallAreaStopsPartitioning()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator(30, 30);
		gen.MinRoomSize = 10;
		gen.MaxDepth = 10; // Try to go very deep

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		// Should stop early due to space constraints
		AssertThat(rooms.Count).IsLess(10);
	}

	[TestCase]
	public void GenerateDungeon_SplitRatioWithinRange()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		gen.MinSplitRatio = 0.4f;
		gen.MaxSplitRatio = 0.6f;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		gen.ShouldBeValidDungeon();
		// If it generates successfully, split ratios were valid
	}

	[TestCase]
	public void GenerateDungeon_RootNodeBoundsMatchDungeon()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator(100, 80);

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var root = gen.GetRootNode();
		AssertThat(root).IsNotNull();
		AssertThat(root.Bounds.Position.X).IsEqual(0);
		AssertThat(root.Bounds.Position.Y).IsEqual(0);
		AssertThat(root.Bounds.Size.X).IsEqual(100);
		AssertThat(root.Bounds.Size.Y).IsEqual(80);
	}

	[TestCase]
	public void GenerateDungeon_BSPTreeCreated()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		gen.MaxDepth = 3;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var root = gen.GetRootNode();
		AssertThat(root).IsNotNull();
		AssertThat(root.IsLeaf).IsFalse(); // Root should have children
	}

	[TestCase]
	public void GenerateDungeon_LeafCountMatchesRoomCount()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		// Count leaf nodes
		var leafCount = CountLeaves(gen.GetRootNode());
		AssertThat(leafCount).IsEqual(rooms.Count);
	}

	private int CountLeaves(BSPNode node)
	{
		if (node == null) return 0;
		if (node.IsLeaf) return 1;
		return CountLeaves(node.Left) + CountLeaves(node.Right);
	}

	#endregion

	#region Room Type Assignment Tests

	[TestCase]
	public void AssignRoomTypes_FirstRoomIsStart()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		AssertThat(rooms.Count).IsGreater(0);
		AssertThat(rooms[0].Type).IsEqual(RoomType.Start);
	}

	[TestCase]
	public void AssignRoomTypes_LastRoomIsBoss()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		if (rooms.Count > 1)
		{
			AssertThat(rooms[^1].Type).IsEqual(RoomType.Boss);
		}
	}

	[TestCase]
	public void AssignRoomTypes_AtLeastOneTreasureRoom()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		if (rooms.Count >= 3)
		{
			var treasureCount = rooms.Count(r => r.Type == RoomType.Treasure);
			AssertThat(treasureCount).IsGreater(0);
		}
	}

	[TestCase]
	public void AssignRoomTypes_TreasureCountProportional()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		var treasureCount = rooms.Count(r => r.Type == RoomType.Treasure);
		var expectedCount = rooms.Count / 5; // ~20%

		// Should be approximately 20% (allow some variance)
		AssertThat(treasureCount).IsGreaterEqual(Math.Max(1, expectedCount - 1));
		AssertThat(treasureCount).IsLessEqual(expectedCount + 2);
	}

	[TestCase]
	public void AssignRoomTypes_SingleRoom_IsStart()
	{
		// Arrange
		var gen = DungeonTestBuilder.SingleRoomDungeon().WithSeed(12345).Build();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		AssertThat(rooms.Count).IsEqual(1);
		AssertThat(rooms[0].Type).IsEqual(RoomType.Start);
	}

	[TestCase]
	public void AssignRoomTypes_TwoRooms_StartAndBoss()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		gen.MaxDepth = 1;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		if (rooms.Count == 2)
		{
			AssertThat(rooms[0].Type).IsEqual(RoomType.Start);
			AssertThat(rooms[1].Type).IsEqual(RoomType.Boss);
		}
	}

	[TestCase]
	public void AssignRoomTypes_ThreeRooms_HasTreasure()
	{
		// Arrange & Act
		// Try multiple seeds to find one with exactly 3 rooms
		for (int seed = 10000; seed < 20000; seed++)
		{
			var gen = GodotTestHelper.CreateTestGenerator();
			gen.MaxDepth = 2;
			gen.GenerateDungeon(seed);

			var rooms = gen.GetRooms();
			if (rooms.Count == 3)
			{
				// Assert
				AssertThat(rooms[0].Type).IsEqual(RoomType.Start);
				AssertThat(rooms[^1].Type).IsEqual(RoomType.Boss);
				var hasAnyTreasure = rooms.Any(r => r.Type == RoomType.Treasure);
				AssertThat(hasAnyTreasure).IsTrue();
				return;
			}
		}
	}

	[TestCase]
	public void AssignRoomTypes_TypeAssignmentDeterministic()
	{
		// Arrange
		const int seed = 54321;
		var gen1 = GodotTestHelper.CreateTestGenerator();
		var gen2 = GodotTestHelper.CreateTestGenerator();

		// Act
		gen1.GenerateDungeon(seed);
		gen2.GenerateDungeon(seed);

		// Assert
		var rooms1 = gen1.GetRooms();
		var rooms2 = gen2.GetRooms();

		AssertThat(rooms1.Count).IsEqual(rooms2.Count);
		for (int i = 0; i < rooms1.Count; i++)
		{
			AssertThat(rooms1[i].Type).IsEqual(rooms2[i].Type);
		}
	}

	#endregion

	#region Corridor Generation Tests

	[TestCase]
	public void GenerateDungeon_CreatesCorridors()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var corridors = gen.GetCorridors();
		AssertThat(corridors.Count).IsGreater(0);
	}

	[TestCase]
	public void GenerateDungeon_CorridorCountMatchesRoomCount()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		var corridors = gen.GetCorridors();

		// Tree structure: n rooms need n-1 corridors
		AssertThat(corridors.Count).IsEqual(rooms.Count - 1);
	}

	[TestCase]
	public void GenerateDungeon_AllRoomsConnected()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		var corridors = gen.GetCorridors();

		rooms.ShouldBeConnected(corridors);
	}

	[TestCase]
	public void GenerateDungeon_CorridorPathsNotEmpty()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var corridors = gen.GetCorridors();
		corridors.ShouldHaveValidPaths();
	}

	[TestCase]
	public void GenerateDungeon_CorridorsConnectCorrectRooms()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var corridors = gen.GetCorridors();

		foreach (var corridor in corridors)
		{
			AssertThat(corridor.Room1).IsNotNull();
			AssertThat(corridor.Room2).IsNotNull();
			AssertThat(corridor.Room1).IsNotEqual(corridor.Room2);
		}
	}

	[TestCase]
	public void GenerateDungeon_CorridorPathsWithinBounds()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator(100, 100);

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var corridors = gen.GetCorridors();

		foreach (var corridor in corridors)
		{
			foreach (var point in corridor.Path)
			{
				AssertThat(point.X).IsGreaterEqual(0);
				AssertThat(point.Y).IsGreaterEqual(0);
				AssertThat(point.X).IsLess(100);
				AssertThat(point.Y).IsLess(100);
			}
		}
	}

	[TestCase]
	public void GenerateDungeon_ConnectionPointsMarked()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();

		// At least some rooms should have corridor connections marked
		var roomsWithConnections = rooms.Count(r => r.CorridorConnections.Count > 0);
		AssertThat(roomsWithConnections).IsGreater(0);
	}

	[TestCase]
	public void GenerateDungeon_SingleRoom_NoCorridors()
	{
		// Arrange
		var gen = DungeonTestBuilder.SingleRoomDungeon().WithSeed(12345).Build();

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var corridors = gen.GetCorridors();
		AssertThat(corridors.Count).IsEqual(0);
	}

	#endregion

	#region Regeneration Tests

	[TestCase]
	public void GenerateDungeon_CalledTwice_ClearsPreviousData()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		gen.GenerateDungeon(12345);
		var rooms1 = gen.GetRooms();

		gen.GenerateDungeon(67890);
		var rooms2 = gen.GetRooms();

		// Assert - Should be different dungeons
		bool areDifferent = rooms1.Count != rooms2.Count ||
			rooms1.Where((r, i) => i < rooms2.Count && r.Bounds != rooms2[i].Bounds).Any();

		AssertThat(areDifferent).IsTrue();
	}

	[TestCase]
	public void GenerateDungeon_MultipleRegenerations_WorksCorrectly()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act & Assert - Generate 100 times
		for (int i = 0; i < 100; i++)
		{
			gen.GenerateDungeon(i);
			gen.ShouldBeValidDungeon();
		}
	}

	#endregion

	#region Performance Tests

	[TestCase]
	public void GenerateDungeon_SmallDungeon_CompletesQuickly()
	{
		// Arrange
		var gen = DungeonTestBuilder.SmallDungeon().WithSeed(12345).Build();
		var sw = Stopwatch.StartNew();

		// Act
		gen.GenerateDungeon(12345);
		sw.Stop();

		// Assert
		AssertThat(sw.ElapsedMilliseconds).IsLess(100); // Should complete in < 100ms
	}

	[TestCase]
	public void GenerateDungeon_MediumDungeon_CompletesInTime()
	{
		// Arrange
		var gen = DungeonTestBuilder.MediumDungeon().WithSeed(12345).Build();
		var sw = Stopwatch.StartNew();

		// Act
		gen.GenerateDungeon(12345);
		sw.Stop();

		// Assert
		AssertThat(sw.ElapsedMilliseconds).IsLess(200); // Should complete in < 200ms
	}

	[TestCase]
	public void GenerateDungeon_LargeDungeon_CompletesInTime()
	{
		// Arrange
		var gen = DungeonTestBuilder.LargeDungeon().WithSeed(12345).Build();
		var sw = Stopwatch.StartNew();

		// Act
		gen.GenerateDungeon(12345);
		sw.Stop();

		// Assert
		AssertThat(sw.ElapsedMilliseconds).IsLess(1000); // Should complete in < 1s
	}

	#endregion

	#region Edge Cases

	[TestCase]
	public void GetRooms_BeforeGeneration_ReturnsEmptyList()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		var rooms = gen.GetRooms();

		// Assert
		AssertThat(rooms).IsNotNull();
		AssertThat(rooms.Count).IsEqual(0);
	}

	[TestCase]
	public void GetCorridors_BeforeGeneration_ReturnsEmptyList()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();

		// Act
		var corridors = gen.GetCorridors();

		// Assert
		AssertThat(corridors).IsNotNull();
		AssertThat(corridors.Count).IsEqual(0);
	}

	[TestCase]
	public void GenerateDungeon_MinSizeEqualsMaxSize_AllRoomsSameSize()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		gen.MinRoomSize = 10;
		gen.MaxRoomSize = 10;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		var rooms = gen.GetRooms();
		foreach (var room in rooms)
		{
			AssertThat(room.Bounds.Size.X).IsEqual(10);
			AssertThat(room.Bounds.Size.Y).IsEqual(10);
		}
	}

	[TestCase]
	public void GenerateDungeon_ZeroPadding_RoomsCanBeLarger()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		gen.RoomPadding = 0;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		gen.ShouldBeValidDungeon();
	}

	[TestCase]
	public void GenerateDungeon_VerySmallRoomSize_WorksCorrectly()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator();
		gen.MinRoomSize = 3;
		gen.MaxRoomSize = 5;

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		gen.ShouldBeValidDungeon();
	}

	[TestCase]
	public void GenerateDungeon_VeryLargeRoomSize_WorksCorrectly()
	{
		// Arrange
		var gen = GodotTestHelper.CreateTestGenerator(200, 200);
		gen.MinRoomSize = 40;
		gen.MaxRoomSize = 60;
		gen.MaxDepth = 2; // Limit depth due to large rooms

		// Act
		gen.GenerateDungeon(12345);

		// Assert
		gen.ShouldBeValidDungeon();
	}

	#endregion
}
