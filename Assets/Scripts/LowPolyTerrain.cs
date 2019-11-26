using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class LowPolyTerrain : MonoBehaviour {
	Mesh mesh;

	Vector3[] vertices;
	int[] triangles;

	public int xSize = 20;
	public int zSize = 20;
	
	void Start() {
		UpdateTerrain();
	}

	void OnValidate() {
		UpdateTerrain();
	}

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
	}

	void CreateShape() {
		vertices = new Vector3[(xSize + 1) * (zSize + 1)];

		float halfXSize = ((float) xSize) / 2;
		float halfZSize = ((float) zSize) / 2;
		for (int z = 0, i = 0; z <= zSize; z++)
			for (int x = 0; x <= xSize; x++) {
				vertices[i] = new Vector3(x - halfXSize, 0, z - halfZSize);
				i++;
			}

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

	void UpdateMesh() {
		mesh.Clear();

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		
		mesh.RecalculateNormals();
		
		GetComponent<MeshCollider>().sharedMesh = mesh;
	}
}