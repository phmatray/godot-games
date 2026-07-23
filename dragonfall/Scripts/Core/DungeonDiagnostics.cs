using Godot;
using Dragonfall.Dungeon;

namespace Dragonfall.Core;

/// <summary>
/// Diagnostic script to validate dungeon generation and lighting.
/// </summary>
public partial class DungeonDiagnostics : Node
{
	public static void ValidateDungeon(DungeonLevel dungeonLevel)
	{
		GD.Print("=== DUNGEON DIAGNOSTICS ===");

		// Check if dungeon generator exists
		var generator = dungeonLevel.GetNodeOrNull<DungeonGenerator>("DungeonGenerator");
		if (generator == null)
		{
			GD.PrintErr("FAIL: No DungeonGenerator found!");
			return;
		}
		GD.Print("✓ DungeonGenerator exists");

		// Validate rooms
		var rooms = generator.GetRooms();
		GD.Print($"✓ Rooms generated: {rooms.Count}");
		if (rooms.Count == 0)
		{
			GD.PrintErr("FAIL: No rooms generated!");
			return;
		}

		// Validate corridors
		var corridors = generator.GetCorridors();
		GD.Print($"✓ Corridors generated: {corridors.Count}");

		// Count lights
		int lightCount = CountLights(dungeonLevel);
		GD.Print($"✓ Lights in scene: {lightCount}");
		if (lightCount == 0)
		{
			GD.PrintErr("WARNING: No lights found in scene!");
		}

		// Check DirectionalLight
		var dirLight = FindDirectionalLight(dungeonLevel);
		if (dirLight != null)
		{
			GD.Print($"✓ DirectionalLight energy: {dirLight.LightEnergy}");
		}
		else
		{
			GD.PrintErr("WARNING: No DirectionalLight found!");
		}

		// Check WorldEnvironment
		var worldEnv = FindWorldEnvironment(dungeonLevel);
		if (worldEnv != null && worldEnv.Environment != null)
		{
			GD.Print($"✓ Ambient light energy: {worldEnv.Environment.AmbientLightEnergy}");
		}
		else
		{
			GD.PrintErr("WARNING: No WorldEnvironment found!");
		}

		// Validate player
		var player = dungeonLevel.GetNode<Node>("../Main").FindChild("*Player*", true, false);
		if (player != null)
		{
			GD.Print($"✓ Player found at position: {((Node3D)player).GlobalPosition}");
		}
		else
		{
			GD.PrintErr("FAIL: Player not found!");
		}

		GD.Print("=== DIAGNOSTICS COMPLETE ===");
	}

	private static int CountLights(Node node)
	{
		int count = 0;
		if (node is Light3D)
			count++;

		foreach (Node child in node.GetChildren())
		{
			count += CountLights(child);
		}
		return count;
	}

	private static DirectionalLight3D FindDirectionalLight(Node node)
	{
		if (node is DirectionalLight3D light)
			return light;

		foreach (Node child in node.GetChildren())
		{
			var result = FindDirectionalLight(child);
			if (result != null)
				return result;
		}
		return null;
	}

	private static WorldEnvironment FindWorldEnvironment(Node node)
	{
		if (node is WorldEnvironment env)
			return env;

		foreach (Node child in node.GetChildren())
		{
			var result = FindWorldEnvironment(child);
			if (result != null)
				return result;
		}
		return null;
	}
}
