using Godot;

namespace Dragonfall.Dungeon;

/// <summary>
/// Binary Space Partitioning tree node for dungeon generation.
/// </summary>
public class BSPNode
{
	/// <summary>
	/// The rectangular area this node occupies.
	/// </summary>
	public Rect2I Bounds { get; set; }

	/// <summary>
	/// Left child node (or null if this is a leaf).
	/// </summary>
	public BSPNode Left { get; set; }

	/// <summary>
	/// Right child node (or null if this is a leaf).
	/// </summary>
	public BSPNode Right { get; set; }

	/// <summary>
	/// The room created in this node (only valid for leaf nodes).
	/// </summary>
	public DungeonRoom Room { get; set; }

	/// <summary>
	/// Position where the split occurred (in grid coordinates).
	/// </summary>
	public Vector2I SplitPosition { get; set; }

	/// <summary>
	/// Whether the split was vertical (true) or horizontal (false).
	/// </summary>
	public bool IsVerticalSplit { get; set; }

	/// <summary>
	/// Checks if this node is a leaf (has no children).
	/// </summary>
	public bool IsLeaf => Left == null && Right == null;

	/// <summary>
	/// Depth of this node in the tree.
	/// </summary>
	public int Depth { get; set; }

	public BSPNode(Rect2I bounds, int depth = 0)
	{
		Bounds = bounds;
		Depth = depth;
	}

	/// <summary>
	/// Gets the room from this node (recursively searches if not a leaf).
	/// Returns the first room found in the subtree.
	/// </summary>
	public DungeonRoom GetRoom()
	{
		if (Room != null)
			return Room;

		if (Left != null)
		{
			var leftRoom = Left.GetRoom();
			if (leftRoom != null)
				return leftRoom;
		}

		if (Right != null)
		{
			return Right.GetRoom();
		}

		return null;
	}

	public override string ToString()
	{
		string type = IsLeaf ? "Leaf" : (IsVerticalSplit ? "VSplit" : "HSplit");
		return $"BSPNode({type}, Depth={Depth}, Bounds={Bounds.Position}->{Bounds.End})";
	}
}
