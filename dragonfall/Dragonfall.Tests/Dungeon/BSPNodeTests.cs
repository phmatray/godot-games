using GdUnit4;
using Godot;
using Dragonfall.Dungeon;
using Dragonfall.Tests.TestHelpers;
using static GdUnit4.Assertions;

namespace Dragonfall.Tests.Dungeon;

/// <summary>
/// Tests for BSPNode (Binary Space Partitioning Node) class.
/// Tests cover construction, leaf node detection, room retrieval, splitting logic, and edge cases.
/// </summary>
[TestSuite]
public class BSPNodeTests
{
	#region Construction Tests

	[TestCase]
	public void Constructor_WithValidBounds_InitializesCorrectly()
	{
		// Arrange
		var bounds = new Rect2I(10, 20, 50, 60);

		// Act
		var node = new BSPNode(bounds);

		// Assert
		AssertThat(node.Bounds).IsEqual(bounds);
		AssertThat(node.Depth).IsEqual(0);
		AssertThat(node.Left).IsNull();
		AssertThat(node.Right).IsNull();
		AssertThat(node.Room).IsNull();
	}

	[TestCase]
	public void Constructor_WithDepth_SetsDepthCorrectly()
	{
		// Arrange
		var bounds = new Rect2I(0, 0, 100, 100);
		var depth = 3;

		// Act
		var node = new BSPNode(bounds, depth);

		// Assert
		AssertThat(node.Depth).IsEqual(depth);
	}

	[TestCase]
	public void Constructor_DefaultDepth_IsZero()
	{
		// Arrange & Act
		var node = GodotTestHelper.CreateTestBSPNode();

		// Assert
		AssertThat(node.Depth).IsEqual(0);
	}

	[TestCase]
	public void Constructor_WithDifferentBounds_StoresBoundsCorrectly()
	{
		// Arrange
		var bounds1 = new Rect2I(5, 10, 20, 30);
		var bounds2 = new Rect2I(100, 200, 300, 400);

		// Act
		var node1 = new BSPNode(bounds1);
		var node2 = new BSPNode(bounds2);

		// Assert
		AssertThat(node1.Bounds).IsEqual(bounds1);
		AssertThat(node2.Bounds).IsEqual(bounds2);
		AssertThat(node1.Bounds).IsNotEqual(node2.Bounds);
	}

	[TestCase]
	public void Constructor_LargeBounds_HandlesCorrectly()
	{
		// Arrange
		var bounds = new Rect2I(0, 0, 10000, 10000);

		// Act
		var node = new BSPNode(bounds);

		// Assert
		AssertThat(node.Bounds).IsEqual(bounds);
	}

	#endregion

	#region Leaf Node Tests

	[TestCase]
	public void IsLeaf_WithNoChildren_ReturnsTrue()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode();

		// Act
		var isLeaf = node.IsLeaf;

