using Godot;
using System.Collections.Generic;

namespace Dragonfall.Core;

/// <summary>
/// Utility class for creating procedural 3D meshes using Godot primitives and SurfaceTool.
/// </summary>
public static class MeshBuilder
{
	/// <summary>
	/// Creates a cube mesh with specified size and color.
	/// </summary>
	public static MeshInstance3D CreateCube(Vector3 size, Color color)
	{
		var meshInstance = new MeshInstance3D();
		var boxMesh = new BoxMesh();
		boxMesh.Size = size;

		meshInstance.Mesh = boxMesh;

		// Create material with color
		var material = new StandardMaterial3D();
		material.AlbedoColor = color;
		// Use default shading (PerPixel) so materials respond to lights

		meshInstance.SetSurfaceOverrideMaterial(0, material);

		return meshInstance;
	}

	/// <summary>
	/// Creates a quad (flat rectangle) mesh.
	/// </summary>
	public static MeshInstance3D CreateQuad(Vector2 size, Color color, bool doubleSided = false)
	{
		var meshInstance = new MeshInstance3D();
		var quadMesh = new QuadMesh();
		quadMesh.Size = size;

		meshInstance.Mesh = quadMesh;

		// Create material
		var material = new StandardMaterial3D();
		material.AlbedoColor = color;
		// Use default shading so materials respond to lights

		if (doubleSided)
		{
			material.CullMode = BaseMaterial3D.CullModeEnum.Disabled;
		}

		meshInstance.SetSurfaceOverrideMaterial(0, material);

		return meshInstance;
	}

	/// <summary>
	/// Creates a cylinder mesh with specified radius, height, and color.
	/// </summary>
	public static MeshInstance3D CreateCylinder(float radius, float height, Color color)
	{
		var meshInstance = new MeshInstance3D();
		var cylinderMesh = new CylinderMesh();
		cylinderMesh.TopRadius = radius;
		cylinderMesh.BottomRadius = radius;
		cylinderMesh.Height = height;

		meshInstance.Mesh = cylinderMesh;

		// Create material
		var material = new StandardMaterial3D();
		material.AlbedoColor = color;
		// Use default shading so materials respond to lights

		meshInstance.SetSurfaceOverrideMaterial(0, material);

		return meshInstance;
	}

	/// <summary>
	/// Creates a sphere mesh with specified radius and color.
	/// </summary>
	public static MeshInstance3D CreateSphere(float radius, Color color)
	{
		var meshInstance = new MeshInstance3D();
		var sphereMesh = new SphereMesh();
		sphereMesh.Radius = radius;
		sphereMesh.Height = radius * 2f;

		meshInstance.Mesh = sphereMesh;

		// Create material
		var material = new StandardMaterial3D();
		material.AlbedoColor = color;
		// Use default shading so materials respond to lights

		meshInstance.SetSurfaceOverrideMaterial(0, material);

		return meshInstance;
	}

	/// <summary>
	/// Creates a custom mesh from vertex data using SurfaceTool.
	/// </summary>
	public static ArrayMesh CreateCustomMesh(
		Vector3[] vertices,
		int[] indices,
		Vector3[] normals = null,
		Vector2[] uvs = null,
		Color[] colors = null)
	{
		if (vertices == null || vertices.Length == 0)
		{
			GD.PrintErr("CreateCustomMesh: vertices array is null or empty");
			return null;
		}

		var surfaceTool = new SurfaceTool();
		surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		// Add vertices with attributes
		for (int i = 0; i < vertices.Length; i++)
		{
			// Set normal if provided
			if (normals != null && i < normals.Length)
			{
				surfaceTool.SetNormal(normals[i]);
			}

			// Set UV if provided
			if (uvs != null && i < uvs.Length)
			{
				surfaceTool.SetUV(uvs[i]);
			}

			// Set color if provided
			if (colors != null && i < colors.Length)
			{
				surfaceTool.SetColor(colors[i]);
			}

			// Add vertex
			surfaceTool.AddVertex(vertices[i]);
		}

		// Add indices if provided
		if (indices != null && indices.Length > 0)
		{
			foreach (int index in indices)
			{
				surfaceTool.AddIndex(index);
			}
		}

		// Generate normals if not provided
		if (normals == null)
		{
			surfaceTool.GenerateNormals();
		}

		// Commit to mesh
		return surfaceTool.Commit();
	}

