using UnityEngine;

public static class Helper {
	public static Vector2 RemoveY(Vector3 vector) {
		return new Vector2(vector.x, vector.z);
	}

	public static Vector3 Flatten(Vector3 vector) {
		vector.y = 0f;
		return vector;
	}
}