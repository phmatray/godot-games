namespace Dragonfall.Tests.Fixtures;

/// <summary>
/// Predefined seeds for deterministic testing and regression tests.
/// Each seed represents a specific dungeon configuration that should remain consistent.
/// </summary>
public static class KnownSeeds
{
	/// <summary>
	/// Simple dungeon with basic layout (8-10 rooms expected).
	/// Good for basic functionality testing.
	/// </summary>
	public const int SimpleDungeon = 12345;

	/// <summary>
	/// Complex dungeon with more intricate layout (12-16 rooms expected).
	/// Tests more complex BSP partitioning.
	/// </summary>
	public const int ComplexDungeon = 67890;

	/// <summary>
	/// Minimal dungeon configuration (4-6 rooms expected).
	/// Tests minimum viable dungeon generation.
	/// </summary>
	public const int MinimalRooms = 11111;

	/// <summary>
	/// Large dungeon with many rooms (16-20 rooms expected).
	/// Tests performance and scalability.
	/// </summary>
	public const int MaximalRooms = 99999;

	/// <summary>
	/// Dungeon with tendency for vertical splits.
	/// Tests split direction logic.
	/// </summary>
	public const int VerticalBias = 54321;

	/// <summary>
	/// Dungeon with tendency for horizontal splits.
	/// Tests split direction logic.
	/// </summary>
	public const int HorizontalBias = 98765;

	/// <summary>
	/// Dungeon with balanced layout.
	/// Tests even distribution of rooms.
	/// </summary>
	public const int BalancedLayout = 42424;

	/// <summary>
	/// Dungeon that tends to create smaller rooms.
	/// Tests room size constraints.
	/// </summary>
	public const int SmallRooms = 13579;

	/// <summary>
	/// Dungeon that tends to create larger rooms.
	/// Tests room size constraints.
	/// </summary>
	public const int LargeRooms = 24680;

	/// <summary>
	/// Predictable dungeon for snapshot testing.
	/// Used for detailed validation of generation output.
	/// </summary>
	public const int Regression = 77777;

	/// <summary>
	/// Returns all known seeds for bulk testing.
	/// </summary>
	public static int[] All => new[]
	{
		SimpleDungeon,
		ComplexDungeon,
		MinimalRooms,
		MaximalRooms,
		VerticalBias,
		HorizontalBias,
		BalancedLayout,
		SmallRooms,
		LargeRooms,
		Regression
	};

	/// <summary>
	/// Returns a subset of seeds for quick smoke tests.
	/// </summary>
	public static int[] QuickTest => new[]
	{
		SimpleDungeon,
		ComplexDungeon,
		MinimalRooms
	};
}