	/// <summary>
	/// Creates a simple plane mesh with specified dimensions using SurfaceTool.
	/// </summary>
	public static MeshInstance3D CreatePlane(Vector2 size, Color color, int subdivisions = 1)
	{
		var vertices = new List<Vector3>();
		var indices = new List<int>();
		var normals = new List<Vector3>();
		var uvs = new List<Vector2>();

		float halfWidth = size.X / 2f;
		float halfDepth = size.Y / 2f;

		// Generate grid vertices
		for (int z = 0; z <= subdivisions; z++)
		{
			for (int x = 0; x <= subdivisions; x++)
			{
				float xPos = -halfWidth + (size.X * x / subdivisions);
				float zPos = -halfDepth + (size.Y * z / subdivisions);

				vertices.Add(new Vector3(xPos, 0, zPos));
				normals.Add(Vector3.Up);
				uvs.Add(new Vector2((float)x / subdivisions, (float)z / subdivisions));
			}
		}

		// Generate indices for triangles
		for (int z = 0; z < subdivisions; z++)
		{
			for (int x = 0; x < subdivisions; x++)
			{
				int topLeft = z * (subdivisions + 1) + x;
				int topRight = topLeft + 1;
				int bottomLeft = (z + 1) * (subdivisions + 1) + x;
				int bottomRight = bottomLeft + 1;

				// First triangle
				indices.Add(topLeft);
				indices.Add(bottomLeft);
				indices.Add(topRight);

				// Second triangle
				indices.Add(topRight);
				indices.Add(bottomLeft);
				indices.Add(bottomRight);
			}
		}

		var mesh = CreateCustomMesh(
			vertices.ToArray(),
			indices.ToArray(),
			normals.ToArray(),
			uvs.ToArray()
		);

		var meshInstance = new MeshInstance3D();
		meshInstance.Mesh = mesh;

		// Create material
		var material = new StandardMaterial3D();
		material.AlbedoColor = color;
		// Use default shading so materials respond to lights

		meshInstance.SetSurfaceOverrideMaterial(0, material);

		return meshInstance;
	}

	/// <summary>
	/// Creates a wall (vertical quad) between two points.
	/// </summary>
	public static MeshInstance3D CreateWall(Vector3 start, Vector3 end, float height, Color color)
	{
		var surfaceTool = new SurfaceTool();
		surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		Vector3 direction = (end - start).Normalized();
		Vector3 normal = new Vector3(-direction.Z, 0, direction.X); // Perpendicular to direction

		// Four corners of the wall
		Vector3 bottomLeft = start;
		Vector3 bottomRight = end;
		Vector3 topLeft = start + new Vector3(0, height, 0);
		Vector3 topRight = end + new Vector3(0, height, 0);

		// Front face
		surfaceTool.SetNormal(normal);
		surfaceTool.SetUV(new Vector2(0, 0));
		surfaceTool.AddVertex(bottomLeft);

		surfaceTool.SetNormal(normal);
		surfaceTool.SetUV(new Vector2(1, 0));
		surfaceTool.AddVertex(bottomRight);

		surfaceTool.SetNormal(normal);
		surfaceTool.SetUV(new Vector2(1, 1));
		surfaceTool.AddVertex(topRight);

		surfaceTool.SetNormal(normal);
		surfaceTool.SetUV(new Vector2(0, 0));
		surfaceTool.AddVertex(bottomLeft);

		surfaceTool.SetNormal(normal);
		surfaceTool.SetUV(new Vector2(1, 1));
		surfaceTool.AddVertex(topRight);

		surfaceTool.SetNormal(normal);
		surfaceTool.SetUV(new Vector2(0, 1));
		surfaceTool.AddVertex(topLeft);

		var mesh = surfaceTool.Commit();
		var meshInstance = new MeshInstance3D();
		meshInstance.Mesh = mesh;

		// Create material
		var material = new StandardMaterial3D();
		material.AlbedoColor = color;
		// Use default shading so materials respond to lights

		meshInstance.SetSurfaceOverrideMaterial(0, material);

		return meshInstance;
	}
}