		// Assert
		AssertThat(isLeaf).IsTrue();
	}

	[TestCase]
	public void IsLeaf_WithLeftChild_ReturnsFalse()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode();
		node.Left = GodotTestHelper.CreateTestBSPNode(0, 0, 25, 50, 1);

		// Act
		var isLeaf = node.IsLeaf;

		// Assert
		AssertThat(isLeaf).IsFalse();
	}

	[TestCase]
	public void IsLeaf_WithRightChild_ReturnsFalse()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode();
		node.Right = GodotTestHelper.CreateTestBSPNode(25, 0, 25, 50, 1);

		// Act
		var isLeaf = node.IsLeaf;

		// Assert
		AssertThat(isLeaf).IsFalse();
	}

	[TestCase]
	public void IsLeaf_WithBothChildren_ReturnsFalse()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode();
		node.Left = GodotTestHelper.CreateTestBSPNode(0, 0, 25, 50, 1);
		node.Right = GodotTestHelper.CreateTestBSPNode(25, 0, 25, 50, 1);

		// Act
		var isLeaf = node.IsLeaf;

		// Assert
		AssertThat(isLeaf).IsFalse();
	}

	[TestCase]
	public void IsLeaf_AfterAddingRoom_StillTrue()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode();
		node.Room = GodotTestHelper.CreateTestRoom();

		// Act
		var isLeaf = node.IsLeaf;

		// Assert
		AssertThat(isLeaf).IsTrue();
	}

	[TestCase]
	public void IsLeaf_DeepTree_LeafNodesReturnTrue()
	{
		// Arrange - Create a deep tree
		var root = GodotTestHelper.CreateTestBSPNode(0, 0, 100, 100, 0);
		var level1 = GodotTestHelper.CreateTestBSPNode(0, 0, 50, 100, 1);
		var level2 = GodotTestHelper.CreateTestBSPNode(0, 0, 25, 100, 2);

		root.Left = level1;
		level1.Left = level2;

		// Assert
		AssertThat(root.IsLeaf).IsFalse();
		AssertThat(level1.IsLeaf).IsFalse();
		AssertThat(level2.IsLeaf).IsTrue();
	}

	[TestCase]
	public void IsLeaf_BalancedTree_CorrectlyIdentifiesLeaves()
	{
		// Arrange - Create a balanced tree
		var root = GodotTestHelper.CreateTestBSPNode(0, 0, 100, 100, 0);
		var leftChild = GodotTestHelper.CreateTestBSPNode(0, 0, 50, 100, 1);
		var rightChild = GodotTestHelper.CreateTestBSPNode(50, 0, 50, 100, 1);

		root.Left = leftChild;
		root.Right = rightChild;

		// Assert
		AssertThat(root.IsLeaf).IsFalse();
		AssertThat(leftChild.IsLeaf).IsTrue();
		AssertThat(rightChild.IsLeaf).IsTrue();
	}

	#endregion

	#region GetRoom Tests

	[TestCase]
	public void GetRoom_LeafNodeWithRoom_ReturnsRoom()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode();
		var room = GodotTestHelper.CreateTestRoom();
		node.Room = room;

		// Act
		var result = node.GetRoom();

		// Assert
		AssertThat(result).IsEqual(room);
	}

	[TestCase]
	public void GetRoom_LeafNodeWithoutRoom_ReturnsNull()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode();

		// Act
		var result = node.GetRoom();

		// Assert
		AssertThat(result).IsNull();
	}

	[TestCase]
	public void GetRoom_NonLeafNode_ReturnsFirstRoomInSubtree()
	{
		// Arrange
		var root = GodotTestHelper.CreateTestBSPNode();
		var leftChild = GodotTestHelper.CreateTestBSPNode(0, 0, 25, 50, 1);
		var room = GodotTestHelper.CreateTestRoom();
		leftChild.Room = room;

		root.Left = leftChild;

		// Act
		var result = root.GetRoom();

		// Assert
		AssertThat(result).IsEqual(room);
	}

	[TestCase]
	public void GetRoom_EmptySubtree_ReturnsNull()
	{
		// Arrange
		var root = GodotTestHelper.CreateTestBSPNode();
		root.Left = GodotTestHelper.CreateTestBSPNode(0, 0, 25, 50, 1);
		root.Right = GodotTestHelper.CreateTestBSPNode(25, 0, 25, 50, 1);

		// Act
		var result = root.GetRoom();

		// Assert
		AssertThat(result).IsNull();
	}

	[TestCase]
	public void GetRoom_LeftSubtreeHasRoom_ReturnsLeftRoom()
	{
		// Arrange
		var root = GodotTestHelper.CreateTestBSPNode();
		var leftChild = GodotTestHelper.CreateTestBSPNode(0, 0, 25, 50, 1);
		var rightChild = GodotTestHelper.CreateTestBSPNode(25, 0, 25, 50, 1);
		var leftRoom = GodotTestHelper.CreateTestRoom(0, 0, 10, 10);
		var rightRoom = GodotTestHelper.CreateTestRoom(25, 0, 10, 10);

		leftChild.Room = leftRoom;
		rightChild.Room = rightRoom;
		root.Left = leftChild;
		root.Right = rightChild;

		// Act
		var result = root.GetRoom();

		// Assert - Should prioritize left subtree
		AssertThat(result).IsEqual(leftRoom);
	}

	[TestCase]
	public void GetRoom_OnlyRightSubtreeHasRoom_ReturnsRightRoom()
	{
		// Arrange
		var root = GodotTestHelper.CreateTestBSPNode();
		var leftChild = GodotTestHelper.CreateTestBSPNode(0, 0, 25, 50, 1);
		var rightChild = GodotTestHelper.CreateTestBSPNode(25, 0, 25, 50, 1);
		var rightRoom = GodotTestHelper.CreateTestRoom(25, 0, 10, 10);

		rightChild.Room = rightRoom;
		root.Left = leftChild;
		root.Right = rightChild;

		// Act
		var result = root.GetRoom();

		// Assert
		AssertThat(result).IsEqual(rightRoom);
	}

	[TestCase]
	public void GetRoom_DeepNesting_ReturnsFirstRoom()
	{
		// Arrange - Create deeply nested tree (10 levels)
		var root = GodotTestHelper.CreateTestBSPNode(0, 0, 100, 100, 0);
		var current = root;

		for (int i = 1; i < 10; i++)
		{
			var left = GodotTestHelper.CreateTestBSPNode(0, 0, 50, 100, i);
			current.Left = left;
			current = left;
		}

		// Add room to deepest node
		var deepRoom = GodotTestHelper.CreateTestRoom();
		current.Room = deepRoom;

		// Act
		var result = root.GetRoom();

		// Assert
		AssertThat(result).IsEqual(deepRoom);
	}

	[TestCase]
	public void GetRoom_ComplexTree_ReturnsFirstRoomFound()
	{
		// Arrange - Create complex tree with multiple branches
		var root = GodotTestHelper.CreateTestBSPNode(0, 0, 100, 100, 0);
		var left1 = GodotTestHelper.CreateTestBSPNode(0, 0, 50, 100, 1);
		var right1 = GodotTestHelper.CreateTestBSPNode(50, 0, 50, 100, 1);
		var left2 = GodotTestHelper.CreateTestBSPNode(0, 0, 25, 100, 2);
		var right2 = GodotTestHelper.CreateTestBSPNode(25, 0, 25, 100, 2);

		root.Left = left1;
		root.Right = right1;
		left1.Left = left2;
		left1.Right = right2;

		// Add rooms
		var room1 = GodotTestHelper.CreateTestRoom(0, 0, 10, 10);
		var room2 = GodotTestHelper.CreateTestRoom(25, 0, 10, 10);
		left2.Room = room1;
		right2.Room = room2;

		// Act
		var result = root.GetRoom();

		// Assert - Should return leftmost room
		AssertThat(result).IsEqual(room1);
	}

	[TestCase]
	public void GetRoom_MultipleRooms_ReturnsFirstEncountered()
	{
		// Arrange
		var root = GodotTestHelper.CreateTestBSPNode();
		var left = GodotTestHelper.CreateTestBSPNode(0, 0, 25, 50, 1);
		var right = GodotTestHelper.CreateTestBSPNode(25, 0, 25, 50, 1);

		var room1 = GodotTestHelper.CreateTestRoom(0, 0, 10, 10);
		var room2 = GodotTestHelper.CreateTestRoom(25, 0, 10, 10);

		left.Room = room1;
		right.Room = room2;
		root.Left = left;
		root.Right = right;

		// Act
		var result = root.GetRoom();

		// Assert - Should return left room first
		AssertThat(result).IsEqual(room1);
		AssertThat(result).IsNotEqual(room2);
	}

	#endregion

	#region Split Tests

	[TestCase]
	public void Split_VerticalSplit_CreatesCorrectChildren()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode(0, 0, 100, 100, 0);
		var splitX = 50;
		node.IsVerticalSplit = true;
		node.SplitPosition = new Vector2I(splitX, 0);

		// Act
		node.Left = new BSPNode(new Rect2I(0, 0, splitX, 100), 1);
		node.Right = new BSPNode(new Rect2I(splitX, 0, 100 - splitX, 100), 1);

		// Assert
		AssertThat(node.Left.Bounds.Position.X).IsEqual(0);
		AssertThat(node.Left.Bounds.Size.X).IsEqual(splitX);
		AssertThat(node.Right.Bounds.Position.X).IsEqual(splitX);
		AssertThat(node.Left.Depth).IsEqual(1);
		AssertThat(node.Right.Depth).IsEqual(1);
	}

	[TestCase]
	public void Split_HorizontalSplit_CreatesCorrectChildren()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode(0, 0, 100, 100, 0);
		var splitY = 60;
		node.IsVerticalSplit = false;
		node.SplitPosition = new Vector2I(0, splitY);

		// Act
		node.Left = new BSPNode(new Rect2I(0, 0, 100, splitY), 1);
		node.Right = new BSPNode(new Rect2I(0, splitY, 100, 100 - splitY), 1);

		// Assert
		AssertThat(node.Left.Bounds.Position.Y).IsEqual(0);
		AssertThat(node.Left.Bounds.Size.Y).IsEqual(splitY);
		AssertThat(node.Right.Bounds.Position.Y).IsEqual(splitY);
	}

	[TestCase]
	public void Split_SplitPosition_StoredCorrectly()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode();
		var splitPos = new Vector2I(42, 37);

		// Act
		node.SplitPosition = splitPos;

		// Assert
		AssertThat(node.SplitPosition).IsEqual(splitPos);
	}

	[TestCase]
	public void Split_VerticalFlag_SetCorrectly()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode();

		// Act
		node.IsVerticalSplit = true;

		// Assert
		AssertThat(node.IsVerticalSplit).IsTrue();
	}

	[TestCase]
	public void Split_HorizontalFlag_SetCorrectly()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode();

		// Act
		node.IsVerticalSplit = false;

		// Assert
		AssertThat(node.IsVerticalSplit).IsFalse();
	}

	[TestCase]
	public void Split_ChildrenDepth_Incremented()
	{
		// Arrange
		var root = GodotTestHelper.CreateTestBSPNode(0, 0, 100, 100, 0);

		// Act
		root.Left = new BSPNode(new Rect2I(0, 0, 50, 100), root.Depth + 1);
		root.Right = new BSPNode(new Rect2I(50, 0, 50, 100), root.Depth + 1);

		// Assert
		AssertThat(root.Left.Depth).IsEqual(root.Depth + 1);
		AssertThat(root.Right.Depth).IsEqual(root.Depth + 1);
	}

	#endregion

	#region Edge Cases

	[TestCase]
	public void EdgeCase_SinglePixelBounds_HandlesCorrectly()
	{
		// Arrange & Act
		var node = new BSPNode(new Rect2I(0, 0, 1, 1));

		// Assert
		AssertThat(node.Bounds.Size.X).IsEqual(1);
		AssertThat(node.Bounds.Size.Y).IsEqual(1);
		AssertThat(node.IsLeaf).IsTrue();
	}

	[TestCase]
	public void EdgeCase_VeryLargeBounds_HandlesCorrectly()
	{
		// Arrange & Act
		var node = new BSPNode(new Rect2I(0, 0, 1000, 1000));

		// Assert
		AssertThat(node.Bounds.Size.X).IsEqual(1000);
		AssertThat(node.Bounds.Size.Y).IsEqual(1000);
	}

	[TestCase]
	public void EdgeCase_AsymmetricBounds_HandlesCorrectly()
	{
		// Arrange & Act
		var wideNode = new BSPNode(new Rect2I(0, 0, 200, 20));
		var tallNode = new BSPNode(new Rect2I(0, 0, 20, 200));

		// Assert
		AssertThat(wideNode.Bounds.Size.X).IsEqual(200);
		AssertThat(wideNode.Bounds.Size.Y).IsEqual(20);
		AssertThat(tallNode.Bounds.Size.X).IsEqual(20);
		AssertThat(tallNode.Bounds.Size.Y).IsEqual(200);
	}

	[TestCase]
	public void EdgeCase_NullRoom_HandledGracefully()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode();
		node.Room = null;

		// Act
		var result = node.GetRoom();

		// Assert
		AssertThat(result).IsNull();
	}

	[TestCase]
	public void ToString_LeafNode_FormatsCorrectly()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode(10, 20, 30, 40, 2);

		// Act
		var str = node.ToString();

		// Assert
		AssertThat(str).Contains("Leaf");
		AssertThat(str).Contains("Depth=2");
	}

	[TestCase]
	public void ToString_NonLeafVerticalSplit_FormatsCorrectly()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode(0, 0, 100, 100, 1);
		node.IsVerticalSplit = true;
		node.Left = GodotTestHelper.CreateTestBSPNode(0, 0, 50, 100, 2);

		// Act
		var str = node.ToString();

		// Assert
		AssertThat(str).Contains("VSplit");
		AssertThat(str).Contains("Depth=1");
	}

	[TestCase]
	public void ToString_NonLeafHorizontalSplit_FormatsCorrectly()
	{
		// Arrange
		var node = GodotTestHelper.CreateTestBSPNode(0, 0, 100, 100, 1);
		node.IsVerticalSplit = false;
		node.Left = GodotTestHelper.CreateTestBSPNode(0, 0, 100, 50, 2);

		// Act
		var str = node.ToString();

		// Assert
		AssertThat(str).Contains("HSplit");
		AssertThat(str).Contains("Depth=1");
	}

	#endregion
}
