using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SnowTerrain : LowPolyTerrain {
	[Range(0f, 0.5f - float.Epsilon)]
	public float hDisplacement = 0.1f;
	[Range(float.Epsilon, 10f)]
	public float amplitude = 1f;
	[Range(float.Epsilon, 1f)]
	public float frequency = 0.1f;
	
	protected override Vector3 ModifyVertex(Vector3 source) {
		if (hDisplacement != 0f) {
			source.x += hDisplacement * (Random.value - 0.5f);
			source.z += hDisplacement * (Random.value - 0.5f);
		}

		source.y += amplitude * (Mathf.PerlinNoise(frequency * source.x + xSize, frequency * source.z + zSize) - 0.5f);

		return source;
	}
}