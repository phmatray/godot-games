using Dragonfall.Dungeon;

namespace Dragonfall.Tests.TestHelpers;

/// <summary>
/// Fluent builder for creating test dungeons with specific configurations.
/// </summary>
public class DungeonTestBuilder
{
	private int _width = 100;
	private int _height = 100;
	private int _maxDepth = 4;
	private int _minRoomSize = 8;
	private int _maxRoomSize = 20;
	private int _roomPadding = 2;
	private float _minSplitRatio = 0.4f;
	private float _maxSplitRatio = 0.6f;
	private int? _seed;

	public DungeonTestBuilder WithSize(int width, int height)
	{
		_width = width;
		_height = height;
		return this;
	}

	public DungeonTestBuilder WithWidth(int width)
	{
		_width = width;
		return this;
	}

	public DungeonTestBuilder WithHeight(int height)
	{
		_height = height;
		return this;
	}

	public DungeonTestBuilder WithMaxDepth(int depth)
	{
		_maxDepth = depth;
		return this;
	}

	public DungeonTestBuilder WithRoomSize(int minSize, int maxSize)
	{
		_minRoomSize = minSize;
		_maxRoomSize = maxSize;
		return this;
	}

	public DungeonTestBuilder WithMinRoomSize(int minSize)
	{
		_minRoomSize = minSize;
		return this;
	}

	public DungeonTestBuilder WithMaxRoomSize(int maxSize)
	{
		_maxRoomSize = maxSize;
		return this;
	}

	public DungeonTestBuilder WithPadding(int padding)
	{
		_roomPadding = padding;
		return this;
	}

	public DungeonTestBuilder WithSplitRatio(float minRatio, float maxRatio)
	{
		_minSplitRatio = minRatio;
		_maxSplitRatio = maxRatio;
		return this;
	}

	public DungeonTestBuilder WithSeed(int seed)
	{
		_seed = seed;
		return this;
	}

	/// <summary>
	/// Builds the dungeon generator but doesn't generate the dungeon yet.
	/// </summary>
	public DungeonGenerator Build()
	{
		var generator = new DungeonGenerator
		{
			DungeonWidth = _width,
			DungeonHeight = _height,
			MaxDepth = _maxDepth,
			MinRoomSize = _minRoomSize,
			MaxRoomSize = _maxRoomSize,
			RoomPadding = _roomPadding,
			MinSplitRatio = _minSplitRatio,
			MaxSplitRatio = _maxSplitRatio,
			DebugVisualization = false
		};

		if (_seed.HasValue)
		{
			generator.Seed = _seed.Value;
		}

		return generator;
	}

	/// <summary>
	/// Builds and generates the dungeon in one step.
	/// </summary>
	public DungeonGenerator BuildAndGenerate()
	{
		var generator = Build();
		generator.GenerateDungeon(_seed);
		return generator;
	}

	/// <summary>
	/// Creates a builder with small dungeon configuration (50x50).
	/// </summary>
	public static DungeonTestBuilder SmallDungeon()
	{
		return new DungeonTestBuilder()
			.WithSize(50, 50)
			.WithMaxDepth(2)
			.WithRoomSize(6, 12);
	}

	/// <summary>
	/// Creates a builder with medium dungeon configuration (100x100).
	/// </summary>
	public static DungeonTestBuilder MediumDungeon()
	{
		return new DungeonTestBuilder()
			.WithSize(100, 100)
			.WithMaxDepth(4)
			.WithRoomSize(8, 20);
	}

	/// <summary>
	/// Creates a builder with large dungeon configuration (200x200).
	/// </summary>
	public static DungeonTestBuilder LargeDungeon()
	{
		return new DungeonTestBuilder()
			.WithSize(200, 200)
			.WithMaxDepth(5)
			.WithRoomSize(10, 30);
	}

	/// <summary>
	/// Creates a builder for a single-room dungeon (depth 0).
	/// </summary>
	public static DungeonTestBuilder SingleRoomDungeon()
	{
		return new DungeonTestBuilder()
			.WithSize(50, 50)
			.WithMaxDepth(0);
	}

	/// <summary>
	/// Creates a builder with extreme aspect ratio (wide).
	/// </summary>
	public static DungeonTestBuilder WideDungeon()
	{
		return new DungeonTestBuilder()
			.WithSize(200, 50)
			.WithMaxDepth(3);
	}

	/// <summary>
	/// Creates a builder with extreme aspect ratio (tall).
	/// </summary>
	public static DungeonTestBuilder TallDungeon()
	{
		return new DungeonTestBuilder()
			.WithSize(50, 200)
			.WithMaxDepth(3);
	}
}
