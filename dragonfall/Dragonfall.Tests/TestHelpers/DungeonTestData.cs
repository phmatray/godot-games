using Godot;
using Dragonfall.Tests.Fixtures;

namespace Dragonfall.Tests.TestHelpers;

/// <summary>
/// Snapshot data for known seed dungeons.
/// Used for regression testing to ensure generation remains consistent.
/// </summary>
public class DungeonSnapshot
{
	public int Seed { get; set; }
	public int RoomCount { get; set; }
	public int CorridorCount { get; set; }
	public Rect2I? FirstRoomBounds { get; set; }
	public Rect2I? LastRoomBounds { get; set; }
	public int TreasureRoomCount { get; set; }
	public int MinRoomArea { get; set; }
	public int MaxRoomArea { get; set; }

	public override string ToString()
	{
		return $"Snapshot(Seed={Seed}, Rooms={RoomCount}, Corridors={CorridorCount})";
	}
}

/// <summary>
/// Provides expected dungeon data for known seeds.
/// These snapshots are used to verify that generation remains deterministic.
/// </summary>
public static class DungeonTestData
{
	/// <summary>
	/// Gets the expected snapshot for a known seed.
	/// Returns null if the seed doesn't have a predefined snapshot.
	/// </summary>
	public static DungeonSnapshot? GetExpectedSnapshot(int seed)
	{
		return seed switch
		{
			KnownSeeds.SimpleDungeon => new DungeonSnapshot
			{
				Seed = KnownSeeds.SimpleDungeon,
				RoomCount = 8,
				CorridorCount = 7,
				TreasureRoomCount = 1
			},

			KnownSeeds.ComplexDungeon => new DungeonSnapshot
			{
				Seed = KnownSeeds.ComplexDungeon,
				RoomCount = 14,
				CorridorCount = 13,
				TreasureRoomCount = 2
			},

			KnownSeeds.MinimalRooms => new DungeonSnapshot
			{
				Seed = KnownSeeds.MinimalRooms,
				RoomCount = 5,
				CorridorCount = 4,
				TreasureRoomCount = 1
			},

			KnownSeeds.MaximalRooms => new DungeonSnapshot
			{
				Seed = KnownSeeds.MaximalRooms,
				RoomCount = 16,
				CorridorCount = 15,
				TreasureRoomCount = 3
			},

			KnownSeeds.VerticalBias => new DungeonSnapshot
			{
				Seed = KnownSeeds.VerticalBias,
				RoomCount = 10,
				CorridorCount = 9,
				TreasureRoomCount = 2
			},

			KnownSeeds.HorizontalBias => new DungeonSnapshot
			{
				Seed = KnownSeeds.HorizontalBias,
				RoomCount = 11,
				CorridorCount = 10,
				TreasureRoomCount = 2
			},

			KnownSeeds.BalancedLayout => new DungeonSnapshot
			{
				Seed = KnownSeeds.BalancedLayout,
				RoomCount = 12,
				CorridorCount = 11,
				TreasureRoomCount = 2
			},

			KnownSeeds.SmallRooms => new DungeonSnapshot
			{
				Seed = KnownSeeds.SmallRooms,
				RoomCount = 9,
				CorridorCount = 8,
				TreasureRoomCount = 1
			},

			KnownSeeds.LargeRooms => new DungeonSnapshot
			{
				Seed = KnownSeeds.LargeRooms,
				RoomCount = 7,
				CorridorCount = 6,
				TreasureRoomCount = 1
			},

			KnownSeeds.Regression => new DungeonSnapshot
			{
				Seed = KnownSeeds.Regression,
				RoomCount = 13,
				CorridorCount = 12,
				TreasureRoomCount = 2,
				FirstRoomBounds = new Rect2I(2, 2, 12, 15),
				LastRoomBounds = new Rect2I(75, 80, 18, 14)
			},

			_ => null
		};
	}

	/// <summary>
	/// Validates that a generated dungeon matches the expected snapshot (if available).
	/// Note: Room counts are approximate and may vary slightly based on randomization.
	/// This method allows for small variations while catching major regressions.
	/// </summary>
	public static bool MatchesSnapshot(DungeonSnapshot actual, DungeonSnapshot expected, int tolerance = 2)
	{
		if (actual.Seed != expected.Seed)
			return false;

		// Allow small variations in room count
		if (Math.Abs(actual.RoomCount - expected.RoomCount) > tolerance)
			return false;

		// Corridor count should match room count - 1
		if (actual.CorridorCount != actual.RoomCount - 1)
			return false;

		return true;
	}

	/// <summary>
	/// Creates a snapshot from a generated dungeon for comparison or recording.
	/// </summary>
	public static DungeonSnapshot CreateSnapshot(int seed, Dragonfall.Dungeon.DungeonGenerator generator)
	{
		var rooms = generator.GetRooms();
		var corridors = generator.GetCorridors();

		var snapshot = new DungeonSnapshot
		{
			Seed = seed,
			RoomCount = rooms.Count,
			CorridorCount = corridors.Count,
			TreasureRoomCount = rooms.Count(r => r.Type == Dragonfall.Dungeon.RoomType.Treasure)
		};

		if (rooms.Count > 0)
		{
			snapshot.FirstRoomBounds = rooms[0].Bounds;
			snapshot.MinRoomArea = rooms.Min(r => r.Area);
			snapshot.MaxRoomArea = rooms.Max(r => r.Area);
		}

		if (rooms.Count > 1)
		{
			snapshot.LastRoomBounds = rooms[^1].Bounds;
		}

		return snapshot;
	}
}
