using Godot;
using System.Collections.Generic;

namespace Dragonfall.Dungeon;

/// <summary>
/// Types of rooms in the dungeon.
/// </summary>
public enum RoomType
{
	Standard,
	Start,
	Boss,
	Treasure,
	Empty
}

/// <summary>
/// Represents a room in the dungeon with its bounds, type, and connections.
/// </summary>
public class DungeonRoom
{
	public Rect2I Bounds { get; set; }
	public RoomType Type { get; set; }
	public List<Vector2I> Doors { get; set; } = new();
	public List<Vector2I> CorridorConnections { get; set; } = new();

	/// <summary>
	/// Gets the center position of the room.
	/// </summary>
	public Vector2I Center => new Vector2I(
		Bounds.Position.X + Bounds.Size.X / 2,
		Bounds.Position.Y + Bounds.Size.Y / 2
	);

	/// <summary>
	/// Gets the area of the room in grid cells.
	/// </summary>
	public int Area => Bounds.Size.X * Bounds.Size.Y;

	public DungeonRoom(Rect2I bounds, RoomType type = RoomType.Standard)
	{
		Bounds = bounds;
		Type = type;
	}

	/// <summary>
	/// Checks if a point is inside the room.
	/// </summary>
	public bool Contains(Vector2I point)
	{
		return Bounds.HasPoint(point);
	}

	/// <summary>
	/// Checks if this room intersects with another room.
	/// </summary>
	public bool Intersects(DungeonRoom other)
	{
		return Bounds.Intersects(other.Bounds);
	}

	public override string ToString()
	{
		return $"Room({Type}) at {Bounds.Position}, size {Bounds.Size}";
	}
}
