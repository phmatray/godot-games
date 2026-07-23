using Godot;

namespace Dragonfall.Dungeon;

/// <summary>
/// Resource class defining the visual appearance of a dungeon.
/// </summary>
[GlobalClass]
public partial class DungeonTheme : Resource
{
	[ExportGroup("Dimensions")]
	[Export] public float WallHeight { get; set; } = 3.0f;
	[Export] public float WallThickness { get; set; } = 0.2f;
	[Export] public float CorridorWidth { get; set; } = 2.0f;

	[ExportGroup("Colors")]
	[Export] public Color FloorColor { get; set; } = new Color(0.3f, 0.3f, 0.3f);
	[Export] public Color WallColor { get; set; } = new Color(0.5f, 0.5f, 0.5f);
	[Export] public Color CeilingColor { get; set; } = new Color(0.2f, 0.2f, 0.2f);
	[Export] public Color CorridorColor { get; set; } = new Color(0.25f, 0.25f, 0.25f);

	/// <summary>
	/// Creates a default dungeon theme.
	/// </summary>
	public static DungeonTheme CreateDefault()
	{
		return new DungeonTheme()
		{
			WallHeight = 3.0f,
			WallThickness = 0.2f,
			CorridorWidth = 2.0f,
			FloorColor = new Color(0.3f, 0.3f, 0.3f),
			WallColor = new Color(0.5f, 0.5f, 0.5f),
			CeilingColor = new Color(0.2f, 0.2f, 0.2f),
			CorridorColor = new Color(0.25f, 0.25f, 0.25f)
		};
	}

	/// <summary>
	/// Creates a crypt-themed dungeon.
	/// </summary>
	public static DungeonTheme CreateCryptTheme()
	{
		return new DungeonTheme()
		{
			WallHeight = 4.0f,
			WallThickness = 0.3f,
			CorridorWidth = 2.5f,
			FloorColor = new Color(0.2f, 0.2f, 0.25f),
			WallColor = new Color(0.35f, 0.35f, 0.4f),
			CeilingColor = new Color(0.15f, 0.15f, 0.2f),
			CorridorColor = new Color(0.18f, 0.18f, 0.22f)
		};
	}
}
