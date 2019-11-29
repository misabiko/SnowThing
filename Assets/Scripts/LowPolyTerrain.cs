using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
public class LowPolyTerrain : MonoBehaviour {
	Mesh mesh;
	Vector3[] vertices;
	int[] triangles;
	public MeshCollider meshCollider {get; private set;}

	public float resolution = 0.75f;
	public int xSize = 75;
	public int zSize = 75;
	
	void Start() {
		UpdateTerrain();
	}

	/*void OnValidate() {
		UpdateTerrain();
	}*/

	void UpdateTerrain() {
		InitMesh();

		CreateShape();
		UpdateMesh();
	}

	void InitMesh() {
		if (mesh != null)
			return;
		
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		if (!meshCollider)
			meshCollider = GetComponent<MeshCollider>();
	}

	void CreateShape() {
		vertices = new Vector3[(xSize + 1) * (zSize + 1)];

		float halfXSize = ((float) xSize) / 2;
		float halfZSize = ((float) zSize) / 2;
		for (int z = 0, i = 0; z <= zSize; z++)
			for (int x = 0; x <= xSize; x++) {
				vertices[i] = ModifyVertex(new Vector3(x - halfXSize, 0, z - halfZSize) * resolution);
				i++;
			}

		InitTriangles();
	}

	protected virtual Vector3 ModifyVertex(Vector3 source) => source;

	void InitTriangles() {
		triangles = new int[6 * xSize * zSize];
		int vert = 0;
		int tris = 0;

		for (int z = 0; z < zSize; z++) {
			for (int x = 0; x < xSize; x++) {
				triangles[tris] = vert;
				triangles[tris + 1] = vert + xSize + 1;
				triangles[tris + 2] = vert + 1;
				triangles[tris + 3] = vert + 1;
				triangles[tris + 4] = vert + xSize + 1;
				triangles[tris + 5] = vert + xSize + 2;

				vert++;
				tris += 6;
			}

			vert++;
		}
	}

	void UpdateMesh(bool updateTriangles = true) {
		if (updateTriangles)
			mesh.Clear();

		mesh.vertices = vertices;
		
		if (updateTriangles)
			mesh.triangles = triangles;
		else
			mesh.RecalculateBounds();
		
		mesh.RecalculateNormals();
		
		meshCollider.sharedMesh = mesh;
	}

	public void DrawHeight(Vector3 position, float height, float radius) {
		float radiusSquared = radius * radius;
		
		for (int i = 0; i < vertices.Length; i++) {
			if ((vertices[i] - position).sqrMagnitude <= radiusSquared)
				vertices[i].y += height;
		}
		
		UpdateMesh(false);
	}

	public void DrawHeightSmooth(Vector3 position, float height, float radius) {
		float radiusSquared = radius * radius;
		List<int> indexes = new List<int>();
		float averageY = 0f;
		
		for (int i = 0; i < vertices.Length; i++)
			if ((vertices[i] - position).sqrMagnitude <= radiusSquared) {
				indexes.Add(i);
				averageY += vertices[i].y + height;
			}

		averageY /= indexes.Count;
		foreach (int index in indexes) {
			vertices[index].y = averageY;
		}
		
		UpdateMesh(false);
	}
}